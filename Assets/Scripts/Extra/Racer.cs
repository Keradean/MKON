using System.Collections;
using UnityEngine;

public class Racer : MonoBehaviour
{
    public int rankingPos;
    private Rigidbody rb;
    private PlayerKartControl kartControl;
    [SerializeField] Transform kartmesh;

    public bool isShielded = false;

    private Coroutine shieldCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        kartControl = GetComponentInParent<PlayerKartControl>();
    }


    public void GetHit()
    {
        if (isShielded) return;
        rb.linearVelocity *= 0.2f;
        kartControl.enabled = false;
        isShielded = true;
        hittimer = 1f;
        Invoke("RecoverControl", 1.1f);
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
        rb.AddForce(transform.forward * amount, ForceMode.VelocityChange);
        Invoke("EndSpeedBoost", duration);
    }

    private void EndSpeedBoost()
    {
        if(rb.linearVelocity.magnitude > kartControl.MaxSteerSpeed )
            rb.linearVelocity = rb.transform.forward * kartControl.MaxSteerSpeed;
    }
}