using UnityEngine;
using System.Collections;
using UnityEngine.AI;
public class AIRivalKart : MonoBehaviour
{
    private NavMeshAgent _agent;
    private int _currentWaypoint = 0;
    private bool _checkDistance = false;
    public Transform[] AiRivalWaypoints;
    public GameObject[] Wheels;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        StartCoroutine(SetCheckDistance());
    }

    void Update()
    {
        _agent.SetDestination(AiRivalWaypoints[_currentWaypoint].position);
        CheckDistanceToNextTarget();
        RotateWheels();
        
    }

    private void CheckDistanceToNextTarget()
    {
        if (_agent.remainingDistance <= _agent.stoppingDistance + 0.1f && _checkDistance)
        {
            if (_currentWaypoint < AiRivalWaypoints.Length - 1)
            {
                _currentWaypoint++;
            }
            else
            {
                _currentWaypoint = 0;
            }
            _checkDistance = false;
            StartCoroutine(SetCheckDistance());
        }
    }

    private void RotateWheels()
    {
        for (int i = 0; i < Wheels.Length; i++)
        {
            Wheels[i].transform.Rotate(-10, 0, 0);
        }
    }

    IEnumerator SetCheckDistance()
    {
        yield return new WaitForSeconds(0.1f);
        _checkDistance = true;
    }
}
