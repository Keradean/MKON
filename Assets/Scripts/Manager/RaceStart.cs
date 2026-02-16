using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.Playables;

public class RaceStart : MonoBehaviour
{
    public GameObject PlayerOneKart;
    public Transform PlayerOneSpawnPoint;
    
    public PlayableDirector timeline;
    public GameObject timelineCamera;
    public GameObject playerCameraRig;
    
    public TextMeshProUGUI StartText;
    public TextMeshProUGUI Title;
    
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
        
        
        if (timelineCamera != null)
        {
            timelineCamera.SetActive(false);
        }
        
        if (playerCameraRig != null)
        {
            playerCameraRig.SetActive(true);
        }
        
        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        Instantiate(PlayerOneKart, PlayerOneSpawnPoint.position, PlayerOneSpawnPoint.rotation);
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        Title.text = "";
        StartText.text = "Get Ready";
        yield return new WaitForSeconds(1);
        StartText.text = "3";
        yield return new WaitForSeconds(1);
        StartText.text = "2";
        yield return new WaitForSeconds(1);
        StartText.text = "1";
        yield return new WaitForSeconds(1);
        StartText.text = "GO";
        SaveProgress.RaceHasStarted = true;
        yield return new WaitForSeconds(1);
        StartText.text = "";
    }
}
