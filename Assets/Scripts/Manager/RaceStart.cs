using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Playables;

public class RaceStart : MonoBehaviour
{
    public GameObject[] playerKart;         // player kart prefabs (not used directly yet)
    public GameObject[] aiKartPrefabs;      // all the AI kart options we can spawn
    public Transform[] spawnPoints;         // where each kart drops into the world

    public PlayableDirector timeline;       // the fancy intro cinematic
    public GameObject timelineCamera;       // camera used during the cinematic
    public GameObject playerCameraRig;      // the actual player camera, hidden until race starts
    public GameObject[] kartPrefabs;        // kart prefabs the player can pick from

    public TextMeshProUGUI startText;       // shows "3, 2, 1, GO" etc.
    public TextMeshProUGUI title;           // the title text shown before countdown

    // cached so they survive even if the transforms get destroyed later
    private Vector3[] _spawnPositions;
    private Quaternion[] _spawnRotations;


    void Start()
    {
        // save spawn positions right away before anything can destroy them
        _spawnPositions = new Vector3[spawnPoints.Length];
        _spawnRotations = new Quaternion[spawnPoints.Length];

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            _spawnPositions[i] = spawnPoints[i].position;
            _spawnRotations[i] = spawnPoints[i].rotation;
        }

        // kick off with the cinematic camera, hide the player cam for now
        if (timelineCamera != null)
            timelineCamera.SetActive(true);

        if (playerCameraRig != null)
            playerCameraRig.SetActive(false);

        // if there's a timeline, play it and wait for it to finish
        // otherwise just skip straight to spawning
        if (timeline != null)
        {
            timeline.stopped += OnTimelineStopped;
            timeline.Play();
        }
        else
        {
            SpawnPlayers();
        }
    }

    void OnTimelineStopped(PlayableDirector director)
    {
        // cinematic is done, unsubscribe so this doesn't fire again
        timeline.stopped -= OnTimelineStopped;

        timelineCamera?.SetActive(false);
        playerCameraRig?.SetActive(true);

        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        // basic safety checks to prevent crashes
        if (kartPrefabs == null || kartPrefabs.Length == 0)
        {
            Debug.LogError("No kart prefabs assigned!");
            return;
        }

        if (_spawnPositions == null || _spawnPositions.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        if (MemoryManager.SinglePlayerMode)
        {
            // singleplayer — just spawn the one kart the player picked (clamped to safe range)
            int safeIndex = Mathf.Clamp(
                MemoryManager.KartId,
                0,
                kartPrefabs.Length - 1
            );

            Instantiate(
                kartPrefabs[safeIndex],
                _spawnPositions[0],
                _spawnRotations[0]
            );

            // fill the rest of the spots with AI
            for (int i = 1; i < _spawnPositions.Length; i++)
            {
                if (aiKartPrefabs.Length == 0) break;

                int aiKartIndex = i % aiKartPrefabs.Length;

                Instantiate(
                    aiKartPrefabs[aiKartIndex],
                    _spawnPositions[i],
                    _spawnRotations[i]
                );
            }
        }
        else if (MemoryManager.MultiplayerPlayerMode)
        {
            // grab all the kart selections in order so we can loop through them
            int[] playerKartSelections = new int[]
            {
                MemoryManager.Player1KartSelected,
                MemoryManager.Player2KartSelected,
                MemoryManager.Player3KartSelected,
                MemoryManager.Player4KartSelected
            };

            // limit player amount to available spawn points
            int playerCount = Mathf.Clamp(
                MemoryManager.MultiplayerAmount,
                0,
                _spawnPositions.Length
            );

            // spawn each player at their own spawn point (clamped to safe range)
            for (int i = 0; i < playerCount; i++)
            {
                int safeIndex = Mathf.Clamp(
                    playerKartSelections[i],
                    0,
                    kartPrefabs.Length - 1
                );

                Instantiate(
                    kartPrefabs[safeIndex],
                    _spawnPositions[i],
                    _spawnRotations[i]
                );
            }

            // fill whatever spots are left with AI
            for (int i = playerCount; i < _spawnPositions.Length; i++)
            {
                if (aiKartPrefabs.Length == 0) break;

                int aiKartIndex = i % aiKartPrefabs.Length;

                Instantiate(
                    aiKartPrefabs[aiKartIndex],
                    _spawnPositions[i],
                    _spawnRotations[i]
                );
            }
        }
        else
        {
            // fallback: if no mode was set, spawn default kart
            Instantiate(
                kartPrefabs[0],
                _spawnPositions[0],
                _spawnRotations[0]
            );
        }

        // run the countdown on GameManager so it survives no matter what
        GameManager.Instance.StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        // clear the title and get everyone hyped
        title.text = "";
        startText.text = "Get Ready";
        yield return new WaitForSeconds(1);

        startText.text = "3";
        yield return new WaitForSeconds(1);

        startText.text = "2";
        yield return new WaitForSeconds(1);

        startText.text = "1";
        yield return new WaitForSeconds(1);

        // GO! start the race for real
        startText.text = "GO";
        SaveProgress.RaceHasStarted = true;
        GameManager.Instance.StartRace();

        // let "GO" sit on screen for a moment then clear it
        yield return new WaitForSeconds(1);
        startText.text = "";
    }
}