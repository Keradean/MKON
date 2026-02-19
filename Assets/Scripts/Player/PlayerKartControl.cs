using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI; 
using TMPro;

public class PlayerKartControl : MonoBehaviour
{
    [Header("Input Variables")]
    [SerializeField] private float kartGas;
    [SerializeField] private float kartBrake;
    [SerializeField] private Vector2 kartSteer;
    [SerializeField] private bool kartDrift;

    [Header("Drift Settings")]
    [SerializeField] private float driftSteerMultiplier = 1.5f;
    [SerializeField] private float driftSidewaysFriction = 0.6f;
    [SerializeField] private float normalSidewaysFriction = 1.0f;
    [SerializeField] private float driftForwardForce = 200f;

    [Header("Speed Based Steering")]
    [SerializeField] private float minSteerAngle = 8f;
    [SerializeField] private float maxSteerAngle = 22f;
    [SerializeField] private float maxSteerSpeed = 35f;
    public float MaxSteerSpeed { get { return maxSteerSpeed; } }

    [Header("Drive Physics")]
    [SerializeField] private WheelCollider[] wheelColliders;
    [SerializeField] private GameObject[] kartWheels;
    [SerializeField] private float driveTorque = 100f;
    [SerializeField] private float brakeTorque = 500f;

    [Header("Stick to Ground")]
    [SerializeField] private float stickToGroundForce = 10f;    
    
    [Header("KartReset")]
    public Transform kartReset;

    [Header("Wrong Way")] 
    private bool _changeDirection;
    
    [Header("Speed TMP")]
    [SerializeField] private TextMeshProUGUI speedTMP;
    private float _kartSpeed;

    [Header("position TMP")]
    [SerializeField] private TextMeshProUGUI positionTMP;
    
    [Header("Wrong Way UI")]
    [SerializeField] private TextMeshProUGUI  wrongWayWarning; 
    
    [Header("Lap TMP")]
    [SerializeField] private TextMeshProUGUI lapTMP;

    private Racer racer;
    private Rigidbody rb;
    public bool BreakAssist = true;
    private bool isGrounded;
    private bool airConstraintsActive = false;

    #region Input ActionMap
    public void OnAccelerate(InputValue button)
    {
        kartGas = button.isPressed ? 1f : 0f;
    }

    public void OnBrake(InputValue button)
    {
        kartBrake = button.isPressed ? 1f : 0f;
    }

    public void OnSteer(InputValue value)
    {
        kartSteer = value.Get<Vector2>();
    }

    public void OnDrift(InputValue value)
    {
        kartDrift = value.isPressed;
    }

    public void OnReset(InputValue value)
    {
        ResetKart();
    }
    #endregion

    private void Awake()
    {
        speedTMP = GameObject.Find("SpeedTMP").GetComponent<TextMeshProUGUI>();
        positionTMP = GameObject.Find("PositionTMP").GetComponent<TextMeshProUGUI>();
        lapTMP = GameObject.Find("LapTMP").GetComponent<TextMeshProUGUI>();
        GameObject wrongWayPanel = GameObject.Find("Panel - WrongWay");
        wrongWayWarning = wrongWayPanel.GetComponentInChildren<TextMeshProUGUI>(true); 
    }
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        racer = GetComponentInChildren<Racer>();
        Goal goal = FindFirstObjectByType<Goal>();
        
