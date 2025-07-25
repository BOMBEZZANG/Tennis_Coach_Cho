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
        [SerializeField] private DialogueUI dialogueUI;
        
        [Header("UI Toggle")]
        [SerializeField] private KeyCode smartphoneKey = KeyCode.Tab;
        
        private bool isSmartphoneOpen;
        
        public void Initialize()
        {
            Debug.Log($"[UIManager] Initialize() called on GameObject: {gameObject.name}");
            Debug.Log($"[UIManager] LocationPrompt assigned: {locationPrompt != null}");
            if (locationPrompt != null)
            {
                Debug.Log($"[UIManager] LocationPrompt GameObject: {locationPrompt.gameObject.name}");
            }
            
            // Initialize all UI components
            mainHUD?.Initialize();
            
            if (smartphoneUI != null)
            {
                // Debug.Log("Calling smartphoneUI.Initialize()");
                smartphoneUI.Initialize();
            }
            else
            {
                // Debug.LogError("SmartphoneUI is null! Check UIManager inspector assignments.");
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
            Debug.Log($"[UIManager] ShowLocationPrompt called with location: {locationName}, canStart: {canStartLesson}");
            Debug.Log($"[UIManager] UIManager instance: {gameObject.name}");
            Debug.Log($"[UIManager] locationPrompt is null: {locationPrompt == null}");
            
            if (locationPrompt == null)
            {
                Debug.LogError($"[UIManager] LocationPrompt is null on {gameObject.name}! Trying to find LocationPrompt in scene...");
                
                // Try to find LocationPrompt in the scene as fallback
                var foundPrompt = FindObjectOfType<LocationPrompt>();
                if (foundPrompt != null)
                {
                    Debug.LogWarning($"[UIManager] Found LocationPrompt in scene: {foundPrompt.gameObject.name}. Using as fallback.");
                    locationPrompt = foundPrompt;
                }
                else
                {
                    Debug.LogError("[UIManager] No LocationPrompt found in scene!");
                    return;
                }
            }
            
            Debug.Log("[UIManager] Calling locationPrompt.Show()");
            locationPrompt.Show(locationName, canStartLesson);
        }
        
        public void HideLocationPrompt()
        {
            Debug.Log("[UIManager] HideLocationPrompt called");
            Debug.Log($"[UIManager] UIManager instance: {gameObject.name}");
            Debug.Log($"[UIManager] locationPrompt is null: {locationPrompt == null}");
            
            if (locationPrompt == null)
            {
                Debug.LogError($"[UIManager] LocationPrompt is null on {gameObject.name}! Trying to find LocationPrompt in scene...");
                
                // Try to find LocationPrompt in the scene as fallback
                var foundPrompt = FindObjectOfType<LocationPrompt>();
                if (foundPrompt != null)
                {
                    Debug.LogWarning($"[UIManager] Found LocationPrompt in scene: {foundPrompt.gameObject.name}. Using as fallback.");
                    locationPrompt = foundPrompt;
                }
                else
                {
                    Debug.LogError("[UIManager] No LocationPrompt found in scene!");
                    return;
                }
            }
            
            Debug.Log("[UIManager] Calling locationPrompt.Hide()");
            locationPrompt.Hide();
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
        
        public void ShowQuestUpdate(string questText)
        {
            Debug.Log($"[UIManager] Quest Update: {questText}");
            
            // Show quest update as a temporary message
            if (notificationPanel != null)
            {
                notificationPanel.ShowQuestUpdate(questText);
            }
            else if (dialogueUI != null)
            {
                dialogueUI.ShowMessage(questText, 3f);
            }
            else
            {
                Debug.LogWarning("[UIManager] No UI component available for quest update!");
            }
        }
        
        public void ShowTemporaryMessage(string message, float duration = 3f)
        {
            Debug.Log($"[UIManager] Temporary Message: {message}");
            
            if (dialogueUI != null)
            {
                dialogueUI.ShowMessage(message, duration);
            }
            else if (notificationPanel != null)
            {
                notificationPanel.ShowTemporaryMessage(message, duration);
            }
            else
            {
                Debug.LogWarning("[UIManager] No UI component available for temporary message!");
            }
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
            
            if (dialogueUI != null && dialogueUI.IsDialogueActive())
            {
                dialogueUI.CloseDialogue();
            }
        }
    }
}