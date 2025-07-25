using UnityEngine;
using TennisCoachCho.Core;
using TennisCoachCho.Data;
using TennisCoachCho.NPCs;
using TennisCoachCho.UI;

namespace TennisCoachCho.Demo
{
    public class HandoverDemo : MonoBehaviour
    {
        [Header("Demo Settings")]
        [SerializeField] private bool autoSetupDemo = true;
        [SerializeField] private Vector3 clientPosition = new Vector3(-5f, 0f, 0f);
        [SerializeField] private Vector3 dogPosition = new Vector3(-4f, 0f, 0f);
        [SerializeField] private Vector3 playerSpawnPosition = new Vector3(0f, 0f, 0f);
        
        [Header("Prefab References (Optional)")]
        [SerializeField] private GameObject clientPrefab;
        [SerializeField] private GameObject dogPrefab;
        [SerializeField] private GameObject questMarkerPrefab;
        
        private ClientNPC spawnedClient;
        private DogNPC spawnedDog;
        private AppointmentData demoAppointment;
        
        private void Start()
        {
            if (autoSetupDemo)
            {
                Invoke(nameof(SetupDemo), 1f); // Delay to ensure GameManager is initialized
            }
        }
        
        [ContextMenu("Setup Demo")]
        public void SetupDemo()
        {
            Debug.Log("[HandoverDemo] Setting up Dog Handover demo...");
            
            // Create demo appointment
            var currentTime = GameManager.Instance?.TimeSystem?.CurrentTime;
            if (currentTime == null)
            {
                Debug.LogWarning("[HandoverDemo] No time system available, using default time");
                CreateDemoAppointmentWithDefaultTime();
            }
            else
            {
                CreateDemoAppointment(currentTime.Value);
            }
            
            // Spawn NPCs
            SpawnClient();
            SpawnDog();
            
            // Link client and dog
            if (spawnedClient != null && spawnedDog != null)
            {
                spawnedClient.SetAppointment(demoAppointment);
                spawnedClient.SetAssignedDog(spawnedDog);
                spawnedDog.SetOwner(spawnedClient.transform);
                spawnedDog.SetDogName(demoAppointment.dogName);
            }
            
            // Position player
            var playerController = GameManager.Instance?.PlayerController;
            if (playerController != null)
            {
                playerController.transform.position = playerSpawnPosition;
                Debug.Log("[HandoverDemo] Player positioned at " + playerSpawnPosition);
            }
            
            // Setup HandoverManager if needed
            EnsureHandoverManager();
            
            Debug.Log("[HandoverDemo] ✅ Demo setup complete!");
            LogDemoInstructions();
        }
        
        private void CreateDemoAppointment(TennisCoachCho.Core.GameDateTime currentTime)
        {
            // Create appointment 5 minutes from now
            int appointmentHour = currentTime.hour;
            int appointmentMinute = currentTime.minute + 5;
            
            if (appointmentMinute >= 60)
            {
                appointmentMinute -= 60;
                appointmentHour++;
            }
            
            demoAppointment = new AppointmentData(
                "Demo Client",      // clientName
                "Demo Dog",         // dogName
                appointmentHour,    // scheduledHour
                appointmentMinute,  // scheduledMinute
                "Client House",     // location
                50,                 // cashReward
                25,                 // playerExpReward
                SpecialistField.Handling, // primaryField
                20,                 // specialistExpReward
                15                  // staminaCost
            );
            
            demoAppointment.isAccepted = true;
            
            Debug.Log("[HandoverDemo] Created demo appointment for " + appointmentHour.ToString("00") + ":" + appointmentMinute.ToString("00"));
        }
        
        private void CreateDemoAppointmentWithDefaultTime()
        {
            demoAppointment = new AppointmentData(
                "Demo Client",
                "Demo Dog",
                10, // 10:00 AM
                30, // 10:30 AM
                "Client House",
                50,
                25,
                SpecialistField.Handling,
                20,
                15
            );
            
            demoAppointment.isAccepted = true;
            
            Debug.Log("[HandoverDemo] Created demo appointment for 10:30");
        }
        
        private void SpawnClient()
        {
            GameObject clientObj;
            
            if (clientPrefab != null)
            {
                clientObj = Instantiate(clientPrefab, clientPosition, Quaternion.identity);
            }
            else
            {
                // Create client from scratch
                clientObj = new GameObject("Demo Client");
                clientObj.transform.position = clientPosition;
                
                // Add required components
                clientObj.AddComponent<SpriteRenderer>();
                clientObj.AddComponent<Collider2D>().isTrigger = true;
            }
            
            // Ensure ClientNPC component
            spawnedClient = clientObj.GetComponent<ClientNPC>();
            if (spawnedClient == null)
            {
                spawnedClient = clientObj.AddComponent<ClientNPC>();
            }
            
            // Create quest marker
            CreateQuestMarker(clientObj);
            
            Debug.Log("[HandoverDemo] Client spawned at " + clientPosition);
        }
        
