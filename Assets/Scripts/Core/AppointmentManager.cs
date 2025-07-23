using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TennisCoachCho.Data;

namespace TennisCoachCho.Core
{
    public class AppointmentManager : MonoBehaviour
    {
        [Header("Appointment Settings")]
        [SerializeField] private GameData gameData;
        [SerializeField] private int maxDailyAppointments = 3;
        
        private List<AppointmentData> todaysAppointments;
        
        public List<AppointmentData> AvailableAppointments => gameData.availableAppointments;
        public List<AppointmentData> AcceptedAppointments => gameData.acceptedAppointments;
        
        public void Initialize()
        {
            if (gameData == null)
            {
                Debug.LogError("GameData not assigned to AppointmentManager!");
                return;
            }
            
            GenerateDailyAppointments();
            
            // Subscribe to time system events
            if (GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.OnNewDay += OnNewDay;
                GameManager.Instance.TimeSystem.OnTimeChanged += CheckAppointmentTimes;
            }
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.OnNewDay -= OnNewDay;
                GameManager.Instance.TimeSystem.OnTimeChanged -= CheckAppointmentTimes;
            }
        }
        
        private void OnNewDay(int day)
        {
            GenerateDailyAppointments();
            ClearCompletedAppointments();
        }
        
        private void GenerateDailyAppointments()
        {
            gameData.availableAppointments.Clear();
            
            string[] clientNames = { "Sarah", "Mike", "Emma", "David", "Lisa", "John", "Anna", "Mark" };
            string[] locations = { "Tennis Court", "Tennis Court" }; // Only tennis court for now
            
            for (int i = 0; i < maxDailyAppointments; i++)
            {
                string client = clientNames[Random.Range(0, clientNames.Length)];
                int hour = Random.Range(9, 20); // 9 AM to 7 PM
                int minute = Random.Range(0, 2) * 30; // 0 or 30 minutes
                string location = locations[Random.Range(0, locations.Length)];
                int cashReward = Random.Range(20, 50);
                int expReward = Random.Range(15, 30);
                
                AppointmentData appointment = new AppointmentData(client, hour, minute, location, cashReward, expReward);
                gameData.availableAppointments.Add(appointment);
            }
        }
        
        public bool AcceptAppointment(AppointmentData appointment)
        {
            if (!gameData.availableAppointments.Contains(appointment)) return false;
            
            appointment.isAccepted = true;
            gameData.availableAppointments.Remove(appointment);
            gameData.acceptedAppointments.Add(appointment);
            
            return true;
        }
        
        private void CheckAppointmentTimes(GameDateTime currentTime)
        {
            foreach (var appointment in gameData.acceptedAppointments.ToList())
            {
                if (!appointment.isCompleted && 
                    appointment.scheduledHour == currentTime.hour && 
                    appointment.scheduledMinute == currentTime.minute)
                {
                    NotifyAppointmentTime(appointment);
                }
            }
        }
        
        private void NotifyAppointmentTime(AppointmentData appointment)
        {
            if (GameManager.Instance?.UIManager != null)
            {
                GameManager.Instance.UIManager.ShowAppointmentNotification(appointment);
            }
        }
        
        public bool TryStartLessonAtLocation(string locationName)
        {
            var currentAppointment = GetCurrentAppointmentAtLocation(locationName);
            if (currentAppointment != null)
            {
                StartLesson(currentAppointment);
                return true;
            }
            return false;
        }
        
        private AppointmentData GetCurrentAppointmentAtLocation(string locationName)
        {
            var currentTime = GameManager.Instance?.TimeSystem?.CurrentTime;
            if (currentTime == null) return null;
            
            return gameData.acceptedAppointments.FirstOrDefault(appointment =>
                !appointment.isCompleted &&
                appointment.location.Equals(locationName, System.StringComparison.OrdinalIgnoreCase) &&
                IsAppointmentTimeNow(appointment, currentTime.Value));
        }
        
        private bool IsAppointmentTimeNow(AppointmentData appointment, GameDateTime currentTime)
        {
            // Allow starting lesson within 30 minutes of scheduled time
            int timeDifference = Mathf.Abs((currentTime.hour * 60 + currentTime.minute) - 
                                         (appointment.scheduledHour * 60 + appointment.scheduledMinute));
            return timeDifference <= 30;
        }
        
        private void StartLesson(AppointmentData appointment)
        {
            if (GameManager.Instance?.UIManager != null)
            {
                GameManager.Instance.UIManager.StartRhythmMiniGame(appointment);
            }
        }
        
        public void CompleteLesson(AppointmentData appointment, float performanceScore)
        {
            appointment.isCompleted = true;
            
            // Calculate rewards based on performance
            int cashEarned = Mathf.RoundToInt(appointment.cashReward * performanceScore);
            int expEarned = Mathf.RoundToInt(appointment.expReward * performanceScore);
            
            // Apply friendliness skill bonus to EXP
            var friendlinessLevel = GetSkillLevel("Friendliness");
            expEarned = Mathf.RoundToInt(expEarned * (1f + friendlinessLevel * 0.1f));
            
            // Award rewards
            GameManager.Instance?.ProgressionManager?.AddCash(cashEarned);
            GameManager.Instance?.ProgressionManager?.AddCoachingExp(expEarned);
            
            // Show completion feedback
            GameManager.Instance?.UIManager?.ShowLessonComplete(cashEarned, expEarned, performanceScore);
        }
        
        private int GetSkillLevel(string skillName)
        {
            var skillNode = gameData.coachingSkillTree.FirstOrDefault(node => 
                node.nodeName.Equals(skillName, System.StringComparison.OrdinalIgnoreCase));
            return skillNode?.level ?? 0;
        }
        
        private void ClearCompletedAppointments()
        {
            gameData.acceptedAppointments.RemoveAll(appointment => appointment.isCompleted);
        }
    }
}