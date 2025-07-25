using UnityEngine;
using System.Collections.Generic;

namespace TennisCoachCho.Data
{
    // New Dog Coach System: Four Specialist Fields
    [System.Serializable]
    public enum SpecialistField
    {
        Handling,      // 훈련 기술 - Training dogs, agility mini-games
        Behaviorism,   // 행동 심리 - Behavior correction, studying
        Artisanry,     // 제작 기술 - Crafting treats, meals, gear
        Management     // 경영/명성 - Events, competitions, finances
    }
    [System.Serializable]
    public class PlayerStats
    {
        [Header("Basic Stats")]
        public int cash = 100;
        
        [Header("Global Player Progression")]
        public int playerLevel = 1;
        public int playerExp = 0;
        public int playerExpToNext = 100;
        
        [Header("Stamina System")]
        public int currentStamina = 50;
        public int maxStamina = 50;
        
        [Header("Progression Currency")]
        public int perkPoints = 0;  // Earned from Player Level ups, spent on specialist trees
        
        // Calculate stamina increase per level (base 10 + 5 per level)
        public int GetStaminaIncreaseForLevel(int level)
        {
            return 10 + (level * 5);
        }
    }
    
    [System.Serializable]
    public class SpecialistSkillData
    {
        public SpecialistField field;
        public int level = 1;
        public int exp = 0;
        public int expToNext = 50;
        
        public SpecialistSkillData(SpecialistField skillField)
        {
            field = skillField;
        }
        
        public string GetFieldName()
        {
            return field.ToString();
        }
        
        public string GetFieldDisplayName()
        {
            switch (field)
            {
                case SpecialistField.Handling: return "Handling (훈련 기술)";
                case SpecialistField.Behaviorism: return "Behaviorism (행동 심리)";
                case SpecialistField.Artisanry: return "Artisanry (제작 기술)";
                case SpecialistField.Management: return "Management (경영/명성)";
                default: return field.ToString();
            }
        }
    }
    
    [System.Serializable]
    public class PerkTreeNode
    {
        public string nodeName;
        public SpecialistField requiredField;
        public int requiredFieldLevel = 1;  // Minimum level in field to unlock this perk
        public int perkCost = 1;           // Perk points required to unlock
        public bool isUnlocked = false;
        public string description;
        public string gameplayEffect;     // What this perk actually does
        
        public PerkTreeNode(string name = "", SpecialistField field = SpecialistField.Handling, 
                           int reqLevel = 1, int cost = 1, string desc = "", string effect = "")
        {
            nodeName = name;
            requiredField = field;
            requiredFieldLevel = reqLevel;
            perkCost = cost;
            description = desc;
            gameplayEffect = effect;
        }
        
        public bool CanUnlock(int fieldLevel, int availablePerkPoints)
        {
            return !isUnlocked && 
                   fieldLevel >= requiredFieldLevel && 
                   availablePerkPoints >= perkCost;
        }
    }
    
    [System.Serializable]
    public class AppointmentData
    {
        [Header("Basic Info")]
        public string clientName = "Unknown Client";
        public string dogName = "Unknown Dog";
        public int scheduledHour = 14;
        public int scheduledMinute = 0;
        public string location = "Dog Training Area A";
        
        [Header("Rewards")]
        public int cashReward = 50;
        public int playerExpReward = 25;          // Player XP (global)
        public SpecialistField primaryField = SpecialistField.Handling;  // Which specialist field gets XP
        public int specialistExpReward = 15;     // Specialist field XP
        public int staminaCost = 10;             // Stamina consumed for this activity
        
        [Header("Status")]
        public bool isCompleted = false;
        public bool isAccepted = false;
        
        public AppointmentData() { }
        
        public AppointmentData(string client, string dog, int hour, int minute, string loc, 
                              int cash, int playerExp, SpecialistField field, int specialistExp, int stamina = 10)
        {
            clientName = client;
            dogName = dog;
            scheduledHour = hour;
            scheduledMinute = minute;
            location = loc;
            cashReward = cash;
            playerExpReward = playerExp;
            primaryField = field;
            specialistExpReward = specialistExp;
            staminaCost = stamina;
            isCompleted = false;
            isAccepted = false;
        }
        
        public string GetTimeString()
        {
            string period = scheduledHour < 12 ? "AM" : "PM";
            int displayHour = scheduledHour == 0 ? 12 : (scheduledHour > 12 ? scheduledHour - 12 : scheduledHour);
            return $"{displayHour:D2}:{scheduledMinute:D2} {period}";
        }
    }
    
    [System.Serializable]
    public class GameDateTime
    {
        public int hour = 8;
        public int minute = 0;
        public int day = 1;
        public int month = 1;
        public int year = 2024;
        
        public string GetTimeString()
        {
            string period = hour < 12 ? "AM" : "PM";
            int displayHour = hour == 0 ? 12 : (hour > 12 ? hour - 12 : hour);
            return $"{displayHour:D2}:{minute:D2} {period}";
        }
        
        public string GetDateString()
        {
            return $"{month:D2}/{day:D2}/{year}";
        }
    }
    
    [CreateAssetMenu(fileName = "GameData", menuName = "DogCoach/GameData")]
    public class GameData : ScriptableObject
    {
        [Header("Player Data")]
        public PlayerStats playerStats = new PlayerStats();
        
        [Header("Specialist Skills - Four Pillars of Expertise")]
        public List<SpecialistSkillData> specialistSkills = new List<SpecialistSkillData>();
        
        [Header("Perk Trees - Unlocked with Perk Points")]
        public List<PerkTreeNode> allPerkTrees = new List<PerkTreeNode>();
        
