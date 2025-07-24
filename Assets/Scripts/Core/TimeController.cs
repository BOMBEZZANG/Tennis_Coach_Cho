using UnityEngine;
using TennisCoachCho.Core;

namespace TennisCoachCho.Core
{
    [System.Serializable]
    public class TimeControlSettings
    {
        [Header("Current Time (Read Only)")]
        [SerializeField] private int currentDay = 1;
        [SerializeField] private int currentHour = 8;
        [SerializeField] private int currentMinute = 0;
        [SerializeField] private string currentTimeString = "08:00 AM";
        [SerializeField] private bool isTimeRunning = true;
        
        [Header("Time Control")]
        [SerializeField] private bool pauseTime = false;
        [SerializeField] private bool resumeTime = false;
        
        [Header("Quick Time Advance")]
        [SerializeField] private bool addHour = false;
        [SerializeField] private bool add15Minutes = false;
        [SerializeField] private bool nextDay = false;
        
        [Header("Set Specific Time")]
        [SerializeField] private int setDay = 1;
        [SerializeField] private int setHour = 8;
        [SerializeField] private int setMinute = 0;
        [SerializeField] private bool applyTime = false;
        
        [Header("Time Speed Control")]
        [SerializeField] private TimeSpeed timeSpeed = TimeSpeed.Normal;
        [SerializeField] private bool applySpeed = false;
        
        // Public getters for the inspector display
        public int CurrentDay => currentDay;
        public int CurrentHour => currentHour;
        public int CurrentMinute => currentMinute;
        public string CurrentTimeString => currentTimeString;
        public bool IsTimeRunning => isTimeRunning;
        
        public bool PauseTime { get => pauseTime; set => pauseTime = value; }
        public bool ResumeTime { get => resumeTime; set => resumeTime = value; }
        public bool AddHour { get => addHour; set => addHour = value; }
        public bool Add15Minutes { get => add15Minutes; set => add15Minutes = value; }
        public bool NextDay { get => nextDay; set => nextDay = value; }
        public int SetDay { get => setDay; set => setDay = value; }
        public int SetHour { get => setHour; set => setHour = value; }
        public int SetMinute { get => setMinute; set => setMinute = value; }
        public bool ApplyTime { get => applyTime; set => applyTime = value; }
        public TimeSpeed TimeSpeedSetting { get => timeSpeed; set => timeSpeed = value; }
        public bool ApplySpeed { get => applySpeed; set => applySpeed = value; }
        
        public void UpdateCurrentTimeDisplay(GameDateTime gameTime, bool running)
        {
            currentDay = gameTime.day;
            currentHour = gameTime.hour;
            currentMinute = gameTime.minute;
            currentTimeString = gameTime.GetTimeString();
            isTimeRunning = running;
        }
    }
    
    public enum TimeSpeed
    {
        VerySlow,   // 20 minutes real = 1 game day
        Normal,     // 10 minutes real = 1 game day
        Fast,       // 1 minute real = 1 game day
        VeryFast    // 10 seconds real = 1 game day
    }
    
    public class TimeController : MonoBehaviour
    {
        [Header("Developer Time Controls")]
        [SerializeField] private TimeControlSettings timeControls = new TimeControlSettings();
        
        private void Update()
        {
            if (GameManager.Instance?.TimeSystem == null) return;
            
            // Update display
            timeControls.UpdateCurrentTimeDisplay(
                GameManager.Instance.TimeSystem.CurrentTime,
                GameManager.Instance.TimeSystem.IsRunning
            );
            
            // Handle inspector controls
            HandleTimeControls();
        }
        
        private void HandleTimeControls()
        {
            var timeSystem = GameManager.Instance.TimeSystem;
            
            // Pause/Resume controls
            if (timeControls.PauseTime)
            {
                timeSystem.StopTime();
                timeControls.PauseTime = false;
                Debug.Log("[TimeController] Time paused via inspector");
            }
            
            if (timeControls.ResumeTime)
            {
                timeSystem.StartTime();
                timeControls.ResumeTime = false;
                Debug.Log("[TimeController] Time resumed via inspector");
            }
            
            // Quick advance controls
            if (timeControls.AddHour)
            {
                timeSystem.AdvanceTime(1, 0);
                timeControls.AddHour = false;
                Debug.Log("[TimeController] Added 1 hour via inspector");
            }
            
            if (timeControls.Add15Minutes)
            {
                timeSystem.AdvanceTime(0, 15);
                timeControls.Add15Minutes = false;
                Debug.Log("[TimeController] Added 15 minutes via inspector");
            }
            
            if (timeControls.NextDay)
            {
                timeSystem.NextDay();
                timeControls.NextDay = false;
                Debug.Log("[TimeController] Advanced to next day via inspector");
            }
            
            // Set specific time
            if (timeControls.ApplyTime)
            {
                timeSystem.SetDay(timeControls.SetDay);
                timeSystem.SetTime(timeControls.SetHour, timeControls.SetMinute);
                timeControls.ApplyTime = false;
                Debug.Log($"[TimeController] Time set to Day {timeControls.SetDay}, {timeControls.SetHour:D2}:{timeControls.SetMinute:D2} via inspector");
            }
            
            // Apply time speed
            if (timeControls.ApplySpeed)
            {
                float speedValue = GetTimeSpeedValue(timeControls.TimeSpeedSetting);
                timeSystem.SetTimeScale(speedValue);
                timeControls.ApplySpeed = false;
                Debug.Log($"[TimeController] Time speed set to {timeControls.TimeSpeedSetting} ({speedValue}s) via inspector");
            }
        }
        
        private float GetTimeSpeedValue(TimeSpeed speed)
        {
            switch (speed)
            {
                case TimeSpeed.VerySlow: return 1200f;  // 20 minutes real
                case TimeSpeed.Normal: return 600f;     // 10 minutes real
                case TimeSpeed.Fast: return 60f;       // 1 minute real
                case TimeSpeed.VeryFast: return 10f;   // 10 seconds real
                default: return 600f;
            }
        }
        
        [ContextMenu("Pause Time")]
        private void PauseTimeContextMenu()
        {
            if (GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.StopTime();
                Debug.Log("[TimeController] Time paused via context menu");
            }
        }
        
        [ContextMenu("Resume Time")]
        private void ResumeTimeContextMenu()
        {
            if (GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.StartTime();
                Debug.Log("[TimeController] Time resumed via context menu");
            }
        }
        
        [ContextMenu("Add 1 Hour")]
        private void AddHourContextMenu()
        {
            if (GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.AdvanceTime(1, 0);
                Debug.Log("[TimeController] Added 1 hour via context menu");
            }
        }
        
        [ContextMenu("Next Day")]
        private void NextDayContextMenu()
        {
            if (GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.NextDay();
                Debug.Log("[TimeController] Advanced to next day via context menu");
            }
        }
    }
}