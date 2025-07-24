using UnityEngine;

namespace TennisCoachCho.MiniGames
{
    [System.Serializable]
    public class PlayerPaddleSettings
    {
        [Header("Movement")]
        public float moveSpeed = 8f;
        public float movementBounds = 4f; // How far up/down player can move
        
        [Header("Hitting")]
        public float swingDuration = 0.3f;
        public float swingCooldown = 0.5f;
        
        [Header("Visual")]
        public Transform paddleSprite;
        public Animator paddleAnimator;
    }

    public class TennisPlayerPaddle : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private PlayerPaddleSettings settings;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // Game references
        private TennisDrillMiniGame gameManager;
        private Collider2D paddleCollider;
        
        // Input and movement
        private bool controlsEnabled = false;
        private float verticalInput;
        private Vector3 startPosition;
        
        // Hitting system
        private bool isSwinging = false;
        private float swingTimer = 0f;
        private float cooldownTimer = 0f;
        
        public bool IsSwinging => isSwinging;
        public bool ControlsEnabled => controlsEnabled;
        
        private void Awake()
        {
            paddleCollider = GetComponent<Collider2D>();
            if (paddleCollider == null)
            {
                Debug.LogError("[TennisPlayerPaddle] No Collider2D found on player paddle!");
            }
            
            startPosition = transform.position;
            ValidateComponents();
        }
        
        private void ValidateComponents()
        {
            if (settings.paddleSprite == null)
            {
                Debug.LogWarning("[TennisPlayerPaddle] Paddle sprite not assigned, using transform");
                settings.paddleSprite = transform;
            }
        }
        
        public void Initialize(TennisDrillMiniGame manager)
        {
            gameManager = manager;
            startPosition = transform.position;
            
            Debug.Log($"[TennisPlayerPaddle] Initialized at position: {startPosition}");
            Debug.Log($"[TennisPlayerPaddle] Movement bounds: ±{settings.movementBounds} (Y range: {startPosition.y - settings.movementBounds} to {startPosition.y + settings.movementBounds})");
        }
        
        private void Update()
        {
            if (!controlsEnabled) return;
            
            HandleInput();
            HandleMovement();
            HandleSwinging();
            
            if (showDebugInfo)
            {
                DebugDisplay();
            }
        }
        
        private void HandleInput()
        {
            // W/S movement input
            verticalInput = 0f;
            if (Input.GetKey(KeyCode.W))
                verticalInput = 1f;
            else if (Input.GetKey(KeyCode.S))
                verticalInput = -1f;
            
            // E swing input
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log($"[TennisPlayerPaddle] E key pressed - CanSwing: {CanSwing()}");
                if (CanSwing())
                {
                    StartSwing();
                }
            }
        }
        
        private void HandleMovement()
        {
            if (Mathf.Abs(verticalInput) > 0.1f)
            {
                Vector3 movement = Vector3.up * verticalInput * settings.moveSpeed * Time.deltaTime;
                Vector3 newPosition = transform.position + movement;
                
                // Clamp to movement bounds
                float clampedY = Mathf.Clamp(newPosition.y, 
                    startPosition.y - settings.movementBounds, 
                    startPosition.y + settings.movementBounds);
                
                newPosition.y = clampedY;
                transform.position = newPosition;
                
                // Moving paddle
            }
        }
        
        private void HandleSwinging()
        {
            // Update timers
            if (cooldownTimer > 0f)
                cooldownTimer -= Time.deltaTime;
            
            if (isSwinging)
            {
                swingTimer -= Time.deltaTime;
                if (swingTimer <= 0f)
                {
                    EndSwing();
                }
            }
        }
        
        private bool CanSwing()
        {
            return !isSwinging && cooldownTimer <= 0f;
        }
        
        private void StartSwing()
        {
            Debug.Log($"[TennisPlayerPaddle] Starting swing - position: {transform.position}");
            
            isSwinging = true;
            swingTimer = settings.swingDuration;
            cooldownTimer = settings.swingCooldown;
            
            // Play swing animation
            if (settings.paddleAnimator != null)
            {
                settings.paddleAnimator.SetTrigger("Swing");
            }
            
            // Visual feedback
            StartCoroutine(SwingVisualFeedback());
        }
        
        private void EndSwing()
        {
            // Ending swing
            isSwinging = false;
        }
        
        private System.Collections.IEnumerator SwingVisualFeedback()
        {
            // Simple visual feedback - scale up and down
            Vector3 originalScale = settings.paddleSprite.localScale;
            Vector3 swingScale = originalScale * 1.2f;
            
            float halfDuration = settings.swingDuration * 0.5f;
            
            // Scale up
            float elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                settings.paddleSprite.localScale = Vector3.Lerp(originalScale, swingScale, t);
                yield return null;
            }
            
            // Scale down
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                settings.paddleSprite.localScale = Vector3.Lerp(swingScale, originalScale, t);
                yield return null;
            }
            
            settings.paddleSprite.localScale = originalScale;
        }
        
        public void SetControlsEnabled(bool enabled)
        {
            controlsEnabled = enabled;
            
            if (!enabled)
            {
                // Stop any current swing
                isSwinging = false;
                swingTimer = 0f;
                verticalInput = 0f;
            }
            
            // Controls state changed
        }
        
        public void ResetPosition()
        {
            transform.position = startPosition;
            verticalInput = 0f;
            isSwinging = false;
            swingTimer = 0f;
            cooldownTimer = 0f;
        }
        
        private void DebugDisplay()
        {
            if (isSwinging)
            {
                Debug.DrawRay(transform.position, Vector3.right * 2f, Color.red);
            }
            
            // Draw movement bounds
            Debug.DrawLine(
                new Vector3(transform.position.x, startPosition.y + settings.movementBounds, 0f),
                new Vector3(transform.position.x, startPosition.y - settings.movementBounds, 0f),
                Color.yellow);
        }
        
        private void OnDrawGizmos()
        {
            if (!showDebugInfo) return;
            
            // Draw paddle position
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.8f);
            
            // Draw movement bounds
            if (Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Vector3 upperBound = new Vector3(transform.position.x, startPosition.y + settings.movementBounds, 0f);
                Vector3 lowerBound = new Vector3(transform.position.x, startPosition.y - settings.movementBounds, 0f);
                
                Gizmos.DrawLine(upperBound + Vector3.left * 0.5f, upperBound + Vector3.right * 0.5f);
                Gizmos.DrawLine(lowerBound + Vector3.left * 0.5f, lowerBound + Vector3.right * 0.5f);
            }
            
            // Draw swing indicator
            if (Application.isPlaying && isSwinging)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position + Vector3.right, 0.5f);
            }
        }
        
        // Input visualization for debugging
        private void OnGUI()
        {
            if (!showDebugInfo || !controlsEnabled) return;
            
            GUILayout.BeginArea(new Rect(10f, 200f, 200f, 100f));
            GUILayout.Label($"Player Paddle Debug:");
            GUILayout.Label($"Vertical Input: {verticalInput:F2}");
            GUILayout.Label($"Is Swinging: {isSwinging}");
            GUILayout.Label($"Cooldown: {cooldownTimer:F2}");
            GUILayout.EndArea();
        }
    }
}