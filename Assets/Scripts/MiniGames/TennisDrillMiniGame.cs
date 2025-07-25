using UnityEngine;
using TennisCoachCho.Core;
using TennisCoachCho.UI;
using System.Collections;

namespace TennisCoachCho.MiniGames
{
    public enum DrillGameState
    {
        AWAITING_START,
        PREPARING,
        ACTIVE,
        ENDED
    }

    public enum HitJudgment
    {
        PERFECT,
        GOOD,
        BAD,
        MISS
    }

    [System.Serializable]
    public class DrillGameSettings
    {
        [Header("Game Duration")]
        public float lessonDuration = 60f; // seconds
        
        [Header("Court Layout")]
        public Transform playerStartPosition;
        public Transform studentStartPosition;
        public Transform ballStartPosition;
        
        [Header("Judgment Zones")]
        public Transform perfectZone;
        public Transform goodZone;
        [Range(-3.0f, 3.0f)]
        public float judgmentZoneOffsetX = -1.5f; // Distance in front of student paddle (negative = in front)
        [Range(-8.0f, 2.0f)]
        public float judgmentZoneOffsetY = 0f; // Vertical offset from student paddle (0 = same level as paddle)
        
        [Header("Zone Visualization")]
        public SpriteRenderer perfectZoneVisual;
        public SpriteRenderer goodZoneVisual;
        public bool showZonesDuringGame = true;
        [Range(0.1f, 1.0f)]
        public float zoneVisualAlpha = 0.3f; // Transparency of zone visuals
        
        [Header("Camera")]
        public Camera gameCamera;
        public float zoomedInSize = 8f;
        public float normalSize = 12f;
        
        [Header("UI References")]
        public TennisDrillUI drillUI;
    }

    public class TennisDrillMiniGame : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private DrillGameSettings settings;
        
        [Header("Game Objects")]
        [SerializeField] private TennisBall ball;
        [SerializeField] private TennisPlayerPaddle playerPaddle;
        [SerializeField] private TennisAIPaddle studentPaddle;
        
        // Game State
        private DrillGameState currentState = DrillGameState.AWAITING_START;
        private float gameTimer;
        private int currentCombo = 0;
        private int maxCombo = 0;
        private int totalScore = 0;
        
        // Events
        public System.Action<DrillGameState> OnStateChanged;
        public System.Action<HitJudgment> OnHitJudged;
        public System.Action<int> OnComboChanged;
        
        public DrillGameState CurrentState => currentState;
        public float RemainingTime => gameTimer;
        public int CurrentCombo => currentCombo;
        public int MaxCombo => maxCombo;
        public int TotalScore => totalScore;
        
        private void Awake()
        {
            ValidateComponents();
        }
        
        private void Start()
        {
            InitializeGame();
        }
        
        private void Update()
        {
            UpdateGameState();
            
            // DEBUG: Allow manual exit with Escape key for testing
            if (Input.GetKeyDown(KeyCode.Escape) && currentState == DrillGameState.ENDED)
            {
                Debug.Log("[TennisDrillMiniGame] DEBUG: Escape key pressed - manually exiting mini-game");
                ExitMiniGame();
            }
        }
        
        private void ValidateComponents()
        {
            if (ball == null) Debug.LogError("[TennisDrillMiniGame] Ball reference missing!");
            if (playerPaddle == null) Debug.LogError("[TennisDrillMiniGame] Player paddle reference missing!");
            if (studentPaddle == null) Debug.LogError("[TennisDrillMiniGame] Student paddle reference missing!");
            if (settings.drillUI == null) Debug.LogError("[TennisDrillMiniGame] Drill UI reference missing!");
        }
        
        private void InitializeGame()
        {
            // Initializing mini-game
            
            // Set initial positions
            if (settings.playerStartPosition != null)
                playerPaddle.transform.position = settings.playerStartPosition.position;
            if (settings.studentStartPosition != null)
                studentPaddle.transform.position = settings.studentStartPosition.position;
            if (settings.ballStartPosition != null)
                ball.transform.position = settings.ballStartPosition.position;
            
            // Initialize components
            ball.Initialize(this);
            playerPaddle.Initialize(this);
            studentPaddle.Initialize(this);
            
            // Initialize zone visuals
            InitializeZoneVisuals();
            
            // Set camera to normal view
            if (settings.gameCamera != null)
                settings.gameCamera.orthographicSize = settings.normalSize;
            
            // Initialize UI
            settings.drillUI?.Initialize(this);
            
            SetState(DrillGameState.AWAITING_START);
        }
        
