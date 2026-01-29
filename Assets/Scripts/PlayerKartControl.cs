using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerKartControl : MonoBehaviour
{
    [Header("Input Variables")]
    [SerializeField] private float kartGas;
    [SerializeField] private float kartBrake;
    [SerializeField] private float kartJump;
    [SerializeField] private Vector2 kartSteer;

    [Header("Drive Physics")]
    [SerializeField] private WheelCollider[] wheelColliders;
    [SerializeField] private GameObject[] kartWheels;
    [SerializeField] private float driveTorque = 100f;
    [SerializeField] private float brakeTorque = 500f;

    [Header("Stick to Ground")]
    [SerializeField] private float stickToGroundForce = 10f;

    [Header("Steer")]
    [SerializeField] private float steerAngle = 15f;

    private Rigidbody rb;
    public bool BreakAssist = true;
    private bool isGrounded;
    private float lastJumpTime = -999f;
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

    public void OnJump(InputValue button)
    {
        kartJump = button.isPressed ? 1f : 0f;
    }
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckGroundStatus();
        Drive(kartGas, kartBrake, kartSteer);
        AddDownForce();
    }

    private void Drive(float gas, float brake, Vector2 steer)
    {
        steer.x = steer.x * steerAngle;

        float motorTorque = 0f;
        float appliedBrakeTorque = 0f;

        if (gas > 0f)
        {
            motorTorque = gas * driveTorque;

            // Release constraints when accelerating
            if (BreakAssist && !airConstraintsActive)
            {
                rb.constraints = RigidbodyConstraints.None;
            }
        }
        // Brake = Either brake or drive backwards
        else if (brake > 0f)
        {
            // Check speed in direction of travel
            float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

            // If slow enough Allow reverse driving
            if (forwardSpeed < 2f)
            {
                motorTorque = -brake * driveTorque * 0.7f; 
                appliedBrakeTorque = 0f;

                // Release constraints when reversing
                if (BreakAssist && !airConstraintsActive)
                {
                    rb.constraints = RigidbodyConstraints.None;
                }
            }
            // When moving forward quickly: Brake
            else
            {
                appliedBrakeTorque = brake * brakeTorque;

                // Brake Assist only during rapid braking
                if (BreakAssist && !airConstraintsActive && forwardSpeed > 5f)
                {
                    rb.constraints =
                        RigidbodyConstraints.FreezeRotationX
                      | RigidbodyConstraints.FreezeRotationZ;
                }
            }
        }
        else
        {
            // No input = Release constraints
            if (BreakAssist && !airConstraintsActive)
            {
                rb.constraints = RigidbodyConstraints.None;
            }
        }

        foreach (WheelCollider wheel in wheelColliders)
        {
            wheel.motorTorque = motorTorque;
            wheel.brakeTorque = appliedBrakeTorque;
        }

        // Update visual wheels
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            Quaternion quat;
            Vector3 position;
            wheelColliders[i].GetWorldPose(out position, out quat);
            kartWheels[i].transform.position = position;
            kartWheels[i].transform.rotation = quat;

            if (i < 2)
            {
                wheelColliders[i].steerAngle = steer.x;
            }
        }
    }

    private void CheckGroundStatus()
    {
        isGrounded = false;

        foreach (WheelCollider wheel in wheelColliders)
        {
            WheelHit wheelHit;
            if (wheel.GetGroundHit(out wheelHit))
            {
                if (wheelHit.normal != Vector3.zero)
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        if (!isGrounded && !airConstraintsActive)
        {
            StartCoroutine(ApplyAirConstraints());
        }
    }

    private void AddDownForce()
    {
        if (isGrounded)
        {
            rb.AddForce(-transform.up * stickToGroundForce * rb.linearVelocity.magnitude);
        }
    }

    IEnumerator ApplyAirConstraints()
    {
        airConstraintsActive = true;

        yield return new WaitForSeconds(0.1f);

        if (kartBrake == 0f || !BreakAssist)
        {
            rb.constraints =
                RigidbodyConstraints.FreezeRotationX
              | RigidbodyConstraints.FreezeRotationZ;
        }

        yield return new WaitForSeconds(0.3f);

        if (kartBrake == 0f || !BreakAssist)
        {
            rb.constraints = RigidbodyConstraints.None;
        }

        airConstraintsActive = false;
    }
}