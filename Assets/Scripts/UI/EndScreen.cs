using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

//Hauk
namespace UI
{
    public class EndScreen : MonoBehaviour
    {
        [SerializeField] private GameObject endScreenPanel;
        [FormerlySerializedAs("PlayerRanking")] [SerializeField] private GameObject playerRanking;
        [SerializeField] private GameObject rankingPrefab;

        [SerializeField] private TextMeshProUGUI playerTimeTMP;
        [SerializeField] private TextMeshProUGUI playerRoundTMP;

        [SerializeField] private TextMeshProUGUI highscoreTMP;
        [SerializeField] private TextMeshProUGUI bestRoundTMP;

        private string _currentMapName;

        public EndScreen(GameObject rankingPrefab)
        {
            this.rankingPrefab = rankingPrefab;
        }

        public void ActivateEndScreen(Racer player)
        {
            endScreenPanel.SetActive(true);

            // Get current map name from GameManager (falls du das anders speicherst, anpassen)
            _currentMapName = GameManager.Instance.currentMapName;

            UpdateRanking();

            if (player != null)
            {
                // Show player times
                playerTimeTMP.text = player.time.ToString("F2") + "s";
                playerRoundTMP.text = player.bestRoundTime.ToString("F2") + "s";

                // Fetch best times from database
                FetchDatabaseTimes(player);
            }
        }

        private void FetchDatabaseTimes(Racer player)
        {
            DatabaseManager.Instance.FetchTimes(_currentMapName, (raceTime, roundTime) =>
            {
                if (raceTime < 0)
                {
                    highscoreTMP.text = "No Data";
                    bestRoundTMP.text = "No Data";
                    return;
                }

                // Show DB times
                highscoreTMP.text = raceTime.ToString("F2") + "s";
                bestRoundTMP.text = roundTime.ToString("F2") + "s";

                // Check if player beat the best times
                TryUpdateBestTimes(player, raceTime, roundTime);
            });
        }

        private void TryUpdateBestTimes(Racer player, float dbRaceTime, float dbRoundTime)
        {
            var updated = false;

            // Check FaceTime
            if (player.time < dbRaceTime)
            {
                DatabaseManager.Instance.UpdateRaceTime(_currentMapName, player.time);
                updated = true;
            }

            // Check RoundTime
            if (player.bestRoundTime < dbRoundTime)
            {
                DatabaseManager.Instance.UpdateRoundTime(_currentMapName, player.bestRoundTime);
                updated = true;
            }

            if (!updated) return;
            Debug.Log("New best time saved to database!");

            // Refresh UI after update
            StartCoroutine(RefreshAfterUpdate());
        }

        private System.Collections.IEnumerator RefreshAfterUpdate()
        {
            // Wait a moment for DB update
            yield return new WaitForSeconds(0.5f);

            // Fetch again to update UI
            DatabaseManager.Instance.FetchTimes(_currentMapName, (raceTime, roundTime) =>
            {
                highscoreTMP.text = raceTime.ToString("F2") + "s";
                bestRoundTMP.text = roundTime.ToString("F2") + "s";
            });
        }

        public void UpdateRanking()
        {
            // Clear existing ranking UI elements
            foreach (Transform child in playerRanking.transform)
            {
                Destroy(child.gameObject);
            }

            var ranking = 1;
            foreach (var racer in GameManager.Instance.finishedRacer)
            {
                var rankingGo = Instantiate(rankingPrefab, playerRanking.transform);
                rankingGo.GetComponent<TextMeshProUGUI>().text = ranking + ". " + racer.racerName;
                ranking++;
            }
        }
    }
}
