using System.IO;
using UnityEngine;
using TennisCoachCho.Data;

namespace TennisCoachCho.Core
{
    [System.Serializable]
    public class SaveData
    {
        public PlayerStats playerStats;
        public SkillData coachingSkill;
        public SkillTreeNode[] coachingSkillTree;
        public PerkData[] availablePerks;
        public GameDateTime currentTime;
        public int currentDay;
        
        public SaveData()
        {
            playerStats = new PlayerStats();
            coachingSkill = new SkillData("Coaching");
            currentTime = new GameDateTime(1, 8, 0);
            currentDay = 1;
        }
    }
    
    public static class SaveSystem
    {
        private static string SavePath => Path.Combine(Application.persistentDataPath, "savegame.json");
        
        public static void SaveGame(GameData gameData)
        {
            try
            {
                SaveData saveData = new SaveData
                {
                    playerStats = gameData.playerStats,
                    coachingSkill = gameData.coachingSkill,
                    coachingSkillTree = gameData.coachingSkillTree.ToArray(),
                    availablePerks = gameData.availablePerks.ToArray()
                };
                
                // Add current time if available
                if (GameManager.Instance?.TimeSystem != null)
                {
                    saveData.currentTime = GameManager.Instance.TimeSystem.CurrentTime;
                }
                
                string jsonData = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(SavePath, jsonData);
                
                Debug.Log($"Game saved to: {SavePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
            }
        }
        
        public static bool LoadGame(GameData gameData)
        {
            try
            {
                if (!File.Exists(SavePath))
                {
                    Debug.Log("No save file found.");
                    return false;
                }
                
                string jsonData = File.ReadAllText(SavePath);
                SaveData saveData = JsonUtility.FromJson<SaveData>(jsonData);
                
                if (saveData != null)
                {
                    // Load player data
                    gameData.playerStats = saveData.playerStats ?? new PlayerStats();
                    gameData.coachingSkill = saveData.coachingSkill ?? new SkillData("Coaching");
                    
                    // Load skill tree
                    if (saveData.coachingSkillTree != null)
                    {
                        gameData.coachingSkillTree.Clear();
                        gameData.coachingSkillTree.AddRange(saveData.coachingSkillTree);
                    }
                    
                    // Load perks
                    if (saveData.availablePerks != null)
                    {
                        gameData.availablePerks.Clear();
                        gameData.availablePerks.AddRange(saveData.availablePerks);
                    }
                    
                    // Load time
                    if (GameManager.Instance?.TimeSystem != null)
                    {
                        GameManager.Instance.TimeSystem.SetTime(saveData.currentTime.hour, saveData.currentTime.minute);
                    }
                    
                    Debug.Log("Game loaded successfully.");
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
            }
            
            return false;
        }
        
        public static bool HasSaveFile()
        {
            return File.Exists(SavePath);
        }
        
        public static void DeleteSave()
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    File.Delete(SavePath);
                    Debug.Log("Save file deleted.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to delete save file: {e.Message}");
            }
        }
    }
}