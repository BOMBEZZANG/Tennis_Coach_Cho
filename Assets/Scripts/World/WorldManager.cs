using UnityEngine;

namespace TennisCoachCho.World
{
    public class WorldManager : MonoBehaviour
    {
        [Header("World Locations")]
        [SerializeField] private Transform homeLocation;
        [SerializeField] private Transform tennisCourtLocation;
        
        [Header("Location Triggers")]
        [SerializeField] private LocationTrigger homeTrigger;
        [SerializeField] private LocationTrigger courtTrigger;
        
        public Transform HomeLocation => homeLocation;
        public Transform TennisCourtLocation => tennisCourtLocation;
        
        private void Start()
        {
            InitializeLocations();
        }
        
        private void InitializeLocations()
        {
            // Set up home location
            if (homeTrigger != null && homeLocation != null)
            {
                homeTrigger.transform.position = homeLocation.position;
            }
            
            // Set up tennis court location
            if (courtTrigger != null && tennisCourtLocation != null)
            {
                courtTrigger.transform.position = tennisCourtLocation.position;
            }
        }
        
        public Vector3 GetLocationPosition(string locationName)
        {
            switch (locationName.ToLower())
            {
                case "home":
                    return homeLocation != null ? homeLocation.position : Vector3.zero;
                case "tennis court":
                case "court":
                    return tennisCourtLocation != null ? tennisCourtLocation.position : Vector3.zero;
                default:
                    Debug.LogWarning($"Unknown location: {locationName}");
                    return Vector3.zero;
            }
        }
        
        public string[] GetAvailableLocations()
        {
            return new string[] { "Home", "Tennis Court" };
        }
    }
}