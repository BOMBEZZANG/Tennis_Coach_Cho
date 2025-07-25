using UnityEngine;

namespace TennisCoachCho.MiniGames
{
    [System.Serializable]
    public class BallPhysicsSettings
    {
        [Header("3D Physics")]
        public float gravity = -20f;
        [Range(0.5f, 1.0f)]
        public float bounceEnergyLoss = 0.95f; // Vertical energy retention after bounce
        [Range(0.7f, 1.0f)]
        public float horizontalEnergyLoss = 0.98f; // Horizontal energy retention after bounce
        public float minimumBounceVelocity = 2f;
        
        [Header("Serve Settings")]
        public float serveSpeed = 10f;
        [Range(0.8f, 2.0f)]
        public float serveSpeedVariationMin = 1.2f;
        [Range(1.0f, 2.5f)]
        public float serveSpeedVariationMax = 1.6f;
        [Range(1.0f, 2.0f)]
        public float servePowerMultiplier = 1.5f;
        [Range(6f, 15f)]
        public float serveHeightMin = 8f;
        [Range(8f, 18f)]
        public float serveHeightMax = 12f;
        [Range(0.5f, 3.0f)]
        public float serveHeightVariation = 1f; // Random.Range(1f, 2f)
        
        [Header("Player Hit Settings")]
        public float playerHitSpeed = 18f;
        [Range(1.0f, 2.0f)]
        public float playerHitPowerMultiplier = 1.2f;
        [Range(1.0f, 3.0f)]
        public float playerHitPowerBonus = 1.5f; // Applied in collision detection
        [Range(2f, 15f)]
        public float playerHitZVelocity = 6f;
        
        [Header("Student Hit Settings")]
        public float studentHitSpeed = 15f;
        [Range(0.5f, 1.5f)]
        public float studentHitPowerMultiplier = 0.8f;
        [Range(8f, 18f)]
        public float studentHitZVelocity = 12f;
        
        [Header("Targeting Settings")]
        [Header("Player Hit Targeting")]
        [Range(0.0f, 2.0f)]
        public float playerTargetOffsetX = 1f; // Distance in front of student paddle
        [Range(-2.0f, 2.0f)]
        public float playerTargetOffsetY = -0.5f; // Vertical offset from student paddle
        [Range(-1.0f, 0.5f)]
        public float playerHitDirectionYMin = -0.4f; // Max downward angle
        [Range(-0.5f, 0.1f)]
        public float playerHitDirectionYMax = -0.1f; // Min downward angle
        
        [Header("Student Hit Targeting")]
        public float studentTargetX = -18f; // Target X position toward player area
        [Range(-3.0f, 3.0f)]
        public float studentTargetYVariation = 2f; // Random.Range(-2f, 2f)
        
        [Header("Collision Detection")]
        [Range(0.5f, 3.0f)]
        public float playerCollisionDistance = 1.5f;
        [Range(0.5f, 3.0f)]
        public float studentCollisionDistance = 1.2f;
        [Range(2f, 12f)]
        public float maxHitHeight = 8f; // Maximum Z position for hits
        [Range(0.1f, 1.0f)]
        public float hitCooldownDuration = 0.5f;
        [Range(0.2f, 1.0f)]
        public float studentHittingZoneWidth = 0.5f; // How far in front/behind paddle center ball can be hit
        
        [Header("Court Boundaries")]
        public float courtMinX = -25f;
        public float courtMaxX = 10f;
        public float courtMinY = -18f;
        public float courtMaxY = -6f;
        
        [Header("Movement and Base")]
        public float baseSpeed = 8f;
        
        [Header("Visual")]
        public float shadowOffsetMultiplier = 0.1f;
        public Transform ballSprite;
        public Transform ballShadow;
        
        [Header("Developer Tools")]
        [Space(10)]
        [Tooltip("Click this button to reset all values to recommended defaults")]
        public bool resetToDefaults = false;
        [Space(5)]
        [Tooltip("Disable hit cooldown for debugging collision issues")]
        public bool disableHitCooldown = false;
        
