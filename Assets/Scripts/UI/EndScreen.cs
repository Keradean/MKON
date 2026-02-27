using UnityEngine;
using TMPro;
//Hauk
public class EndScreen : MonoBehaviour
{
    [SerializeField] GameObject endScreenPanel;
    [SerializeField] GameObject PlayerRanking;
    [SerializeField] GameObject rankingPrefab;

    [SerializeField] TextMeshProUGUI playerTimeTMP;
    [SerializeField] TextMeshProUGUI playerRoundTMP;

    [SerializeField] TextMeshProUGUI highscoreTMP;
    [SerializeField] TextMeshProUGUI bestRoundTMP;

    private string currentMapName;

    public void ActivateEndScreen(Racer player)
    {
        endScreenPanel.SetActive(true);

        // Get current map name from GameManager (falls du das anders speicherst, anpassen)
        currentMapName = GameManager.Instance.currentMapName;

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
        DatabaseManager.Instance.FetchTimes(currentMapName, (raceTime, roundTime) =>
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
        bool updated = false;

        // Check RaceTime
        if (player.time < dbRaceTime)
        {
            DatabaseManager.Instance.UpdateRaceTime(currentMapName, player.time);
            updated = true;
        }

        // Check RoundTime
        if (player.bestRoundTime < dbRoundTime)
        {
            DatabaseManager.Instance.UpdateRoundTime(currentMapName, player.bestRoundTime);
            updated = true;
        }

        if (updated)
        {
            Debug.Log("New best time saved to database!");

            // Refresh UI after update
            StartCoroutine(RefreshAfterUpdate());
        }
    }

    private System.Collections.IEnumerator RefreshAfterUpdate()
    {
        // Wait a moment for DB update
        yield return new WaitForSeconds(0.5f);

        // Fetch again to update UI
        DatabaseManager.Instance.FetchTimes(currentMapName, (raceTime, roundTime) =>
        {
            highscoreTMP.text = raceTime.ToString("F2") + "s";
            bestRoundTMP.text = roundTime.ToString("F2") + "s";
        });
    }

    public void UpdateRanking()
    {
        // Clear existing ranking UI elements
        foreach (Transform child in PlayerRanking.transform)
        {
            Destroy(child.gameObject);
        }

        int ranking = 1;
        foreach (Racer racer in GameManager.Instance.FinishedRacer)
        {
            GameObject rankingGO = Instantiate(rankingPrefab, PlayerRanking.transform);
            rankingGO.GetComponent<TextMeshProUGUI>().text = ranking + ". " + racer.racerName;
            ranking++;
        }
    }
}
