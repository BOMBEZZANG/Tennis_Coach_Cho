using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TennisCoachCho.Data;

namespace TennisCoachCho.Core
{
    public class AppointmentManager : MonoBehaviour
    {
        [Header("Appointment Settings")]
        [SerializeField] private GameData gameData;
        [SerializeField] private int maxDailyAppointments = 3;
        
        private List<AppointmentData> todaysAppointments;
        
        public List<AppointmentData> AvailableAppointments => gameData.availableAppointments;
        public List<AppointmentData> AcceptedAppointments => gameData.acceptedAppointments;
        
        public void Initialize()
        {
            if (gameData == null)
            {
                Debug.LogError("GameData not assigned to AppointmentManager!");
                return;
            }
            
            GenerateDailyAppointments();
            
            // Create test appointments for debugging - clear existing and create new ones
            CreateTestAppointments();
            
            // Subscribe to time system events
            if (GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.OnNewDay += OnNewDay;
                GameManager.Instance.TimeSystem.OnTimeChanged += CheckAppointmentTimes;
            }
            
            Debug.Log($"[AppointmentManager] Initialized with {gameData.acceptedAppointments.Count} accepted appointments");
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.OnNewDay -= OnNewDay;
                GameManager.Instance.TimeSystem.OnTimeChanged -= CheckAppointmentTimes;
            }
        }
        
        private void OnNewDay(int day)
        {
            GenerateDailyAppointments();
            ClearCompletedAppointments();
        }
        
        private void GenerateDailyAppointments()
        {
            gameData.availableAppointments.Clear();
            
            string[] clientNames = { "Sarah", "Mike", "Emma", "David", "Lisa", "John", "Anna", "Mark" };
            string[] dogNames = { "Buddy", "Max", "Luna", "Charlie", "Bella", "Rocky", "Daisy", "Cooper" };
            string[] locations = { "Tennis Court", "Tennis Court" }; // Only tennis court for now
            TennisCoachCho.Data.SpecialistField[] fields = { 
                TennisCoachCho.Data.SpecialistField.Handling, 
                TennisCoachCho.Data.SpecialistField.Behaviorism, 
                TennisCoachCho.Data.SpecialistField.Artisanry, 
                TennisCoachCho.Data.SpecialistField.Management 
            };
            
            for (int i = 0; i < maxDailyAppointments; i++)
            {
                string client = clientNames[Random.Range(0, clientNames.Length)];
                string dog = dogNames[Random.Range(0, dogNames.Length)];
                int hour = Random.Range(9, 20); // 9 AM to 7 PM
                int minute = Random.Range(0, 2) * 30; // 0 or 30 minutes
                string location = locations[Random.Range(0, locations.Length)];
                int cashReward = Random.Range(20, 50);
                int playerExpReward = Random.Range(15, 30);
                TennisCoachCho.Data.SpecialistField primaryField = fields[Random.Range(0, fields.Length)];
                int specialistExpReward = Random.Range(10, 25);
                int staminaCost = Random.Range(10, 20);
                
                AppointmentData appointment = new AppointmentData(client, dog, hour, minute, location, 
                                                                  cashReward, playerExpReward, primaryField, 
                                                                  specialistExpReward, staminaCost);
                gameData.availableAppointments.Add(appointment);
            }
        }
        
        public bool AcceptAppointment(AppointmentData appointment)
        {
            if (!gameData.availableAppointments.Contains(appointment)) return false;
            
            appointment.isAccepted = true;
            gameData.availableAppointments.Remove(appointment);
            gameData.acceptedAppointments.Add(appointment);
            
            return true;
        }
        
        private void CheckAppointmentTimes(GameDateTime currentTime)
        {
            foreach (var appointment in gameData.acceptedAppointments.ToList())
            {
                if (!appointment.isCompleted && 
                    appointment.scheduledHour == currentTime.hour && 
                    appointment.scheduledMinute == currentTime.minute)
                {
                    NotifyAppointmentTime(appointment);
                }
            }
        }
        
        private void NotifyAppointmentTime(AppointmentData appointment)
        {
            if (GameManager.Instance?.UIManager != null)
            {
                GameManager.Instance.UIManager.ShowAppointmentNotification(appointment);
            }
        }
        
