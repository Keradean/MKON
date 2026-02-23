using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject[] _canvasObjects;       // all the screens we swap between
    [SerializeField] private int _kartId = 0;                   // which kart is currently selected
    [SerializeField] private GameObject DisplayKarts;           // the spinning kart display
    [SerializeField] private GameObject[] Karts;                // all available karts
    [SerializeField] private GameObject[] BackAndForwardButtons;// the left/right arrows
    [SerializeField] private string[] KartNames;                // names matching the kart array
    [SerializeField] private TextMeshProUGUI DisplayName;       // shows the kart name on screen
    [SerializeField] private bool _KartsVisible = false;        // are we currently showing karts?
    [SerializeField] private float KartRotationSpeed = 1.5f;    // how fast the kart spins (looking cool)

    [SerializeField] private GameObject MultiplayerAmount;      // the "how many players?" screen
    [SerializeField] private GameObject[] KartSelectedObjects;  // UI elements for kart selection
    [SerializeField] private TextMeshProUGUI SelectTitle;       // tells each player when it's their turn

    private bool _amountChosen;     // did we already pick how many players?
    private int _currentKart = 1;   // which player is currently picking their kart


    void Start()
    {
        // nothing to show yet, hide the karts until someone actually clicks something
        DisplayKarts.SetActive(false);
    }

    void Update()
    {
        // keep spinning that kart like it's on a car dealership floor
        if (_KartsVisible)
        {
            DisplayKarts.transform.Rotate(0, KartRotationSpeed * Time.deltaTime, 0);
        }
    }

    #region Menu Button Functions

    public void SinglePlayer()
    {
        // solo rider, no friends needed
        MemoryManager.SinglePlayerMode = true;
        MemoryManager.MultiplayerPlayerMode = false;
        _amountChosen = false;
        _currentKart = 1;
        StartCoroutine(DisplayPlayerSelection());
    }

    public void Multiplayer()
    {
        // bring your friends, things are about to get chaotic
        MemoryManager.SinglePlayerMode = false;
        MemoryManager.MultiplayerPlayerMode = true;
        _amountChosen = false;
        _currentKart = 1;
        StartCoroutine(DisplayPlayerSelection());
    }

    public void ChoosePlayerAmount(int amount)
    {
        // locked in, we know how many people are playing
        MemoryManager.MultiplayerAmount = amount;
        _amountChosen = true;

        // done with the player count screen, bye
        MultiplayerAmount.SetActive(false);

        // time to pick karts, show everything
        DisplayKarts.SetActive(true);
        for (int i = 0; i < BackAndForwardButtons.Length; i++)
            BackAndForwardButtons[i].SetActive(true);

        // always start from the first kart
        _kartId = 0;
        UpdateKartDisplay();
        _KartsVisible = true;

        // player 1 goes first, obviously
        if (SelectTitle != null)
            SelectTitle.text = "Player 1 Choose your kart";
    }

    public void ExitGame()
    {
        // smell ya later
        StartCoroutine(WaitToExit());
    }

    public void Forward()
    {
        // next kart, don't go out of bounds
        if (_kartId < Karts.Length - 1)
            _kartId++;

        UpdateKartDisplay();
    }

    public void Back()
    {
        // previous kart, don't go negative
        if (_kartId > 0)
            _kartId--;

        UpdateKartDisplay();
    }

    public void BackToMainMenu()
    {
        // reset everything like nothing ever happened
        _canvasObjects[0].SetActive(true);
        _canvasObjects[1].SetActive(false);
        DisplayKarts.SetActive(false);
        for (int i = 0; i < BackAndForwardButtons.Length; i++)
        {
            BackAndForwardButtons[i].SetActive(false);
        }
        _KartsVisible = false;
        _amountChosen = false;
        _currentKart = 1;
    }

    private void UpdateKartDisplay()
    {
        // turn off all karts first so we start clean
        for (int i = 0; i < Karts.Length; i++)
            Karts[i].SetActive(false);

        // now show just the one we want
        Karts[_kartId].SetActive(true);
        DisplayName.text = KartNames[_kartId];
    }

    public void ChooseKart()
    {
        if (MemoryManager.SinglePlayerMode)
        {
            // solo player picked their kart, let's go race
            MemoryManager.KartId = _kartId;
            SceneManager.LoadScene("TrackSelection");
        }

        if (MemoryManager.MultiplayerPlayerMode)
        {
            if (_currentKart <= MemoryManager.MultiplayerAmount)
            {
                // save this player's kart choice
                if (_currentKart == 1) MemoryManager.Player1KartSelected = _kartId;
                if (_currentKart == 2) MemoryManager.Player2KartSelected = _kartId;
                if (_currentKart == 3) MemoryManager.Player3KartSelected = _kartId;
                if (_currentKart == 4) MemoryManager.Player4KartSelected = _kartId;

                if (_currentKart == MemoryManager.MultiplayerAmount)
                {
                    // everyone's picked, let's get out of here
                    SceneManager.LoadScene("TrackSelection");
                }
                else
                {
                    // next player's turn, update the title so they know
                    _currentKart++;
                    SelectTitle.text = "Player " + _currentKart.ToString() + " Choose your kart";
                    _kartId = 0;
                    UpdateKartDisplay();
                }
            }
        }
    }

    #endregion

    #region IEnumerators

    private IEnumerator DisplayPlayerSelection()
    {
        // tiny delay so the transition doesn't feel instant
        yield return new WaitForSeconds(0.5f);
        _canvasObjects[0].SetActive(false);
        _canvasObjects[1].SetActive(true);

        if (MemoryManager.MultiplayerPlayerMode && !_amountChosen)
        {
            // multiplayer but we don't know how many players yet, ask first
            MultiplayerAmount.SetActive(true);
            DisplayKarts.SetActive(false);
            for (int i = 0; i < BackAndForwardButtons.Length; i++)
                BackAndForwardButtons[i].SetActive(false);
        }
        else
        {
            // singleplayer or amount already chosen, go straight to kart selection
            DisplayKarts.SetActive(true);
            for (int i = 0; i < BackAndForwardButtons.Length; i++)
                BackAndForwardButtons[i].SetActive(true);

            _kartId = 0;
            UpdateKartDisplay();
            _KartsVisible = true;

            if (SelectTitle != null)
                SelectTitle.text = "Choose your kart";
        }
    }

    private IEnumerator WaitToExit()
    {
        // give it a moment before closing so it doesn't feel abrupt
        yield return new WaitForSeconds(2f);
        Application.Quit();
    }

    #endregion
}