        private void InitializeZoneVisuals()
        {
            // Validate zone visual components
            if (settings.perfectZoneVisual == null)
                Debug.LogWarning("[TennisDrillMiniGame] Perfect zone visual not assigned!");
            if (settings.goodZoneVisual == null)
                Debug.LogWarning("[TennisDrillMiniGame] Good zone visual not assigned!");
            
            // Initially hide zone visuals
            ShowZoneVisuals(false);
        }
        
        public void StartMiniGame()
        {
            // Starting mini-game
            SetState(DrillGameState.PREPARING);
        }
        
        private void UpdateGameState()
        {
            switch (currentState)
            {
                case DrillGameState.AWAITING_START:
                    // Waiting for player to press Start Lesson button
                    break;
                    
                case DrillGameState.PREPARING:
                    // Handled by coroutine
                    break;
                    
                case DrillGameState.ACTIVE:
                    UpdateActiveGame();
                    break;
                    
                case DrillGameState.ENDED:
                    // Waiting for player to acknowledge results
                    break;
            }
        }
        
        private void UpdateActiveGame()
        {
            // Update timer
            gameTimer -= Time.deltaTime;
            settings.drillUI?.UpdateTimer(gameTimer);
            
            // Periodically update zone positions to ensure they follow the sprite (every 2 seconds)
            if (Time.frameCount % 120 == 0) // Every 2 seconds at 60 FPS
            {
                PositionJudgmentZones();
            }
            
            // Check if time is up
            if (gameTimer <= 0f)
            {
                EndGame();
            }
        }
        
        private void SetState(DrillGameState newState)
        {
            // State changing
            
            var previousState = currentState;
            currentState = newState;
            
            OnStateChanged?.Invoke(newState);
            HandleStateTransition(previousState, newState);
        }
        
        private void HandleStateTransition(DrillGameState from, DrillGameState to)
        {
            switch (to)
            {
                case DrillGameState.AWAITING_START:
                    HandleAwaitingStart();
                    break;
                    
                case DrillGameState.PREPARING:
                    StartCoroutine(HandlePreparing());
                    break;
                    
                case DrillGameState.ACTIVE:
                    HandleActive();
                    break;
                    
                case DrillGameState.ENDED:
                    HandleEnded();
                    break;
            }
        }
        
        private void HandleAwaitingStart()
        {
            // Disable mini-game controls
            playerPaddle.SetControlsEnabled(false);
            
            // Hide zone visuals when not in game
            ShowZoneVisuals(false);
            
            // UI remains hidden during this state - LocationPrompt handles user interaction
        }
        
        private IEnumerator HandlePreparing()
        {
            // Preparing game
            
            // Disable player free movement and hide player unit (handled by GameManager)
            if (GameManager.Instance?.PlayerController != null)
            {
                GameManager.Instance.PlayerController.SetMovementEnabled(false);
                // Hide the player unit GameObject during mini-game
                GameManager.Instance.PlayerController.gameObject.SetActive(false);
            }
            
            // Hide LocationPrompt UI during mini-game
            var locationPrompt = FindObjectOfType<LocationPrompt>();
            if (locationPrompt != null)
            {
                locationPrompt.gameObject.SetActive(false);
                Debug.Log("[TennisDrillMiniGame] LocationPrompt UI hidden during mini-game");
            }
            
            // Show tennis game objects for mini-game
            if (playerPaddle != null)
            {
                playerPaddle.gameObject.SetActive(true);
                Debug.Log("[TennisDrillMiniGame] Player paddle shown for mini-game");
            }
            
            if (studentPaddle != null)
            {
                studentPaddle.gameObject.SetActive(true);
                Debug.Log("[TennisDrillMiniGame] Student paddle shown for mini-game");
            }
            
            if (ball != null)
            {
                ball.gameObject.SetActive(true);
                Debug.Log("[TennisDrillMiniGame] Tennis ball shown for mini-game");
            }
            
            // Move paddles to starting positions smoothly
            yield return StartCoroutine(MovePaddlesToStartPositions());
            
            // Zoom camera in
            yield return StartCoroutine(ZoomCameraIn());
            
            // Show countdown
            yield return StartCoroutine(ShowCountdown());
            
            // Transition to ACTIVE
            SetState(DrillGameState.ACTIVE);
        }
        
