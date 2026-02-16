using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<Racer> RacerRanking = new List<Racer>();
    [SerializeField] List<GameObject> ProgressPoints = new List<GameObject>();

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
        ProgressPoints[0].GetComponent<Goal>().wayPointCount = ProgressPoints.Count;
        for (int i = 1; i < ProgressPoints.Count; i++)
        {
            ProgressPoints[i].GetComponent<ProgressPoint>().waypointIndex = i;
        }
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
}
