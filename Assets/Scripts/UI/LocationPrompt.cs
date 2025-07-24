using UnityEngine;
using UnityEngine.UI;
using TennisCoachCho.World;
using TennisCoachCho.Core;
using TennisCoachCho.Data;
using TMPro;
using System.Collections;


namespace TennisCoachCho.UI
{
    public class LocationPrompt : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI locationNameText;
        [SerializeField] private Button startLessonButton;
        [SerializeField] private Button closeButton;

        private string currentLocationName;
        private bool canStartLesson;
        private bool isActivatedByShow = false;
        
        [System.Serializable]
        private struct AppointmentStatus
        {
            public bool canStart;
            public string buttonText;
            
            public AppointmentStatus(bool canStart, string buttonText)
            {
                this.canStart = canStart;
                this.buttonText = buttonText;
            }
        }

        private void Awake()
        {
            Debug.Log("[LocationPrompt] Awake() called - setting up buttons");
            
            if (startLessonButton != null)
                startLessonButton.onClick.AddListener(StartLesson);
            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);
        }
        
        private void Start()
        {
            Debug.Log("[LocationPrompt] Start() called - hiding LocationPrompt");
            Debug.Log($"[LocationPrompt] isActivatedByShow: {isActivatedByShow}");
            
            // Only hide if this is NOT activated by the Show() method
            if (isActivatedByShow)
            {
                Debug.Log("[LocationPrompt] Start() called during gameplay - NOT hiding (activated by Show() method)");
                return;
            }
            
            Debug.Log("[LocationPrompt] Initial game start - hiding LocationPrompt");
            gameObject.SetActive(false);
        }

        public void Show(string locationName, bool canStart)
        {
            Debug.Log($"[LocationPrompt] Show called with location: {locationName}, canStart: {canStart}");
            Debug.Log($"[LocationPrompt] GameObject active before: {gameObject.activeInHierarchy}");
            
            // Set flag to indicate this activation is from Show() method
            isActivatedByShow = true;
            
            currentLocationName = locationName;
            canStartLesson = canStart;

            if (locationNameText != null)
            {
                locationNameText.text = $"You are at: {locationName}";
                Debug.Log($"[LocationPrompt] Set location text to: {locationNameText.text}");
            }
            else
            {
                Debug.LogError("[LocationPrompt] locationNameText is null!");
            }

            if (startLessonButton != null)
            {
                startLessonButton.gameObject.SetActive(canStart);
                Debug.Log($"[LocationPrompt] Set startLessonButton active: {canStart}");

                // Check if there's actually a lesson to start
                if (canStart)
                {
                    var appointmentStatus = GetAppointmentStatus(locationName);
                    startLessonButton.interactable = appointmentStatus.canStart;
                    Debug.Log($"[LocationPrompt] Can start lesson: {appointmentStatus.canStart}");

                    var buttonText = startLessonButton.GetComponentInChildren<Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = appointmentStatus.buttonText;
                        Debug.Log($"[LocationPrompt] Set button text to: {buttonText.text}");
                    }
                }
            }
            else
            {
                Debug.LogError("[LocationPrompt] startLessonButton is null!");
            }

            Debug.Log("[LocationPrompt] About to call SetActive(true)...");
            gameObject.SetActive(true);
            Debug.Log($"[LocationPrompt] Immediately after SetActive(true) - activeSelf: {gameObject.activeSelf}");
            Debug.Log($"[LocationPrompt] GameObject active after: {gameObject.activeInHierarchy}");
            Debug.Log($"[LocationPrompt] GameObject activeSelf after: {gameObject.activeSelf}");
            
            // List all components on this GameObject
            var components = gameObject.GetComponents<Component>();
            Debug.Log($"[LocationPrompt] Components on LocationPrompt: {string.Join(", ", System.Array.ConvertAll(components, c => c.GetType().Name))}");
            
            // Check if any component might be disabling it
            var canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                Debug.Log($"[LocationPrompt] CanvasGroup found - alpha: {canvasGroup.alpha}, interactable: {canvasGroup.interactable}, blocksRaycasts: {canvasGroup.blocksRaycasts}");
            }
            
            // Debug parent hierarchy
            Transform current = transform;
            int level = 0;
            while (current != null && level < 5)
            {
                Debug.Log($"[LocationPrompt] Parent Level {level}: {current.name} | Active: {current.gameObject.activeInHierarchy} | ActiveSelf: {current.gameObject.activeSelf}");
                current = current.parent;
                level++;
            }
            
            // Force check again after a frame delay
            StartCoroutine(CheckAfterFrame());
            
            Debug.Log($"[LocationPrompt] LocationPrompt should now be visible!");
        }

        public void Hide()
        {
            Debug.Log("[LocationPrompt] Hide called");
            Debug.Log($"[LocationPrompt] GameObject active before hide: {gameObject.activeInHierarchy}");
            
            // Reset the flag when hiding
            isActivatedByShow = false;
            
            gameObject.SetActive(false);
            Debug.Log("[LocationPrompt] LocationPrompt hidden");
        }

        private void StartLesson()
        {
            Debug.Log($"[LocationPrompt] StartLesson button clicked for location: {currentLocationName}");
            
            // Double-check if lesson can actually be started
            var appointmentStatus = GetAppointmentStatus(currentLocationName);
            if (!appointmentStatus.canStart)
            {
                Debug.LogWarning($"[LocationPrompt] ⚠️ Lesson cannot be started: {appointmentStatus.buttonText}");
                return;
            }
            
            Debug.Log($"[LocationPrompt] ✅ Lesson validated - proceeding to start lesson");
            
            // Find the location trigger and start lesson
            var locationTriggers = FindObjectsOfType<LocationTrigger>();
            foreach (var trigger in locationTriggers)
            {
                if (trigger.LocationName.Equals(currentLocationName, System.StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"[LocationPrompt] Found matching LocationTrigger: {trigger.LocationName}");
                    Debug.Log($"[LocationPrompt] Starting lesson via LocationTrigger...");
                    
                    trigger.StartLesson();
                    Hide();
                    return;
                }
            }
            
            Debug.LogError($"[LocationPrompt] ❌ No LocationTrigger found for location: {currentLocationName}");
        }

        private AppointmentStatus GetAppointmentStatus(string locationName)
        {
            if (GameManager.Instance?.AppointmentManager == null) 
                return new AppointmentStatus(false, "No Appointment Manager");

            var acceptedAppointments = GameManager.Instance.AppointmentManager.AcceptedAppointments;
            var currentTime = GameManager.Instance.TimeSystem?.CurrentTime;

            if (currentTime == null) 
                return new AppointmentStatus(false, "No Time System");

            Debug.Log($"[LocationPrompt] Checking appointments for location: {locationName}");
            Debug.Log($"[LocationPrompt] Total accepted appointments: {acceptedAppointments?.Count ?? 0}");
            
            if (acceptedAppointments != null && acceptedAppointments.Count > 0)
            {
                for (int i = 0; i < acceptedAppointments.Count; i++)
                {
                    var apt = acceptedAppointments[i];
                    Debug.Log($"[LocationPrompt] Appointment {i}: Client={apt.clientName}, Location='{apt.location}', Time={apt.scheduledHour:D2}:{apt.scheduledMinute:D2}, Completed={apt.isCompleted}");
                }
            }
            else
            {
                Debug.Log("[LocationPrompt] ⚠️ No accepted appointments found in the system!");
                Debug.Log("[LocationPrompt] You need to create and accept appointments first.");
            }

            AppointmentData nextValidAppointment = null;
            int currentMinutes = currentTime.Value.hour * 60 + currentTime.Value.minute;
            
            // Find the next valid appointment (not completed and at this location)
            foreach (var appointment in acceptedAppointments)
            {
                if (!appointment.isCompleted &&
                    appointment.location.Equals(locationName, System.StringComparison.OrdinalIgnoreCase))
                {
                    int appointmentMinutes = appointment.scheduledHour * 60 + appointment.scheduledMinute;
                    int timeDifference = currentMinutes - appointmentMinutes;
                    
                    Debug.Log($"[LocationPrompt] Checking appointment - Client: {appointment.clientName}");
                    Debug.Log($"[LocationPrompt] Appointment time: {appointment.scheduledHour:D2}:{appointment.scheduledMinute:D2} ({appointmentMinutes} min)");
                    Debug.Log($"[LocationPrompt] Time difference: {timeDifference} minutes");
                    
                    // If this appointment is still valid (not passed by more than 10 minutes)
                    if (timeDifference <= 0)
                    {
                        // If no valid appointment found yet, or this one is earlier than the current candidate
                        if (nextValidAppointment == null || appointmentMinutes < (nextValidAppointment.scheduledHour * 60 + nextValidAppointment.scheduledMinute))
                        {
                            nextValidAppointment = appointment;
                            Debug.Log($"[LocationPrompt] Setting as next valid appointment: {appointment.clientName}");
                        }
                    }
                    else
                    {
                        Debug.Log($"[LocationPrompt] Appointment has passed: {appointment.clientName}");
                    }
                }
            }
            
            if (nextValidAppointment != null)
            {
                int appointmentMinutes = nextValidAppointment.scheduledHour * 60 + nextValidAppointment.scheduledMinute;
                int timeDifference = currentMinutes - appointmentMinutes;
                
                Debug.Log($"[LocationPrompt] Next valid appointment found - Client: {nextValidAppointment.clientName}");
                Debug.Log($"[LocationPrompt] Current time: {currentTime.Value.GetTimeString()} ({currentMinutes} min)");
                Debug.Log($"[LocationPrompt] Appointment time: {nextValidAppointment.scheduledHour:D2}:{nextValidAppointment.scheduledMinute:D2} ({appointmentMinutes} min)");
                Debug.Log($"[LocationPrompt] Time difference: {timeDifference} minutes (negative = early, positive = late)");
                
                // Player can start lesson if they arrive 10 minutes early to on-time
                if (timeDifference >= -10 && timeDifference <= 0)
                {
                    Debug.Log($"[LocationPrompt] ✅ Lesson can be started! Player is within 10-minute window");
                    return new AppointmentStatus(true, "Start Lesson");
                }
                else if (timeDifference < -10)
                {
                    int minutesUntilStart = -timeDifference - 10;
                    Debug.Log($"[LocationPrompt] ⏰ Too early - {minutesUntilStart} minutes until lesson can start");
                    return new AppointmentStatus(false, $"Too Early (Wait {minutesUntilStart}m)");
                }
                else // timeDifference > 0
                {
                    Debug.Log($"[LocationPrompt] ❌ Too late - lesson time has passed");
                    return new AppointmentStatus(false, "Lesson Time Passed");
                }
            }

            Debug.Log($"[LocationPrompt] No appointment found at {locationName}");
            return new AppointmentStatus(false, "No Scheduled Lesson");
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
                    // Check if player is within the lesson start window (10 minutes early to on-time)
                    int currentMinutes = currentTime.Value.hour * 60 + currentTime.Value.minute;
                    int appointmentMinutes = appointment.scheduledHour * 60 + appointment.scheduledMinute;
                    
                    // Calculate time difference (positive means current time is after appointment time)
                    int timeDifference = currentMinutes - appointmentMinutes;
                    
                    // Player can start lesson if they arrive 10 minutes early (timeDifference = -10) to on-time (timeDifference = 0)
                    bool canStartLesson = timeDifference >= -10 && timeDifference <= 0;
                    
                    Debug.Log($"[LocationPrompt] Appointment check - Location: {locationName}");
                    Debug.Log($"[LocationPrompt] Current time: {currentTime.Value.GetTimeString()} ({currentMinutes} min)");
                    Debug.Log($"[LocationPrompt] Appointment time: {appointment.scheduledHour:D2}:{appointment.scheduledMinute:D2} ({appointmentMinutes} min)");
                    Debug.Log($"[LocationPrompt] Time difference: {timeDifference} minutes (negative = early, positive = late)");
                    Debug.Log($"[LocationPrompt] Can start lesson: {canStartLesson} (must be between -10 and 0)");
                    
                    if (canStartLesson)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        private IEnumerator CheckAfterFrame()
        {
            yield return new WaitForEndOfFrame();
            Debug.Log("[LocationPrompt] AFTER FRAME CHECK:");
            Debug.Log($"[LocationPrompt] activeSelf: {gameObject.activeSelf}");
            Debug.Log($"[LocationPrompt] activeInHierarchy: {gameObject.activeInHierarchy}");
            
            if (!gameObject.activeSelf)
            {
                Debug.LogError("[LocationPrompt] ❌ GameObject was disabled after SetActive(true)! Something is turning it off!");
                Debug.LogError("[LocationPrompt] Check for other scripts that might be calling SetActive(false)");
            }
        }
    }
}