        private void HandleActive()
        {
            // Start game timer
            gameTimer = settings.lessonDuration;
            
            // Enable mini-game controls
            playerPaddle.SetControlsEnabled(true);
            
            // Position judgment zones relative to student paddle sprite - FORCE IMMEDIATE UPDATE
            Debug.Log("[TennisDrillMiniGame] HandleActive - Positioning judgment zones immediately");
            PositionJudgmentZones();
            
            // Wait a frame then position again to ensure it takes effect
            StartCoroutine(DelayedZonePositioning());
            
            // Show zone visuals during active gameplay
            ShowZoneVisuals(settings.showZonesDuringGame);
            
            // Player will serve the first ball by pressing 'E' key
            // No automatic serve - player controls when to start the rally
            
            // Show game UI
            settings.drillUI?.ShowGameHUD();
        }
        
        private System.Collections.IEnumerator DelayedZonePositioning()
        {
            yield return null; // Wait one frame
            Debug.Log("[TennisDrillMiniGame] DelayedZonePositioning - Positioning zones again after 1 frame");
            PositionJudgmentZones();
            
            yield return new WaitForSeconds(0.5f); // Wait half a second
            Debug.Log("[TennisDrillMiniGame] DelayedZonePositioning - Final zone positioning after 0.5 seconds");
            PositionJudgmentZones();
        }
        
        private void HandleEnded()
        {
            // Game ended
            
            // Freeze all gameplay
            ball.FreezeMovement();
            playerPaddle.SetControlsEnabled(false);
            studentPaddle.SetControlsEnabled(false);
            
            // Hide zone visuals when game ends
            ShowZoneVisuals(false);
            
            // Show results
            settings.drillUI?.ShowResults(totalScore, maxCombo);
        }
        
        public void EndGame()
        {
            SetState(DrillGameState.ENDED);
        }
        
        public void ExitMiniGame()
        {
            Debug.Log("[TennisDrillMiniGame] ExitMiniGame called - starting exit process...");
            
            // Re-enable player movement and show player unit
            if (GameManager.Instance?.PlayerController != null)
            {
                GameManager.Instance.PlayerController.SetMovementEnabled(true);
                // Show the player unit GameObject again
                GameManager.Instance.PlayerController.gameObject.SetActive(true);
            }
            
            // Show LocationPrompt UI again
            var locationPrompt = FindObjectOfType<LocationPrompt>();
            if (locationPrompt != null)
            {
                locationPrompt.gameObject.SetActive(true);
                Debug.Log("[TennisDrillMiniGame] LocationPrompt UI restored after mini-game");
            }
            
            // Zoom camera back out
            StartCoroutine(ZoomCameraOut());
            
            // Hide UI
            settings.drillUI?.Hide();
            
            // Hide tennis paddles when returning to normal game
            if (playerPaddle != null)
            {
                playerPaddle.gameObject.SetActive(false);
                Debug.Log("[TennisDrillMiniGame] Player paddle hidden");
            }
            
            if (studentPaddle != null)
            {
                studentPaddle.gameObject.SetActive(false);
                Debug.Log("[TennisDrillMiniGame] Student paddle hidden");
            }
            
            // Hide tennis ball as well
            if (ball != null)
            {
                ball.gameObject.SetActive(false);
                Debug.Log("[TennisDrillMiniGame] Tennis ball hidden");
            }
            
            // Reset game state
            Debug.Log("[TennisDrillMiniGame] Resetting game state to AWAITING_START");
            SetState(DrillGameState.AWAITING_START);
            
            Debug.Log("[TennisDrillMiniGame] ExitMiniGame completed successfully!");
        }
        
        // Public method to manually force zone repositioning (for debugging)
        [ContextMenu("Force Reposition Judgment Zones")]
        public void ForceRepositionJudgmentZones()
        {
            Debug.Log("[TennisDrillMiniGame] Manually forcing judgment zone repositioning...");
            PositionJudgmentZones();
        }
        
