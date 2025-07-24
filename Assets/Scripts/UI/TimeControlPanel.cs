using UnityEngine;
using UnityEngine.UI;
using TennisCoachCho.Core;
using TMPro;

namespace TennisCoachCho.UI
{
    public class TimeControlPanel : MonoBehaviour
    {
        [Header("Time Display")]
        [SerializeField] private TextMeshProUGUI currentTimeText;
        [SerializeField] private TextMeshProUGUI currentDayText;
        [SerializeField] private TextMeshProUGUI timeScaleText;
        
        [Header("Time Controls")]
        [SerializeField] private Button playPauseButton;
        [SerializeField] private Button nextDayButton;
        [SerializeField] private Button addHourButton;
        [SerializeField] private Button addMinuteButton;
        
        [Header("Time Scale Controls")]
        [SerializeField] private Button slowTimeButton;
        [SerializeField] private Button normalTimeButton;
        [SerializeField] private Button fastTimeButton;
        [SerializeField] private Button veryFastTimeButton;
        
        [Header("Manual Time Input")]
        [SerializeField] private TMP_InputField hourInput;
        [SerializeField] private TMP_InputField minuteInput;
        [SerializeField] private TMP_InputField dayInput;
        [SerializeField] private Button setTimeButton;
        
        [Header("Toggle")]
        [SerializeField] private KeyCode toggleKey = KeyCode.T;
        [SerializeField] private bool startVisible = false;
        
        private bool isVisible;
        private TextMeshProUGUI playPauseButtonText;
        
        private void Awake()
        {
            if (playPauseButton != null)
            {
                playPauseButtonText = playPauseButton.GetComponentInChildren<TextMeshProUGUI>();
                playPauseButton.onClick.AddListener(TogglePlayPause);
            }
            
            if (nextDayButton != null)
                nextDayButton.onClick.AddListener(NextDay);
            if (addHourButton != null)
                addHourButton.onClick.AddListener(AddHour);
            if (addMinuteButton != null)
                addMinuteButton.onClick.AddListener(AddMinute);
                
            if (slowTimeButton != null)
                slowTimeButton.onClick.AddListener(() => SetTimeScale(1200f)); // Very slow
            if (normalTimeButton != null)
                normalTimeButton.onClick.AddListener(() => SetTimeScale(600f)); // Normal
            if (fastTimeButton != null)
                fastTimeButton.onClick.AddListener(() => SetTimeScale(60f)); // Fast
            if (veryFastTimeButton != null)
                veryFastTimeButton.onClick.AddListener(() => SetTimeScale(10f)); // Very fast
                
            if (setTimeButton != null)
                setTimeButton.onClick.AddListener(SetManualTime);
                
            isVisible = startVisible;
            gameObject.SetActive(isVisible);
        }
        
        private void Start()
        {
            if (GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.OnTimeChanged += UpdateTimeDisplay;
                UpdateTimeDisplay(GameManager.Instance.TimeSystem.CurrentTime);
            }
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                ToggleVisibility();
            }
        }
        
        private void ToggleVisibility()
        {
            isVisible = !isVisible;
            gameObject.SetActive(isVisible);
            Debug.Log($"[TimeControlPanel] Visibility toggled: {isVisible}");
        }
        
        private void UpdateTimeDisplay(GameDateTime gameTime)
        {
            if (currentTimeText != null)
                currentTimeText.text = $"Time: {gameTime.GetTimeString()}";
            if (currentDayText != null)
                currentDayText.text = $"{gameTime.GetDateString()}";
                
            // Update play/pause button text
            if (playPauseButtonText != null && GameManager.Instance?.TimeSystem != null)
            {
                playPauseButtonText.text = GameManager.Instance.TimeSystem.IsRunning ? "Pause" : "Play";
            }
        }
        
        private void TogglePlayPause()
        {
            if (GameManager.Instance?.TimeSystem != null)
            {
                if (GameManager.Instance.TimeSystem.IsRunning)
                {
                    GameManager.Instance.TimeSystem.StopTime();
                    Debug.Log("[TimeControlPanel] Time paused");
                }
                else
                {
                    GameManager.Instance.TimeSystem.StartTime();
                    Debug.Log("[TimeControlPanel] Time resumed");
                }
            }
        }
        
        private void NextDay()
        {
            if (GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.NextDay();
                Debug.Log("[TimeControlPanel] Advanced to next day");
            }
        }
        
        private void AddHour()
        {
            if (GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.AdvanceTime(1, 0);
                Debug.Log("[TimeControlPanel] Added 1 hour");
            }
        }
        
        private void AddMinute()
        {
            if (GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.AdvanceTime(0, 15); // Add 15 minutes at a time
                Debug.Log("[TimeControlPanel] Added 15 minutes");
            }
        }
        
        private void SetTimeScale(float newTimeScale)
        {
            if (GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.SetTimeScale(newTimeScale);
                UpdateTimeScaleDisplay(newTimeScale);
                Debug.Log($"[TimeControlPanel] Time scale set to: {newTimeScale}");
            }
        }
        
        private void UpdateTimeScaleDisplay(float timeScale)
        {
            if (timeScaleText != null)
            {
                string speedName = GetSpeedName(timeScale);
                timeScaleText.text = $"Speed: {speedName}";
            }
        }
        
        private string GetSpeedName(float timeScale)
        {
            if (timeScale >= 1200f) return "Very Slow";
            if (timeScale >= 600f) return "Normal";
            if (timeScale >= 60f) return "Fast";
            if (timeScale >= 10f) return "Very Fast";
            return "Ultra Fast";
        }
        
        private void SetManualTime()
        {
            if (GameManager.Instance?.TimeSystem == null) return;
            
            int hour = 8; // Default hour
            int minute = 0; // Default minute
            int day = 1; // Default day
            
            if (hourInput != null && int.TryParse(hourInput.text, out int inputHour))
                hour = inputHour;
            if (minuteInput != null && int.TryParse(minuteInput.text, out int inputMinute))
                minute = inputMinute;
            if (dayInput != null && int.TryParse(dayInput.text, out int inputDay))
                day = inputDay;
                
            GameManager.Instance.TimeSystem.SetDay(day);
            GameManager.Instance.TimeSystem.SetTime(hour, minute);
            
            Debug.Log($"[TimeControlPanel] Time set manually to Day {day}, {hour:D2}:{minute:D2}");
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.OnTimeChanged -= UpdateTimeDisplay;
            }
        }
    }
}