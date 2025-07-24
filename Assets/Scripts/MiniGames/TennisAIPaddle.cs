using UnityEngine;

namespace TennisCoachCho.MiniGames
{
    [System.Serializable]
    public class AIPaddleSettings
    {
        [Header("AI Behavior")]
        public float reactionDelay = 0.2f;
        public float maxMoveSpeed = 6f;
        public float trackingAccuracy = 0.8f; // 0-1, how accurately AI tracks ball
        
        [Header("Movement")]
        public float movementBounds = 3f;
        public float smoothingSpeed = 5f;
        
        [Header("Hitting")]
        public float hitReactionTime = 0.3f;
        public float hitRadius = 1.5f;
        
        [Header("Visual")]
        public Transform paddleSprite;
        public Animator paddleAnimator;
    }

    public class TennisAIPaddle : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private AIPaddleSettings settings;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // Game references
        private TennisDrillMiniGame gameManager;
        private TennisBall targetBall;
        private Collider2D paddleCollider;
        
        // AI state
        private bool isEnabled = true;
        private Vector3 startPosition;
        private Vector3 targetPosition;
        private float reactionTimer = 0f;
        private bool isTrackingBall = false;
        
        // Hitting
        private bool canHit = true;
        private float hitCooldown = 0f;
        
        public bool IsEnabled => isEnabled;
        
        private void Awake()
        {
            paddleCollider = GetComponent<Collider2D>();
            if (paddleCollider == null)
            {
                Debug.LogError("[TennisAIPaddle] No Collider2D found on AI paddle!");
            }
            
            startPosition = transform.position;
            targetPosition = startPosition;
            
            ValidateComponents();
        }
        
        private void ValidateComponents()
        {
            if (settings.paddleSprite == null)
            {
                Debug.LogWarning("[TennisAIPaddle] Paddle sprite not assigned, using transform");
                settings.paddleSprite = transform;
            }
        }
        
        public void Initialize(TennisDrillMiniGame manager)
        {
            gameManager = manager;
            startPosition = transform.position;
            targetPosition = startPosition;
            
            // Find the ball
            targetBall = FindObjectOfType<TennisBall>();
            if (targetBall == null)
            {
                Debug.LogError("[TennisAIPaddle] Could not find TennisBall!");
            }
            
            // AI paddle initialized
        }
        
        private void Update()
        {
            if (!isEnabled) return;
            
            UpdateAI();
            HandleMovement();
            HandleHitting();
            
            if (showDebugInfo)
            {
                DebugDisplay();
            }
        }
        
        private void UpdateAI()
        {
            if (targetBall == null || !targetBall.IsMoving) return;
            
            // Check if ball is moving toward AI (positive X velocity)
            bool ballComingToward = targetBall.Velocity2D.x > 0f;
            
            if (ballComingToward && !isTrackingBall)
            {
                StartTrackingBall();
            }
            else if (!ballComingToward && isTrackingBall)
            {
                StopTrackingBall();
            }
            
            if (isTrackingBall)
            {
                UpdateBallTracking();
            }
        }
        
        private void StartTrackingBall()
        {
            isTrackingBall = true;
            reactionTimer = settings.reactionDelay;
            
            // Started tracking ball
        }
        
        private void StopTrackingBall()
        {
            isTrackingBall = false;
            
            // Return to center position
            targetPosition = startPosition;
            
            // Stopped tracking ball
        }
        
        private void UpdateBallTracking()
        {
            // Update reaction timer
            if (reactionTimer > 0f)
            {
                reactionTimer -= Time.deltaTime;
                return; // Still in reaction delay
            }
            
            // Calculate target position based on ball position
            Vector3 ballPosition = targetBall.transform.position;
            
            // Add some inaccuracy to make AI beatable
            float inaccuracy = (1f - settings.trackingAccuracy) * 2f; // -1 to 1 range
            float yOffset = Random.Range(-inaccuracy, inaccuracy);
            
            float targetY = ballPosition.y + yOffset;
            
            // Clamp to movement bounds
            targetY = Mathf.Clamp(targetY, 
                startPosition.y - settings.movementBounds,
                startPosition.y + settings.movementBounds);
            
            targetPosition = new Vector3(transform.position.x, targetY, transform.position.z);
        }
        
        private void HandleMovement()
        {
            // Smooth movement toward target position
            float maxDistance = settings.maxMoveSpeed * Time.deltaTime;
            Vector3 direction = (targetPosition - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPosition);
            
            if (distance > 0.1f)
            {
                Vector3 movement = direction * Mathf.Min(maxDistance, distance);
                transform.position += movement;
                
                // Moving toward target
            }
        }
        
