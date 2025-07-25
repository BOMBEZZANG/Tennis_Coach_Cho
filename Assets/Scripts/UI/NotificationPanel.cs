using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TennisCoachCho.Data;

namespace TennisCoachCho.UI
{
    public class NotificationPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Text notificationText;
        [SerializeField] private Button dismissButton;
        [SerializeField] private float autoHideDelay = 5f;
        
        private Coroutine autoHideCoroutine;
        
        private void Awake()
        {
            if (dismissButton != null)
                dismissButton.onClick.AddListener(Hide);
                
            gameObject.SetActive(false);
        }
        
        public void ShowAppointmentNotification(AppointmentData appointment)
        {
            if (notificationText != null)
            {
                notificationText.text = $"Appointment with {appointment.clientName} is starting now!\n" +
                                      $"Location: {appointment.location}\n" +
                                      $"Go there to begin the lesson.";
            }
            
            Show();
        }
        
        public void ShowGenericNotification(string message)
        {
            if (notificationText != null)
                notificationText.text = message;
                
            Show();
        }
        
        public void ShowQuestUpdate(string questText)
        {
            if (notificationText != null)
                notificationText.text = questText;
                
            Show();
        }
        
        public void ShowTemporaryMessage(string message, float duration)
        {
            if (notificationText != null)
                notificationText.text = message;
                
            // Override auto-hide delay for custom duration
            float originalDelay = autoHideDelay;
            autoHideDelay = duration;
            
            Show();
            
            // Restore original delay after showing
            StartCoroutine(RestoreAutoHideDelay(originalDelay));
        }
        
        private IEnumerator RestoreAutoHideDelay(float originalDelay)
        {
            yield return new WaitForSeconds(0.1f); // Small delay to ensure Show() has started
            autoHideDelay = originalDelay;
        }
        
        private void Show()
        {
            gameObject.SetActive(true);
            
            // Cancel previous auto-hide if running
            if (autoHideCoroutine != null)
            {
                StopCoroutine(autoHideCoroutine);
            }
            
            // Start auto-hide timer
            autoHideCoroutine = StartCoroutine(AutoHide());
        }
        
        public void Hide()
        {
            if (autoHideCoroutine != null)
            {
                StopCoroutine(autoHideCoroutine);
                autoHideCoroutine = null;
            }
            
            gameObject.SetActive(false);
        }
        
        private IEnumerator AutoHide()
        {
            yield return new WaitForSeconds(autoHideDelay);
            Hide();
        }
    }
}