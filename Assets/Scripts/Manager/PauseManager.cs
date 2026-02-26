using UnityEngine;

namespace Manager
{
    public class PauseManager : MonoBehaviour
    {
        public static GameObject Instance;

        void Awake()
        {
            Instance = gameObject;  
            // verstecke das gameObject, damit es nicht im Spiel sichtbar ist
            gameObject.SetActive(false);
        } 
    }
}