        private void PositionJudgmentZones()
        {
            if (studentPaddle == null) 
            {
                Debug.LogError("[TennisDrillMiniGame] Student paddle is null - cannot position judgment zones!");
                return;
            }
            
            // Get both positions for comparison
            Vector3 studentGameObjectPos = studentPaddle.transform.position;
            Vector3 studentSpritePos = studentPaddle.GetSpritePosition();
            
            Debug.Log($"[TennisDrillMiniGame] Student paddle GameObject position: {studentGameObjectPos}");
            Debug.Log($"[TennisDrillMiniGame] Student paddle Sprite position: {studentSpritePos}");
            Debug.Log($"[TennisDrillMiniGame] Position difference: {studentSpritePos - studentGameObjectPos}");
            
            // Use sprite position as the reference for zone positioning
            Vector3 targetPos = new Vector3(
                studentSpritePos.x + settings.judgmentZoneOffsetX, 
                studentSpritePos.y + settings.judgmentZoneOffsetY, 
                0f
            );
            
            // Ensure zones are positioned in a reasonable area for ball landings
            // Based on court boundaries and typical ball physics
            targetPos.y = Mathf.Clamp(targetPos.y, -20f, -10f); // Keep in reasonable court area
            targetPos.x = Mathf.Clamp(targetPos.x, -5f, 5f);    // Keep in student's side
            
            Debug.Log($"[TennisDrillMiniGame] Calculated target position: {targetPos}");
            Debug.Log($"[TennisDrillMiniGame] Offset settings - X: {settings.judgmentZoneOffsetX}, Y: {settings.judgmentZoneOffsetY}");
            
            // Check if settings are null or values are wrong
            if (settings == null)
            {
                Debug.LogError("[TennisDrillMiniGame] Settings is null!");
                return;
            }
            
            // Force Y offset to 1 if it's 0 (might be Unity serialization issue)
            if (Mathf.Approximately(settings.judgmentZoneOffsetY, 0f))
            {
                Debug.LogWarning("[TennisDrillMiniGame] Y offset is 0, forcing to 1f");
                settings.judgmentZoneOffsetY = 1f;
                
                // Recalculate with corrected offset
                targetPos = new Vector3(
                    studentSpritePos.x + settings.judgmentZoneOffsetX, 
                    studentSpritePos.y + settings.judgmentZoneOffsetY, 
                    0f
                );
                Debug.Log($"[TennisDrillMiniGame] Recalculated target position with corrected Y offset: {targetPos}");
            }
            
            // Position judgment zones
            if (settings.perfectZone != null)
            {
                Vector3 oldPerfectPos = settings.perfectZone.position;
                settings.perfectZone.position = targetPos;
                Debug.Log($"[TennisDrillMiniGame] Perfect zone moved from {oldPerfectPos} to {targetPos}");
            }
            else
            {
                Debug.LogError("[TennisDrillMiniGame] Perfect zone Transform is null!");
            }
            
            if (settings.goodZone != null)
            {
                Vector3 oldGoodPos = settings.goodZone.position;
                settings.goodZone.position = targetPos;
                Debug.Log($"[TennisDrillMiniGame] Good zone moved from {oldGoodPos} to {targetPos}");
            }
            else
            {
                Debug.LogError("[TennisDrillMiniGame] Good zone Transform is null!");
            }
            
            // Position and configure visual indicators
            SetupZoneVisuals(targetPos);
            
            // Force update the zone positioning immediately
            if (Application.isPlaying)
            {
                Debug.Log("[TennisDrillMiniGame] Forcing immediate zone position update in play mode");
            }
        }
        
        private void SetupZoneVisuals(Vector3 zonePosition)
        {
            // Setup perfect zone visual
            if (settings.perfectZoneVisual != null)
            {
                settings.perfectZoneVisual.transform.position = zonePosition;
                ConfigureZoneVisual(settings.perfectZoneVisual, Color.yellow, settings.showZonesDuringGame);
                Debug.Log($"[TennisDrillMiniGame] Perfect zone visual positioned at: {zonePosition}");
            }
            
            // Setup good zone visual  
            if (settings.goodZoneVisual != null)
            {
                settings.goodZoneVisual.transform.position = zonePosition;
                ConfigureZoneVisual(settings.goodZoneVisual, Color.green, settings.showZonesDuringGame);
                Debug.Log($"[TennisDrillMiniGame] Good zone visual positioned at: {zonePosition}");
            }
        }
        
        private void ConfigureZoneVisual(SpriteRenderer visual, Color baseColor, bool visible)
        {
            if (visual == null) return;
            
            // Set color with transparency
            Color visualColor = baseColor;
            visualColor.a = visible ? settings.zoneVisualAlpha : 0f;
            visual.color = visualColor;
            
            // Ensure visual is active
            visual.gameObject.SetActive(true);
            
            // Set sorting order to render above ground but below ball
            visual.sortingOrder = 1;
        }
        
