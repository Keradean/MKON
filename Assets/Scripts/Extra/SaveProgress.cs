using UnityEngine;
using System.Collections;

public class SaveProgress : MonoBehaviour
{
    private int ProgressNumber = 0;
    public GameObject[] InsertProgressPoints;
    public static bool RaceHasStarted = false;
    public static bool RaceHasEnded = false;
   
    public static int[] ProgressAmts = new int[4]; 
    public static int[] CurrentPosition = new int[4]; 
    void Start()
    {
        for (int i = 0; i < InsertProgressPoints.Length; i++)
        {
            InsertProgressPoints[i].GetComponent<ProgressPoint>().ProgressNumber = ProgressNumber;
            ProgressNumber++;
        }
    }
}