using UnityEngine;
//De Col
namespace Manager
{
    public class PauseManager : MonoBehaviour
    {
        public static GameObject Instance;
        private void Awake()
        {
            Instance = gameObject;
            gameObject.SetActive(false);
        } 
    }
}