        private void ShowZoneVisuals(bool visible)
        {
            // Show/hide perfect zone visual
            if (settings.perfectZoneVisual != null)
            {
                ConfigureZoneVisual(settings.perfectZoneVisual, Color.yellow, visible);
                Debug.Log($"[TennisDrillMiniGame] Perfect zone visual visibility: {visible}");
            }
            
            // Show/hide good zone visual
            if (settings.goodZoneVisual != null)
            {
                ConfigureZoneVisual(settings.goodZoneVisual, Color.green, visible);
                Debug.Log($"[TennisDrillMiniGame] Good zone visual visibility: {visible}");
            }
        }
        
        public void OnBallHit(Vector3 ballPosition)
        {
            Debug.Log($"[TennisDrillMiniGame] OnBallHit called with ball position: {ballPosition}");
            
            // Determine hit judgment based on where ball lands
            HitJudgment judgment = EvaluateHit(ballPosition);
            Debug.Log($"[TennisDrillMiniGame] Hit judgment result: {judgment}");
            
            ProcessHitJudgment(judgment);
        }
        
        public void OnBallMissed()
        {
            // Ball missed
            ProcessHitJudgment(HitJudgment.MISS);
        }
        
        private HitJudgment EvaluateHit(Vector3 ballPosition)
        {
            Debug.Log($"[TennisDrillMiniGame] Evaluating hit at ball position: {ballPosition}");
            
            // Check if ball landed in judgment zones
            if (IsInZone(ballPosition, settings.perfectZone))
            {
                Debug.Log($"[TennisDrillMiniGame] Ball landed in PERFECT zone! Zone position: {settings.perfectZone?.position}");
                return HitJudgment.PERFECT;
            }
            else if (IsInZone(ballPosition, settings.goodZone))
            {
                Debug.Log($"[TennisDrillMiniGame] Ball landed in GOOD zone! Zone position: {settings.goodZone?.position}");
                return HitJudgment.GOOD;
            }
            else
            {
                Debug.Log($"[TennisDrillMiniGame] Ball landed outside judgment zones - BAD hit");
                if (settings.perfectZone != null)
                    Debug.Log($"[TennisDrillMiniGame] Perfect zone position: {settings.perfectZone.position}");
                if (settings.goodZone != null)
                    Debug.Log($"[TennisDrillMiniGame] Good zone position: {settings.goodZone.position}");
                return HitJudgment.BAD;
            }
        }
        
        private bool IsInZone(Vector3 position, Transform zone)
        {
            if (zone == null) 
            {
                Debug.Log($"[TennisDrillMiniGame] Zone is null");
                return false;
            }
            
            var collider = zone.GetComponent<Collider2D>();
            if (collider == null)
            {
                Debug.Log($"[TennisDrillMiniGame] Zone {zone.name} has no Collider2D");
                return false;
            }
            
            var bounds = collider.bounds;
            bool contains = bounds.Contains(position);
            
            // Calculate distance from zone center for debugging
            float distanceFromCenter = Vector3.Distance(position, bounds.center);
            
            Debug.Log($"[TennisDrillMiniGame] Zone {zone.name}:");
            Debug.Log($"  - Position: {zone.position}");
            Debug.Log($"  - Bounds Center: {bounds.center}, Size: {bounds.size}");
            Debug.Log($"  - Ball position: {position}");
            Debug.Log($"  - Distance from center: {distanceFromCenter:F2}");
            Debug.Log($"  - Contains ball: {contains}");
            
            return contains;
        }
        
        private void ProcessHitJudgment(HitJudgment judgment)
        {
            OnHitJudged?.Invoke(judgment);
            
            // Provide visual feedback on the zones
            StartCoroutine(FlashZoneOnHit(judgment));
            
            switch (judgment)
            {
                case HitJudgment.PERFECT:
                    currentCombo++;
                    totalScore += 100 + (currentCombo * 10); // Bonus for combo
                    settings.drillUI?.ShowJudgment("PERFECT!", Color.yellow);
                    break;
                    
                case HitJudgment.GOOD:
                    currentCombo++;
                    totalScore += 50 + (currentCombo * 5);
                    settings.drillUI?.ShowJudgment("GOOD!", Color.green);
                    break;
                    
                case HitJudgment.BAD:
                    currentCombo = 0;
                    settings.drillUI?.ShowJudgment("BAD", Color.red);
                    break;
                    
                case HitJudgment.MISS:
                    currentCombo = 0;
                    settings.drillUI?.ShowJudgment("MISS", Color.gray);
                    break;
            }
            
            // Update max combo
            if (currentCombo > maxCombo)
                maxCombo = currentCombo;
            
            OnComboChanged?.Invoke(currentCombo);
            settings.drillUI?.UpdateCombo(currentCombo);
            settings.drillUI?.UpdateScore(totalScore);
            
            // Hit processed
        }
        
