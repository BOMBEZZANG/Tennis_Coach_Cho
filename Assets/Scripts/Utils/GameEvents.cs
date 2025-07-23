using System;
using UnityEngine;
using TennisCoachCho.Data;

namespace TennisCoachCho.Utils
{
    public static class GameEvents
    {
        // Player Events
        public static event Action<int> OnPlayerLevelUp;
        public static event Action<int> OnCashChanged;
        public static event Action<int> OnSkillPointsChanged;
        public static event Action<int> OnPerkPointsChanged;
        
        // Skill Events
        public static event Action<string, int> OnSkillLevelUp; // skill name, new level
        public static event Action<string> OnPerkUnlocked; // perk name
        
        // Time Events
        public static event Action<int> OnNewDay; // day number
        public static event Action<int, int> OnTimeChanged; // hour, minute
        
        // Appointment Events
        public static event Action<AppointmentData> OnAppointmentAccepted;
        public static event Action<AppointmentData> OnAppointmentCompleted;
        public static event Action<AppointmentData> OnAppointmentStarted;
        
        // Mini-game Events
        public static event Action<float> OnMiniGameCompleted; // performance score
        public static event Action OnMiniGameStarted;
        
        // UI Events
        public static event Action OnSmartphoneOpened;
        public static event Action OnSmartphoneClosed;
        
        // Trigger Player Events
        public static void TriggerPlayerLevelUp(int newLevel)
        {
            OnPlayerLevelUp?.Invoke(newLevel);
        }
        
        public static void TriggerCashChanged(int newAmount)
        {
            OnCashChanged?.Invoke(newAmount);
        }
        
        public static void TriggerSkillPointsChanged(int newAmount)
        {
            OnSkillPointsChanged?.Invoke(newAmount);
        }
        
        public static void TriggerPerkPointsChanged(int newAmount)
        {
            OnPerkPointsChanged?.Invoke(newAmount);
        }
        
        // Trigger Skill Events
        public static void TriggerSkillLevelUp(string skillName, int newLevel)
        {
            OnSkillLevelUp?.Invoke(skillName, newLevel);
        }
        
        public static void TriggerPerkUnlocked(string perkName)
        {
            OnPerkUnlocked?.Invoke(perkName);
        }
        
        // Trigger Time Events
        public static void TriggerNewDay(int dayNumber)
        {
            OnNewDay?.Invoke(dayNumber);
        }
        
        public static void TriggerTimeChanged(int hour, int minute)
        {
            OnTimeChanged?.Invoke(hour, minute);
        }
        
        // Trigger Appointment Events
        public static void TriggerAppointmentAccepted(AppointmentData appointment)
        {
            OnAppointmentAccepted?.Invoke(appointment);
        }
        
        public static void TriggerAppointmentCompleted(AppointmentData appointment)
        {
            OnAppointmentCompleted?.Invoke(appointment);
        }
        
        public static void TriggerAppointmentStarted(AppointmentData appointment)
        {
            OnAppointmentStarted?.Invoke(appointment);
        }
        
        // Trigger Mini-game Events
        public static void TriggerMiniGameCompleted(float performanceScore)
        {
            OnMiniGameCompleted?.Invoke(performanceScore);
        }
        
        public static void TriggerMiniGameStarted()
        {
            OnMiniGameStarted?.Invoke();
        }
        
        // Trigger UI Events
        public static void TriggerSmartphoneOpened()
        {
            OnSmartphoneOpened?.Invoke();
        }
        
        public static void TriggerSmartphoneClosed()
        {
            OnSmartphoneClosed?.Invoke();
        }
    }
}