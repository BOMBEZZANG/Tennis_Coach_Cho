using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TennisCoachCho.UI
{
    public class LessonCompletePanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Text resultText;
        [SerializeField] private Text cashEarnedText;
        [SerializeField] private Text expEarnedText;
        [SerializeField] private Text performanceText;
        [SerializeField] private Button continueButton;
        [SerializeField] private Image performanceBar;
        
        [Header("Performance Colors")]
        [SerializeField] private Color poorColor = Color.red;
        [SerializeField] private Color averageColor = Color.yellow;
        [SerializeField] private Color goodColor = Color.green;
        [SerializeField] private Color perfectColor = Color.cyan;
        
        private void Awake()
        {
            if (continueButton != null)
                continueButton.onClick.AddListener(Hide);
                
            gameObject.SetActive(false);
        }
        
        public void Show(int cashEarned, int expEarned, float performanceScore)
        {
            UpdateResults(cashEarned, expEarned, performanceScore);
            gameObject.SetActive(true);
            
            // Animate the results
            StartCoroutine(AnimateResults());
        }
        
        private void UpdateResults(int cashEarned, int expEarned, float performanceScore)
        {
            if (resultText != null)
                resultText.text = "Lesson Complete!";
                
            if (cashEarnedText != null)
                cashEarnedText.text = $"Cash Earned: ${cashEarned}";
                
            if (expEarnedText != null)
                expEarnedText.text = $"Coaching EXP: +{expEarned}";
                
            if (performanceText != null)
            {
                string performanceGrade = GetPerformanceGrade(performanceScore);
                performanceText.text = $"Performance: {performanceGrade} ({performanceScore:P0})";
            }
            
            if (performanceBar != null)
            {
                performanceBar.fillAmount = performanceScore;
                performanceBar.color = GetPerformanceColor(performanceScore);
            }
        }
        
        private string GetPerformanceGrade(float score)
        {
            if (score >= 0.9f) return "Excellent";
            if (score >= 0.75f) return "Good";
            if (score >= 0.5f) return "Average";
            if (score >= 0.25f) return "Poor";
            return "Needs Improvement";
        }
        
        private Color GetPerformanceColor(float score)
        {
            if (score >= 0.9f) return perfectColor;
            if (score >= 0.75f) return goodColor;
            if (score >= 0.5f) return averageColor;
            return poorColor;
        }
        
        private IEnumerator AnimateResults()
        {
            // Simple animation: fade in elements one by one
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
                
            canvasGroup.alpha = 0f;
            
            float duration = 0.5f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                yield return null;
            }
            
            canvasGroup.alpha = 1f;
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}