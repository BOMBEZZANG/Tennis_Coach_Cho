using UnityEngine;
using TennisCoachCho.Core;

namespace TennisCoachCho.World
{
    public class LocationTrigger : MonoBehaviour
    {
        [Header("Location Settings")]
        [SerializeField] private string locationName;
        [SerializeField] private bool canStartLessons = false;
        
        [Header("Debug Info")]
        [SerializeField] private bool debugMode = true;
        
        public string LocationName => locationName;
        public bool CanStartLessons => canStartLessons;
        
        private BoxCollider2D boxCollider;
        
        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            if (boxCollider == null)
            {
                Debug.LogError($"[LocationTrigger] No BoxCollider2D found on {gameObject.name}!");
                return;
            }
            
            EnsureTriggerEnabled();
        }
        
        private void Start()
        {
            EnsureTriggerEnabled();
        }
        
        private void Update()
        {
            // Continuously ensure trigger stays enabled (for debugging)
            if (debugMode)
            {
                EnsureTriggerEnabled();
            }
        }
        
        private void EnsureTriggerEnabled()
        {
            if (boxCollider != null && !boxCollider.isTrigger)
            {
                Debug.LogWarning($"[LocationTrigger] ⚠️ BoxCollider2D on {gameObject.name} was not a trigger! Fixing...");
                boxCollider.isTrigger = true;
                Debug.Log($"[LocationTrigger] ✅ BoxCollider2D.isTrigger set to true on {gameObject.name}");
            }
        }
        
        public void OnPlayerEnter()
        {
            Debug.Log($"[LocationTrigger] OnPlayerEnter called for location: {locationName}");
            Debug.Log($"[LocationTrigger] GameManager.Instance is null: {GameManager.Instance == null}");
            
            if (GameManager.Instance != null)
            {
                Debug.Log($"[LocationTrigger] UIManager is null: {GameManager.Instance.UIManager == null}");
                
                if (GameManager.Instance.UIManager != null)
                {
                    Debug.Log($"[LocationTrigger] Calling ShowLocationPrompt with location: {locationName}, canStart: {canStartLessons}");
                    GameManager.Instance.UIManager.ShowLocationPrompt(locationName, canStartLessons);
                }
                else
                {
                    Debug.LogError("[LocationTrigger] UIManager is null!");
                }
            }
            else
            {
                Debug.LogError("[LocationTrigger] GameManager.Instance is null!");
            }
        }
        
        public void OnPlayerExit()
        {
            Debug.Log($"[LocationTrigger] OnPlayerExit called for location: {locationName}");
            
            if (GameManager.Instance != null && GameManager.Instance.UIManager != null)
            {
                Debug.Log("[LocationTrigger] Calling HideLocationPrompt");
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