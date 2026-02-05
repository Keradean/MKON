using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    [SerializeField] private Transform kartReset;

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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckGroundStatus();
        Drive(kartGas, kartBrake, kartSteer, kartDrift);
        AddDownForce();
    }

    private void Drive(float gas, float brake, Vector2 steer, bool drift)
    {
        float speed = rb.linearVelocity.magnitude;

        // ---------- Speed based steering ----------
        float speed01 = Mathf.Clamp01(speed / maxSteerSpeed);
        float dynamicSteer = Mathf.Lerp(minSteerAngle, maxSteerAngle, speed01);

        float steerDir = Mathf.Clamp(steer.x, -1f, 1f);
        float targetSteerAngle = steerDir * dynamicSteer;

        // ---------- Drift ----------
        bool isDrifting = drift && gas > 0.1f && isGrounded && Mathf.Abs(steerDir) > 0.1f;

        if (isDrifting)
        {
            targetSteerAngle *= driftSteerMultiplier;

            SetSidewaysFriction(driftSidewaysFriction);

            rb.AddTorque(
                Vector3.up * steerDir * 220f * Time.deltaTime,
                ForceMode.Acceleration
            );

            rb.AddForce(
                transform.forward * driftForwardForce * Time.deltaTime,
                ForceMode.Acceleration
            );
        }
        else
        {
            SetSidewaysFriction(normalSidewaysFriction);
        }

        // ---------- Motor / Brake / Reverse ----------
        float motorTorque = 0f;
        float appliedBrakeTorque = 0f;

        if (gas > 0f)
        {
            // Vorwärts fahren
            motorTorque = gas * driveTorque;
        }
        else if (brake > 0f)
        {
            float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

            if (forwardSpeed < 2f)
            {
                // Rückwärts fahren
                motorTorque = -brake * driveTorque * 0.7f;
                appliedBrakeTorque = 0f;
            }
            else
            {
                // Bremsen
                motorTorque = 0f;
                appliedBrakeTorque = brake * brakeTorque;
            }
        }

        foreach (WheelCollider wheel in wheelColliders)
        {
            wheel.motorTorque = motorTorque;
            wheel.brakeTorque = appliedBrakeTorque;
        }

        // ---------- Steering to front wheels ----------
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            if (i < 2)
                wheelColliders[i].steerAngle = targetSteerAngle;
        }

        // ---------- High speed stability ----------
        if (speed > 15)
            rb.angularVelocity *= 0.97f;
    }


    private void SetSidewaysFriction(float stiffness)
    {
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            WheelFrictionCurve friction = wheelColliders[i].sidewaysFriction;

            friction.stiffness = (i >= 2)
                ? stiffness
                : normalSidewaysFriction;

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
}
