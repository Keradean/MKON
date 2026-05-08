using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Manager
{
    //Hauk
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public string currentMapName = "DesertCity";

        [FormerlySerializedAs("RacerRanking")] public List<Racer> racerRanking = new List<Racer>();
        [FormerlySerializedAs("FinishedRacer")] public List<Racer> finishedRacer = new List<Racer>();
        [FormerlySerializedAs("ProgressPoints")] [SerializeField] List<GameObject> progressPoints = new List<GameObject>();
        public GameMode gameMode;
        public Track track;
        public bool raceStarted;


        private void Awake()
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
            var saveMode = PlayerPrefs.GetInt("GameMode", 0); // Standard is 0, LastOut is 1
            gameMode = (GameMode)saveMode; // Cast int to GameMode enum
            progressPoints[0].GetComponent<Goal>().wayPointCount = progressPoints.Count;
            for (var i = 1; i < progressPoints.Count; i++)
            {
                progressPoints[i].GetComponent<ProgressPoint>().waypointIndex = i;
            }

            currentMapName = track switch
            {
                Track.Desert => "DesertCity",
                Track.Snow => "Snowland",
                Track.Devil => "LevelDevil",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void StartRace()
        {
            raceStarted = true;
        }

        // Update is called once per frame
        private void Update()
        {
            racerRanking.Sort((a, b) => b.TotalProgress.CompareTo(a.TotalProgress));
        }

        public int GetRankingPos(Racer racer)
        {
            return racerRanking.IndexOf(racer) + 1;
        }

        public Transform GetWayPoint(int index)
        {
            if (index >= progressPoints.Count) index = 0;
            return progressPoints[index].transform;
        }

        public Transform[] GetAiRivalPoints()
        {
            var points = new Transform[progressPoints.Count];
            for (var i = 0; i < progressPoints.Count; i++)
            {
                points[i] = progressPoints[i].transform;
            }
            return points;
        }
    }

    public enum GameMode
    {
        Normal,
        LastOut
    }

    public enum Track
    {
        Desert,
        Snow,
        Devil
    }
}