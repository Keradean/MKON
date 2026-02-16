using System.Collections;
using UnityEngine;

public class Racer : MonoBehaviour
{
    public int rankingPos;
    public int lap;
    public int waypointIndex;
    public float distanceToNext;
    public float TotalProgress => lap * 100000 + waypointIndex * 1000 - distanceToNext;

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

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RacerRanking.Add(this);
        }
        else
        {
            Debug.LogError("Scheiße gell???");
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
        {
            aiRivalKart.ModifySpeed(0, 1.1f);
        }
        hittimer = 1f;
    }

    private float hittimer = 0f;
    private void Update()
    {
        distanceToNext = Vector3.Distance(transform.position, GameManager.Instance.GetWayPoint(waypointIndex + 1).position);
        rankingPos = GameManager.Instance.GetRankingPos(this);
        if (hittimer > 0)
        {
            kartmesh.Rotate(new Vector3(0, 1080, 0) * Time.deltaTime);
            hittimer -= Time.deltaTime;
            if (hittimer <= 0)
            {
                kartmesh.localRotation = Quaternion.identity;
                isShielded = false;
            }
        }
    }

    private void RecoverControl()
    {
        kartControl.enabled = true;
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

    public void Speedboost(float amount, float duration, Vector3 direction)
    {
        CancelInvoke("EndSpeedBoost");
        if (isAI)
        {
            aiRivalKart.ModifySpeed(25 + amount, duration);
            return;
        }
        if (direction == Vector3.zero)
        {
            direction = transform.forward;
        }
        else
        {
            if (Vector3.Dot(direction, transform.forward) < 0)
            {
                direction *= 2;
            }
        }
        rb.AddForce(direction * amount, ForceMode.VelocityChange);
        Invoke("EndSpeedBoost", duration);
    }

    private void EndSpeedBoost()
    {
        if(rb.linearVelocity.magnitude > kartControl.MaxSteerSpeed )
            rb.linearVelocity = rb.transform.forward * kartControl.MaxSteerSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Speed"))
        {
            Speedboost(20f, 2f, other.transform.forward);
        }
    }
}