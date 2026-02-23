using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    public static bool SinglePlayerMode;
    public static bool MultiplayerPlayerMode;
    public static string PlayerName;
    public static int KartId;

    public static int MultiplayerAmount;
    public static int Player1KartSelected;
    public static int Player2KartSelected;
    public static int Player3KartSelected;
    public static int Player4KartSelected;

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
