using UnityEngine;

public class ProgressPoint : MonoBehaviour
{
    public int waypointIndex;
    public bool AIRivalSpeedControl = false;
    public float AIRivalSetSpeed;

    private void OnTriggerEnter(Collider other)
    {
        Racer racer = other.GetComponent<Racer>();
        if (racer != null)
        {
            if (racer.waypointIndex == waypointIndex -1)
            {
                racer.waypointIndex = waypointIndex;
                if (!racer.isAI) other.GetComponentInParent<PlayerKartControl>().kartReset = transform;
            }
        }
        
        
        if(other.CompareTag("Rival_1") || other.CompareTag("Rival_2") 
                                       || other.CompareTag("Rival_3") 
                                       || other.CompareTag("Rival_4"))
        {
            if (AIRivalSpeedControl)
            {
                other.gameObject.GetComponentInParent<AIRivalKart>().MaxSpeed = AIRivalSetSpeed;
            }
        }
    }
}
