using UnityEngine;
using UnityEngine.UI;
using TennisCoachCho.World;
using TennisCoachCho.Core;

namespace TennisCoachCho.UI
{
    public class LocationPrompt : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Text locationNameText;
        [SerializeField] private Button startLessonButton;
        [SerializeField] private Button closeButton;
        
        private string currentLocationName;
        private bool canStartLesson;
        
        private void Awake()
        {
            if (startLessonButton != null)
                startLessonButton.onClick.AddListener(StartLesson);
            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);
                
            gameObject.SetActive(false);
        }
        
        public void Show(string locationName, bool canStart)
        {
            currentLocationName = locationName;
            canStartLesson = canStart;
            
            if (locationNameText != null)
                locationNameText.text = $"You are at: {locationName}";
                
            if (startLessonButton != null)
            {
                startLessonButton.gameObject.SetActive(canStart);
                
                // Check if there's actually a lesson to start
                if (canStart)
                {
                    bool hasAppointmentHere = CheckForAppointmentAtLocation(locationName);
                    startLessonButton.interactable = hasAppointmentHere;
                    
                    var buttonText = startLessonButton.GetComponentInChildren<Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = hasAppointmentHere ? "Start Lesson" : "No Scheduled Lesson";
                    }
                }
            }
            
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void StartLesson()
        {
            // Find the location trigger and start lesson
            var locationTriggers = FindObjectsOfType<LocationTrigger>();
            foreach (var trigger in locationTriggers)
            {
                if (trigger.LocationName.Equals(currentLocationName, System.StringComparison.OrdinalIgnoreCase))
                {
                    trigger.StartLesson();
                    Hide();
                    return;
                }
            }
        }
        
        private bool CheckForAppointmentAtLocation(string locationName)
        {
            if (GameManager.Instance?.AppointmentManager == null) return false;
            
            var acceptedAppointments = GameManager.Instance.AppointmentManager.AcceptedAppointments;
            var currentTime = GameManager.Instance.TimeSystem?.CurrentTime;
            
            if (currentTime == null) return false;
            
            foreach (var appointment in acceptedAppointments)
            {
                if (!appointment.isCompleted && 
                    appointment.location.Equals(locationName, System.StringComparison.OrdinalIgnoreCase))
                {
                    // Check if appointment time is close (within 30 minutes)
                    int currentMinutes = currentTime.Value.hour * 60 + currentTime.Value.minute;
                    int appointmentMinutes = appointment.scheduledHour * 60 + appointment.scheduledMinute;
                    int timeDifference = Mathf.Abs(currentMinutes - appointmentMinutes);
                    
                    if (timeDifference <= 30)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
    }
}