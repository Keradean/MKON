using UnityEngine;
//De Col
public class MemoryManager : MonoBehaviour
{
    public static bool SinglePlayerMode;
    public static bool MultiplayerPlayerMode;
    public static int PlayerKartSelected;
    public static string PlayerName;
    public static int KartId;

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