        public void ResetToDefaults()
        {
            gravity = -20f;
            bounceEnergyLoss = 0.95f;
            horizontalEnergyLoss = 0.98f;
            minimumBounceVelocity = 2f;
            
            serveSpeed = 10f;
            serveSpeedVariationMin = 1.2f;
            serveSpeedVariationMax = 1.6f;
            servePowerMultiplier = 1.5f;
            serveHeightMin = 8f;
            serveHeightMax = 12f;
            serveHeightVariation = 1f;
            
            playerHitSpeed = 18f;
            playerHitPowerMultiplier = 1.2f;
            playerHitPowerBonus = 1.5f;
            playerHitZVelocity = 6f;
            
            studentHitSpeed = 15f;
            studentHitPowerMultiplier = 0.8f;
            studentHitZVelocity = 12f;
            
            playerTargetOffsetX = 1f;
            playerTargetOffsetY = -0.5f;
            playerHitDirectionYMin = -0.4f;
            playerHitDirectionYMax = -0.1f;
            
            studentTargetX = -18f;
            studentTargetYVariation = 2f;
            
            playerCollisionDistance = 1.5f;
            studentCollisionDistance = 1.2f;
            maxHitHeight = 8f;
            hitCooldownDuration = 0.5f;
            studentHittingZoneWidth = 0.5f;
            
            courtMinX = -25f;
            courtMaxX = 10f;
            courtMinY = -18f;
            courtMaxY = -6f;
            
            baseSpeed = 8f;
            shadowOffsetMultiplier = 0.1f;
        }
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
        private float hitCooldown = 0f; // Prevent multiple hits
        
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
            if (settings.playerHitSpeed <= 0f) settings.playerHitSpeed = 18f;
            if (settings.studentHitSpeed <= 0f) settings.studentHitSpeed = 15f;
            
            Debug.Log($"[TennisBall] Initialized - Position: {transform.position}, Active: {gameObject.activeInHierarchy}");
            Debug.Log($"[TennisBall] Settings - ServeSpeed: {settings.serveSpeed}, Gravity: {settings.gravity}");
            Debug.Log($"[TennisBall] Visual components - BallSprite: {settings.ballSprite != null}, BallShadow: {settings.ballShadow != null}");
            
            if (settings.ballSprite != null)
            {
                Debug.Log($"[TennisBall] BallSprite active: {settings.ballSprite.gameObject.activeInHierarchy}, position: {settings.ballSprite.position}");
            }
            
            // Check if this GameObject has a SpriteRenderer
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Debug.Log($"[TennisBall] Main GameObject has SpriteRenderer - enabled: {spriteRenderer.enabled}, sprite: {spriteRenderer.sprite != null}");
            }
            else
            {
                Debug.Log("[TennisBall] Main GameObject has NO SpriteRenderer");
            }
            
