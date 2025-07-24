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
            Debug.Log("[TennisDrillMiniGame] Initializing mini-game...");
            
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
            
            // Set camera to normal view
            if (settings.gameCamera != null)
                settings.gameCamera.orthographicSize = settings.normalSize;
            
            // Initialize UI
            settings.drillUI?.Initialize(this);
            
            SetState(DrillGameState.AWAITING_START);
        }
        
        public void StartMiniGame()
        {
            Debug.Log("[TennisDrillMiniGame] Starting mini-game...");
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
            
            // Check if time is up
            if (gameTimer <= 0f)
            {
                EndGame();
            }
        }
        
        private void SetState(DrillGameState newState)
        {
            Debug.Log($"[TennisDrillMiniGame] State changing: {currentState} → {newState}");
            
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
            
            // UI remains hidden during this state - LocationPrompt handles user interaction
        }
        
        private IEnumerator HandlePreparing()
        {
            Debug.Log("[TennisDrillMiniGame] Preparing game...");
            
            // Disable player free movement (handled by GameManager)
            if (GameManager.Instance?.PlayerController != null)
                GameManager.Instance.PlayerController.SetMovementEnabled(false);
            
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
            Debug.Log("[TennisDrillMiniGame] Game active!");
            Debug.Log($"[TennisDrillMiniGame] Ball reference: {ball != null}");
            Debug.Log($"[TennisDrillMiniGame] Player paddle reference: {playerPaddle != null}");
            Debug.Log($"[TennisDrillMiniGame] Student paddle reference: {studentPaddle != null}");
            
            // Start game timer
            gameTimer = settings.lessonDuration;
            Debug.Log($"[TennisDrillMiniGame] Game timer set to: {gameTimer} seconds");
            
            // Enable mini-game controls
            playerPaddle.SetControlsEnabled(true);
            Debug.Log("[TennisDrillMiniGame] Player controls enabled");
            
            // Serve the first ball
            if (ball != null)
            {
                Debug.Log("[TennisDrillMiniGame] Calling ball.ServeFromStudent()...");
                ball.ServeFromStudent();
            }
            else
            {
                Debug.LogError("[TennisDrillMiniGame] Ball is null! Cannot serve.");
            }
            
            // Show game UI
            settings.drillUI?.ShowGameHUD();
        }
        
        private void HandleEnded()
        {
            Debug.Log("[TennisDrillMiniGame] Game ended!");
            
            // Freeze all gameplay
            ball.FreezeMovement();
            playerPaddle.SetControlsEnabled(false);
            studentPaddle.SetControlsEnabled(false);
            
            // Show results
            settings.drillUI?.ShowResults(totalScore, maxCombo);
        }
        
        public void EndGame()
        {
            SetState(DrillGameState.ENDED);
        }
        
        public void ExitMiniGame()
        {
            Debug.Log("[TennisDrillMiniGame] Exiting mini-game...");
            
            // Re-enable player movement
            if (GameManager.Instance?.PlayerController != null)
                GameManager.Instance.PlayerController.SetMovementEnabled(true);
            
            // Zoom camera back out
            StartCoroutine(ZoomCameraOut());
            
            // Hide UI
            settings.drillUI?.Hide();
            
            // Reset game state
            SetState(DrillGameState.AWAITING_START);
        }
        
        public void OnBallHit(Vector3 ballPosition)
        {
            Debug.Log($"[TennisDrillMiniGame] Ball hit at position: {ballPosition}");
            
            // Determine hit judgment based on where ball lands
            HitJudgment judgment = EvaluateHit(ballPosition);
            ProcessHitJudgment(judgment);
        }
        
        public void OnBallMissed()
        {
            Debug.Log("[TennisDrillMiniGame] Ball missed!");
            ProcessHitJudgment(HitJudgment.MISS);
        }
        
        private HitJudgment EvaluateHit(Vector3 ballPosition)
        {
            // Check if ball landed in judgment zones
            if (IsInZone(ballPosition, settings.perfectZone))
            {
                return HitJudgment.PERFECT;
            }
            else if (IsInZone(ballPosition, settings.goodZone))
            {
                return HitJudgment.GOOD;
            }
            else
            {
                return HitJudgment.BAD;
            }
        }
        
        private bool IsInZone(Vector3 position, Transform zone)
        {
            if (zone == null) return false;
            
            var bounds = zone.GetComponent<Collider2D>()?.bounds;
            if (bounds == null) return false;
            
            return bounds.Value.Contains(position);
        }
        
        private void ProcessHitJudgment(HitJudgment judgment)
        {
            OnHitJudged?.Invoke(judgment);
            
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
            
            Debug.Log($"[TennisDrillMiniGame] Hit judged: {judgment}, Combo: {currentCombo}, Score: {totalScore}");
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
            var targetSize = settings.zoomedInSize;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / duration;
                
                settings.gameCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
                yield return null;
            }
        }
        
        private IEnumerator ZoomCameraOut()
        {
            if (settings.gameCamera == null) yield break;
            
            var duration = 0.5f;
            var elapsed = 0f;
            var startSize = settings.gameCamera.orthographicSize;
            var targetSize = settings.normalSize;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / duration;
                
                settings.gameCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
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