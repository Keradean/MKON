using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<Racer> RacerRanking = new List<Racer>();
    public List<Racer> FinishedRacer = new List<Racer>();
    [SerializeField] List<GameObject> ProgressPoints = new List<GameObject>();
    public GameMode gameMode;
    public Track track;
    public bool raceStarted = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        // Load game mode and track from PlayerPrefs - By Dennis De Col
        int saveMode = PlayerPrefs.GetInt("GameMode", 0); // Standard is 0, LastOut is 1
        gameMode = (GameMode)saveMode; // Cast int to GameMode enum
        ProgressPoints[0].GetComponent<Goal>().wayPointCount = ProgressPoints.Count;
        for (int i = 1; i < ProgressPoints.Count; i++)
        {
            ProgressPoints[i].GetComponent<ProgressPoint>().waypointIndex = i;
        }
    }

    public void StartRace()
    {
        raceStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        RacerRanking.Sort((a, b) => b.TotalProgress.CompareTo(a.TotalProgress));
    }

    public int GetRankingPos(Racer racer)
    {
        return RacerRanking.IndexOf(racer) + 1;
    }

    public Transform GetWayPoint(int index)
    {
        if (index >= ProgressPoints.Count) index = 0;
        return ProgressPoints[index].transform;
    }

    public Transform[] GetAiRivalPoints()
    {
        Transform[] points = new Transform[ProgressPoints.Count];
        for (int i = 0; i < ProgressPoints.Count; i++)
        {
            points[i] = ProgressPoints[i].transform;
        }
        return points;
    }
}

public enum GameMode
{
    normal,
    lastOut
}

public enum Track
{
    Desert,
    Snow,
    Devil
}