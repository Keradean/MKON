using UnityEngine;
//Hauk
public class Goal : MonoBehaviour
{
    public int wayPointCount;
    public int roundCount = 3;
    private int currentLap = 0;

    private void OnTriggerEnter(Collider other)
    {
        Racer racer = other.GetComponent<Racer>();
        if (racer != null)
        {
            if (racer.time == 0) racer.time = Time.time; // Start the timer when the racer hits the goal for the first time
            else
            {
                float roundTime = Time.time - racer.time;
                racer.time = Time.time; // Reset the timer for the next round
                if (roundTime < racer.bestRoundTime)
                {
                    racer.bestRoundTime = roundTime; // Update best round time if the current round is faster
                }
            }

            if (racer.waypointIndex == wayPointCount - 1)
            {
                racer.lap++;
                if (GameManager.Instance.gameMode == GameMode.lastOut)
                {
                    racer.GetLastOutModifire();
                }
                if (racer.lap > currentLap)
                {
                    currentLap = racer.lap;
                    if (GameManager.Instance.gameMode == GameMode.lastOut)
                    {
                    Debug.Log("Goal Triggered by " + other.name);
                        Racer lastRacer = GameManager.Instance.RacerRanking[GameManager.Instance.RacerRanking.Count - 1];
                        Debug.Log("Racer " + lastRacer.name + " is out!");
                        GameManager.Instance.RacerRanking.Remove(lastRacer);
                        lastRacer.LastOut();
                    }
                }
                racer.waypointIndex = 0;
                
                if (!racer.isAI) other.GetComponentInParent<KartController>().kartReset = transform;
                
                if (racer.lap > roundCount && GameManager.Instance.gameMode == GameMode.normal) 
                {
                    racer.EndGame();
                }
            }
        }
    }
}