        private IEnumerator FlashZoneOnHit(HitJudgment judgment)
        {
            SpriteRenderer targetZone = null;
            Color flashColor = Color.white;
            
            // Determine which zone to flash based on judgment
            switch (judgment)
            {
                case HitJudgment.PERFECT:
                    targetZone = settings.perfectZoneVisual;
                    flashColor = Color.yellow;
                    break;
                case HitJudgment.GOOD:
                    targetZone = settings.goodZoneVisual;
                    flashColor = Color.green;
                    break;
                default:
                    // No zone flash for BAD or MISS
                    yield break;
            }
            
            if (targetZone == null) yield break;
            
            // Store original color
            Color originalColor = targetZone.color;
            
            // Flash brighter for visual feedback
            Color brightColor = flashColor;
            brightColor.a = 0.8f; // More opaque when flashing
            
            // Flash effect: bright -> normal -> bright -> normal
            float flashDuration = 0.15f;
            
            targetZone.color = brightColor;
            yield return new WaitForSeconds(flashDuration);
            
            targetZone.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
            
            targetZone.color = brightColor;
            yield return new WaitForSeconds(flashDuration);
            
            targetZone.color = originalColor;
        }
        
        // Coroutine helpers
        private IEnumerator MovePaddlesToStartPositions()
        {
            var duration = 1f;
            var elapsed = 0f;
            
            var playerStart = playerPaddle.transform.position;
            var studentStart = studentPaddle.transform.position;
            var playerTarget = settings.playerStartPosition.position;
            var studentTarget = settings.studentStartPosition.position;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / duration;
                
                playerPaddle.transform.position = Vector3.Lerp(playerStart, playerTarget, t);
                studentPaddle.transform.position = Vector3.Lerp(studentStart, studentTarget, t);
                
                yield return null;
            }
        }
        
        private IEnumerator ZoomCameraIn()
        {
            if (settings.gameCamera == null) yield break;
            
            var duration = 0.5f;
            var elapsed = 0f;
            var startSize = settings.gameCamera.orthographicSize;
            var startPosition = settings.gameCamera.transform.position;
            var targetSize = settings.zoomedInSize;
            
            // Position camera to center on actual tennis court area
            // Player paddle at Y: -12.28, court area roughly X: 4 to -22, Y: -16 to -8
            Vector3 targetPosition = new Vector3(-9f, -12f, startPosition.z);
            Debug.Log($"[TennisDrillMiniGame] Camera zooming from {startPosition} to {targetPosition}");
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / duration;
                
                settings.gameCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
                settings.gameCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }
        }
        
        private IEnumerator ZoomCameraOut()
        {
            if (settings.gameCamera == null) yield break;
            
            var duration = 0.5f;
            var elapsed = 0f;
            var startSize = settings.gameCamera.orthographicSize;
            var startPosition = settings.gameCamera.transform.position;
            var targetSize = settings.normalSize;
            
            // Return camera to player's position (assuming player controller has the main camera position)
            Vector3 targetPosition = startPosition;
            if (GameManager.Instance?.PlayerController != null)
            {
                targetPosition = new Vector3(
                    GameManager.Instance.PlayerController.transform.position.x,
                    GameManager.Instance.PlayerController.transform.position.y,
                    startPosition.z
                );
            }
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / duration;
                
                settings.gameCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
                settings.gameCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }
        }
        
        private IEnumerator ShowCountdown()
        {
            settings.drillUI?.ShowCountdown(3);
            yield return new WaitForSeconds(1f);
            
            settings.drillUI?.ShowCountdown(2);
            yield return new WaitForSeconds(1f);
            
            settings.drillUI?.ShowCountdown(1);
            yield return new WaitForSeconds(1f);
            
            settings.drillUI?.ShowCountdown("GO!");
            yield return new WaitForSeconds(0.5f);
            
            settings.drillUI?.HideCountdown();
        }
    }
}