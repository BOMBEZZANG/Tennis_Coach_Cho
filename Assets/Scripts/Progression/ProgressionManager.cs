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
        
        // Dog Coach System Events
        public event Action<int> OnCashChanged;
        public event Action<int, int> OnPlayerLevelChanged; // level, exp
        public event Action<int, int> OnStaminaChanged;     // current, max
        public event Action<SpecialistField, int, int> OnSpecialistLevelChanged; // field, level, exp
        public event Action<int> OnPerkPointsChanged;
        public event Action<string> OnPerkUnlocked;
        
        // Properties for new system
        public PlayerStats PlayerStats => gameData.playerStats;
        public GameData GameDataRef => gameData;
        
        public void Initialize()
        {
            if (gameData == null)
            {
                Debug.LogError("GameData not assigned to ProgressionManager!");
                return;
            }
            
            Debug.Log("[ProgressionManager] Initialized with Dog Coach progression system");
        }
        
        // CASH SYSTEM (unchanged)
        public void AddCash(int amount)
        {
            gameData.playerStats.cash += amount;
            Debug.Log($"[ProgressionManager] Added {amount} cash. Total: {gameData.playerStats.cash}");
            OnCashChanged?.Invoke(gameData.playerStats.cash);
        }
        
        public bool SpendCash(int amount)
        {
            if (gameData.playerStats.cash >= amount)
            {
                gameData.playerStats.cash -= amount;
                OnCashChanged?.Invoke(gameData.playerStats.cash);
                Debug.Log($"[ProgressionManager] Spent {amount} cash. Remaining: {gameData.playerStats.cash}");
                return true;
            }
            Debug.LogWarning($"[ProgressionManager] Not enough cash! Need: {amount}, Have: {gameData.playerStats.cash}");
            return false;
        }
        
        // NEW DOG COACH SYSTEM: PLAYER LEVEL PROGRESSION
        public void AddPlayerExp(int amount)
        {
            gameData.playerStats.playerExp += amount;
            Debug.Log($"[ProgressionManager] Added {amount} Player XP. Total: {gameData.playerStats.playerExp}/{gameData.playerStats.playerExpToNext}");
            
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
            
            // REWARD 1: Increase max stamina
            int staminaIncrease = gameData.playerStats.GetStaminaIncreaseForLevel(gameData.playerStats.playerLevel);
            gameData.playerStats.maxStamina += staminaIncrease;
            gameData.playerStats.currentStamina = gameData.playerStats.maxStamina; // Restore to full on level up
            
            // REWARD 2: Gain 1 perk point
            gameData.playerStats.perkPoints++;
            
            // Increase exp requirement for next level (15% scaling)
            gameData.playerStats.playerExpToNext = Mathf.RoundToInt(gameData.playerStats.playerExpToNext * 1.15f);
            
            Debug.Log($"[ProgressionManager] 🎉 PLAYER LEVEL UP! Level {gameData.playerStats.playerLevel}");
            Debug.Log($"  - Max Stamina increased by {staminaIncrease} (now {gameData.playerStats.maxStamina})");
            Debug.Log($"  - Gained 1 Perk Point (now {gameData.playerStats.perkPoints})");
            Debug.Log($"  - Next level requires {gameData.playerStats.playerExpToNext} XP");
            
            OnPerkPointsChanged?.Invoke(gameData.playerStats.perkPoints);
            OnStaminaChanged?.Invoke(gameData.playerStats.currentStamina, gameData.playerStats.maxStamina);
        }
        
        // NEW DOG COACH SYSTEM: SPECIALIST FIELD PROGRESSION
        public void AddSpecialistExp(SpecialistField field, int amount)
        {
            var skillData = gameData.GetSpecialistSkill(field);
            if (skillData == null)
            {
                Debug.LogError($"[ProgressionManager] Specialist skill {field} not found!");
                return;
            }
            
            skillData.exp += amount;
            Debug.Log($"[ProgressionManager] Added {amount} {field} XP. Total: {skillData.exp}/{skillData.expToNext}");
            
            while (skillData.exp >= skillData.expToNext)
            {
                LevelUpSpecialistSkill(skillData);
            }
            
            OnSpecialistLevelChanged?.Invoke(field, skillData.level, skillData.exp);
        }
        
        private void LevelUpSpecialistSkill(SpecialistSkillData skillData)
        {
            skillData.exp -= skillData.expToNext;
            skillData.level++;
            
            // Increase exp requirement for next level (20% scaling)
            skillData.expToNext = Mathf.RoundToInt(skillData.expToNext * 1.2f);
            
            Debug.Log($"[ProgressionManager] 🌟 {skillData.field} LEVEL UP! Level {skillData.level}");
            Debug.Log($"  - Unlocked new perk tiers for {skillData.GetFieldDisplayName()}");
            Debug.Log($"  - Next level requires {skillData.expToNext} XP");
        }
        
        // NEW DOG COACH SYSTEM: STAMINA MANAGEMENT
        public bool SpendStamina(int amount)
        {
            if (gameData.playerStats.currentStamina >= amount)
            {
                gameData.playerStats.currentStamina -= amount;
                Debug.Log($"[ProgressionManager] Spent {amount} stamina. Remaining: {gameData.playerStats.currentStamina}/{gameData.playerStats.maxStamina}");
                OnStaminaChanged?.Invoke(gameData.playerStats.currentStamina, gameData.playerStats.maxStamina);
                return true;
            }
            Debug.LogWarning($"[ProgressionManager] Not enough stamina! Need: {amount}, Have: {gameData.playerStats.currentStamina}");
            return false;
        }
        
        public void RestoreStamina(int amount)
        {
            gameData.playerStats.currentStamina = Mathf.Min(gameData.playerStats.currentStamina + amount, gameData.playerStats.maxStamina);
            Debug.Log($"[ProgressionManager] Restored {amount} stamina. Current: {gameData.playerStats.currentStamina}/{gameData.playerStats.maxStamina}");
            OnStaminaChanged?.Invoke(gameData.playerStats.currentStamina, gameData.playerStats.maxStamina);
        }
        
        public void RestoreStaminaFull()
        {
            gameData.playerStats.currentStamina = gameData.playerStats.maxStamina;
            Debug.Log($"[ProgressionManager] Stamina fully restored to {gameData.playerStats.maxStamina}");
            OnStaminaChanged?.Invoke(gameData.playerStats.currentStamina, gameData.playerStats.maxStamina);
        }
        
        // NEW DOG COACH SYSTEM: PERK TREE MANAGEMENT
        public bool UnlockPerk(string perkName)
        {
            var perk = gameData.allPerkTrees.FirstOrDefault(p => 
                p.nodeName.Equals(perkName, StringComparison.OrdinalIgnoreCase));
                
            if (perk == null)
            {
                Debug.LogError($"[ProgressionManager] Perk {perkName} not found!");
                return false;
            }
            
            // Check if perk can be unlocked
            var fieldSkill = gameData.GetSpecialistSkill(perk.requiredField);
            if (fieldSkill == null)
            {
                Debug.LogError($"[ProgressionManager] Required field {perk.requiredField} not found!");
                return false;
            }
            
            if (perk.CanUnlock(fieldSkill.level, gameData.playerStats.perkPoints))
            {
                perk.isUnlocked = true;
                gameData.playerStats.perkPoints -= perk.perkCost;
                
                Debug.Log($"[ProgressionManager] 🔓 PERK UNLOCKED: {perkName} ({perk.requiredField})");
                Debug.Log($"  - Cost: {perk.perkCost} perk points");
                Debug.Log($"  - Effect: {perk.gameplayEffect}");
                Debug.Log($"  - Remaining perk points: {gameData.playerStats.perkPoints}");
                
                OnPerkUnlocked?.Invoke(perkName);
                OnPerkPointsChanged?.Invoke(gameData.playerStats.perkPoints);
                return true;
            }
            
            Debug.LogWarning($"[ProgressionManager] Cannot unlock {perkName}: Level {fieldSkill.level}/{perk.requiredFieldLevel}, Points {gameData.playerStats.perkPoints}/{perk.perkCost}");
            return false;
        }
        
        // NEW DOG COACH SYSTEM: UTILITY METHODS
        public int GetSpecialistLevel(SpecialistField field)
        {
            var skillData = gameData.GetSpecialistSkill(field);
            return skillData?.level ?? 1;
        }
        
        public bool IsPerkUnlocked(string perkName)
        {
            var perk = gameData.allPerkTrees.FirstOrDefault(p => 
                p.nodeName.Equals(perkName, StringComparison.OrdinalIgnoreCase));
            return perk?.isUnlocked ?? false;
        }
        
        public bool CanAffordActivity(int staminaCost)
        {
            return gameData.playerStats.currentStamina >= staminaCost;
        }
        
        // DOG COACH SYSTEM: SPECIALIST FIELD BONUSES
        public float GetHandlingBonus()
        {
            int handlingLevel = GetSpecialistLevel(SpecialistField.Handling);
            bool hasQuickReflexes = IsPerkUnlocked("Quick Reflexes");
            
            float bonus = 1f + (handlingLevel * 0.1f); // 10% per level
            if (hasQuickReflexes) bonus *= 1.2f; // +20% from perk
            return bonus;
        }
        
        public float GetBehaviorismBonus()
        {
            int behaviorismLevel = GetSpecialistLevel(SpecialistField.Behaviorism);
            bool hasCalmPresence = IsPerkUnlocked("Calm Presence");
            
            float bonus = 1f + (behaviorismLevel * 0.1f); // 10% per level
            if (hasCalmPresence) bonus *= 1.15f; // +15% from perk
            return bonus;
        }
        
        public float GetArtisanryBonus()
        {
            int artisanryLevel = GetSpecialistLevel(SpecialistField.Artisanry);
            bool hasEfficientCrafting = IsPerkUnlocked("Efficient Crafting");
            
            float bonus = 1f + (artisanryLevel * 0.1f); // 10% per level
            if (hasEfficientCrafting) bonus *= 0.5f; // -50% time/materials
            return bonus;
        }
        
        public float GetManagementBonus()
        {
            int managementLevel = GetSpecialistLevel(SpecialistField.Management);
            bool hasNetworking = IsPerkUnlocked("Networking");
            
            float bonus = 1f + (managementLevel * 0.1f); // 10% per level
            return bonus; // Networking provides +1 daily appointment, not multiplier
        }
        
        public int GetNetworkingBonusAppointments()
        {
            return IsPerkUnlocked("Networking") ? 1 : 0;
        }
    }
}