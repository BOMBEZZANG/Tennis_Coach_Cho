using UnityEngine;
using System;
using System.Linq;
using TennisCoachCho.Data;

namespace TennisCoachCho.Progression
{
    public class ProgressionManager : MonoBehaviour
    {
        [Header("Progression Settings")]
        [SerializeField] private GameData gameData;
        
        public event Action<int> OnCashChanged;
        public event Action<int, int> OnPlayerLevelChanged; // level, exp
        public event Action<int, int> OnCoachingLevelChanged; // level, exp
        public event Action<int> OnSkillPointsChanged;
        public event Action<int> OnPerkPointsChanged;
        public event Action<string> OnSkillUpgraded;
        public event Action<string> OnPerkUnlocked;
        
        public PlayerStats PlayerStats => gameData.playerStats;
        public SkillData CoachingSkill => gameData.coachingSkill;
        public GameData GameDataRef => gameData;
        
        public void Initialize()
        {
            if (gameData == null)
            {
                Debug.LogError("GameData not assigned to ProgressionManager!");
                return;
            }
        }
        
        public void AddCash(int amount)
        {
            gameData.playerStats.cash += amount;
            OnCashChanged?.Invoke(gameData.playerStats.cash);
        }
        
        public bool SpendCash(int amount)
        {
            if (gameData.playerStats.cash >= amount)
            {
                gameData.playerStats.cash -= amount;
                OnCashChanged?.Invoke(gameData.playerStats.cash);
                return true;
            }
            return false;
        }
        
        public void AddCoachingExp(int amount)
        {
            gameData.coachingSkill.exp += amount;
            
            while (gameData.coachingSkill.exp >= gameData.coachingSkill.expToNext)
            {
                LevelUpCoachingSkill();
            }
            
            OnCoachingLevelChanged?.Invoke(gameData.coachingSkill.level, gameData.coachingSkill.exp);
        }
        
        private void LevelUpCoachingSkill()
        {
            gameData.coachingSkill.exp -= gameData.coachingSkill.expToNext;
            gameData.coachingSkill.level++;
            gameData.playerStats.skillPoints++;
            
            // Increase exp requirement for next level
            gameData.coachingSkill.expToNext = Mathf.RoundToInt(gameData.coachingSkill.expToNext * 1.2f);
            
            // Add player EXP for skill level up
            AddPlayerExp(20); // Fixed amount per skill level
            
            OnSkillPointsChanged?.Invoke(gameData.playerStats.skillPoints);
        }
        
        private void AddPlayerExp(int amount)
        {
            gameData.playerStats.playerExp += amount;
            
            while (gameData.playerStats.playerExp >= gameData.playerStats.playerExpToNext)
            {
                LevelUpPlayer();
            }
            
            OnPlayerLevelChanged?.Invoke(gameData.playerStats.playerLevel, gameData.playerStats.playerExp);
        }
        
        private void LevelUpPlayer()
        {
            gameData.playerStats.playerExp -= gameData.playerStats.playerExpToNext;
            gameData.playerStats.playerLevel++;
            gameData.playerStats.perkPoints++;
            
            // Increase exp requirement for next level
            gameData.playerStats.playerExpToNext = Mathf.RoundToInt(gameData.playerStats.playerExpToNext * 1.15f);
            
            OnPerkPointsChanged?.Invoke(gameData.playerStats.perkPoints);
        }
        
        public bool UpgradeSkill(string skillName)
        {
            var skill = gameData.coachingSkillTree.FirstOrDefault(s => 
                s.nodeName.Equals(skillName, StringComparison.OrdinalIgnoreCase));
                
            if (skill != null && skill.CanUpgrade() && gameData.playerStats.skillPoints > 0)
            {
                skill.level++;
                gameData.playerStats.skillPoints--;
                
                OnSkillUpgraded?.Invoke(skillName);
                OnSkillPointsChanged?.Invoke(gameData.playerStats.skillPoints);
                return true;
            }
            
            return false;
        }
        
        public bool UnlockPerk(string perkName)
        {
            var perk = gameData.availablePerks.FirstOrDefault(p => 
                p.perkName.Equals(perkName, StringComparison.OrdinalIgnoreCase));
                
            if (perk != null && !perk.isUnlocked && gameData.playerStats.perkPoints > 0)
            {
                perk.isUnlocked = true;
                gameData.playerStats.perkPoints--;
                
                OnPerkUnlocked?.Invoke(perkName);
                OnPerkPointsChanged?.Invoke(gameData.playerStats.perkPoints);
                return true;
            }
            
            return false;
        }
        
        public int GetSkillLevel(string skillName)
        {
            var skill = gameData.coachingSkillTree.FirstOrDefault(s => 
                s.nodeName.Equals(skillName, StringComparison.OrdinalIgnoreCase));
            return skill?.level ?? 0;
        }
        
        public bool IsPerkUnlocked(string perkName)
        {
            var perk = gameData.availablePerks.FirstOrDefault(p => 
                p.perkName.Equals(perkName, StringComparison.OrdinalIgnoreCase));
            return perk?.isUnlocked ?? false;
        }
        
        public float GetForehandBonus()
        {
            int forehandLevel = GetSkillLevel("Forehand");
            return 1f + (forehandLevel * 0.2f); // 20% increase per level
        }
        
        public float GetFriendlinessBonus()
        {
            int friendlinessLevel = GetSkillLevel("Friendliness");
            return 1f + (friendlinessLevel * 0.15f); // 15% increase per level
        }
    }
}