using UnityEngine;

public class SaveProgress : MonoBehaviour
{
    private int ProgressNumber = 0;
    public GameObject[] InsertProgressPoints;
    
    // der aktuelle progress von den Karts Player wie RivalAi
    public static int[] ProgressAmts = new int[8]; 
    public static int[] CurrentPosition = new int[8]; 

    void Start()
    {
        for (int i = 0; i < InsertProgressPoints.Length; i++)
        {
            InsertProgressPoints[i].GetComponent<ProgressPoint>().ProgressNumber = ProgressNumber;
            ProgressNumber++;
        }
    }

}
