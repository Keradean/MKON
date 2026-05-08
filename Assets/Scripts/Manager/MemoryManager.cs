using UnityEngine;

//De Col
namespace Manager
{
    public class MemoryManager : MonoBehaviour
    {
        public static bool SinglePlayerMode;
        public static bool MultiplayerPlayerMode;
        public static int PlayerKartSelected;
        public static string PlayerName;
        public static int KartId;

        private void Start()
        {
            DontDestroyOnLoad(this);
        }
    }
}
