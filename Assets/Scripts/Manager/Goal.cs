using UnityEngine;

public class Goal : MonoBehaviour
{
    public int wayPointCount;
    public int roundCount;

    private void OnTriggerEnter(Collider other)
    {
        Racer racer = other.GetComponent<Racer>();
        if (racer != null)
        {
            if (racer.waypointIndex == wayPointCount - 1)
            {
                racer.lap++;
                racer.waypointIndex = 0;
                if (!racer.isAI) other.GetComponentInParent<PlayerKartControl>().kartReset = transform;
                if (racer.lap > roundCount) 
                {
                    //racer.EndGame();
                }
            }
        }
    }
}
