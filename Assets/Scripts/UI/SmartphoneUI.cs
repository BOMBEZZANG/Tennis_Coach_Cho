using UnityEngine;
using UnityEngine.UI;
using TennisCoachCho.Core;
using TennisCoachCho.Utilities;

namespace TennisCoachCho.UI
{
    public class SmartphoneUI : MonoBehaviour
    {
        [Header("App Buttons")]
        [SerializeField] private Button lessonBoardButton;
        [SerializeField] private Button schedulerButton;
        [SerializeField] private Button skillsPerksButton;
        [SerializeField] private Button closeButton;
        
        [Header("App Panels")]
        [SerializeField] private LessonBoardApp lessonBoardApp;
        [SerializeField] private SchedulerApp schedulerApp;
        [SerializeField] private SkillsPerksApp skillsPerksApp;
        
        [Header("UI Elements")]
        [SerializeField] private GameObject appMenu;
        
        private enum AppState
        {
            Menu,
            LessonBoard,
            Scheduler,
            SkillsPerks
        }
        
        private AppState currentState = AppState.Menu;
        
        public void Initialize()
        {
            Debug.Log("SmartphoneUI.Initialize() called!");
            // Setup button listeners
            if (lessonBoardButton != null)
            {
                lessonBoardButton.onClick.AddListener(() => {
                    Debug.Log("LessonBoard button clicked!");
                    OpenApp(AppState.LessonBoard);
                });
            }
            else
            {
                Debug.LogError("LessonBoard button is null! Check SmartphoneUI inspector assignments.");
            }
            if (schedulerButton != null)
                schedulerButton.onClick.AddListener(() => OpenApp(AppState.Scheduler));
            if (skillsPerksButton != null)
                skillsPerksButton.onClick.AddListener(() => OpenApp(AppState.SkillsPerks));
            if (closeButton != null)
                closeButton.onClick.AddListener(CloseSmartphone);
                
            // Initialize apps
            lessonBoardApp?.Initialize();
            schedulerApp?.Initialize();
            skillsPerksApp?.Initialize();
            
            // Show menu by default
            ShowAppMenu();
        }
        
        private void OpenApp(AppState appState)
        {
            DebugLogger.LogSeparator($"SMARTPHONE UI - OPEN APP: {appState}");
            DebugLogger.LogUIEvent($"OpenApp called with state: {appState}");
            currentState = appState;
            
            // Hide menu
            if (appMenu != null)
                appMenu.SetActive(false);
                
            // Hide all apps
            HideAllApps();
            
            // Show selected app
            switch (appState)
            {
                case AppState.LessonBoard:
                    Debug.Log($"Opening LessonBoard app. lessonBoardApp is null: {lessonBoardApp == null}");
                    if (lessonBoardApp != null)
                    {
                        lessonBoardApp.gameObject.SetActive(true);
                        Debug.Log("LessonBoardApp activated");
                        lessonBoardApp.RefreshAppointments();
                    }
                    else
                    {
                        Debug.LogError("LessonBoardApp is null! Check SmartphoneUI inspector assignments.");
                    }
                    break;
                case AppState.Scheduler:
                    DebugLogger.LogUIEvent("Opening Scheduler app");
                    DebugLogger.LogUIEvent($"schedulerApp is null: {schedulerApp == null}");
                    if (schedulerApp != null)
                    {
                        DebugLogger.LogGameObject(schedulerApp.gameObject, "SchedulerApp before activation");
                        schedulerApp.gameObject.SetActive(true);
                        DebugLogger.LogGameObject(schedulerApp.gameObject, "SchedulerApp after activation");
                        schedulerApp.RefreshSchedule();
                        DebugLogger.LogUIEvent("SchedulerApp.RefreshSchedule() called");
                    }
                    else
                    {
                        DebugLogger.LogError("SchedulerApp is null in SmartphoneUI!");
                    }
                    break;
                case AppState.SkillsPerks:
                    if (skillsPerksApp != null)
                    {
                        skillsPerksApp.gameObject.SetActive(true);
                        skillsPerksApp.RefreshData();
                    }
                    break;
            }
        }
        
        public void ShowAppMenu()
        {
            currentState = AppState.Menu;
            
            if (appMenu != null)
                appMenu.SetActive(true);
                
            HideAllApps();
        }
        
        private void HideAllApps()
        {
            if (lessonBoardApp != null)
                lessonBoardApp.gameObject.SetActive(false);
            if (schedulerApp != null)
                schedulerApp.gameObject.SetActive(false);
            if (skillsPerksApp != null)
                skillsPerksApp.gameObject.SetActive(false);
        }
        
        private void CloseSmartphone()
        {
            if (GameManager.Instance?.UIManager != null)
            {
                GameManager.Instance.UIManager.ToggleSmartphone();
            }
        }
        
        private void Update()
        {
            // Handle back navigation
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
            {
                if (currentState != AppState.Menu)
                {
                    ShowAppMenu();
                }
                else
                {
                    CloseSmartphone();
                }
            }
        }
    }
}