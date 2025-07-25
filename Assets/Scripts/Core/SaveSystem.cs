using System.IO;
using UnityEngine;
using TennisCoachCho.Data;

namespace TennisCoachCho.Core
{
    [System.Serializable]
    public class SaveData
    {
        [Header("Dog Coach System Save Data")]
        public PlayerStats playerStats;
        public SpecialistSkillData[] specialistSkills;
        public PerkTreeNode[] allPerkTrees;
        public GameDateTime currentTime;
        public int currentDay;
        
        public SaveData()
        {
            playerStats = new PlayerStats();
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
                    specialistSkills = gameData.specialistSkills.ToArray(),
                    allPerkTrees = gameData.allPerkTrees.ToArray()
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
                    // DOG COACH SYSTEM: Load player data
                    gameData.playerStats = saveData.playerStats ?? new PlayerStats();
                    
                    // Load specialist skills
                    if (saveData.specialistSkills != null)
                    {
                        gameData.specialistSkills.Clear();
                        gameData.specialistSkills.AddRange(saveData.specialistSkills);
                    }
                    
                    // Load perk trees
                    if (saveData.allPerkTrees != null)
                    {
                        gameData.allPerkTrees.Clear();
                        gameData.allPerkTrees.AddRange(saveData.allPerkTrees);
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