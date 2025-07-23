using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TennisCoachCho.Core;

namespace TennisCoachCho.UI
{
    public class MainHUD : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI cashText;
        [SerializeField] private TextMeshProUGUI playerLevelText;
        [SerializeField] private Slider playerExpSlider;
        [SerializeField] private TextMeshProUGUI coachingLevelText;
        [SerializeField] private Slider coachingExpSlider;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI dateText;
        [SerializeField] private TextMeshProUGUI skillPointsText;
        [SerializeField] private TextMeshProUGUI perkPointsText;
        
        public void Initialize()
        {
            // Subscribe to events
            if (GameManager.Instance?.ProgressionManager != null)
            {
                var pm = GameManager.Instance.ProgressionManager;
                pm.OnCashChanged += UpdateCash;
                pm.OnPlayerLevelChanged += UpdatePlayerLevel;
                pm.OnCoachingLevelChanged += UpdateCoachingLevel;
                pm.OnSkillPointsChanged += UpdateSkillPoints;
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
                pm.OnCoachingLevelChanged -= UpdateCoachingLevel;
                pm.OnSkillPointsChanged -= UpdateSkillPoints;
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
                var coachingSkill = GameManager.Instance.ProgressionManager.CoachingSkill;
                
                UpdateCash(stats.cash);
                UpdatePlayerLevel(stats.playerLevel, stats.playerExp);
                UpdateCoachingLevel(coachingSkill.level, coachingSkill.exp);
                UpdateSkillPoints(stats.skillPoints);
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
        
        private void UpdateCoachingLevel(int level, int exp)
        {
            if (coachingLevelText != null)
                coachingLevelText.text = $"Coaching: {level}";
                
            if (coachingExpSlider != null && GameManager.Instance?.ProgressionManager != null)
            {
                var coachingSkill = GameManager.Instance.ProgressionManager.CoachingSkill;
                coachingExpSlider.value = (float)exp / coachingSkill.expToNext;
            }
        }
        
        private void UpdateTime(GameDateTime gameTime)
        {
            if (timeText != null)
                timeText.text = gameTime.GetTimeString();
                
            if (dateText != null)
                dateText.text = gameTime.GetDateString();
        }
        
        private void UpdateSkillPoints(int points)
        {
            if (skillPointsText != null)
                skillPointsText.text = $"Skill Points: {points}";
        }
        
        private void UpdatePerkPoints(int points)
        {
            if (perkPointsText != null)
                perkPointsText.text = $"Perk Points: {points}";
        }
    }
}