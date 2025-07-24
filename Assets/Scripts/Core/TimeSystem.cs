using System;
using UnityEngine;

namespace TennisCoachCho.Core
{
    [System.Serializable]
    public struct GameDateTime
    {
        public int day;
        public int hour;
        public int minute;
        
        public GameDateTime(int day, int hour, int minute)
        {
            this.day = day;
            this.hour = hour;
            this.minute = minute;
        }
        
        public string GetTimeString()
        {
            string period = hour < 12 ? "AM" : "PM";
            int displayHour = hour == 0 ? 12 : (hour > 12 ? hour - 12 : hour);
            return $"{displayHour:D2}:{minute:D2} {period}";
        }
        
        public string GetDateString()
        {
            return $"Day {day}";
        }
    }
    
    public class TimeSystem : MonoBehaviour
    {
        [Header("Time Settings")]
        [SerializeField] private float realTimeToGameTime = 600f; // 10 minutes real = 1 game day
        [SerializeField] private int startHour = 8;
        [SerializeField] private int startMinute = 0;
        [SerializeField] private int endHour = 24; // Midnight
        
        private GameDateTime currentTime;
        private float timeScale;
        private bool isRunning;
        
        public event Action<GameDateTime> OnTimeChanged;
        public event Action<int> OnNewDay;
        public event Action<int, int> OnTimeReachedForAppointment;
        
        public GameDateTime CurrentTime => currentTime;
        public bool IsRunning => isRunning;
        
        public void Initialize()
        {
            currentTime = new GameDateTime(1, startHour, startMinute);
            CalculateTimeScale();
            StartTime(); // Auto-start time when initialized
            Debug.Log($"[TimeSystem] Initialized - Starting time at Day {currentTime.day}, {currentTime.GetTimeString()}");
        }
        
        private void CalculateTimeScale()
        {
            // Game day = 16 hours (8 AM to Midnight)
            // Real time = 10 minutes = 600 seconds
            // timeScale = game minutes per real second
            float gameMinutesPerDay = (endHour - startHour) * 60f;
            timeScale = gameMinutesPerDay / realTimeToGameTime;
        }
        
        public void StartTime()
        {
            isRunning = true;
        }
        
        public void StopTime()
        {
            isRunning = false;
        }
        
        private void Update()
        {
            if (!isRunning) return;
            
            UpdateTime();
        }
        
        private float accumulatedMinutes = 0f;
        
        private void UpdateTime()
        {
            float minutesToAdd = timeScale * Time.deltaTime;
            
            // Accumulate fractional minutes for smoother time progression
            accumulatedMinutes += minutesToAdd;
            
            int wholeMinutes = Mathf.FloorToInt(accumulatedMinutes);
            if (wholeMinutes > 0)
            {
                accumulatedMinutes -= wholeMinutes;
                AddMinutes(wholeMinutes);
            }
        }
        
        private void AddMinutes(int minutes)
        {
            currentTime.minute += minutes;
            bool timeChanged = false;
            
            while (currentTime.minute >= 60)
            {
                currentTime.minute -= 60;
                currentTime.hour++;
                timeChanged = true;
                
                if (currentTime.hour >= endHour)
                {
                    currentTime.hour = startHour;
                    currentTime.day++;
                    OnNewDay?.Invoke(currentTime.day);
                    Debug.Log($"[TimeSystem] New day started: Day {currentTime.day}");
                }
            }
            
            OnTimeChanged?.Invoke(currentTime);
            
            // Log time changes every hour for debugging
            if (timeChanged)
            {
                Debug.Log($"[TimeSystem] Time updated: Day {currentTime.day}, {currentTime.GetTimeString()}");
            }
        }
        
        public bool IsTimeForAppointment(int appointmentHour, int appointmentMinute)
        {
            return currentTime.hour == appointmentHour && currentTime.minute == appointmentMinute;
        }
        
        public void SetTime(int hour, int minute)
        {
            currentTime.hour = Mathf.Clamp(hour, startHour, endHour - 1);
            currentTime.minute = Mathf.Clamp(minute, 0, 59);
            OnTimeChanged?.Invoke(currentTime);
        }
        
        public void SetDay(int day)
        {
            currentTime.day = Mathf.Max(1, day);
            OnTimeChanged?.Invoke(currentTime);
        }
        
        public void AdvanceTime(int hours, int minutes)
        {
            AddMinutes(minutes + (hours * 60));
        }
        
        public void NextDay()
        {
            currentTime.day++;
            currentTime.hour = startHour;
            currentTime.minute = startMinute;
            OnNewDay?.Invoke(currentTime.day);
            OnTimeChanged?.Invoke(currentTime);
        }
        
        public void SetTimeScale(float newRealTimeToGameTime)
        {
            realTimeToGameTime = newRealTimeToGameTime;
            CalculateTimeScale();
        }
    }
}