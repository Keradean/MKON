using UnityEngine;
using TMPro;

public class EndScreen : MonoBehaviour
{
    [SerializeField] GameObject endScreenPanel;
    [SerializeField] GameObject PlayerRanking;
    [SerializeField] GameObject rankingPrefab;

    [SerializeField] TMPro.TextMeshProUGUI playerTimeTMP;
    [SerializeField] TMPro.TextMeshProUGUI playerRoundTMP;

    [SerializeField] TMPro.TextMeshProUGUI highscoreTMP;
    [SerializeField] TMPro.TextMeshProUGUI bestRoundTMP;
    

    public void ActivateEndScreen(Racer player)
    {
        endScreenPanel.SetActive(true);

        UpdateRanking();

        playerTimeTMP.text = player.time.ToString("F2") + "s";
        playerRoundTMP.text = player.bestRoundTime.ToString("F2") + "s";

        // Databasemanager.SaveHighscore(player.time, player.bestRoundTime);
        // Databasemanager.GetHighscroe();


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
            rankingGO.GetComponent<TMPro.TextMeshProUGUI>().text = ranking + ". " + racer.racerName;
            rankingGO.transform.SetParent(PlayerRanking.transform);
            ranking++;
        }

    }
}
