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