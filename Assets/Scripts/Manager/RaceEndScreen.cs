using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class RaceEndScreen : MonoBehaviour
{
    public static RaceEndScreen Instance;

    [Header("End Screen UI")]
    [SerializeField] private GameObject endScreenPanel;       
    [SerializeField] private TextMeshProUGUI finishText;      
    [SerializeField] private TextMeshProUGUI finalTimeText;   
    [SerializeField] private TextMeshProUGUI winnerText;      

    [Header("Buttons")]
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Scene Names")]
    [SerializeField] private string nextLevelName = "";       
    [SerializeField] private string mainMenuName = "MainMenu";

    private float raceTime = 0f;
    private bool raceRunning = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Dont Show the EndScreen
        if (endScreenPanel != null)
            endScreenPanel.SetActive(false);

        // Button Listener
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(LoadNextLevel);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(LoadMainMenu);

        // There is no next level, disable the button
        if (nextLevelButton != null && string.IsNullOrEmpty(nextLevelName))
            nextLevelButton.interactable = false;
    }

    private void Update()
    {
        // Update race time while the race is running
        if (SaveProgress.RaceHasStarted && !SaveProgress.RaceHasEnded)
        {
            raceRunning = true;
            raceTime += Time.deltaTime;
        }
    }

    public void ShowEndScreen()
    {
        raceRunning = false;

        if (endScreenPanel != null)
            endScreenPanel.SetActive(true);

        if (finishText != null)
            finishText.text = "Goal!";

        if (finalTimeText != null)
            finalTimeText.text = "Time: " + FormatTime(raceTime);

        // Show the winner's name 
        if (winnerText != null && GameManager.Instance != null && GameManager.Instance.RacerRanking.Count > 0)
        {
            Racer winner = GameManager.Instance.RacerRanking[0];
            string winnerName = winner.isAI ? "AI - " + winner.gameObject.name : "Player";
            winnerText.text = "The Winner is: " + winnerName;
        }
        
        Debug.Log("The Race is Over! Your Time: " + FormatTime(raceTime));
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }


    # region Switch Level
    public void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            ResetStaticState();
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            // move to the next scene in the build settings if no specific next level is set
            int next = SceneManager.GetActiveScene().buildIndex + 1;
            if (next < SceneManager.sceneCountInBuildSettings)
            {
                ResetStaticState();
                SceneManager.LoadScene(next);
            }
            else
            {
                Debug.Log("Hier gibts nichts mehr geh nach Hause");
            }
        }
    }
  
    public void RestartLevel()
    {
        ResetStaticState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        ResetStaticState();
        SceneManager.LoadScene(mainMenuName);
    }

    // Reset static flags so that the new level starts cleanly
    private void ResetStaticState()
    {
        SaveProgress.RaceHasStarted = false;
        SaveProgress.RaceHasEnded = false;
    }
    # endregion
}