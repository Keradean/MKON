using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class KartController : MonoBehaviour
{
    public CharacterSO characterSO;
    [SerializeField] GameObject[] wheels;
    [SerializeField] float wheelMass;
    private Rigidbody rb;

    [Header("Suspension Spring")]
    [SerializeField] float suspensionRestDist;
    [SerializeField] float verticalStrenght;
    [SerializeField] float dampingStrenght;
    private bool isDrifting = false;


    private float accelerationInput;
    private float brakeInput;
    private Vector2 steerInput;
    private bool driftInput;

    [Header("KartReset")]
    public Transform kartReset;
    [Header("Speed TMP")]
    [SerializeField] private TextMeshProUGUI speedTMP;
    private float _kartSpeed;
    [Header("position TMP")]
    [SerializeField] private TextMeshProUGUI positionTMP;
    [Header("Lap TMP")]
    [SerializeField] private TextMeshProUGUI lapTMP;

    private Racer racer;


    #region Input ActionMap
    public void OnAccelerate(InputValue button)
    {
        // Simple on/off throttle input (1 when pressed, 0 when released)
        accelerationInput = button.isPressed ? 1f : 0f;
    }

    public void OnBrake(InputValue button)
    {
        // Brake input is -1 when pressed, 0 when released
        brakeInput = button.isPressed ? -1f : 0f;
    }

    public void OnSteer(InputValue value)
    {
        // Steering input (x-axis used for left/right)
        steerInput = value.Get<Vector2>();
    }

    public void OnDrift(InputValue value)
    {
        // Drift button (true while held)
        driftInput = value.isPressed;
    }

    public void OnReset(InputValue value)
    {
        if (value.isPressed)
        {
            ResetKart();
        }
    }
    #endregion


    private void Awake()
    {
        speedTMP = GameObject.Find("SpeedTMP").GetComponent<TextMeshProUGUI>();
        positionTMP = GameObject.Find("PositionTMP").GetComponent<TextMeshProUGUI>();
        lapTMP = GameObject.Find("LapTMP").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        racer = GetComponentInChildren<Racer>();
        Goal goal = FindFirstObjectByType<Goal>();

        InvokeRepeating("DisplayPosition", 0.2f, 0.2f);
        //InvokeRepeating("CheckWrongWay", 1f, 0.5f);
        InvokeRepeating("DisplayLap", 0.2f, 0.2f);
        GetComponentInChildren<ItemInventory>().SetItemLuck(characterSO.luck);
    }

    #region UI Display
    private void DisplayPosition()
    {
        if (positionTMP == null) return;

        if (racer != null)
        {
            positionTMP.text = GetPositionText(racer.rankingPos);
        }
    }

    private string GetPositionText(int position)
    {
        if (position <= 0) return "-";

        if (position % 100 >= 11 && position % 100 <= 13)
            return position + "th";

        switch (position % 10)
        {
            case 1: return position + "st";
            case 2: return position + "nd";
            case 3: return position + "rd";
            default: return position + "th";
        }
    }

    public void DisplayLap()
    {
        if (lapTMP == null) return;
        if (racer == null) return;

        int currentLap = racer.lap + 1;

        int totalLaps = 3;

        lapTMP.text = "Lap " + currentLap + "/" + totalLaps;
    }
    #endregion

    void FixedUpdate()
    {
        // --- SPEED DISPLAY ---
        _kartSpeed = rb.linearVelocity.magnitude * 3.6f;
        if (speedTMP != null)
            speedTMP.text = $"{Mathf.RoundToInt(_kartSpeed)} km/h";

        if (driftInput && !isDrifting)
        {
            isDrifting = true;
        }
        else if (!driftInput && isDrifting)
        {
            isDrifting = false;
        }
        // --- WHEEL PHYSICS ---
        foreach (GameObject wheel in wheels)
        {
            if (Physics.Raycast(wheel.transform.position, -wheel.transform.up, out RaycastHit wheelRay, 3f))
            {
                Vector3 springDir = wheel.transform.up;
                Vector3 wheelWorldVel = rb.GetPointVelocity(wheel.transform.position);

                // --- SUSPENSION ---
                float offset = suspensionRestDist - wheelRay.distance;
                float springVel = Vector3.Dot(springDir, wheelWorldVel);

                float springForce = (verticalStrenght * offset) - (dampingStrenght * springVel);
                springForce = Mathf.Clamp(springForce, -verticalStrenght, verticalStrenght);

                Vector3 suspensionForce = Vector3.Project(springDir * springForce, Vector3.up);
                rb.AddForceAtPosition(suspensionForce, wheel.transform.position);


                // --- ACCELERATION ---
                Vector3 accelDir = transform.forward;
                float carSpeed = Vector3.Dot(transform.forward, rb.linearVelocity);

                float availableTorque = CalculateAcceleration(Mathf.Abs(carSpeed))
                                        * accelerationInput
                                        * characterSO.enginePower;

                if (accelerationInput > 0.1f)
                    availableTorque = Mathf.Max(20f, availableTorque);

                rb.AddForceAtPosition(accelDir * availableTorque, wheel.transform.position);


                // --- BRAKING ---
                float brakeForce = 0f;

                if (brakeInput < 0f)
                {
                    if (carSpeed > 0.5f)
                        brakeForce = -characterSO.brakePower;
                    else
                        brakeForce = -characterSO.brakePower / 2;
                }

                rb.AddForceAtPosition(transform.forward * brakeForce, wheel.transform.position);


                // --- STEERING (LATERAL DAMPING ONLY) ---
                Vector3 steerDir = wheel.transform.right;
                wheelWorldVel = rb.GetPointVelocity(wheel.transform.position);

                float steerVel = Vector3.Dot(steerDir, wheelWorldVel);
                float grip = CalculateGripFactor(steerVel);

                // --- DRIFT GRIP LOSS ---
                // --- DRIFT GRIP LOSS ---
                if (isDrifting)
                {
                    // Hinterräder verlieren viel Grip
                    if (wheel == wheels[2] || wheel == wheels[3])
                        grip *= characterSO.backDriftGripLoss;   // z.B. 0.2f
                    else
                        grip *= characterSO.frontDriftGripLoss;  // z.B. 0.75f
                }
                else
                {
                    // Smooth zurück zum normalen Grip
                    grip = Mathf.Lerp(grip, 1f, Time.fixedDeltaTime * 3f);
                }


                float velChange = Mathf.Clamp(-steerVel * grip, -5f, 5f);
                float desiredAccel = Mathf.Clamp(velChange / Time.fixedDeltaTime, -100f, 100f);

                rb.AddForceAtPosition(steerDir * wheelMass * desiredAccel, wheel.transform.position);


                // --- DRIFT FORCE ---
                if (isDrifting && Mathf.Abs(steerInput.x) > 0.1f)
                {
                    Vector3 driftDir = transform.right * Mathf.Sign(steerInput.x);
                    float driftForce = characterSO.driftSideForce * Mathf.Abs(carSpeed) * 0.01f;
                    rb.AddForceAtPosition(driftDir * driftForce, wheel.transform.position);
                }

            }


            // --- VISUAL STEERING (FRONT WHEELS ONLY) ---
            if (wheel == wheels[0] || wheel == wheels[1])
            {
                float steerAngle = steerInput.x * characterSO.maxSteerAngle;

                if (isDrifting)
                    steerAngle *= characterSO.driftSteerMultiplier;

                wheel.transform.localRotation = Quaternion.Euler(0f, steerAngle, 0f);
            }
        }
    }



    private float CalculateGripFactor(float lateralVel)
    {
        // Grip curve based on character stats
        float k = Mathf.Lerp(0.05f, 0.5f, characterSO.gripFactor * -1 + 1);
        k = Mathf.Clamp(k, 0.01f, 1f);

        float grip = 1f / (1f + Mathf.Abs(lateralVel) * k);
        grip = Mathf.Clamp(grip, 0.2f, 1f);

        return grip;
    }

    private float CalculateAcceleration(float speed)
    {
        float normalized = Mathf.Clamp01(speed / characterSO.maxSpeed);

        float accelStat = Mathf.Clamp01(characterSO.acceleration);

        // Earlier torque peak for high-acceleration characters
        float peakShift = Mathf.Lerp(0.2f, 0.1f, accelStat);

        // Sigmoid rise at low speed
        float earlyBoost = 1f / (1f + Mathf.Exp(-10f * (normalized - peakShift)));

        // Exponential falloff at high speed
        float falloffStrength = Mathf.Lerp(2.5f, 1.5f, accelStat);
        float falloff = Mathf.Exp(-falloffStrength * normalized);

        float torqueFactor = earlyBoost * falloff;

        // Scale by character acceleration stat
        torqueFactor *= Mathf.Lerp(1.0f, 2.0f, accelStat);

        return Mathf.Max(0f, torqueFactor);
    }

    private void ResetKart()
    {
        transform.position = kartReset.position;
        transform.rotation = kartReset.rotation;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void LastOutModify(int ranking)
    {
        characterSO.enginePower *= 1 + ranking * 0.02f;
        characterSO.maxSpeed *= 1 + ranking * 0.02f;
        characterSO.acceleration = Mathf.Clamp01(characterSO.acceleration * (1 + ranking * 0.02f));
    }

}