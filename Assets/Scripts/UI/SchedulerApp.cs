using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TennisCoachCho.Core;
using TennisCoachCho.Data;

namespace TennisCoachCho.UI
{
    public class SchedulerApp : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Transform scheduleListParent;
        [SerializeField] private GameObject scheduleItemPrefab;
        [SerializeField] private Button backButton;
        [SerializeField] private Text noScheduleText;
        [SerializeField] private Text currentTimeText;
        
        private List<GameObject> scheduleItems = new List<GameObject>();
        
        public void Initialize()
        {
            if (backButton != null)
                backButton.onClick.AddListener(GoBack);
                
            RefreshSchedule();
        }
        
        public void RefreshSchedule()
        {
            UpdateCurrentTime();
            ClearScheduleList();
            PopulateScheduleList();
        }
        
        private void UpdateCurrentTime()
        {
            if (currentTimeText != null && GameManager.Instance?.TimeSystem != null)
            {
                var currentTime = GameManager.Instance.TimeSystem.CurrentTime;
                currentTimeText.text = $"Current Time: {currentTime.GetTimeString()} - {currentTime.GetDateString()}";
            }
        }
        
        private void ClearScheduleList()
        {
            foreach (var item in scheduleItems)
            {
                if (item != null)
                    Destroy(item);
            }
            scheduleItems.Clear();
        }
        
        private void PopulateScheduleList()
        {
            if (GameManager.Instance?.AppointmentManager == null) return;
            
            var acceptedAppointments = GameManager.Instance.AppointmentManager.AcceptedAppointments;
            
            if (acceptedAppointments.Count == 0)
            {
                if (noScheduleText != null)
                    noScheduleText.gameObject.SetActive(true);
                return;
            }
            
            if (noScheduleText != null)
                noScheduleText.gameObject.SetActive(false);
                
            // Sort appointments by time
            var sortedAppointments = new List<AppointmentData>(acceptedAppointments);
            sortedAppointments.Sort((a, b) => 
            {
                int timeA = a.scheduledHour * 60 + a.scheduledMinute;
                int timeB = b.scheduledHour * 60 + b.scheduledMinute;
                return timeA.CompareTo(timeB);
            });
            
            foreach (var appointment in sortedAppointments)
            {
                CreateScheduleItem(appointment);
            }
        }
        
        private void CreateScheduleItem(AppointmentData appointment)
        {
            if (scheduleItemPrefab == null || scheduleListParent == null) return;
            
            GameObject item = Instantiate(scheduleItemPrefab, scheduleListParent);
            scheduleItems.Add(item);
            
            // Setup item data
            var itemComponent = item.GetComponent<ScheduleItem>();
            if (itemComponent != null)
            {
                itemComponent.Setup(appointment);
            }
            else
            {
                // Fallback: setup with basic UI components
                SetupBasicScheduleItem(item, appointment);
            }
        }
        
        private void SetupBasicScheduleItem(GameObject item, AppointmentData appointment)
        {
            var texts = item.GetComponentsInChildren<Text>();
            
            if (texts.Length >= 4)
            {
                texts[0].text = appointment.clientName;
                texts[1].text = appointment.GetTimeString();
                texts[2].text = appointment.location;
                
                // Show status
                string status = appointment.isCompleted ? "Completed" : 
                               IsUpcoming(appointment) ? "Upcoming" : "Pending";
                texts[3].text = status;
                
                // Color code based on status
                Color statusColor = appointment.isCompleted ? Color.green :
                                   IsUpcoming(appointment) ? Color.yellow : Color.white;
                texts[3].color = statusColor;
            }
        }
        
        private bool IsUpcoming(AppointmentData appointment)
        {
            if (GameManager.Instance?.TimeSystem == null) return false;
            
            var currentTime = GameManager.Instance.TimeSystem.CurrentTime;
            int currentMinutes = currentTime.hour * 60 + currentTime.minute;
            int appointmentMinutes = appointment.scheduledHour * 60 + appointment.scheduledMinute;
            
            // Consider appointment "upcoming" if it's within the next 30 minutes
            return appointmentMinutes > currentMinutes && appointmentMinutes - currentMinutes <= 30;
        }
        
        private void GoBack()
        {
            var smartphoneUI = GetComponentInParent<SmartphoneUI>();
            if (smartphoneUI != null)
            {
                smartphoneUI.ShowAppMenu();
            }
        }
    }
    
    [System.Serializable]
    public class ScheduleItem : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Text clientNameText;
        [SerializeField] private Text timeText;
        [SerializeField] private Text locationText;
        [SerializeField] private Text statusText;
        [SerializeField] private Image backgroundImage;
        
        public void Setup(AppointmentData appointment)
        {
            if (clientNameText != null)
                clientNameText.text = appointment.clientName;
            if (timeText != null)
                timeText.text = appointment.GetTimeString();
            if (locationText != null)
                locationText.text = appointment.location;
                
            UpdateStatus(appointment);
        }
        
        private void UpdateStatus(AppointmentData appointment)
        {
            string status;
            Color backgroundColor = Color.white;
            Color textColor = Color.black;
            
            if (appointment.isCompleted)
            {
                status = "Completed";
                backgroundColor = new Color(0.8f, 1f, 0.8f); // Light green
                textColor = Color.green;
            }
            else if (IsUpcoming(appointment))
            {
                status = "Upcoming";
                backgroundColor = new Color(1f, 1f, 0.8f); // Light yellow
                textColor = new Color(1f, 0.5f, 0f); // Orange color
            }
            else
            {
                status = "Scheduled";
                backgroundColor = Color.white;
                textColor = Color.black;
            }
            
            if (statusText != null)
            {
                statusText.text = status;
                statusText.color = textColor;
            }
            
            if (backgroundImage != null)
                backgroundImage.color = backgroundColor;
        }
        
        private bool IsUpcoming(AppointmentData appointment)
        {
            if (GameManager.Instance?.TimeSystem == null) return false;
            
            var currentTime = GameManager.Instance.TimeSystem.CurrentTime;
            int currentMinutes = currentTime.hour * 60 + currentTime.minute;
            int appointmentMinutes = appointment.scheduledHour * 60 + appointment.scheduledMinute;
            
            return appointmentMinutes > currentMinutes && appointmentMinutes - currentMinutes <= 30;
        }
    }
}