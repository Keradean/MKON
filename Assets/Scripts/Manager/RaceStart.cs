using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class RaceStart : MonoBehaviour
{

    public GameObject PlayerOneKart;

    public Transform PlayerOneSpawnPoint;
    
    public GameObject TimelineHolder;

    public TextMeshProUGUI StartText;

    public TextMeshProUGUI Title;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(TimelineHolder.gameObject);
        SpawnPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        
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
