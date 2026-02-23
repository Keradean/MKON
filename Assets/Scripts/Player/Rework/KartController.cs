using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class KartController : MonoBehaviour
{
    public CharacterSO characterSO;
    [SerializeField] GameObject[] wheels;
    [SerializeField] float wheelMass;
    private Rigidbody rb;
    private Racer racer;
    private bool isDrifting = false;

    [Header("Suspension Spring")]
    [SerializeField] float suspensionRestDist;
    [SerializeField] float verticalStrenght;
    [SerializeField] float dampingStrenght;

    private float accelerationInput;
    private float brakeInput;
    private Vector2 steerInput;
    private bool driftInput;

    [Header("Kart Reset")]
    public Transform kartReset;

    [Header("Speed TMP")]
    [SerializeField] private TextMeshProUGUI speedTMP;
    private float _kartSpeed;

    [Header("Position TMP")]
    [SerializeField] private TextMeshProUGUI positionTMP;

    [Header("Lap TMP")]
    [SerializeField] private TextMeshProUGUI lapTMP;

    // splitscreen setup — by Dennis De Col
    [Header("Splitscreen Config")]
    [SerializeField] private CinemachineBrain cinemachineBrain;
    [SerializeField] private Canvas canvas;


    #region Input ActionMap

    public void OnAccelerate(InputValue button)
    {
        // full gas when pressed, nothing when released
        accelerationInput = button.isPressed ? 1f : 0f;
    }

    public void OnBrake(InputValue button)
    {
        // slam the brakes when held, let go to release
        brakeInput = button.isPressed ? -1f : 0f;
    }

    public void OnSteer(InputValue value)
    {
        // left/right from the stick or keyboard
        steerInput = value.Get<Vector2>();
    }

    public void OnDrift(InputValue value)
    {
        // held down = drifting
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
        // grab the UI elements from the scene by name
        speedTMP = GameObject.Find("SpeedTMP").GetComponent<TextMeshProUGUI>();
        positionTMP = GameObject.Find("PositionTMP").GetComponent<TextMeshProUGUI>();
        lapTMP = GameObject.Find("LapTMP").GetComponent<TextMeshProUGUI>();

        // tiny delay so everything is spawned before we mess with cameras — by Dennis De Col
        Invoke("ChangeCameras", 0.05f);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        racer = GetComponentInChildren<Racer>();
        Goal goal = FindFirstObjectByType<Goal>();

        // update position and lap text every 0.2 seconds, no need to do it every frame
        InvokeRepeating("DisplayPosition", 0.2f, 0.2f);
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
        // handles 1st, 2nd, 3rd, and the weird 11th/12th/13th edge cases
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
        if (!GameManager.Instance.raceStarted) return;

        // update the speed display in km/h
        _kartSpeed = rb.linearVelocity.magnitude * 3.6f;
        if (speedTMP != null)
            speedTMP.text = $"{Mathf.RoundToInt(_kartSpeed)} km/h";

        // start drifting when button held, stop when released or not steering
        if (driftInput && !isDrifting)
        {
            isDrifting = true;
        }
        else if ((!driftInput || !(Mathf.Abs(steerInput.x) > 0.1f)) && isDrifting)
        {
            isDrifting = false;
        }

        // if we're in the air, keep the kart level and pull it down faster
        if (!Physics.Raycast(transform.position, -transform.up, out RaycastHit groundRay, 3.01f))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), Time.fixedDeltaTime * 2f);
            rb.AddForce(Vector3.down, ForceMode.Force);
        }

        // process physics for each wheel individually
        foreach (GameObject wheel in wheels)
        {
            if (Physics.Raycast(wheel.transform.position, -wheel.transform.up, out RaycastHit wheelRay, 3f))
            {
                // --- SUSPENSION ---
                // spring force pushes the kart up, damping stops it from bouncing forever
                Vector3 springDir = wheel.transform.up;
                Vector3 wheelWorldVel = rb.GetPointVelocity(wheel.transform.position);

                float offset = suspensionRestDist - wheelRay.distance;
                float springVel = Vector3.Dot(springDir, wheelWorldVel);

                float springForce = (verticalStrenght * offset) - (dampingStrenght * springVel);
                springForce = Mathf.Clamp(springForce, -verticalStrenght, verticalStrenght);

                Vector3 suspensionForce = Vector3.Project(springDir * springForce, Vector3.up);
                rb.AddForceAtPosition(suspensionForce, wheel.transform.position);


                // --- ACCELERATION ---
                Vector3 accelDir = transform.forward;
                float carSpeed = Vector3.Dot(transform.forward, rb.linearVelocity);

                float availableTorque = CalculateAcceleration(Mathf.Abs(carSpeed)) * accelerationInput * characterSO.enginePower;

                // minimum torque so the kart doesn't stall from a dead stop
                if (accelerationInput > 0.1f)
                    availableTorque = Mathf.Max(20f, availableTorque);

                rb.AddForceAtPosition(accelDir * availableTorque, wheel.transform.position);


                // --- BRAKING ---
                float brakeForce = 0f;

                if (brakeInput < 0f)
                {
                    // brake harder at speed, softer when nearly stopped to avoid sliding backwards
                    if (carSpeed > 0.5f)
                        brakeForce = -characterSO.brakePower;
                    else
                        brakeForce = -characterSO.brakePower / 2;
                }

                rb.AddForceAtPosition(transform.forward * brakeForce, wheel.transform.position);


                // --- STEERING (LATERAL DAMPING) ---
                // resist sideways movement based on grip — less grip = more slide
                Vector3 steerDir = wheel.transform.right;
                wheelWorldVel = rb.GetPointVelocity(wheel.transform.position);

                float steerVel = Vector3.Dot(steerDir, wheelWorldVel);
                float grip = CalculateGripFactor(steerVel);

                // --- DRIFT GRIP LOSS ---
                if (isDrifting)
                {
                    // rear wheels lose most of their grip — that's what causes the drift
                    if (wheel == wheels[2] || wheel == wheels[3])
                        grip *= characterSO.backDriftGripLoss;
                    else
                        grip *= characterSO.frontDriftGripLoss;
                }
                else
                {
                    // smoothly recover grip when we stop drifting
                    grip = Mathf.Lerp(grip, 1f, Time.fixedDeltaTime * 3f);
                }

                float velChange = Mathf.Clamp(-steerVel * grip, -5f, 5f);
                float desiredAccel = Mathf.Clamp(velChange / Time.fixedDeltaTime, -100f, 100f);

                rb.AddForceAtPosition(steerDir * wheelMass * desiredAccel, wheel.transform.position);


                // --- DRIFT FORCE ---
                // push the kart sideways while drifting and steering
                if (isDrifting && Mathf.Abs(steerInput.x) > 0.1f)
                {
                    Vector3 driftDir = transform.right * Mathf.Sign(steerInput.x);
                    float driftForce = characterSO.driftSideForce * Mathf.Abs(carSpeed) * 0.01f;
                    rb.AddForceAtPosition(driftDir * driftForce, wheel.transform.position);
                }
            }

            // --- VISUAL STEERING (FRONT WHEELS ONLY) ---
            // rotate the front wheels so they actually look like they're turning
            if (wheel == wheels[0] || wheel == wheels[1])
            {
                float steerAngle = steerInput.x * characterSO.maxSteerAngle;

                if (isDrifting)
                    steerAngle *= characterSO.driftSteerMultiplier;

                wheel.transform.localRotation = Quaternion.Euler(0f, steerAngle, 0f);
            }
        }
    }


    // swap to splitscreen cameras when multiplayer is active — by Dennis De Col
    void ChangeCameras()
    {
        if (MemoryManager.MultiplayerPlayerMode)
        {
            // automatically find all cinemachine cameras on this kart
            CinemachineCamera[] cams = GetComponentsInChildren<CinemachineCamera>();

            if (cams.Length >= 2)
                cinemachineBrain.SetCameraOverride(1, 1, cams[0], cams[1], 1, 1);

            // switch the canvas to render through the cinemachine camera
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cinemachineBrain.GetComponent<Camera>();
        }
    }


    private float CalculateGripFactor(float lateralVel)
    {
        // higher grip stat = steeper curve = less slide at the same speed
        float k = Mathf.Lerp(0.05f, 0.5f, characterSO.gripFactor * -1 + 1);
        k = Mathf.Clamp(k, 0.01f, 1f);

        float grip = 1f / (1f + Mathf.Abs(lateralVel) * k);
        grip = Mathf.Clamp(grip, 0.2f, 1f);

        return grip;
    }

    private float CalculateAcceleration(float speed)
    {
        // how far along the speed range we are (0 = stopped, 1 = max speed)
        float normalized = Mathf.Clamp01(speed / characterSO.maxSpeed);

        float accelStat = Mathf.Clamp01(characterSO.acceleration);

        // high acceleration chars hit their torque peak earlier
        float peakShift = Mathf.Lerp(0.3f, 0.1f, accelStat);

        // sigmoid curve gives a nice punch off the line
        float earlyBoost = 1f / (1f + Mathf.Exp(-10f * (normalized - peakShift)));

        // torque falls off as we approach top speed
        float falloffStrength = Mathf.Lerp(2.5f, 1.5f, accelStat);
        float falloff = Mathf.Exp(-falloffStrength * normalized);

        float torqueFactor = earlyBoost * falloff;

        // scale the whole thing by the character's acceleration stat
        torqueFactor *= Mathf.Lerp(1.0f, 2.0f, accelStat);

        return Mathf.Max(0f, torqueFactor);
    }

    private void ResetKart()
    {
        // snap back to the last checkpoint, wipe all velocity
        transform.position = kartReset.position;
        transform.rotation = kartReset.rotation;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void LastOutModify(int ranking)
    {
        // rubber band boost for players falling behind in last-out mode
        characterSO.enginePower *= 1 + ranking * 0.02f;
        characterSO.maxSpeed *= 1 + ranking * 0.02f;
        characterSO.acceleration = Mathf.Clamp01(characterSO.acceleration * (1 + ranking * 0.02f));
    }
}