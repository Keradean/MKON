using System.Collections;
using UnityEngine;

public class Coconut : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent _agent;
    public Racer target;

    [SerializeField] LayerMask layerMask;
    [SerializeField] float speed = 40f;
    private float windUpTime = 0.4f;

    void Start()
    {
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _agent.speed = speed;
        _agent.acceleration = 999f;
        _agent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
        _agent.enabled = false;
    }

    void Update()
    {
        if (windUpTime > 0)
        {
            windUpTime -= Time.deltaTime;
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        if (!_agent.enabled)
        {
            _agent.enabled = true;
            StartCoroutine(UpdatePath());
        }
    }

    private IEnumerator UpdatePath()
    {
        // 1 Frame warten, bis Agent initialisiert ist
        yield return null;

        while (true)
        {
            _agent.SetDestination(target.transform.position);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((layerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            other.GetComponent<Racer>()?.GetHit();
            Destroy(gameObject);
        }
    }
}
