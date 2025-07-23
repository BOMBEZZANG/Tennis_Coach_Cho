using UnityEngine;
using TennisCoachCho.Core;

namespace TennisCoachCho.World
{
    public class LocationTrigger : MonoBehaviour
    {
        [Header("Location Settings")]
        [SerializeField] private string locationName;
        [SerializeField] private bool canStartLessons = false;
        
        public string LocationName => locationName;
        public bool CanStartLessons => canStartLessons;
        
        public void OnPlayerEnter()
        {
            if (GameManager.Instance != null && GameManager.Instance.UIManager != null)
            {
                GameManager.Instance.UIManager.ShowLocationPrompt(locationName, canStartLessons);
            }
        }
        
        public void OnPlayerExit()
        {
            if (GameManager.Instance != null && GameManager.Instance.UIManager != null)
            {
                GameManager.Instance.UIManager.HideLocationPrompt();
            }
        }
        
        public void StartLesson()
        {
            if (!canStartLessons) return;
            
            var appointmentManager = GameManager.Instance?.AppointmentManager;
            if (appointmentManager != null)
            {
                appointmentManager.TryStartLessonAtLocation(locationName);
            }
        }
    }
}