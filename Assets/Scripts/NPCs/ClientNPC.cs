using UnityEngine;
using TennisCoachCho.Core;
using TennisCoachCho.Data;

namespace TennisCoachCho.NPCs
{
    public class ClientNPC : MonoBehaviour
    {
        [Header("Client Settings")]
        [SerializeField] private string clientName = "Sarah Johnson";
        [SerializeField] private AppointmentData assignedAppointment;
        [SerializeField] private DogNPC assignedDog;
        
        [Header("Visual Elements")]
        [SerializeField] private GameObject questMarker;
        [SerializeField] private SpriteRenderer clientRenderer;
        
        [Header("Interaction")]
        [SerializeField] private float interactionRange = 2f;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        
        private bool playerInRange = false;
        private bool handoverCompleted = false;
        private Transform playerTransform;
        
        public enum ClientState
        {
            WaitingForPlayer,
            ReadyForHandover,
            HandoverComplete,
            Idle
        }
        
        [SerializeField] private ClientState currentState = ClientState.Idle;
        
        private void Start()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            // Set placeholder appearance (Grey-box: simple colored square)
            if (clientRenderer == null)
            {
                clientRenderer = GetComponent<SpriteRenderer>();
                if (clientRenderer == null)
                {
                    clientRenderer = gameObject.AddComponent<SpriteRenderer>();
                }
            }
            
            SetupPlaceholderAppearance();
            
            // Find player
            var playerController = GameManager.Instance?.PlayerController;
            if (playerController != null)
            {
                playerTransform = playerController.transform;
            }
            
            // Initialize state
            UpdateClientState();
            
            Debug.Log("[ClientNPC] " + clientName + " initialized at position " + transform.position);
        }
        
        private void SetupPlaceholderAppearance()
        {
            // Grey-box implementation: Create simple colored geometric shape
            // Client = Green Square
            
            // Create a simple square sprite (placeholder)
            Texture2D texture = new Texture2D(32, 32);
            Color clientColor = Color.green;
            
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    texture.SetPixel(x, y, clientColor);
                }
            }
            texture.Apply();
            
            Sprite clientSprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            clientRenderer.sprite = clientSprite;
            
            Debug.Log("[ClientNPC] Placeholder appearance set: Green Square");
        }
        
        private void Update()
        {
            CheckPlayerDistance();
            HandleInput();
            UpdateQuestMarker();
        }
        
        private void CheckPlayerDistance()
        {
            if (playerTransform == null) return;
            
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            bool wasInRange = playerInRange;
            playerInRange = distance <= interactionRange;
            
            if (playerInRange != wasInRange)
            {
                if (playerInRange)
                {
                    Debug.Log("[ClientNPC] Player entered interaction range of " + clientName);
                }
                else
                {
                    Debug.Log("[ClientNPC] Player left interaction range of " + clientName);
                }
            }
        }
        
        private void HandleInput()
        {
            if (!playerInRange || handoverCompleted) return;
            
            if (Input.GetKeyDown(interactKey))
            {
                if (CanStartHandover())
                {
                    StartHandover();
                }
                else
                {
                    ShowCannotInteractMessage();
                }
            }
        }
        
        private bool CanStartHandover()
        {
            if (assignedAppointment == null)
            {
                Debug.LogWarning("[ClientNPC] " + clientName + " has no assigned appointment!");
                return false;
            }
            
            if (HandoverManager.Instance == null)
            {
                Debug.LogWarning("[ClientNPC] No HandoverManager found!");
                return false;
            }
            
            return HandoverManager.Instance.CanStartHandover(assignedAppointment);
        }
        
        private void StartHandover()
        {
            Debug.Log("[ClientNPC] " + clientName + " starting handover sequence...");
            
            if (HandoverManager.Instance != null && assignedDog != null)
            {
                HandoverManager.Instance.StartHandover(this, assignedDog, assignedAppointment);
            }
            else
            {
                Debug.LogError("[ClientNPC] Missing components for handover! HandoverManager: " + (HandoverManager.Instance != null) + ", Dog: " + (assignedDog != null));
            }
        }
        
        private void ShowCannotInteractMessage()
        {
            // Show why player can't interact (wrong time, no appointment, etc.)
            var currentTime = GameManager.Instance?.TimeSystem?.CurrentTime;
            if (currentTime != null && assignedAppointment != null)
            {
                string timeMessage = "Come back at " + assignedAppointment.GetTimeString() + " for your appointment!";
                Debug.Log("[ClientNPC] " + timeMessage);
                
                // TODO: Show UI message to player
                if (GameManager.Instance?.UIManager != null)
                {
                    GameManager.Instance.UIManager.ShowTemporaryMessage(timeMessage);
                }
            }
        }
        
        public void CompleteHandover()
        {
            handoverCompleted = true;
            currentState = ClientState.HandoverComplete;
            
            Debug.Log("[ClientNPC] " + clientName + " handover completed. Returning to idle state.");
            
            // Hide quest marker
            if (questMarker != null)
            {
                questMarker.SetActive(false);
            }
        }
        
        private void UpdateClientState()
        {
            if (assignedAppointment == null || !assignedAppointment.isAccepted)
            {
                currentState = ClientState.Idle;
                return;
            }
            
            if (handoverCompleted)
            {
                currentState = ClientState.HandoverComplete;
                return;
            }
            
            var currentTime = GameManager.Instance?.TimeSystem?.CurrentTime;
            if (currentTime == null)
            {
                currentState = ClientState.WaitingForPlayer;
                return;
            }
            
            // Check if within appointment window
            int timeDifference = Mathf.Abs((currentTime.Value.hour * 60 + currentTime.Value.minute) - 
                                         (assignedAppointment.scheduledHour * 60 + assignedAppointment.scheduledMinute));
            
            if (timeDifference <= 30) // Within 30 minutes
            {
                currentState = ClientState.ReadyForHandover;
            }
            else
            {
                currentState = ClientState.WaitingForPlayer;
            }
        }
        
        private void UpdateQuestMarker()
        {
            if (questMarker == null) return;
            
            bool shouldShowMarker = (currentState == ClientState.ReadyForHandover && !handoverCompleted);
            questMarker.SetActive(shouldShowMarker);
        }
        
        public void SetAppointment(AppointmentData appointment)
        {
            assignedAppointment = appointment;
            Debug.Log("[ClientNPC] " + clientName + " assigned appointment: " + appointment.GetTimeString());
            UpdateClientState();
        }
        
        public void SetAssignedDog(DogNPC dog)
        {
            assignedDog = dog;
            Debug.Log("[ClientNPC] " + clientName + " assigned dog: " + (dog != null ? dog.name : "null"));
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw interaction range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}