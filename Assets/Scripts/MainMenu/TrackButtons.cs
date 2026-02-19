using UnityEngine;

public class TrackButtons : MonoBehaviour
{
    
    public void LoadTrackDesertCity()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DesertCity");
    }   
    
    public void LoadTrackSnowLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SnowLvl");
    }
    
    public void LoadTrackLevelDevil()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelDevil");
    }
    
    public void BackToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

}
