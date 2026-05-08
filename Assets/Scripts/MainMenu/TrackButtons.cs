using UnityEngine;

//De Col
namespace MainMenu
{
    public class TrackButtons : MonoBehaviour
    {
    
        public void LoadTrackDesertCity()
        {
            PlayerPrefs.SetInt("GameMode", 0);
            UnityEngine.SceneManagement.SceneManager.LoadScene("DesertCity");
        }
    
        public void LoadTrackDesertCityLastOut()
        {
            PlayerPrefs.SetInt("GameMode", 1);
            UnityEngine.SceneManagement.SceneManager.LoadScene("DesertCity");
        }   
    
        public void LoadTrackSnowLevel()
        {
            PlayerPrefs.SetInt("GameMode", 0);
            UnityEngine.SceneManagement.SceneManager.LoadScene("SnowLvl");
        }    
        public void LoadTrackSnowLevelLastOut()
        {
            PlayerPrefs.SetInt("GameMode", 1);
            UnityEngine.SceneManagement.SceneManager.LoadScene("SnowLvl");
        }
    
        public void LoadTrackLevelDevil()
        {
            PlayerPrefs.SetInt("GameMode", 0);
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelDevil");
        } 
        public void LoadTrackLevelDevilLastOut()
        {
            PlayerPrefs.SetInt("GameMode", 1);
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelDevil");
        }
    
        public void BackToMainMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    

    }
}