        private void SpawnDog()
        {
            GameObject dogObj;
            
            if (dogPrefab != null)
            {
                dogObj = Instantiate(dogPrefab, dogPosition, Quaternion.identity);
            }
            else
            {
                // Create dog from scratch
                dogObj = new GameObject("Demo Dog");
                dogObj.transform.position = dogPosition;
                
                // Add required components
                dogObj.AddComponent<SpriteRenderer>();
            }
            
            // Ensure DogNPC component
            spawnedDog = dogObj.GetComponent<DogNPC>();
            if (spawnedDog == null)
            {
                spawnedDog = dogObj.AddComponent<DogNPC>();
            }
            
            Debug.Log("[HandoverDemo] Dog spawned at " + dogPosition);
        }
        
        private void CreateQuestMarker(GameObject parent)
        {
            GameObject marker;
            
            if (questMarkerPrefab != null)
            {
                marker = Instantiate(questMarkerPrefab, parent.transform);
            }
            else
            {
                // Create simple quest marker
                marker = new GameObject("Quest Marker");
                marker.transform.SetParent(parent.transform);
                marker.transform.localPosition = new Vector3(0, 1f, 0); // Above client
                
                // Create simple exclamation mark sprite
                var renderer = marker.AddComponent<SpriteRenderer>();
                
                // Create a simple texture for the exclamation mark
                Texture2D texture = new Texture2D(16, 32);
                Color markerColor = Color.yellow;
                
                // Draw a simple exclamation mark pattern
                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        // Simple exclamation mark pattern
                        bool shouldColor = false;
                        if (x >= 6 && x <= 9) // Vertical line
                        {
                            if (y >= 8 && y <= 24) shouldColor = true; // Main line
                            if (y >= 2 && y <= 5) shouldColor = true;  // Dot
                        }
                        
                        texture.SetPixel(x, y, shouldColor ? markerColor : Color.clear);
                    }
                }
                texture.Apply();
                
                Sprite markerSprite = Sprite.Create(texture, new Rect(0, 0, 16, 32), new Vector2(0.5f, 0f));
                renderer.sprite = markerSprite;
                renderer.sortingOrder = 10; // Render above other objects
            }
            
            // Initially hide the marker (will be shown based on appointment time)
            marker.SetActive(false);
            
            Debug.Log("[HandoverDemo] Quest marker created");
        }
        
        private void EnsureHandoverManager()
        {
            if (HandoverManager.Instance == null)
            {
                GameObject handoverManagerObj = new GameObject("HandoverManager");
                var handoverManager = handoverManagerObj.AddComponent<HandoverManager>();
                
                // Try to find DialogueUI in scene
                var dialogueUI = FindObjectOfType<DialogueUI>();
                if (dialogueUI == null)
                {
                    // Create DialogueUI if not found
                    GameObject dialogueObj = new GameObject("DialogueUI");
                    dialogueUI = dialogueObj.AddComponent<DialogueUI>();
                }
                
                Debug.Log("[HandoverDemo] HandoverManager created and configured");
            }
        }
        
        private void LogDemoInstructions()
        {
            Debug.Log("===============================================");
            Debug.Log("🐕 DOG HANDOVER DEMO INSTRUCTIONS");
            Debug.Log("===============================================");
            Debug.Log("1. Move your player near the green square (Client)");
            Debug.Log("2. Wait for the quest marker (!) to appear");
            Debug.Log("3. Press 'E' to interact with the client");
            Debug.Log("4. Click 'Take the leash' in the dialogue");
            Debug.Log("5. Watch the dog (yellow circle) start following you!");
            Debug.Log("===============================================");
            Debug.Log("Client Position: " + clientPosition);
            Debug.Log("Dog Position: " + dogPosition);
            Debug.Log("Appointment Time: " + demoAppointment.GetTimeString());
            Debug.Log("===============================================");
        }
        
        [ContextMenu("Fast Forward to Appointment Time")]
        public void FastForwardToAppointmentTime()
        {
            if (demoAppointment != null && GameManager.Instance?.TimeSystem != null)
            {
                GameManager.Instance.TimeSystem.SetTime(demoAppointment.scheduledHour, demoAppointment.scheduledMinute);
                Debug.Log("[HandoverDemo] Time set to " + demoAppointment.GetTimeString());
            }
        }
        
        [ContextMenu("Clean Up Demo")]
        public void CleanUpDemo()
        {
            if (spawnedClient != null)
                DestroyImmediate(spawnedClient.gameObject);
            if (spawnedDog != null)
                DestroyImmediate(spawnedDog.gameObject);
                
            Debug.Log("[HandoverDemo] Demo cleaned up");
        }
        
        private void OnDrawGizmos()
        {
            // Draw demo positions
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(clientPosition, Vector3.one);
            Gizmos.DrawWireCube(dogPosition, Vector3.one);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(playerSpawnPosition, Vector3.one);
            
            // Draw labels
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(clientPosition + Vector3.up, "Client");
            UnityEditor.Handles.Label(dogPosition + Vector3.up, "Dog");
            UnityEditor.Handles.Label(playerSpawnPosition + Vector3.up, "Player Start");
            #endif
        }
    }
}