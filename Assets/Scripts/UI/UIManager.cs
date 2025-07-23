using UnityEngine;
using TennisCoachCho.Data;
using TennisCoachCho.MiniGames;
using TennisCoachCho.Core;

namespace TennisCoachCho.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private MainHUD mainHUD;
        [SerializeField] private SmartphoneUI smartphoneUI;
        [SerializeField] private RhythmMiniGame rhythmMiniGame;
        [SerializeField] private NotificationPanel notificationPanel;
        [SerializeField] private LocationPrompt locationPrompt;
        [SerializeField] private LessonCompletePanel lessonCompletePanel;
        
        [Header("UI Toggle")]
        [SerializeField] private KeyCode smartphoneKey = KeyCode.Tab;
        
        private bool isSmartphoneOpen;
        
        public void Initialize()
        {
            Debug.Log("UIManager.Initialize() called!");
            Debug.Log($"SmartphoneUI is null: {smartphoneUI == null}");
            
            // Initialize all UI components
            mainHUD?.Initialize();
            
            if (smartphoneUI != null)
            {
                Debug.Log("Calling smartphoneUI.Initialize()");
                smartphoneUI.Initialize();
            }
            else
            {
                Debug.LogError("SmartphoneUI is null! Check UIManager inspector assignments.");
            }
            
            // Hide panels that should start closed
            if (smartphoneUI != null)
                smartphoneUI.gameObject.SetActive(false);
            if (rhythmMiniGame != null)
                rhythmMiniGame.gameObject.SetActive(false);
            if (notificationPanel != null)
                notificationPanel.gameObject.SetActive(false);
            if (locationPrompt != null)
                locationPrompt.gameObject.SetActive(false);
            if (lessonCompletePanel != null)
                lessonCompletePanel.gameObject.SetActive(false);
        }
        
        private void Update()
        {
            HandleInput();
        }
        
        private void HandleInput()
        {
            if (Input.GetKeyDown(smartphoneKey))
            {
                ToggleSmartphone();
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseAllPanels();
            }
        }
        
        public void ShowMainHUD()
        {
            if (mainHUD != null)
                mainHUD.gameObject.SetActive(true);
        }
        
        public void ToggleSmartphone()
        {
            if (smartphoneUI == null) return;
            
            isSmartphoneOpen = !isSmartphoneOpen;
            smartphoneUI.gameObject.SetActive(isSmartphoneOpen);
            
            // Pause/unpause time when smartphone is open
            if (GameManager.Instance?.TimeSystem != null)
            {
                if (isSmartphoneOpen)
                    GameManager.Instance.TimeSystem.StopTime();
                else
                    GameManager.Instance.TimeSystem.StartTime();
            }
        }
        
        public void ShowLocationPrompt(string locationName, bool canStartLesson)
        {
            if (locationPrompt != null)
            {
                locationPrompt.Show(locationName, canStartLesson);
            }
        }
        
        public void HideLocationPrompt()
        {
            if (locationPrompt != null)
            {
                locationPrompt.Hide();
            }
        }
        
        public void ShowAppointmentNotification(AppointmentData appointment)
        {
            if (notificationPanel != null)
            {
                notificationPanel.ShowAppointmentNotification(appointment);
            }
        }
        
        public void StartRhythmMiniGame(AppointmentData appointment)
        {
            if (rhythmMiniGame != null)
            {
                rhythmMiniGame.gameObject.SetActive(true);
                rhythmMiniGame.StartMiniGame(appointment);
                
                // Hide other UI during mini-game
                if (mainHUD != null)
                    mainHUD.gameObject.SetActive(false);
                    
                // Pause movement
                GameManager.Instance?.PlayerController?.SetMovementEnabled(false);
            }
        }
        
        public void ShowLessonComplete(int cashEarned, int expEarned, float performanceScore)
        {
            if (lessonCompletePanel != null)
            {
                lessonCompletePanel.Show(cashEarned, expEarned, performanceScore);
            }
            
            // Re-enable movement and show main HUD
            GameManager.Instance?.PlayerController?.SetMovementEnabled(true);
            if (mainHUD != null)
                mainHUD.gameObject.SetActive(true);
        }
        
        private void CloseAllPanels()
        {
            if (isSmartphoneOpen)
            {
                ToggleSmartphone();
            }
            
            if (locationPrompt != null && locationPrompt.gameObject.activeSelf)
            {
                locationPrompt.Hide();
            }
            
            if (notificationPanel != null && notificationPanel.gameObject.activeSelf)
            {
                notificationPanel.gameObject.SetActive(false);
            }
            
            if (lessonCompletePanel != null && lessonCompletePanel.gameObject.activeSelf)
            {
                lessonCompletePanel.gameObject.SetActive(false);
            }
        }
    }
}