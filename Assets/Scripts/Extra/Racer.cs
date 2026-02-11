using System.Collections;
using UnityEngine;

public class Racer : MonoBehaviour
{
    public int rankingPos;
    private Rigidbody rb;
    private PlayerKartControl kartControl;
    [SerializeField] Transform kartmesh;

    public bool isAI = false;
    private AIRivalKart aiRivalKart;

    public bool isShielded = false;

    private Coroutine shieldCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        kartControl = GetComponentInParent<PlayerKartControl>();
        if (isAI)
        {
            aiRivalKart = GetComponentInParent<AIRivalKart>();
        }
    }


    public void GetHit()
    {
        if (isShielded) return;
        if (!isAI)
        {
            rb.linearVelocity *= 0.2f;
            kartControl.enabled = false;
            Invoke("RecoverControl", 1.1f);
        }
        else 
            aiRivalKart.ModifySpeed(0, 1.1f);
        hittimer = 1f;
    }

    private float hittimer = 0f;
    private void Update()
    {
        if (hittimer > 0)
        {
            kartmesh.Rotate(new Vector3(0, 1080, 0) * Time.deltaTime);
            hittimer -= Time.deltaTime;
        }
    }

    private void RecoverControl()
    {
        kartControl.enabled = true;
        isShielded = false;
        //reset rotation
        kartmesh.localRotation = Quaternion.identity;
    }
    public void GetShieldBoost(float duration)
    {
        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
        }
        shieldCoroutine = StartCoroutine(ApplyShieldBoost(duration));
    }
    private IEnumerator ApplyShieldBoost(float duration)
    {
        isShielded = true;
        yield return new WaitForSeconds(duration);
        isShielded = false;
    }

    public void Speedboost(float amount, float duration)
    {
        if(isAI)
        {
            aiRivalKart.ModifySpeed(25 + amount, duration);
            return;
        }
        rb.AddForce(transform.forward * amount, ForceMode.VelocityChange);
        Invoke("EndSpeedBoost", duration);
    }

    private void EndSpeedBoost()
    {
        if(rb.linearVelocity.magnitude > kartControl.MaxSteerSpeed )
            rb.linearVelocity = rb.transform.forward * kartControl.MaxSteerSpeed;
    }
}