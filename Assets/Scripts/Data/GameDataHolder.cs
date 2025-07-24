using UnityEngine;
using TennisCoachCho.Data;

namespace TennisCoachCho.Data
{
    public class GameDataHolder : MonoBehaviour
    {
        [SerializeField] private GameData gameData;
        
        public GameData GameData => gameData;
        
        private void Awake()
        {
            // Make sure GameData asset is assigned
            if (gameData == null)
            {
                Debug.LogError("GameData asset not assigned to GameDataHolder!");
            }
        }
        
        // Helper method for SkillsPerksApp
        public static GameData GetGameData()
        {
            var holder = FindObjectOfType<GameDataHolder>();
            return holder?.gameData;
        }
    }
}