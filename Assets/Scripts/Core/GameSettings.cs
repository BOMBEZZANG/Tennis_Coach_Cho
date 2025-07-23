using UnityEngine;

namespace TennisCoachCho.Core
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "TennisCoachCho/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Time Settings")]
        [Range(1f, 20f)]
        public float realTimeToGameTimeRatio = 10f; // 10 minutes real time = 1 game day
        public int gameStartHour = 8;
        public int gameEndHour = 24;
        
        [Header("Progression Settings")]
        [Range(10, 200)]
        public int basePlayerExpToNext = 100;
        [Range(1.1f, 2f)]
        public float playerExpGrowthRate = 1.15f;
        
        [Range(10, 100)]
        public int baseSkillExpToNext = 50;
        [Range(1.1f, 2f)]
        public float skillExpGrowthRate = 1.2f;
        
        [Range(10, 50)]
        public int playerExpPerSkillLevel = 20;
        
        [Header("Appointment Settings")]
        [Range(1, 10)]
        public int maxDailyAppointments = 3;
        
        [Range(10, 100)]
        public int minCashReward = 20;
        [Range(20, 200)]
        public int maxCashReward = 50;
        
        [Range(5, 50)]
        public int minExpReward = 15;
        [Range(10, 100)]
        public int maxExpReward = 30;
        
        [Header("Mini-Game Settings")]
        [Range(10f, 60f)]
        public float rhythmGameDuration = 30f;
        [Range(10, 50)]
        public int rhythmGameNotes = 20;
        
        [Range(0.05f, 0.2f)]
        public float perfectTimingWindow = 0.1f;
        [Range(0.1f, 0.4f)]
        public float goodTimingWindow = 0.2f;
        [Range(0.2f, 0.6f)]
        public float badTimingWindow = 0.3f;
        
        [Header("Skill Effects")]
        [Range(0.1f, 0.5f)]
        public float forehandBonusPerLevel = 0.2f; // 20% hit zone increase per level
        [Range(0.05f, 0.3f)]
        public float friendlinessBonusPerLevel = 0.15f; // 15% EXP bonus per level
        
        [Header("Audio Settings")]
        [Range(0f, 1f)]
        public float masterVolume = 1f;
        [Range(0f, 1f)]
        public float musicVolume = 0.7f;
        [Range(0f, 1f)]
        public float sfxVolume = 0.8f;
        
        [Header("Controls")]
        public KeyCode smartphoneKey = KeyCode.Tab;
        public KeyCode interactKey = KeyCode.E;
        public KeyCode rhythmGameKey = KeyCode.Space;
    }
}