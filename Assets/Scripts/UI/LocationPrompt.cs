using UnityEngine;
using UnityEngine.UI;
using TennisCoachCho.World;
using TennisCoachCho.Core;
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
                    bool hasAppointmentHere = CheckForAppointmentAtLocation(locationName);
                    startLessonButton.interactable = hasAppointmentHere;
                    Debug.Log($"[LocationPrompt] Has appointment here: {hasAppointmentHere}");

                    var buttonText = startLessonButton.GetComponentInChildren<Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = hasAppointmentHere ? "Start Lesson" : "No Scheduled Lesson";
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