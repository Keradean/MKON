using System.Collections;
using UnityEngine;

public class Coconut : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent _agent;
    public Racer target;

    [SerializeField] LayerMask layerMask;
    [SerializeField] float speed = 40f;

    private Vector3 lastKnownTargetPos;

    void Start()
    {
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _agent.speed = speed;
        _agent.acceleration = 999f;
        _agent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
        _agent.updateRotation = false;
        _agent.updatePosition = false;
        _agent.enabled = false;
    }

    void Update()
    {

        // activate agent
        if (!_agent.enabled)
        {
            _agent.enabled = true;
            StartCoroutine(UpdatePath());
        }
        //rotate the nut 
        if (_agent.hasPath)
        {
            Vector3 dir = _agent.steeringTarget - transform.position;
            dir.y = 0;

            if (dir.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }

        // Move Coconut
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private IEnumerator UpdatePath()
    {
        yield return null;

        while (true)
        {
            if (target != null)
            {
                lastKnownTargetPos = target.transform.position;
                _agent.SetDestination(lastKnownTargetPos);
            }

            // change update rate based on distance to target, update more often when close to target for better tracking, less often when far away to save performance
            float updateRate = Vector3.Distance(transform.position, lastKnownTargetPos) < 10f ? 0.2f : 0.5f;

            yield return new WaitForSeconds(updateRate);
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
