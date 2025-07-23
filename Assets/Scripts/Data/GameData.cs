using UnityEngine;
using System.Collections.Generic;

namespace TennisCoachCho.Data
{
    [System.Serializable]
    public class PlayerStats
    {
        public int cash = 100;
        public int playerLevel = 1;
        public int playerExp = 0;
        public int playerExpToNext = 100;
        public int skillPoints = 0;
        public int perkPoints = 0;
    }
    
    [System.Serializable]
    public class SkillData
    {
        public string skillName;
        public int level = 1;
        public int exp = 0;
        public int expToNext = 50;
        
        public SkillData(string name = "Coaching")
        {
            skillName = name;
        }
    }
    
    [System.Serializable]
    public class SkillTreeNode
    {
        public string nodeName;
        public int level = 0;
        public int maxLevel = 5;
        public string description;
        
        public SkillTreeNode(string name = "", string desc = "", int max = 5)
        {
            nodeName = name;
            description = desc;
            maxLevel = max;
        }
        
        public bool CanUpgrade()
        {
            return level < maxLevel;
        }
    }
    
    [System.Serializable]
    public class PerkData
    {
        public string perkName;
        public bool isUnlocked = false;
        public string description;
        
        public PerkData(string name = "", string desc = "")
        {
            perkName = name;
            description = desc;
        }
    }
    
    [System.Serializable]
    public class AppointmentData
    {
        public string clientName = "Unknown Client";
        public int scheduledHour = 14;
        public int scheduledMinute = 0;
        public string location = "Tennis Court A";
        public int cashReward = 50;
        public int expReward = 25;
        public bool isCompleted = false;
        public bool isAccepted = false;
        
        public AppointmentData() { }
        
        public AppointmentData(string client, int hour, int minute, string loc, int cash, int exp)
        {
            clientName = client;
            scheduledHour = hour;
            scheduledMinute = minute;
            location = loc;
            cashReward = cash;
            expReward = exp;
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
    
    [CreateAssetMenu(fileName = "GameData", menuName = "TennisCoachCho/GameData")]
    public class GameData : ScriptableObject
    {
        [Header("Player Data")]
        public PlayerStats playerStats = new PlayerStats();
        
        [Header("Skills")]
        public SkillData coachingSkill = new SkillData("Coaching");
        public List<SkillTreeNode> coachingSkillTree = new List<SkillTreeNode>();
        
        [Header("Perks")]
        public List<PerkData> availablePerks = new List<PerkData>();
        
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
                
            // Initialize coaching skill if null
            if (coachingSkill == null)
                coachingSkill = new SkillData("Coaching");
                
            // Initialize skill tree
            InitializeSkillTree();
            
            // Initialize perks
            InitializePerks();
            
            // Initialize sample appointments
            InitializeSampleAppointments();
            
            // Initialize game time if null
            if (currentGameTime == null)
                currentGameTime = new GameDateTime();
        }
        
        private void InitializeSkillTree()
        {
            if (coachingSkillTree.Count == 0)
            {
                coachingSkillTree.Add(new SkillTreeNode("Forehand", "Increases Perfect hit-zone size in mini-game", 5));
                coachingSkillTree.Add(new SkillTreeNode("Friendliness", "Increases Coaching EXP earned from lessons", 5));
                coachingSkillTree.Add(new SkillTreeNode("Patience", "Reduces mini-game difficulty", 3));
                coachingSkillTree.Add(new SkillTreeNode("Communication", "Improves lesson success rate", 5));
            }
        }
        
        private void InitializePerks()
        {
            if (availablePerks.Count == 0)
            {
                availablePerks.Add(new PerkData("Well-Rested", "Slightly increases daily stamina"));
                availablePerks.Add(new PerkData("Early Bird", "Start each day with bonus energy"));
                availablePerks.Add(new PerkData("Networking", "Occasionally receive bonus appointments"));
                availablePerks.Add(new PerkData("Efficient", "Lessons finish 10% faster"));
            }
        }
        
        private void InitializeSampleAppointments()
        {
            if (availableAppointments.Count == 0)
            {
                availableAppointments.Add(new AppointmentData("John Smith", 14, 0, "Tennis Court A", 50, 25));
                availableAppointments.Add(new AppointmentData("Mary Johnson", 16, 0, "Tennis Court B", 75, 35));
                availableAppointments.Add(new AppointmentData("Bob Wilson", 10, 30, "Tennis Court A", 60, 30));
                availableAppointments.Add(new AppointmentData("Sarah Davis", 15, 30, "Tennis Court C", 80, 40));
            }
        }
        
        [ContextMenu("Reset to Defaults")]
        public void ResetToDefaults()
        {
            playerStats = new PlayerStats();
            coachingSkill = new SkillData("Coaching");
            coachingSkillTree.Clear();
            availablePerks.Clear();
            availableAppointments.Clear();
            acceptedAppointments.Clear();
            currentGameTime = new GameDateTime();
            InitializeDefaults();
        }
    }
}