        [Header("Appointments")]
        public List<AppointmentData> availableAppointments = new List<AppointmentData>();
        public List<AppointmentData> acceptedAppointments = new List<AppointmentData>();
        
        [Header("Game Time")]
        public GameDateTime currentGameTime = new GameDateTime();
        
        private void OnEnable()
        {
            InitializeDefaults();
        }
        
        private void InitializeDefaults()
        {
            // Initialize player stats if null
            if (playerStats == null)
                playerStats = new PlayerStats();
                
            // Initialize specialist skills (four pillars)
            InitializeSpecialistSkills();
            
            // Initialize perk trees for all four fields
            InitializePerkTrees();
            
            // Initialize sample appointments for Dog Coach
            InitializeDogCoachAppointments();
            
            // Initialize game time if null
            if (currentGameTime == null)
                currentGameTime = new GameDateTime();
        }
        
        private void InitializeSpecialistSkills()
        {
            if (specialistSkills.Count == 0)
            {
                specialistSkills.Add(new SpecialistSkillData(SpecialistField.Handling));
                specialistSkills.Add(new SpecialistSkillData(SpecialistField.Behaviorism));
                specialistSkills.Add(new SpecialistSkillData(SpecialistField.Artisanry));
                specialistSkills.Add(new SpecialistSkillData(SpecialistField.Management));
            }
        }
        
        public SpecialistSkillData GetSpecialistSkill(SpecialistField field)
        {
            return specialistSkills.Find(skill => skill.field == field);
        }
        
        private void InitializePerkTrees()
        {
            if (allPerkTrees.Count == 0)
            {
                // HANDLING PERKS - Training & Agility Focus
                allPerkTrees.Add(new PerkTreeNode("Quick Reflexes", SpecialistField.Handling, 1, 1, 
                    "Improve your reaction time during agility training", "+20% mini-game timing windows"));
                allPerkTrees.Add(new PerkTreeNode("Advanced Techniques", SpecialistField.Handling, 3, 2, 
                    "Unlock complex training methods", "Access to advanced agility courses"));
                allPerkTrees.Add(new PerkTreeNode("Master Trainer", SpecialistField.Handling, 5, 3, 
                    "Become the ultimate dog trainer", "+50% Handling XP gain"));
                
                // BEHAVIORISM PERKS - Psychology & Correction Focus  
                allPerkTrees.Add(new PerkTreeNode("Calm Presence", SpecialistField.Behaviorism, 1, 1, 
                    "Dogs feel more relaxed around you", "Reduces aggressive behavior incidents"));
                allPerkTrees.Add(new PerkTreeNode("Behavioral Analysis", SpecialistField.Behaviorism, 3, 2, 
                    "Quickly identify problem behaviors", "Faster behavior correction sessions"));
                allPerkTrees.Add(new PerkTreeNode("Dog Whisperer", SpecialistField.Behaviorism, 5, 3, 
                    "Perfect understanding of canine psychology", "+50% Behaviorism XP gain"));
                
                // ARTISANRY PERKS - Crafting & Creation Focus
                allPerkTrees.Add(new PerkTreeNode("Efficient Crafting", SpecialistField.Artisanry, 1, 1, 
                    "Create items faster with less waste", "-50% crafting time and materials"));
                allPerkTrees.Add(new PerkTreeNode("Gourmet Recipes", SpecialistField.Artisanry, 3, 2, 
                    "Access to premium treat recipes", "Unlock high-tier nutritional meals"));
                allPerkTrees.Add(new PerkTreeNode("Master Artisan", SpecialistField.Artisanry, 5, 3, 
                    "Create legendary quality items", "+50% Artisanry XP gain"));
                
                // MANAGEMENT PERKS - Business & Reputation Focus
                allPerkTrees.Add(new PerkTreeNode("Networking", SpecialistField.Management, 1, 1, 
                    "Build connections in the dog community", "+1 bonus appointment per day"));
                allPerkTrees.Add(new PerkTreeNode("Event Organizer", SpecialistField.Management, 3, 2, 
                    "Host your own competitions", "Unlock competition hosting"));
                allPerkTrees.Add(new PerkTreeNode("Business Mogul", SpecialistField.Management, 5, 3, 
                    "Maximize profit from all activities", "+50% Management XP gain"));
            }
        }
        
        private void InitializeDogCoachAppointments()
        {
            if (availableAppointments.Count == 0)
            {
                // Handling-focused appointments
                availableAppointments.Add(new AppointmentData("Sarah Kim", "Max", 14, 0, "Agility Course A", 
                    50, 25, SpecialistField.Handling, 20, 15));
                    
                // Behaviorism-focused appointments  
                availableAppointments.Add(new AppointmentData("Mike Johnson", "Bella", 16, 0, "Behavior Room B", 
                    60, 30, SpecialistField.Behaviorism, 18, 12));
                    
                // Management-focused appointments
                availableAppointments.Add(new AppointmentData("Lisa Chen", "Rocky", 10, 30, "Competition Arena", 
                    80, 35, SpecialistField.Management, 25, 20));
                    
                // Artisanry-focused appointments (nutritional consultation)
                availableAppointments.Add(new AppointmentData("David Park", "Luna", 15, 30, "Consultation Room", 
                    45, 20, SpecialistField.Artisanry, 15, 8));
            }
        }
        
        [ContextMenu("Reset to Defaults")]
        public void ResetToDefaults()
        {
            playerStats = new PlayerStats();
            specialistSkills.Clear();
            allPerkTrees.Clear();
            availableAppointments.Clear();
            acceptedAppointments.Clear();
            currentGameTime = new GameDateTime();
            InitializeDefaults();
        }
    }
}