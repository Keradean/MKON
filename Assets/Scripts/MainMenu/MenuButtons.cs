using UnityEngine;
using TMPro;
using System.Collections;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject[] _canvasObjects;
    [SerializeField] private int _kartId = 0;
    [SerializeField] private GameObject DisplayKarts;
    [SerializeField] private GameObject[] Karts;
    [SerializeField] private GameObject[] BackAndForwardButtons;
    [SerializeField] private string[] KartNames;
    [SerializeField] private TextMeshProUGUI DisplayName;
    [SerializeField] private bool _KartsVisible = false;
    [SerializeField] private float KartRotationSpeed = 1.5f;
    
    
    void Start()
    {
       DisplayKarts.SetActive(false); 
    }
    
    void Update()
    {
        // if the karts are visible, rotate them
        if (_KartsVisible)
        {
            DisplayKarts.transform.Rotate(0, KartRotationSpeed * Time.deltaTime, 0);
        }
    }
    
    #region Menu Button Functions

    public void SinglePlayer()
    {
        MemoryManager.SinglePlayerMode = true;
        MemoryManager.MultiplayerPlayerMode = false;
        StartCoroutine(DisplayPlayerSelection());
    }
    
    public void Multiplayer()
    {
        MemoryManager.SinglePlayerMode = false;
        MemoryManager.MultiplayerPlayerMode = true;
        StartCoroutine(DisplayPlayerSelection());
    }

    public void ExitGame()
    {
        StartCoroutine(WaitToExit());
    }
    
    public void Forward()
    {
        if (_kartId < Karts.Length - 1)
            _kartId++;
    
        UpdateKartDisplay();
    }

    public void Back()
    {
        if (_kartId > 0)
            _kartId--;
    
        UpdateKartDisplay();
    }
    
    public void BackToMainMenu()
    {
        //Resets the menu to the main menu state
        _canvasObjects[0].SetActive(true);
        _canvasObjects[1].SetActive(false);
        DisplayKarts.SetActive(false);
        for(int i = 0; i < BackAndForwardButtons.Length; i++)
        {
            BackAndForwardButtons[i].SetActive(false);
        }
        _KartsVisible = false;
    }

    private void UpdateKartDisplay()
    {
        //Switches off all the karts
        for (int i = 0; i < Karts.Length; i++)
            Karts[i].SetActive(false);
    
        //Switches on the selected kart and displays its name
        Karts[_kartId].SetActive(true);
        DisplayName.text = KartNames[_kartId];
    }
    
    public void ChooseKart()
    {
        // Saves the selected kart ID to the MemoryManager
        MemoryManager.KartId = _kartId;
    
        //  Goes to the next scene 
        UnityEngine.SceneManagement.SceneManager.LoadScene("TrackSelection");
    }
    #endregion
    
    #region IEnumerators
    
    private IEnumerator DisplayPlayerSelection()
    {
        yield return new WaitForSeconds(0.5f);
        _canvasObjects[0].SetActive(false);
        _canvasObjects[1].SetActive(true);
        DisplayKarts.SetActive(true);
        //Ensures both the back and forward buttons are visible
        for(int i = 0; i < BackAndForwardButtons.Length; i++)
        {
            BackAndForwardButtons[i].SetActive(true);
        }
        //Switches on the first kart and displays its name
        _kartId = 0;
        UpdateKartDisplay();
        //Makes the karts visible and start rotating
        _KartsVisible = true;
    }
    // Waits a moment before quitting the application
    private IEnumerator WaitToExit()
    {
        yield return new WaitForSeconds(2f);
        Application.Quit();
    }
    #endregion
}