        InvokeRepeating("DisplayPosition", 0.2f, 0.2f);
        //InvokeRepeating("CheckWrongWay", 1f, 0.5f);
        InvokeRepeating("DisplayLap", 0.2f, 0.2f); 
    }

    private void Update()
    {
        CheckGroundStatus();
        Drive(kartGas, kartBrake, kartSteer, kartDrift);
        AddDownForce();
        UpdateWheelPositions();
    }

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

    private void Drive(float gas, float brake, Vector2 steer, bool drift)
    {
        if (!SaveProgress.RaceHasStarted)
        {
            gas = 0;
            brake = 0;
            steer = Vector2.zero;
            drift = false; 
        }
        _kartSpeed = rb.linearVelocity.magnitude * 3.6f;
        if (speedTMP != null)
        {
            speedTMP.text = $"{Mathf.RoundToInt(_kartSpeed)} km/h";
        }
        
        float speed = rb.linearVelocity.magnitude;

        float speed01 = Mathf.Clamp01(speed / maxSteerSpeed);
        float dynamicSteer = Mathf.Lerp(minSteerAngle, maxSteerAngle, speed01);

        float steerDir = Mathf.Clamp(steer.x, -1f, 1f);
        float targetSteerAngle = steerDir * dynamicSteer;

        bool isDrifting = drift && gas > 0.1f && isGrounded && Mathf.Abs(steerDir) > 0.1f;

        if (isDrifting)
        {
            targetSteerAngle *= driftSteerMultiplier;
            SetSidewaysFriction(driftSidewaysFriction);
            rb.AddTorque(Vector3.up * steerDir * 220f * Time.deltaTime, ForceMode.Acceleration);
            rb.AddForce(transform.forward * driftForwardForce * Time.deltaTime, ForceMode.Acceleration);
        }
        else
        {
            SetSidewaysFriction(normalSidewaysFriction);
        }

        float motorTorque = 0f;
        float appliedBrakeTorque = 0f;

        if (gas > 0f)
        {
            motorTorque = gas * driveTorque;
        }
        else if (brake > 0f)
        {
            float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

            if (forwardSpeed < 2f)
            {
                motorTorque = -brake * driveTorque * 0.7f;
                appliedBrakeTorque = 0f;

                if (rb.isKinematic)
                {
                    StartCoroutine(ResetChangeDirection());
                }
            }
            else
            {
                motorTorque = 0f;
                appliedBrakeTorque = brake * brakeTorque;
            }
        }

        foreach (WheelCollider wheel in wheelColliders)
        {
            wheel.motorTorque = motorTorque;
            wheel.brakeTorque = appliedBrakeTorque;
        }

        for (int i = 0; i < wheelColliders.Length; i++)
        {
            if (i < 2)
                wheelColliders[i].steerAngle = targetSteerAngle;
        }

        if (speed > 15)
            rb.angularVelocity *= 0.97f;
    }

    private void SetSidewaysFriction(float stiffness)
    {
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            WheelFrictionCurve friction = wheelColliders[i].sidewaysFriction;
            friction.stiffness = (i >= 2) ? stiffness : normalSidewaysFriction;
            wheelColliders[i].sidewaysFriction = friction;
        }
    }

    private void CheckGroundStatus()
    {
        isGrounded = false;

        foreach (WheelCollider wheel in wheelColliders)
        {
            if (wheel.GetGroundHit(out WheelHit hit) && hit.normal != Vector3.zero)
            {
                isGrounded = true;
                break;
            }
        }

        if (!isGrounded && !airConstraintsActive)
            StartCoroutine(ApplyAirConstraints());
    }

    private void AddDownForce()
    {
        if (isGrounded)
            rb.AddForce(-transform.up * stickToGroundForce * rb.linearVelocity.magnitude);
    }

    IEnumerator ApplyAirConstraints()
    {
        airConstraintsActive = true;
        yield return new WaitForSeconds(0.1f);

        if (kartBrake == 0f || !BreakAssist)
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        yield return new WaitForSeconds(0.3f);

        if (kartBrake == 0f || !BreakAssist)
            rb.constraints = RigidbodyConstraints.None;

        airConstraintsActive = false;
    }
    
    private void UpdateWheelPositions()
    {
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            if (i < kartWheels.Length && kartWheels[i] != null)
            {
                Vector3 pos;
                Quaternion rot;
                wheelColliders[i].GetWorldPose(out pos, out rot);
                kartWheels[i].transform.position = pos;
                kartWheels[i].transform.rotation = rot;
            }
        }
    }
    
    private void ResetKart()
    {
        transform.position = kartReset.position;
        transform.rotation = Quaternion.identity;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        kartGas = 0f;
        kartBrake = 0f;
        kartSteer = Vector2.zero;
        kartDrift = false;

        foreach (WheelCollider wheel in wheelColliders)
        {
            wheel.motorTorque = 0f;
            wheel.brakeTorque = 0f;
            wheel.steerAngle = 0f;
        }
    }
    
    private void FaceForward()
    {
        if (_changeDirection) return;
        _changeDirection = true;
        transform.Rotate(0, 180, 0);
        rb.isKinematic = true;
        StartCoroutine(ResetChangeDirection());
    }

    IEnumerator ResetChangeDirection()
    {
        yield return new WaitForSeconds(2f);
        rb.isKinematic = false;
        yield return new WaitForSeconds(1f);
        _changeDirection = false;
    }
   /* 
    private void CheckWrongWay()
    {
        if (racer == null || GameManager.Instance == null) return;
    
        Transform nextWaypoint = GameManager.Instance.GetWayPoint(racer.waypointIndex + 1);
        if (nextWaypoint == null) return;
    
        Vector3 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;
        float alignment = Vector3.Dot(transform.forward, directionToWaypoint);
    
        bool isWrongWay = alignment < -0.3f && rb.linearVelocity.magnitude > 3f;
        
        if (wrongWayWarning != null)
        {
            wrongWayWarning.enabled = isWrongWay;
        }
        
        if (alignment < -0.7f && rb.linearVelocity.magnitude > 5f)
        {
            FaceForward();
        }
    }
   */
    public void LastOutModify(int ranking)
    {

    }
}