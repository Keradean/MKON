using System.Collections;
using UnityEngine;
//Hauk
public class Racer : MonoBehaviour
{
    public string racerName;
    public int rankingPos;
    public int lap;
    public int waypointIndex;
    public float distanceToNext;
    public float TotalProgress => lap * 10000000 + waypointIndex * 10000 - distanceToNext;

    public float time;
    public float bestRoundTime = Mathf.Infinity;

    private Rigidbody rb;
    private KartController kartControl;
    private ObstacleSound obstacleSound;
    [SerializeField] Transform kartmesh;

    public bool isAI = false;
    private AIRivalKart aiRivalKart;

    public bool isShielded = false;

    private Coroutine shieldCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        kartControl = GetComponentInParent<KartController>();
        obstacleSound = GetComponentInParent<ObstacleSound>();
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
            aiRivalKart.SpeedBoost(0, 1.1f);
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
            aiRivalKart.SpeedBoost(amount, duration);
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
        if(rb.linearVelocity.magnitude > kartControl.characterSO.maxSpeed )
            rb.linearVelocity = rb.transform.forward * kartControl.characterSO.maxSpeed;
    }

    public void LastOut()
    {
        if (GameManager.Instance.gameMode != GameMode.lastOut) return;
        obstacleSound.BurnTheKart(false);
        Invoke("EndGame", 3.5f);
    }

    public void GetLastOutModifire()
    {
        if (!isAI)
        {
            kartControl.LastOutModify(rankingPos);
        }
        else
        {
            aiRivalKart.LastOutSpeed(rankingPos);
        }
    }

    public void EndGame()
    {
        GameManager.Instance.FinishedRacer.Add(this);
        if (!isAI)//if its the player of this kart
        {
            GameObject.Find("EndScreen").GetComponent<EndScreen>().ActivateEndScreen(this);
        }
        else
        {
            GameObject.Find("EndScreen").GetComponent<EndScreen>().UpdateRanking();

        }
        if(GameManager.Instance.FinishedRacer.Count == GameManager.Instance.RacerRanking.Count)
        {
            // All racers have finished, show end screen for all
            GameObject.Find("EndScreen").GetComponent<EndScreen>().ActivateEndScreen(null);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Speed"))
        {
            Speedboost(10f, 2f, other.transform.forward);
        }
    }
}