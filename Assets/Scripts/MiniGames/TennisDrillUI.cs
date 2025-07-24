using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace TennisCoachCho.MiniGames
{
    [System.Serializable]
    public class TennisDrillUIElements
    {
        [Header("HUD Elements")]
        public Canvas gameCanvas;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI comboText;
        public TextMeshProUGUI scoreText;
        
        [Header("Judgment Feedback")]
        public TextMeshProUGUI judgmentText;
        public float judgmentDisplayDuration = 1.5f;
        
        [Header("Countdown")]
        public GameObject countdownPanel;
        public TextMeshProUGUI countdownText;
        
        [Header("Results Screen")]
        public GameObject resultsPanel;
        public TextMeshProUGUI finalScoreText;
        public TextMeshProUGUI maxComboText;
        public TextMeshProUGUI gradeText;
        public Button continueButton;
    }

    public class TennisDrillUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TennisDrillUIElements ui;
        
        [Header("Animation Settings")]
        [SerializeField] private float fadeSpeed = 2f;
        [SerializeField] private float judgmentFadeSpeed = 3f;
        
        // Game reference
        private TennisDrillMiniGame gameManager;
        
        // State
        private bool isInitialized = false;
        private Coroutine judgmentCoroutine;
        
        public void Initialize(TennisDrillMiniGame manager)
        {
            gameManager = manager;
            
            ValidateUIElements();
            SetupEventListeners();
            InitializeUI();
            
            isInitialized = true;
            Debug.Log("[TennisDrillUI] Initialized");
        }
        
        private void ValidateUIElements()
        {
            if (ui.gameCanvas == null) Debug.LogError("[TennisDrillUI] Game canvas not assigned!");
            if (ui.timerText == null) Debug.LogWarning("[TennisDrillUI] Timer text not assigned!");
            if (ui.comboText == null) Debug.LogWarning("[TennisDrillUI] Combo text not assigned!");
            if (ui.scoreText == null) Debug.LogWarning("[TennisDrillUI] Score text not assigned!");
            if (ui.judgmentText == null) Debug.LogWarning("[TennisDrillUI] Judgment text not assigned!");
            if (ui.countdownPanel == null) Debug.LogWarning("[TennisDrillUI] Countdown panel not assigned!");
            if (ui.resultsPanel == null) Debug.LogWarning("[TennisDrillUI] Results panel not assigned!");
        }
        
        private void SetupEventListeners()
        {
            if (ui.continueButton != null)
            {
                ui.continueButton.onClick.AddListener(OnContinueButtonClicked);
            }
            
            if (gameManager != null)
            {
                gameManager.OnStateChanged += OnGameStateChanged;
                gameManager.OnHitJudged += OnHitJudged;
                gameManager.OnComboChanged += OnComboChanged;
            }
        }
        
        private void InitializeUI()
        {
            // Hide all UI elements initially - they should only show when mini-game starts
            if (ui.gameCanvas != null)
                ui.gameCanvas.gameObject.SetActive(false);
            
            SetPanelActive(ui.countdownPanel, false);
            SetPanelActive(ui.resultsPanel, false);
            
            // Initialize text elements
            UpdateTimer(0f);
            UpdateCombo(0);
            UpdateScore(0);
            
            if (ui.judgmentText != null)
            {
                ui.judgmentText.text = "";
                ui.judgmentText.color = new Color(1f, 1f, 1f, 0f);
            }
        }
        
        private void OnGameStateChanged(DrillGameState newState)
        {
            Debug.Log($"[TennisDrillUI] Game state changed to: {newState}");
            
            switch (newState)
            {
                case DrillGameState.AWAITING_START:
                    // Do nothing - UI should remain hidden until mini-game starts
                    break;
                    
                case DrillGameState.PREPARING:
                    // Show canvas and start countdown
                    ShowMiniGameUI();
                    break;
                    
                case DrillGameState.ACTIVE:
                    ShowGameHUD();
                    break;
                    
                case DrillGameState.ENDED:
                    // Results will be shown by ShowResults method
                    break;
            }
        }
        
        private void ShowMiniGameUI()
        {
            Debug.Log("[TennisDrillUI] Showing mini-game UI");
            
            // Activate the canvas when mini-game starts
            if (ui.gameCanvas != null)
                ui.gameCanvas.gameObject.SetActive(true);
        }
        
        public void ShowGameHUD()
        {
            Debug.Log("[TennisDrillUI] Showing game HUD");
            
            // Canvas should already be active from ShowMiniGameUI, but ensure HUD elements are visible
            SetTextActive(ui.timerText, true);
            SetTextActive(ui.comboText, true);
            SetTextActive(ui.scoreText, true);
        }
        
        public void ShowCountdown(object countdownValue)
        {
            SetPanelActive(ui.countdownPanel, true);
            
            if (ui.countdownText != null)
            {
                ui.countdownText.text = countdownValue.ToString();
                
                // Animate countdown text
                StartCoroutine(AnimateCountdownText());
            }
        }
        
        public void HideCountdown()
        {
            SetPanelActive(ui.countdownPanel, false);
        }
        
        private IEnumerator AnimateCountdownText()
        {
            if (ui.countdownText == null) yield break;
            
            // Scale animation
            Vector3 originalScale = ui.countdownText.transform.localScale;
            Vector3 targetScale = originalScale * 1.5f;
            
            float duration = 0.3f;
            float elapsed = 0f;
            
            // Scale up
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                ui.countdownText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }
            
            // Scale back down
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                ui.countdownText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }
            
            ui.countdownText.transform.localScale = originalScale;
        }
        
        public void UpdateTimer(float remainingTime)
        {
            if (ui.timerText == null) return;
            
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            
            ui.timerText.text = $"Time: {minutes:00}:{seconds:00}";
            
            // Change color as time runs low
            if (remainingTime <= 10f)
            {
                ui.timerText.color = Color.red;
            }
            else if (remainingTime <= 30f)
            {
                ui.timerText.color = Color.yellow;
            }
            else
            {
                ui.timerText.color = Color.white;
            }
        }
        
        public void UpdateCombo(int combo)
        {
            if (ui.comboText == null) return;
            
            if (combo > 0)
            {
                ui.comboText.text = $"Combo: x{combo}";
                ui.comboText.color = combo >= 10 ? Color.yellow : Color.white;
            }
            else
            {
                ui.comboText.text = "Combo: x0";
                ui.comboText.color = Color.gray;
            }
        }
        
        public void UpdateScore(int score)
        {
            if (ui.scoreText == null) return;
            
            ui.scoreText.text = $"Score: {score:N0}";
        }
        
        public void ShowJudgment(string judgmentText, Color color)
        {
            Debug.Log($"[TennisDrillUI] Showing judgment: {judgmentText}");
            
            if (ui.judgmentText == null) return;
            
            // Stop any existing judgment animation
            if (judgmentCoroutine != null)
            {
                StopCoroutine(judgmentCoroutine);
            }
            
            // Start new judgment animation
            judgmentCoroutine = StartCoroutine(AnimateJudgment(judgmentText, color));
        }
        
        private IEnumerator AnimateJudgment(string text, Color color)
        {
            ui.judgmentText.text = text;
            ui.judgmentText.color = color;
            
            // Fade in quickly
            float elapsed = 0f;
            float fadeInDuration = 0.2f;
            
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = elapsed / fadeInDuration;
                ui.judgmentText.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }
            
            // Hold at full alpha
            ui.judgmentText.color = color;
            yield return new WaitForSeconds(ui.judgmentDisplayDuration - fadeInDuration - 0.5f);
            
            // Fade out
            elapsed = 0f;
            float fadeOutDuration = 0.5f;
            
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = 1f - (elapsed / fadeOutDuration);
                ui.judgmentText.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }
            
            // Fully transparent
            ui.judgmentText.color = new Color(color.r, color.g, color.b, 0f);
            ui.judgmentText.text = "";
        }
        
        public void ShowResults(int finalScore, int maxCombo)
        {
            Debug.Log($"[TennisDrillUI] Showing results - Score: {finalScore}, Max Combo: {maxCombo}");
            
            SetPanelActive(ui.resultsPanel, true);
            
            if (ui.finalScoreText != null)
                ui.finalScoreText.text = $"Final Score: {finalScore:N0}";
            
            if (ui.maxComboText != null)
                ui.maxComboText.text = $"Max Combo: x{maxCombo}";
            
            if (ui.gradeText != null)
            {
                string grade = CalculateGrade(finalScore, maxCombo);
                ui.gradeText.text = $"Grade: {grade}";
            }
        }
        
        private string CalculateGrade(int score, int maxCombo)
        {
            // Simple grading system based on score and combo
            if (score >= 2000 && maxCombo >= 15)
                return "S";
            else if (score >= 1500 && maxCombo >= 10)
                return "A";
            else if (score >= 1000 && maxCombo >= 7)
                return "B";
            else if (score >= 500 && maxCombo >= 5)
                return "C";
            else
                return "D";
        }
        
        public void Hide()
        {
            Debug.Log("[TennisDrillUI] Hiding all UI");
            
            // Hide all panels
            SetPanelActive(ui.countdownPanel, false);
            SetPanelActive(ui.resultsPanel, false);
            
            // Hide the entire canvas
            if (ui.gameCanvas != null)
                ui.gameCanvas.gameObject.SetActive(false);
        }
        
        private void OnHitJudged(HitJudgment judgment)
        {
            // This is handled by ShowJudgment calls from the game manager
        }
        
        private void OnComboChanged(int newCombo)
        {
            UpdateCombo(newCombo);
        }
        
        private void OnContinueButtonClicked()
        {
            Debug.Log("[TennisDrillUI] Continue button clicked");
            gameManager?.ExitMiniGame();
        }
        
        // Helper methods
        private void SetPanelActive(GameObject panel, bool active)
        {
            if (panel != null)
                panel.SetActive(active);
        }
        
        private void SetTextActive(TextMeshProUGUI textComponent, bool active)
        {
            if (textComponent != null)
                textComponent.gameObject.SetActive(active);
        }
        
        private void OnDestroy()
        {
            // Clean up event listeners
            if (gameManager != null)
            {
                gameManager.OnStateChanged -= OnGameStateChanged;
                gameManager.OnHitJudged -= OnHitJudged;
                gameManager.OnComboChanged -= OnComboChanged;
            }
            
            if (ui.continueButton != null)
            {
                ui.continueButton.onClick.RemoveListener(OnContinueButtonClicked);
            }
        }
        
        // Debug GUI
        private void OnGUI()
        {
            if (!isInitialized || gameManager == null) return;
            
            // Show debug info in top-right corner
            GUILayout.BeginArea(new Rect(Screen.width - 220f, 10f, 200f, 150f));
            GUILayout.Label($"Tennis Drill Debug:");
            GUILayout.Label($"State: {gameManager.CurrentState}");
            GUILayout.Label($"Time: {gameManager.RemainingTime:F1}s");
            GUILayout.Label($"Combo: x{gameManager.CurrentCombo}");
            GUILayout.Label($"Max Combo: x{gameManager.MaxCombo}");
            GUILayout.Label($"Score: {gameManager.TotalScore}");
            GUILayout.EndArea();
        }
    }
}