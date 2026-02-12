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

    private Coroutine rocoverSpeed;

    public float MaxSpeed = 15f;

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
        if (GetComponent<ObstacleSound>().RivalIsHit == false)
        ChangeSpeed();
    }

    private void ChangeSpeed()
    {
        //Speed ist zu KLEIN -> Erhöhe
        if (_agent.speed < MaxSpeed)
        {
            _agent.speed += 10 * Time.deltaTime;
        }
    
        // Speed ist zu GROSS -> Verringere
        else if (_agent.speed > MaxSpeed)
        {
            _agent.speed -= 10 * Time.deltaTime;
        }
    }

    private void CheckDistanceToNextTarget()
    {
        if (_agent.remainingDistance <= _agent.stoppingDistance + 15f && _checkDistance)
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

    public void ModifySpeed(float amount, float duration)
    {
        _agent.speed = amount;
        if (rocoverSpeed != null)StopCoroutine(rocoverSpeed);
        rocoverSpeed = StartCoroutine(RecoverSpeed(duration));
    }

    private IEnumerator RecoverSpeed(float duration)
    {
        yield return new WaitForSeconds(duration);
        _agent.speed = 25f; // Reset to original speed
    }

}
