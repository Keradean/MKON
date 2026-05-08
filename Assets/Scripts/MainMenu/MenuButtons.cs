using System.Collections;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

//De Col
namespace MainMenu
{
    public class MenuButtons : MonoBehaviour
    {
        [FormerlySerializedAs("_canvasObjects")] [SerializeField] private GameObject[] canvasObjects;
        [FormerlySerializedAs("_kartId")] [SerializeField] private int kartId;
        [FormerlySerializedAs("DisplayKarts")] [SerializeField] private GameObject displayKarts;
        [FormerlySerializedAs("Karts")] [SerializeField] private GameObject[] karts;
        [FormerlySerializedAs("BackAndForwardButtons")] [SerializeField] private GameObject[] backAndForwardButtons;
        [FormerlySerializedAs("KartNames")] [SerializeField] private string[] kartNames;
        [FormerlySerializedAs("DisplayName")] [SerializeField] private TextMeshProUGUI displayName;
        [FormerlySerializedAs("_KartsVisible")] [SerializeField] private bool kartsVisible;
        [FormerlySerializedAs("KartRotationSpeed")] [SerializeField] private float kartRotationSpeed = 1.5f;


        private void Start()
        {
            Time.timeScale = 1;
            displayKarts.SetActive(false); 
        }

        private void Update()
        {
            // if the karts are visible, rotate them
            if (kartsVisible)
            {
                displayKarts.transform.Rotate(0, kartRotationSpeed * Time.deltaTime, 0);
            }
        }
    
        #region Menu Button Functions

        public void SinglePlayer()
        {
            MemoryManager.SinglePlayerMode = true;
            //MemoryManager.MultiplayerPlayerMode = false;
            StartCoroutine(DisplayPlayerSelection());
            return;

            IEnumerator DisplayPlayerSelection()
            {
                Debug.Log("4+4=");
                yield return new WaitForSeconds(0.5f);
                Debug.Log("8");
                canvasObjects[0].SetActive(false);
                canvasObjects[1].SetActive(true);
                displayKarts.SetActive(true);
                //Ensures both the back and forward buttons are visible
                foreach (var t in backAndForwardButtons)
                {
                    t.SetActive(true);
                }
                //Switches on the first kart and displays its name
                kartId = 0;
                UpdateKartDisplay();
                //Makes the karts visible and start rotating
                kartsVisible = true;
            }
        }
         /*
        public void Multiplayer()
        {
            MemoryManager.SinglePlayerMode = false;
            MemoryManager.MultiplayerPlayerMode = true;
            StartCoroutine(DisplayPlayerSelection());
        }
        */
        public void ExitGame()
        {
            Application.Quit();
        }
        public void Forward()
        {
            if (kartId < karts.Length - 1)
                kartId++;
            UpdateKartDisplay();
        }
        public void Back()
        {
            if (kartId > 0)
                kartId--;
            UpdateKartDisplay();
        }
        public void BackToMainMenu()
        {
            //Resets the menu to the main menu state
            canvasObjects[0].SetActive(true);
            canvasObjects[1].SetActive(false);
            displayKarts.SetActive(false);
            foreach (var t in backAndForwardButtons)
            {
                t.SetActive(false);
            }
            kartsVisible = false;
        }
        private void UpdateKartDisplay()
        {
            //Switches off all the karts
            foreach (var t in karts)
                t.SetActive(false);
            //Switches on the selected kart and displays its name
            karts[kartId].SetActive(true);
            displayName.text = kartNames[kartId];
        }
        public void ChooseKart()
        {
            // Saves the selected kart ID to the MemoryManager
            MemoryManager.KartId = kartId;
            //  Goes to the next scene 
            UnityEngine.SceneManagement.SceneManager.LoadScene("TrackSelection");
        }
        #endregion
    }
}