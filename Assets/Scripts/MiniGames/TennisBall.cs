using UnityEngine;

namespace TennisCoachCho.MiniGames
{
    [System.Serializable]
    public class BallPhysicsSettings
    {
        [Header("3D Physics")]
        public float gravity = -20f;
        public float bounceEnergyLoss = 0.85f;
        public float minimumBounceVelocity = 2f;
        
        [Header("Movement")]
        public float baseSpeed = 8f;
        public float serveSpeed = 10f;
        public float hitSpeed = 12f;
        
        [Header("Visual")]
        public float shadowOffsetMultiplier = 0.1f;
        public Transform ballSprite;
        public Transform ballShadow;
    }

    public class TennisBall : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private BallPhysicsSettings settings;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // 3D Physics simulation
        private Vector3 velocity2D; // X and Y velocity
        private float zPosition; // Virtual Z height
        private float zVelocity; // Vertical velocity
        
        // Game references
        private TennisDrillMiniGame gameManager;
        private Collider2D ballCollider;
        
        // State
        private bool isMoving = false;
        private bool hasBounced = false;
        private int bounceCount = 0;
        private Vector3 lastHitPosition;
        
        // Court boundaries (set by game manager)
        private Bounds courtBounds;
        
        public bool IsMoving => isMoving;
        public float ZPosition => zPosition;
        public Vector3 Velocity2D => velocity2D;
        public bool HasBounced => hasBounced;
        
        private void Awake()
        {
            ballCollider = GetComponent<Collider2D>();
            if (ballCollider == null)
            {
                Debug.LogError("[TennisBall] No Collider2D found on ball!");
            }
            
            ValidateComponents();
        }
        
        private void ValidateComponents()
        {
            if (settings.ballSprite == null)
            {
                Debug.LogWarning("[TennisBall] Ball sprite not assigned, using transform");
                settings.ballSprite = transform;
            }
            
            if (settings.ballShadow == null)
            {
                Debug.LogWarning("[TennisBall] Ball shadow not assigned");
            }
        }
        
        public void Initialize(TennisDrillMiniGame manager)
        {
            gameManager = manager;
            
            // Set initial state
            ResetBall();
            
            // Ensure ball GameObject is active
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
                Debug.Log("[TennisBall] Ball GameObject was inactive, activated it");
            }
            
            // Validate settings have reasonable defaults
            if (settings.serveSpeed <= 0f) settings.serveSpeed = 10f;
            if (settings.baseSpeed <= 0f) settings.baseSpeed = 8f;
            if (settings.hitSpeed <= 0f) settings.hitSpeed = 12f;
            