            // Check camera position for reference
            var mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Debug.Log($"[TennisBall] Main Camera position: {mainCamera.transform.position}, orthographic size: {mainCamera.orthographicSize}");
            }
            
            // Check player paddle position for serve targeting
            var playerPaddle = FindObjectOfType<TennisPlayerPaddle>();
            if (playerPaddle != null)
            {
                Debug.Log($"[TennisBall] Player paddle position: {playerPaddle.transform.position}");
            }
            else
            {
                Debug.Log("[TennisBall] Player paddle not found!");
            }
            
            // Check student paddle position too
            var studentPaddle = FindObjectOfType<TennisAIPaddle>();
            if (studentPaddle != null)
            {
                Debug.Log($"[TennisBall] Student paddle position: {studentPaddle.transform.position}");
            }
            else
            {
                Debug.Log("[TennisBall] Student paddle not found!");
            }
        }
        
        private void Update()
        {
            if (isMoving)
            {
                UpdatePhysics();
                UpdateVisuals();
                CheckBoundaries();
            }
            
            // Update hit cooldown
            if (hitCooldown > 0f)
            {
                hitCooldown -= Time.deltaTime;
            }
            
            if (showDebugInfo)
            {
                DebugDraw();
            }
            
            // Always check for manual collision (not just in debug mode)
            CheckProximityToPlayer();
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
                
                // Apply energy loss to horizontal velocity for realistic physics
                velocity2D *= settings.horizontalEnergyLoss;
                
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
                
                // Debug visual state every 60 frames
                if (showDebugInfo && Time.frameCount % 60 == 0)
                {
                    Debug.Log($"[TennisBall] Visual update - SpritePos: {spritePos}, BallSprite active: {settings.ballSprite.gameObject.activeInHierarchy}");
                }
            }
            else
            {
                if (showDebugInfo && Time.frameCount % 60 == 0)
                {
                    Debug.Log("[TennisBall] Ball sprite is null!");
                }
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
            
            // Court boundaries using settings
            if (pos.x < settings.courtMinX || pos.x > settings.courtMaxX || 
                pos.y < settings.courtMinY || pos.y > settings.courtMaxY)
            {
                Debug.Log($"[TennisBall] Ball went out of bounds at position: {pos}");
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
                Debug.Log("[TennisBall] Second bounce on player side - MISS! Serving new ball...");
                gameManager?.OnBallMissed();
                StopBall(); // This will automatically serve a new ball
            }
        }
        
        private void OnBallOutOfBounds()
        {
            Debug.Log("[TennisBall] Ball out of bounds - Serving new ball...");
            gameManager?.OnBallMissed();
            StopBall(); // This will automatically serve a new ball
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
            
            // Find player paddle to target its playable area
            var playerPaddle = FindObjectOfType<TennisPlayerPaddle>();
            Vector3 targetArea = new Vector3(-10f, -3f, 0f); // Default target
            
            if (playerPaddle != null)
            {
                Vector3 paddlePos = playerPaddle.transform.position;
                // Target center of player's movement range (assuming 4f movement bounds)
                targetArea = new Vector3(paddlePos.x + 2f, paddlePos.y, 0f); // Slightly in front of paddle
                Debug.Log($"[TennisBall] Player paddle found at: {paddlePos}, targeting: {targetArea}");
            }
            
            // Position ball at student's side for serve (right side of court)
            // Add some variation to make serves more interesting
            float serveVariationY = Random.Range(-1.5f, 1.5f);
            Vector3 servePosition = new Vector3(4f, targetArea.y + serveVariationY, 0f);
            transform.position = servePosition;
            
            // Calculate trajectory toward target area with slight variation
            Vector3 targetWithVariation = targetArea + new Vector3(Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f), 0f);
            Vector3 direction = (targetWithVariation - servePosition).normalized;
            
            // Vary serve speed using settings
            float speedVariation = Random.Range(settings.serveSpeedVariationMin, settings.serveSpeedVariationMax);
            float enhancedServeSpeed = settings.serveSpeed * speedVariation * settings.servePowerMultiplier;
            velocity2D = new Vector3(direction.x, direction.y, 0f) * enhancedServeSpeed;
            
            // Vary serve height using settings
            zVelocity = Random.Range(settings.serveHeightMin, settings.serveHeightMax);
            zPosition = Random.Range(1f, 1f + settings.serveHeightVariation);
            
            isMoving = true;
            
            Debug.Log($"[TennisBall] Serve started from position: {transform.position}");
            Debug.Log($"[TennisBall] Serving toward target area: {targetArea}");
            Debug.Log($"[TennisBall] Serve velocity - 2D: {velocity2D}, Z: {zVelocity}, Speed: {enhancedServeSpeed}");
        }
        
        public void HitByPlayer(Vector3 hitDirection, float hitPower = 1f)
        {
            // Prevent multiple hits on same ball
            if (!isMoving) return;
            
            Debug.Log($"[TennisBall] Hit by player with direction: {hitDirection}");
            
            // Stop any automatic serving coroutines since ball was hit
            StopCoroutine("ServeNewBallAfterDelay");
            
            // Calculate hit trajectory using settings
            Vector3 normalizedDirection = hitDirection.normalized;
            float speed = settings.playerHitSpeed * hitPower * settings.playerHitPowerMultiplier;
            
            velocity2D = new Vector3(normalizedDirection.x, normalizedDirection.y, 0f) * speed;
            zVelocity = settings.playerHitZVelocity;
            
            // Reset bounce tracking for new trajectory
            bounceCount = 0;
            hasBounced = false;
            lastHitPosition = transform.position;
            
            isMoving = true;
            
            Debug.Log($"[TennisBall] Ball hit successfully - new velocity: {velocity2D}, zVel: {zVelocity}");
            Debug.Log($"[TennisBall] Ball trajectory: X={velocity2D.x:F2} (positive=toward student), Y={velocity2D.y:F2}, from pos: {transform.position}");
        }
        
        public void HitByStudent(Vector3 targetPosition)
        {
            // Prevent multiple hits on same ball
            if (!isMoving) return;
            
            Debug.Log($"[TennisBall] Hit by student toward: {targetPosition}");
            
            // Stop any automatic serving coroutines since ball was hit
            StopCoroutine("ServeNewBallAfterDelay");
            
            // Calculate trajectory toward target using settings
            Vector3 direction = (targetPosition - transform.position).normalized;
            float studentHitSpeed = settings.studentHitSpeed * settings.studentHitPowerMultiplier;
            velocity2D = direction * studentHitSpeed;
            zVelocity = settings.studentHitZVelocity;
            
            // Reset bounce tracking for rally
            bounceCount = 0;
            hasBounced = false;
            
            isMoving = true;
            
            Debug.Log($"[TennisBall] Rally continues - ball hit back to player with velocity: {velocity2D}, zVel: {zVelocity}");
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
                
                // After successful hit, serve new ball after short delay
                StartCoroutine(ServeNewBallAfterDelay(1.5f));
            }
            else
            {
                // Ball was missed or went out of bounds - serve new ball
                StartCoroutine(ServeNewBallAfterDelay(1f));
            }
        }
        
        private System.Collections.IEnumerator ServeNewBallAfterDelay(float delay)
        {
            Debug.Log($"[TennisBall] Waiting {delay} seconds before serving new ball");
            yield return new WaitForSeconds(delay);
            
            // Only serve new ball if game is still active and ball is not already moving
            if (gameManager != null && gameManager.CurrentState == DrillGameState.ACTIVE && !isMoving)
            {
                Debug.Log("[TennisBall] Serving new ball automatically");
                ServeFromStudent();
            }
            else if (isMoving)
            {
                Debug.Log("[TennisBall] Ball already moving, skipping automatic serve");
            }
        }
        
        public void FreezeMovement()
        {
            isMoving = false;
            velocity2D = Vector3.zero;
            zVelocity = 0f;
            
            // Stop any pending automatic serves
            StopAllCoroutines();
            Debug.Log("[TennisBall] Movement frozen - stopped all automatic serving");
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
            
            Debug.Log($"[TennisBall] Collision detected with: {other.gameObject.name}");
            Debug.Log($"[TennisBall] Ball position: {transform.position}, zPosition: {zPosition}");
            
            // Check for paddle hits
            var playerPaddle = other.GetComponent<TennisPlayerPaddle>();
            if (playerPaddle != null)
            {
                Debug.Log($"[TennisBall] Player paddle collision - IsSwinging: {playerPaddle.IsSwinging}");
                Debug.Log($"[TennisBall] Paddle position: {playerPaddle.transform.position}");
                
                // Check if ball is at hittable height using settings
                if (zPosition <= settings.maxHitHeight)
                {
                    if (playerPaddle.IsSwinging)
                    {
                        Debug.Log("[TennisBall] Ball hit by player paddle - successful hit!");
                        
                        // Calculate hit direction toward actual student paddle position
                        Vector3 paddlePos = playerPaddle.GetSpritePosition();
                        Vector3 ballPos = transform.position;
                        
                        // Find student paddle and target its actual position
                        var aiPaddle = FindObjectOfType<TennisAIPaddle>();
                        Vector3 targetArea;
                        if (aiPaddle != null)
                        {
                            // Target student paddle sprite position for accurate hitting
                            Vector3 studentSpritePos = aiPaddle.GetSpritePosition();
                            targetArea = new Vector3(studentSpritePos.x + settings.playerTargetOffsetX, 
                                                    studentSpritePos.y + settings.playerTargetOffsetY, 0f);
                            Debug.Log($"[TennisBall] Student paddle found at: {studentSpritePos}, targeting: {targetArea}");
                        }
                        else
                        {
                            // Fallback target - toward right side of court
                            targetArea = new Vector3(-10f, ballPos.y + settings.playerTargetOffsetY, 0f);
                            Debug.Log($"[TennisBall] No student paddle found! Using fallback target: {targetArea}");
                        }
                        
                        Vector3 hitDirection = (targetArea - ballPos).normalized;
                        
                        // Ensure proper trajectory using settings
                        if (hitDirection.y > 0f) hitDirection.y = settings.playerHitDirectionYMax;
                        if (hitDirection.y < settings.playerHitDirectionYMin) hitDirection.y = settings.playerHitDirectionYMin;
                        hitDirection = hitDirection.normalized;
                        
                        Debug.Log($"[TennisBall] Player hitting toward student area: {targetArea}");
                        HitByPlayer(hitDirection, settings.playerHitPowerBonus);
                        return;
                    }
                    else
                    {
                        Debug.Log("[TennisBall] Ball at good height but player not swinging - continuing...");
                    }
                }
                else
                {
                    Debug.Log($"[TennisBall] Ball too high to hit - zPosition: {zPosition}");
                }
            }
            
            // NOTE: Student paddle collision is now handled by manual detection in CheckProximityToStudent()
            // to use the settings properly. Unity trigger detection is disabled for student paddle.
            var studentPaddle = other.GetComponent<TennisAIPaddle>();
            if (studentPaddle != null)
            {
                Debug.Log("[TennisBall] Unity trigger detected student paddle - but using manual detection instead");
                return; // Don't process, let manual detection handle it
            }
        }
        
        // Additional collision detection for more reliable hitting
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!isMoving) return;
            
            // Only check player paddle on stay (AI paddle uses Enter only)
            var playerPaddle = other.GetComponent<TennisPlayerPaddle>();
            if (playerPaddle != null && zPosition <= settings.maxHitHeight && playerPaddle.IsSwinging)
            {
                Debug.Log("[TennisBall] Collision detected via OnTriggerStay2D - hitting ball!");
                
                // Calculate hit direction toward actual student paddle position
                Vector3 paddlePos = playerPaddle.GetSpritePosition();
                Vector3 ballPos = transform.position;
                
                // Find student paddle and target its actual position
                var aiPaddle = FindObjectOfType<TennisAIPaddle>();
                Vector3 targetArea;
                if (aiPaddle != null)
                {
                    // Target student paddle sprite position for accurate hitting
                    Vector3 studentSpritePos = aiPaddle.GetSpritePosition();
                    targetArea = new Vector3(studentSpritePos.x + settings.playerTargetOffsetX, 
                                            studentSpritePos.y + settings.playerTargetOffsetY, 0f);
                }
                else
                {
                    // Fallback target - should be toward right side of court
                    targetArea = new Vector3(-10f, ballPos.y + settings.playerTargetOffsetY, 0f);
                }
                
                Vector3 hitDirection = (targetArea - ballPos).normalized;
                
                // Ensure proper trajectory using settings
                if (hitDirection.y > 0f) hitDirection.y = settings.playerHitDirectionYMax;
                if (hitDirection.y < settings.playerHitDirectionYMin) hitDirection.y = settings.playerHitDirectionYMin;
                hitDirection = hitDirection.normalized;
                
                Debug.Log($"[TennisBall] Player hitting toward student area: {targetArea}");
                HitByPlayer(hitDirection, settings.playerHitPowerBonus);
                return;
            }
        }
        
        private void CheckProximityToPlayer()
        {
            // Check if ball is near player paddle - helps debug timing issues  
            var playerPaddle = FindObjectOfType<TennisPlayerPaddle>();
            if (playerPaddle != null && isMoving)
            {
                float distance = Vector3.Distance(transform.position, playerPaddle.GetSpritePosition());
                if (distance <= 3f && Time.frameCount % 30 == 0) // Log every 30 frames when close
                {
                    Debug.Log($"[TennisBall] Near player paddle - Distance: {distance:F2}, Ball zPos: {zPosition:F2}, Player swinging: {playerPaddle.IsSwinging}");
                }
                
                // Manual collision detection as fallback since triggers aren't working
                if (distance <= settings.playerCollisionDistance && zPosition <= settings.maxHitHeight && playerPaddle.IsSwinging && hitCooldown <= 0f)
                {
                    Debug.Log($"[TennisBall] MANUAL COLLISION DETECTION - Ball hit by player paddle! Distance: {distance:F2}, zPos: {zPosition:F2}");
                    
                    // Calculate hit direction toward actual student paddle position
                    Vector3 paddlePos = playerPaddle.GetSpritePosition();
                    Vector3 ballPos = transform.position;
                    
                    // Find student paddle and target its actual position
                    var aiPaddle = FindObjectOfType<TennisAIPaddle>();
                    Vector3 targetArea;
                    if (aiPaddle != null)
                    {
                        // Target student paddle sprite position for accurate hitting
                        Vector3 studentSpritePos = aiPaddle.GetSpritePosition();
                        targetArea = new Vector3(studentSpritePos.x + settings.playerTargetOffsetX, 
                                                studentSpritePos.y + settings.playerTargetOffsetY, 0f);
                    }
                    else
                    {
                        // Fallback target - should be toward right side of court
                        targetArea = new Vector3(-10f, ballPos.y + settings.playerTargetOffsetY, 0f);
                    }
                    
                    Vector3 hitDirection = (targetArea - ballPos).normalized;
                    
                    // Ensure proper trajectory using settings
                    if (hitDirection.y > 0f) hitDirection.y = settings.playerHitDirectionYMax;
                    if (hitDirection.y < settings.playerHitDirectionYMin) hitDirection.y = settings.playerHitDirectionYMin;
                    hitDirection = hitDirection.normalized;
                    
                    Debug.Log($"[TennisBall] Player hitting toward student area: {targetArea}");
                    HitByPlayer(hitDirection, settings.playerHitPowerBonus);
                    hitCooldown = settings.hitCooldownDuration;
                }
            }
            
            // Check student paddle collision too
            CheckProximityToStudent();
        }
        
        private void CheckProximityToStudent()
        {
            var studentPaddle = FindObjectOfType<TennisAIPaddle>();
            if (studentPaddle != null && isMoving)
            {
                // Use sprite position for collision detection to match visual representation
                Vector3 paddlePos = studentPaddle.GetSpritePosition();
                Vector3 ballPos = transform.position;
                
                // Calculate distance and relative position
                float distance = Vector3.Distance(ballPos, paddlePos);
                float deltaX = ballPos.x - paddlePos.x; // Positive if ball is to the right of paddle
                
                if (distance <= 3f && Time.frameCount % 30 == 0) // Log every 30 frames when close
                {
                    Debug.Log($"[TennisBall] Near student paddle - Distance: {distance:F2}, Ball zPos: {zPosition:F2}, Ball moving right: {velocity2D.x > 0}");
                    Debug.Log($"[TennisBall] Paddle pos: {paddlePos}, Ball pos: {ballPos}, DeltaX: {deltaX:F2}");
                }
                
                // Manual collision detection for student paddle - IMPROVED VERSION  
                // Student should ONLY hit balls that come from PLAYER HITS (positive X velocity)
                // Student should NOT hit serve balls (negative X velocity going to player)
                // Ball should be close to paddle's hitting zone, not just center point
                
                // Debug each condition separately
                bool distanceOk = distance <= settings.studentCollisionDistance;
                bool heightOk = zPosition <= settings.maxHitHeight;
                bool directionOk = velocity2D.x > 0f; // Ball moving toward student (from player hit)
                bool notServeBall = !isMoving || lastHitPosition.x < transform.position.x; // Don't hit serve balls
                bool cooldownOk = hitCooldown <= 0f || settings.disableHitCooldown;
                
                // NEW: Check if ball is actually in paddle's hitting zone (only in front of paddle)
                float hittingZoneRange = settings.studentHittingZoneWidth;
                bool ballInHittingZone = deltaX >= 0f && deltaX <= hittingZoneRange; // Ball must be in front of or at paddle position
                
                if (distance <= 3f && Time.frameCount % 10 == 0) // More frequent debugging when close
                {
                    Debug.Log($"[TennisBall] Student collision check - Distance: {distance:F2}/{settings.studentCollisionDistance:F2}({distanceOk}), Height: {zPosition:F2}/{settings.maxHitHeight:F2}({heightOk}), Direction: {velocity2D.x:F2}>0({directionOk}), NotServe: ({notServeBall}), Cooldown: {hitCooldown:F2}<=0({cooldownOk}), HitZone: {deltaX:F2} in [0.0,{hittingZoneRange:F1}]({ballInHittingZone})");
                }
                
                if (distanceOk && heightOk && directionOk && notServeBall && cooldownOk && ballInHittingZone)
                {
                    Debug.Log($"[TennisBall] MANUAL COLLISION DETECTION - Ball hit by student paddle! Distance: {distance:F2}, zPos: {zPosition:F2}, DeltaX: {deltaX:F2}");
                    Debug.Log($"[TennisBall] Paddle position: {paddlePos}, Ball position: {ballPos}");
                    
                    // Calculate hit direction toward player area using settings
                    Vector3 targetArea = new Vector3(settings.studentTargetX, ballPos.y + Random.Range(-settings.studentTargetYVariation, settings.studentTargetYVariation), 0f);
                    
                    Debug.Log($"[TennisBall] Student hitting toward player area: {targetArea}");
                    HitByStudent(targetArea);
                    hitCooldown = settings.hitCooldownDuration;
                }
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