        public bool TryStartLessonAtLocation(string locationName)
        {
            var currentAppointment = GetCurrentAppointmentAtLocation(locationName);
            if (currentAppointment != null)
            {
                // DOG COACH SYSTEM: Check stamina before starting lesson
                var progressionManager = GameManager.Instance?.ProgressionManager;
                if (progressionManager != null)
                {
                    if (!progressionManager.CanAffordActivity(currentAppointment.staminaCost))
                    {
                        Debug.LogWarning($"[AppointmentManager] Not enough stamina for lesson! Need: {currentAppointment.staminaCost}, Have: {progressionManager.PlayerStats.currentStamina}");
                        // TODO: Show UI message about insufficient stamina
                        return false;
                    }
                    
                    // Spend stamina before starting
                    progressionManager.SpendStamina(currentAppointment.staminaCost);
                }
                
                StartLesson(currentAppointment);
                return true;
            }
            return false;
        }
        
        private AppointmentData GetCurrentAppointmentAtLocation(string locationName)
        {
            var currentTime = GameManager.Instance?.TimeSystem?.CurrentTime;
            if (currentTime == null) return null;
            
            return gameData.acceptedAppointments.FirstOrDefault(appointment =>
                !appointment.isCompleted &&
                appointment.location.Equals(locationName, System.StringComparison.OrdinalIgnoreCase) &&
                IsAppointmentTimeNow(appointment, currentTime.Value));
        }
        
        private bool IsAppointmentTimeNow(AppointmentData appointment, GameDateTime currentTime)
        {
            // Allow starting lesson within 30 minutes of scheduled time
            int timeDifference = Mathf.Abs((currentTime.hour * 60 + currentTime.minute) - 
                                         (appointment.scheduledHour * 60 + appointment.scheduledMinute));
            return timeDifference <= 30;
        }
        
        private void StartLesson(AppointmentData appointment)
        {
            if (GameManager.Instance?.UIManager != null)
            {
                Debug.Log($"[AppointmentManager] 🐕 Starting Dog Coach lesson:");
                Debug.Log($"  Field: {appointment.primaryField}");
                Debug.Log($"  Location: {appointment.location}");
                Debug.Log($"  Client: {appointment.clientName} with {appointment.dogName}");
                
                // DOG COACH SYSTEM: Route to appropriate mini-game based on specialist field
                switch (appointment.primaryField)
                {
                    case TennisCoachCho.Data.SpecialistField.Handling:
                        // Handling activities use agility training mini-game
                        StartHandlingActivity(appointment);
                        break;
                        
                    case TennisCoachCho.Data.SpecialistField.Behaviorism:
                        // Behaviorism activities use behavior correction mini-game
                        StartBehaviorismActivity(appointment);
                        break;
                        
                    case TennisCoachCho.Data.SpecialistField.Artisanry:
                        // Artisanry activities use crafting mini-game
                        StartArtisanryActivity(appointment);
                        break;
                        
                    case TennisCoachCho.Data.SpecialistField.Management:
                        // Management activities use event management mini-game
                        StartManagementActivity(appointment);
                        break;
                        
                    default:
                        Debug.LogWarning($"[AppointmentManager] Unknown specialist field: {appointment.primaryField}, falling back to rhythm game");
                        GameManager.Instance.UIManager.StartRhythmMiniGame(appointment);
                        break;
                }
            }
        }
        
        private void StartHandlingActivity(AppointmentData appointment)
        {
            // For now, use the existing tennis drill mini-game for Handling activities
            // This represents agility training with dogs
            var tennisDrillGame = UnityEngine.Object.FindObjectOfType<TennisCoachCho.MiniGames.TennisDrillMiniGame>();
            if (tennisDrillGame != null)
            {
                Debug.Log($"[AppointmentManager] Starting Handling (Agility Training) mini-game");
                tennisDrillGame.StartMiniGame(appointment);
            }
            else
            {
                Debug.LogWarning("[AppointmentManager] Agility training mini-game not found, using rhythm game");
                GameManager.Instance.UIManager.StartRhythmMiniGame(appointment);
            }
        }
        
        private void StartBehaviorismActivity(AppointmentData appointment)
        {
            // For now, use rhythm game for Behaviorism activities
            // TODO: Create dedicated behavior correction mini-game
            Debug.Log($"[AppointmentManager] Starting Behaviorism (Behavior Correction) activity");
            GameManager.Instance.UIManager.StartRhythmMiniGame(appointment);
        }
        
        private void StartArtisanryActivity(AppointmentData appointment)
        {
            // For now, use rhythm game for Artisanry activities  
            // TODO: Create dedicated crafting mini-game
            Debug.Log($"[AppointmentManager] Starting Artisanry (Crafting) activity");
            GameManager.Instance.UIManager.StartRhythmMiniGame(appointment);
        }
        
        private void StartManagementActivity(AppointmentData appointment)
        {
            // For now, use rhythm game for Management activities
            // TODO: Create dedicated event management mini-game
            Debug.Log($"[AppointmentManager] Starting Management (Event/Competition) activity");
            GameManager.Instance.UIManager.StartRhythmMiniGame(appointment);
        }
        