        private void HandleHitting()
        {
            if (hitCooldown > 0f)
            {
                hitCooldown -= Time.deltaTime;
            }
            
            if (!canHit || hitCooldown > 0f || targetBall == null) return;
            
            // Check if ball is within hit radius
            float distanceToBall = Vector3.Distance(transform.position, targetBall.transform.position);
            
            if (distanceToBall <= settings.hitRadius && targetBall.IsMoving)
            {
                // Check if ball is moving toward AI and has bounced
                bool ballComingToward = targetBall.Velocity2D.x > 0f;
                
                if (ballComingToward && targetBall.HasBounced)
                {
                    AttemptHit();
                }
            }
        }
        
        private void AttemptHit()
        {
            // Attempting to hit ball
            
            // Calculate hit direction (toward player area with some variation)
            Vector3 baseDirection = Vector3.left;
            
            // Add some randomness to make rallies interesting
            float yVariation = Random.Range(-0.3f, 0.3f);
            Vector3 hitDirection = (baseDirection + Vector3.up * yVariation).normalized;
            
            // The actual hit will be handled by the ball's collision detection
            // This just sets up for the hit timing
            
            // Play hit animation
            if (settings.paddleAnimator != null)
            {
                settings.paddleAnimator.SetTrigger("Hit");
            }
            
            // Start hit cooldown
            hitCooldown = settings.hitReactionTime;
            
            // Visual feedback
            StartCoroutine(HitVisualFeedback());
        }
        
        private System.Collections.IEnumerator HitVisualFeedback()
        {
            // Simple visual feedback - brief scale change
            Vector3 originalScale = settings.paddleSprite.localScale;
            Vector3 hitScale = originalScale * 1.15f;
            
            float duration = 0.2f;
            float elapsed = 0f;
            
            // Scale up quickly
            while (elapsed < duration * 0.3f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration * 0.3f);
                settings.paddleSprite.localScale = Vector3.Lerp(originalScale, hitScale, t);
                yield return null;
            }
            
            // Scale back down
            elapsed = 0f;
            while (elapsed < duration * 0.7f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration * 0.7f);
                settings.paddleSprite.localScale = Vector3.Lerp(hitScale, originalScale, t);
                yield return null;
            }
            
            settings.paddleSprite.localScale = originalScale;
        }
        
        public void SetControlsEnabled(bool enabled)
        {
            isEnabled = enabled;
            
            if (!enabled)
            {
                isTrackingBall = false;
                targetPosition = startPosition;
            }
            
            // Controls state changed
        }
        
        public void ResetPosition()
        {
            transform.position = startPosition;
            targetPosition = startPosition;
            isTrackingBall = false;
            reactionTimer = 0f;
            hitCooldown = 0f;
        }
        
        private void DebugDisplay()
        {
            // Draw target position
            if (targetPosition != transform.position)
            {
                Debug.DrawLine(transform.position, targetPosition, Color.blue);
            }
            
            // Draw hit radius (approximated with lines)
            DrawCircle(transform.position, settings.hitRadius, Color.red, 12);
            
            // Draw tracking line to ball
            if (isTrackingBall && targetBall != null)
            {
                Debug.DrawLine(transform.position, targetBall.transform.position, Color.green);
            }
        }
        
        private void DrawCircle(Vector3 center, float radius, Color color, int segments)
        {
            float angleStep = 360f / segments;
            Vector3 prevPoint = center + Vector3.right * radius;
            
            for (int i = 1; i <= segments; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                Vector3 newPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
                Debug.DrawLine(prevPoint, newPoint, color);
                prevPoint = newPoint;
            }
        }
        
        private void OnDrawGizmos()
        {
            if (!showDebugInfo) return;
            
            // Draw paddle position
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.8f);
            
            // Draw movement bounds
            if (Application.isPlaying)
            {
                Gizmos.color = Color.cyan;
                Vector3 upperBound = new Vector3(transform.position.x, startPosition.y + settings.movementBounds, 0f);
                Vector3 lowerBound = new Vector3(transform.position.x, startPosition.y - settings.movementBounds, 0f);
                
                Gizmos.DrawLine(upperBound + Vector3.left * 0.5f, upperBound + Vector3.right * 0.5f);
                Gizmos.DrawLine(lowerBound + Vector3.left * 0.5f, lowerBound + Vector3.right * 0.5f);
            }
            
            // Draw hit radius
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, settings.hitRadius);
            }
            
            // Draw target position
            if (Application.isPlaying && targetPosition != transform.position)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(targetPosition, 0.3f);
            }
        }
        
        // Debug GUI
        private void OnGUI()
        {
            if (!showDebugInfo || !isEnabled) return;
            
            GUILayout.BeginArea(new Rect(10f, 320f, 200f, 120f));
            GUILayout.Label($"AI Paddle Debug:");
            GUILayout.Label($"Tracking Ball: {isTrackingBall}");
            GUILayout.Label($"Reaction Timer: {reactionTimer:F2}");
            GUILayout.Label($"Hit Cooldown: {hitCooldown:F2}");
            GUILayout.Label($"Target Y: {targetPosition.y:F2}");
            GUILayout.EndArea();
        }
    }
}