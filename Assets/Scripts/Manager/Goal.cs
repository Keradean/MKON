using UnityEngine;

//Hauk
namespace Manager
{
    public class Goal : MonoBehaviour
    {
        public int wayPointCount;
        public int roundCount = 3;
        private int _currentLap;

        private void OnTriggerEnter(Collider other)
        {
            var racer = other.GetComponent<Racer>();
            if (racer == null) return;
            if (racer.time == 0) racer.time = Time.time; // Start the timer when the racer hits the goal for the first time
            else
            {
                var roundTime = Time.time - racer.time;
                racer.time = Time.time; // Reset the timer for the next round
                if (roundTime < racer.bestRoundTime)
                {
                    racer.bestRoundTime = roundTime; // Update best round time if the current round is faster
                }
            }

            if (racer.waypointIndex != wayPointCount - 1) return;
            racer.lap++;
            if (GameManager.Instance.gameMode == GameMode.LastOut)
            {
                racer.GetLastOutModifire();
            }
            if (racer.lap > _currentLap)
            {
                _currentLap = racer.lap;
                if (GameManager.Instance.gameMode == GameMode.LastOut)
                {
                    Debug.Log("Goal Triggered by " + other.name);
                    var lastRacer = GameManager.Instance.racerRanking[^1];
                    Debug.Log("Racer " + lastRacer.name + " is out!");
                    GameManager.Instance.racerRanking.Remove(lastRacer);
                    lastRacer.LastOut();
                }
            }
            racer.waypointIndex = 0;
                
            if (!racer.isAI) other.GetComponentInParent<KartController>().kartReset = transform;
                
            if (racer.lap > roundCount && GameManager.Instance.gameMode == GameMode.Normal) 
            {
                racer.EndGame();
            }
        }
    }
}
