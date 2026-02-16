using UnityEngine;

public class ProgressPoint : MonoBehaviour
{
    public int waypointIndex;
    public bool AIRivalSpeedControl = false;
    public float AIRivalSetSpeed;

    public int ProgressNumber; 
    

    private void OnTriggerEnter(Collider other)
    {
        Racer racer = other.GetComponent<Racer>();
        if (racer == null) return;

        if (racer.waypointIndex == waypointIndex - 1)
        {
            racer.waypointIndex = waypointIndex;
            
            if (!racer.isAI)
            {
                PlayerKartControl playerKart = other.GetComponentInParent<PlayerKartControl>();
                if (playerKart != null)
                    playerKart.kartReset = transform;
            }
        }

        if (racer.isAI && AIRivalSpeedControl)
        {
            AIRivalKart aiKart = other.GetComponentInParent<AIRivalKart>();
            if (aiKart != null)
                aiKart.MaxSpeed = AIRivalSetSpeed;
        }
    }
}