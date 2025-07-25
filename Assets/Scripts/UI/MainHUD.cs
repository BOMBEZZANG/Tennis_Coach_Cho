using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TennisCoachCho.Core;

namespace TennisCoachCho.UI
{
    public class MainHUD : MonoBehaviour
    {
        [Header("Dog Coach UI Elements")]
        [SerializeField] private TextMeshProUGUI cashText;
        [SerializeField] private TextMeshProUGUI playerLevelText;
        [SerializeField] private Slider playerExpSlider;
        [SerializeField] private TextMeshProUGUI staminaText;
        [SerializeField] private Slider staminaSlider;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI dateText;
        [SerializeField] private TextMeshProUGUI perkPointsText;
        
        public void Initialize()
        {
            // Subscribe to events
            if (GameManager.Instance?.ProgressionManager != null)
            {
                var pm = GameManager.Instance.ProgressionManager;
                pm.OnCashChanged += UpdateCash;
                pm.OnPlayerLevelChanged += UpdatePlayerLevel;
                pm.OnStaminaChanged += UpdateStamina;
                pm.OnPerkPointsChanged += UpdatePerkPoints;
            }
            
            if (GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.OnTimeChanged += UpdateTime;
            }
            
            // Initial update
            UpdateAllUI();
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (GameManager.Instance?.ProgressionManager != null)
            {
                var pm = GameManager.Instance.ProgressionManager;
                pm.OnCashChanged -= UpdateCash;
                pm.OnPlayerLevelChanged -= UpdatePlayerLevel;
                pm.OnStaminaChanged -= UpdateStamina;
                pm.OnPerkPointsChanged -= UpdatePerkPoints;
            }
            
            if (GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.OnTimeChanged -= UpdateTime;
            }
        }
        
        private void UpdateAllUI()
        {
            if (GameManager.Instance?.ProgressionManager != null)
            {
                var stats = GameManager.Instance.ProgressionManager.PlayerStats;
                
                UpdateCash(stats.cash);
                UpdatePlayerLevel(stats.playerLevel, stats.playerExp);
                UpdateStamina(stats.currentStamina, stats.maxStamina);
                UpdatePerkPoints(stats.perkPoints);
            }
            
            if (GameManager.Instance?.TimeSystem != null)
            {
                UpdateTime(GameManager.Instance.TimeSystem.CurrentTime);
            }
        }
        
        private void UpdateCash(int cash)
        {
            if (cashText != null)
                cashText.text = $"Cash: ${cash}";
        }
        
        private void UpdatePlayerLevel(int level, int exp)
        {
            if (playerLevelText != null)
                playerLevelText.text = $"Level: {level}";
                
            if (playerExpSlider != null && GameManager.Instance?.ProgressionManager != null)
            {
                var stats = GameManager.Instance.ProgressionManager.PlayerStats;
                playerExpSlider.value = (float)exp / stats.playerExpToNext;
            }
        }
        
        private void UpdateStamina(int current, int max)
        {
            if (staminaText != null)
                staminaText.text = $"Stamina: {current}/{max}";
                
            if (staminaSlider != null)
            {
                staminaSlider.value = max > 0 ? (float)current / max : 0f;
            }
        }
        
        private void UpdateTime(GameDateTime gameTime)
        {
            if (timeText != null)
                timeText.text = gameTime.GetTimeString();
                
            if (dateText != null)
                dateText.text = gameTime.GetDateString();
        }
        
        private void UpdatePerkPoints(int points)
        {
            if (perkPointsText != null)
                perkPointsText.text = $"Perk Points: {points}";
        }
    }
}