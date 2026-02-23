using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Playables;

public class RaceStart : MonoBehaviour
{
    public GameObject[] playerKart;
    public GameObject[] aiKartPrefabs;
    public Transform[] spawnPoints;
    
    public PlayableDirector timeline;
    public GameObject timelineCamera;
    public GameObject playerCameraRig;
    public GameObject[] kartPrefabs;
    
    public TextMeshProUGUI startText;
    public TextMeshProUGUI title;
    
    void Start()
    {
        
        if (timelineCamera != null)
        {
            timelineCamera.SetActive(true);
        }
        
        
        if (playerCameraRig != null)
        {
            playerCameraRig.SetActive(false);

        }

        
        if (timeline != null)
        {
            timeline.Play();
            StartCoroutine(WaitForTimeline());
        }
        else
        {
            SpawnPlayers();
        }
    }

    IEnumerator WaitForTimeline()
    {
        yield return new WaitForSeconds((float)timeline.duration);
        
        timelineCamera?.SetActive(false);

        playerCameraRig?.SetActive(true);

        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        int selectedKartIndex = MemoryManager.KartId;
        Instantiate(kartPrefabs[selectedKartIndex], spawnPoints[0].position, spawnPoints[0].rotation);
        // Ai Rivals Spawning
        for (int i = 1; i < spawnPoints.Length; i++)
        {
            int aiKartIndex = i % aiKartPrefabs.Length;
            Instantiate(aiKartPrefabs[aiKartIndex], spawnPoints[i].position, spawnPoints[i].rotation);
        }
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        title.text = "";
        startText.text = "Get Ready";
        yield return new WaitForSeconds(1);
        startText.text = "3";
        yield return new WaitForSeconds(1);
        startText.text = "2";
        yield return new WaitForSeconds(1);
        startText.text = "1";
        yield return new WaitForSeconds(1);
        startText.text = "GO";
        SaveProgress.RaceHasStarted = true;
        yield return new WaitForSeconds(1);
        startText.text = "";
    }
}