        public void CompleteLesson(AppointmentData appointment, float performanceScore)
        {
            appointment.isCompleted = true;
            
            // DOG COACH SYSTEM: Fixed rewards (not performance-based)
            int cashEarned = appointment.cashReward;
            int playerExpEarned = appointment.playerExpReward;
            int specialistExpEarned = appointment.specialistExpReward;
            
            Debug.Log($"[AppointmentManager] 🐕 Dog Coach Lesson Completed:");
            Debug.Log($"  Client: {appointment.clientName} with {appointment.dogName}");
            Debug.Log($"  Location: {appointment.location}");
            Debug.Log($"  Cash Earned: {cashEarned}");
            Debug.Log($"  Player XP: {playerExpEarned}");
            Debug.Log($"  {appointment.primaryField} XP: {specialistExpEarned}");
            Debug.Log($"  Performance Score: {performanceScore:F2}");
            
            // Award rewards through new Dog Coach system
            var progressionManager = GameManager.Instance?.ProgressionManager;
            if (progressionManager != null)
            {
                // Award cash (unchanged)
                progressionManager.AddCash(cashEarned);
                
                // Award Player XP (global progression)
                progressionManager.AddPlayerExp(playerExpEarned);
                
                // Award Specialist Field XP (specific pillar progression)
                progressionManager.AddSpecialistExp(appointment.primaryField, specialistExpEarned);
            }
            
            // Show completion feedback with new system
            GameManager.Instance?.UIManager?.ShowLessonComplete(cashEarned, playerExpEarned, performanceScore);
        }
        
        // DOG COACH SYSTEM: Removed old GetSkillLevel method
        // Skills are now managed through ProgressionManager.GetSpecialistLevel(SpecialistField)
        
        private void ClearCompletedAppointments()
        {
            gameData.acceptedAppointments.RemoveAll(appointment => appointment.isCompleted);
        }
        
        private void CreateTestAppointments()
        {
            Debug.Log("[AppointmentManager] Creating test appointments for debugging...");
            
            // Clear existing appointments for clean testing
            gameData.acceptedAppointments.Clear();
            Debug.Log("[AppointmentManager] Cleared existing appointments");
            
            // Get current time from time system
            var currentTime = GameManager.Instance?.TimeSystem?.CurrentTime;
            if (currentTime == null)
            {
                Debug.LogWarning("[AppointmentManager] Cannot create test appointments - no time system");
                return;
            }
            
            // Create a test appointment at Tennis Court A in 5 minutes from current time
            int testHour = currentTime.Value.hour;
            int testMinute = currentTime.Value.minute + 5;
            
            // Handle minute overflow
            if (testMinute >= 60)
            {
                testMinute -= 60;
                testHour++;
            }
            
            var testAppointment = new AppointmentData(
                "John Smith",
                "Buddy", 
                testHour,
                testMinute,
                "Tennis Court A",  // This must match the LocationTrigger's LocationName exactly
                100,
                50,
                TennisCoachCho.Data.SpecialistField.Handling,
                30,
                15
            );
            testAppointment.isAccepted = true; // Mark as accepted
            
            gameData.acceptedAppointments.Add(testAppointment);
            
            Debug.Log($"[AppointmentManager] ✅ Created test appointment:");
            Debug.Log($"[AppointmentManager] Client: {testAppointment.clientName}");
            Debug.Log($"[AppointmentManager] Time: {testAppointment.GetTimeString()}");
            Debug.Log($"[AppointmentManager] Location: {testAppointment.location}");
            Debug.Log($"[AppointmentManager] Current time: {currentTime.Value.GetTimeString()}");
            
            // Create another appointment 30 minutes later for testing
            int testHour2 = testHour;
            int testMinute2 = testMinute + 30;
            
            if (testMinute2 >= 60)
            {
                testMinute2 -= 60;
                testHour2++;
            }
            
            var testAppointment2 = new AppointmentData(
                "Sarah Johnson",
                "Luna",
                testHour2,
                testMinute2,
                "Tennis Court A",
                120,
                60,
                TennisCoachCho.Data.SpecialistField.Behaviorism,
                35,
                18
            );
            testAppointment2.isAccepted = true;
            
            gameData.acceptedAppointments.Add(testAppointment2);
            
            Debug.Log($"[AppointmentManager] ✅ Created second test appointment:");
            Debug.Log($"[AppointmentManager] Client: {testAppointment2.clientName}");
            Debug.Log($"[AppointmentManager] Time: {testAppointment2.GetTimeString()}");
        }
    }
}