            Debug.Log($"[TennisBall] Initialized - Position: {transform.position}, Active: {gameObject.activeInHierarchy}");
            Debug.Log($"[TennisBall] Settings - ServeSpeed: {settings.serveSpeed}, Gravity: {settings.gravity}");
        }
        
        private void Update()
        {
            if (isMoving)
            {
                UpdatePhysics();
                UpdateVisuals();
                CheckBoundaries();
            }
            
            if (showDebugInfo)
            {
                DebugDraw();
            }
        }
        
        private void UpdatePhysics()
        {
            float deltaTime = Time.deltaTime;
            
            // Update 2D position
            Vector3 newPosition = transform.position + velocity2D * deltaTime;
            transform.position = newPosition;
            
            // Update 3D height physics
            zVelocity += settings.gravity * deltaTime;
            zPosition += zVelocity * deltaTime;
            
            // Debug info every 60 frames to avoid spam
            if (showDebugInfo && Time.frameCount % 60 == 0)
            {
                Debug.Log($"[TennisBall] Physics update - Pos: {transform.position}, velocity2D: {velocity2D}, zPos: {zPosition}, zVel: {zVelocity}");
            }
            
            // Handle ground bounce
            if (zPosition <= 0f && zVelocity < 0f)
            {
                zPosition = 0f;
                zVelocity = -zVelocity * settings.bounceEnergyLoss;
                
                OnBallBounce();
                
                // Stop bouncing if velocity is too low
                if (Mathf.Abs(zVelocity) < settings.minimumBounceVelocity)
                {
                    zVelocity = 0f;
                    StopBall();
                }
            }
        }
        
        private void UpdateVisuals()
        {
            // Update ball sprite position based on height
            if (settings.ballSprite != null)
            {
                Vector3 spritePos = transform.position;
                spritePos.y += zPosition * settings.shadowOffsetMultiplier;
                settings.ballSprite.position = spritePos;
            }
            
            // Keep shadow at ground level
            if (settings.ballShadow != null)
            {
                settings.ballShadow.position = transform.position;
                
                // Make shadow smaller/fainter when ball is higher
                float shadowScale = Mathf.Lerp(1f, 0.7f, zPosition / 5f);
                settings.ballShadow.localScale = Vector3.one * shadowScale;
            }
        }
        
        private void CheckBoundaries()
        {
            Vector3 pos = transform.position;
            
            // Simple boundary check (you can expand this based on your court layout)
            if (pos.x < -15f || pos.x > 15f || pos.y < -10f || pos.y > 10f)
            {
                Debug.Log("[TennisBall] Ball went out of bounds");
                OnBallOutOfBounds();
            }
        }
        
        private void OnBallBounce()
        {
            bounceCount++;
            hasBounced = true;
            
            Debug.Log($"[TennisBall] Ball bounced (count: {bounceCount}) at position: {transform.position}");
            
            // Check for miss condition (second bounce on player side)
            if (bounceCount >= 2 && IsOnPlayerSide())
            {
                Debug.Log("[TennisBall] Second bounce on player side - MISS!");
                gameManager?.OnBallMissed();
                StopBall();
            }
        }
        
        private void OnBallOutOfBounds()
        {
            Debug.Log("[TennisBall] Ball out of bounds");
            gameManager?.OnBallMissed();
            StopBall();
        }
        
        private bool IsOnPlayerSide()
        {
            // Assuming player is on the left side (negative X)
            return transform.position.x < 0f;
        }
        
        public void ServeFromStudent()
        {
            Debug.Log("[TennisBall] ServeFromStudent called");
            Debug.Log($"[TennisBall] Game manager assigned: {gameManager != null}");
            Debug.Log($"[TennisBall] Ball position before serve: {transform.position}");
            
            ResetBall();
            
            // Position ball at student's side for serve (right side of court)
            Vector3 servePosition = new Vector3(4f, Random.Range(-1f, 1f), 0f);
            transform.position = servePosition;
            
            // Set serve trajectory toward player (left side)
            Vector3 targetDirection = (Vector3.left + Vector3.down * 0.3f).normalized;
            velocity2D = targetDirection * settings.serveSpeed;
            zVelocity = 10f; // Give it good height for serve
            zPosition = 1.5f; // Start slightly elevated
            
            isMoving = true;
            
            Debug.Log($"[TennisBall] Serve started from position: {transform.position}");
            Debug.Log($"[TennisBall] Serve velocity - 2D: {velocity2D}, Z: {zVelocity}, isMoving: {isMoving}");
        }
        
        public void HitByPlayer(Vector3 hitDirection, float hitPower = 1f)
        {
            Debug.Log($"[TennisBall] Hit by player with direction: {hitDirection}");
            
            // Calculate hit trajectory
            Vector3 normalizedDirection = hitDirection.normalized;
            float speed = settings.hitSpeed * hitPower;
            
            velocity2D = new Vector3(normalizedDirection.x, normalizedDirection.y, 0f) * speed;
            zVelocity = 12f; // Give it arc height
            
            // Reset bounce tracking
            bounceCount = 0;
            hasBounced = false;
            lastHitPosition = transform.position;
            
            isMoving = true;
        }
        
        public void HitByStudent(Vector3 targetPosition)
        {
            Debug.Log($"[TennisBall] Hit by student toward: {targetPosition}");
            
            // Calculate trajectory toward target
            Vector3 direction = (targetPosition - transform.position).normalized;
            velocity2D = direction * settings.baseSpeed;
            zVelocity = 10f;
            
            // Reset bounce tracking
            bounceCount = 0;
            hasBounced = false;
            
            isMoving = true;
        }
        
        public void StopBall()
        {
            isMoving = false;
            velocity2D = Vector3.zero;
            zVelocity = 0f;
            
            Debug.Log("[TennisBall] Ball stopped");
            
            // Check if this was a successful hit that needs judgment
            if (hasBounced && !IsOnPlayerSide())
            {
                // Ball landed on student's side - judge the hit
                gameManager?.OnBallHit(transform.position);
            }
        }
        
        public void FreezeMovement()
        {
            isMoving = false;
            velocity2D = Vector3.zero;
            zVelocity = 0f;
        }
        
        private void ResetBall()
        {
            isMoving = false;
            velocity2D = Vector3.zero;
            zVelocity = 0f;
            zPosition = 0f;
            bounceCount = 0;
            hasBounced = false;
            
            // Reset visual positions
            if (settings.ballSprite != null)
                settings.ballSprite.position = transform.position;
            if (settings.ballShadow != null)
                settings.ballShadow.position = transform.position;
        }
        
        // Collision detection for hitting
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isMoving) return;
            
            // Check for paddle hits
            var playerPaddle = other.GetComponent<TennisPlayerPaddle>();
            if (playerPaddle != null && playerPaddle.IsSwinging)
            {
                Debug.Log("[TennisBall] Ball hit by player paddle");
                
                // Calculate hit direction based on paddle position
                Vector3 hitDirection = (Vector3.right + Vector3.up * 0.3f).normalized;
                HitByPlayer(hitDirection);
                return;
            }
            
            var studentPaddle = other.GetComponent<TennisAIPaddle>();
            if (studentPaddle != null)
            {
                Debug.Log("[TennisBall] Ball hit by student paddle");
                
                // Student aims back toward player area
                Vector3 targetPos = new Vector3(-5f, Random.Range(-2f, 2f), 0f);
                HitByStudent(targetPos);
                return;
            }
        }
        
        private void DebugDraw()
        {
            // Draw trajectory line
            Debug.DrawRay(transform.position, velocity2D, Color.green);
            
            // Draw height indicator
            Debug.DrawLine(transform.position, transform.position + Vector3.up * zPosition, Color.blue);
        }
        
        private void OnDrawGizmos()
        {
            if (!showDebugInfo) return;
            
            // Draw ball position
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
            
            // Draw height position
            if (Application.isPlaying)
            {
                Gizmos.color = Color.blue;
                Vector3 heightPos = transform.position + Vector3.up * zPosition;
                Gizmos.DrawWireSphere(heightPos, 0.2f);
            }
        }
    }
}