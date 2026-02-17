using UnityEngine;

public class Goal : MonoBehaviour
{
    public int wayPointCount;
    public int roundCount = 3;

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
                    if (!racer.isAI && racer.lap >= roundCount)
                    {
                        SaveProgress.RaceHasEnded = true;
                        SaveProgress.RaceHasStarted = false;
                        
                        if(RaceEndScreen.Instance != null)
                            RaceEndScreen.Instance.ShowEndScreen();
                    }
                }
            }
        }
    }
}
