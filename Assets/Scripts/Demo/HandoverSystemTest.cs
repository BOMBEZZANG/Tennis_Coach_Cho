using UnityEngine;
using TennisCoachCho.Core;
using TennisCoachCho.NPCs;
using TennisCoachCho.UI;

[System.Serializable]
public class HandoverSystemTest : MonoBehaviour
{
    [Header("System Testing")]
    [SerializeField] private bool runTestsOnStart = false;
    
    private void Start()
    {
        if (runTestsOnStart)
        {
            Invoke(nameof(RunAllTests), 2f);
        }
    }
    
    [ContextMenu("Run All Tests")]
    public void RunAllTests()
    {
        Debug.Log("=== DOG HANDOVER SYSTEM TESTS ===");
        TestGameManagerSystems();
        TestHandoverManager();
        TestPlayerController();
        TestUIComponents();
        Debug.Log("=== TESTS COMPLETE ===");
    }
    
    private void TestGameManagerSystems()
    {
        Debug.Log("Testing GameManager systems...");
        
        bool passed = true;
        
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager.Instance is null!");
            passed = false;
        }
        else
        {
            Debug.Log("✅ GameManager.Instance found");
            
            if (GameManager.Instance.PlayerController == null)
            {
                Debug.LogError("❌ PlayerController not assigned to GameManager");
                passed = false;
            }
            else
            {
                Debug.Log("✅ PlayerController found in GameManager");
            }
            
            if (GameManager.Instance.UIManager == null)
            {
                Debug.LogError("❌ UIManager not assigned to GameManager");
                passed = false;
            }
            else
            {
                Debug.Log("✅ UIManager found in GameManager");
            }
            
            if (GameManager.Instance.TimeSystem == null)
            {
                Debug.LogError("❌ TimeSystem not assigned to GameManager");
                passed = false;
            }
            else
            {
                Debug.Log("✅ TimeSystem found in GameManager");
            }
        }
        
        Debug.Log("GameManager Test: " + (passed ? "PASSED" : "FAILED"));
    }
    
    private void TestHandoverManager()
    {
        Debug.Log("Testing HandoverManager...");
        
        bool passed = true;
        
        if (HandoverManager.Instance == null)
        {
            Debug.LogWarning("⚠️ HandoverManager.Instance is null - this is OK if not manually created yet");
            
            // Try to create one for testing
            GameObject handoverObj = new GameObject("Test HandoverManager");
            handoverObj.AddComponent<HandoverManager>();
            
            if (HandoverManager.Instance != null)
            {
                Debug.Log("✅ HandoverManager successfully created");
            }
            else
            {
                Debug.LogError("❌ Failed to create HandoverManager");
                passed = false;
            }
        }
        else
        {
            Debug.Log("✅ HandoverManager.Instance found");
        }
        
        Debug.Log("HandoverManager Test: " + (passed ? "PASSED" : "FAILED"));
    }
    
    private void TestPlayerController()
    {
        Debug.Log("Testing PlayerController...");
        
        bool passed = true;
        var playerController = GameManager.Instance?.PlayerController;
        
        if (playerController == null)
        {
            Debug.LogError("❌ PlayerController not found");
            passed = false;
        }
        else
        {
            Debug.Log("✅ PlayerController found");
            
            // Test input enable/disable
            bool originalCanMove = playerController.CanMove;
            playerController.SetInputEnabled(false);
            
            if (playerController.CanMove)
            {
                Debug.LogError("❌ SetInputEnabled(false) did not disable movement");
                passed = false;
            }
            else
            {
                Debug.Log("✅ SetInputEnabled(false) works");
            }
            
            playerController.SetInputEnabled(true);
            if (!playerController.CanMove)
            {
                Debug.LogError("❌ SetInputEnabled(true) did not enable movement");
                passed = false;
            }
            else
            {
                Debug.Log("✅ SetInputEnabled(true) works");
            }
        }
        
        Debug.Log("PlayerController Test: " + (passed ? "PASSED" : "FAILED"));
    }
    
    private void TestUIComponents()
    {
        Debug.Log("Testing UI Components...");
        
        bool passed = true;
        var uiManager = GameManager.Instance?.UIManager;
        
        if (uiManager == null)
        {
            Debug.LogError("❌ UIManager not found");
            passed = false;
        }
        else
        {
            Debug.Log("✅ UIManager found");
            
            // Test quest update method
            try
            {
                uiManager.ShowQuestUpdate("Test quest update");
                Debug.Log("✅ ShowQuestUpdate method works");
            }
            catch (System.Exception e)
            {
                Debug.LogError("❌ ShowQuestUpdate failed: " + e.Message);
                passed = false;
            }
            
            // Test temporary message method
            try
            {
                uiManager.ShowTemporaryMessage("Test temporary message", 1f);
                Debug.Log("✅ ShowTemporaryMessage method works");
            }
            catch (System.Exception e)
            {
                Debug.LogError("❌ ShowTemporaryMessage failed: " + e.Message);
                passed = false;
            }
        }
        
        // Test DialogueUI
        var dialogueUI = FindObjectOfType<DialogueUI>();
        if (dialogueUI == null)
        {
            Debug.LogWarning("⚠️ DialogueUI not found in scene - this is OK if not manually created");
        }
        else
        {
            Debug.Log("✅ DialogueUI found in scene");
        }
        
        Debug.Log("UI Components Test: " + (passed ? "PASSED" : "FAILED"));
    }
    
    [ContextMenu("Test NPC Creation")]
    public void TestNPCCreation()
    {
        Debug.Log("Testing NPC Creation...");
        
        // Test ClientNPC creation
        GameObject clientObj = new GameObject("Test Client");
        var client = clientObj.AddComponent<ClientNPC>();
        
        if (client != null)
        {
            Debug.Log("✅ ClientNPC component created successfully");
        }
        else
        {
            Debug.LogError("❌ Failed to create ClientNPC component");
        }
        
        // Test DogNPC creation
        GameObject dogObj = new GameObject("Test Dog");
        var dog = dogObj.AddComponent<DogNPC>();
        
        if (dog != null)
        {
            Debug.Log("✅ DogNPC component created successfully");
            
            // Test state change
            dog.ChangeState(DogNPC.DogState.Follow);
            if (dog.CurrentState == DogNPC.DogState.Follow)
            {
                Debug.Log("✅ DogNPC state change works");
            }
            else
            {
                Debug.LogError("❌ DogNPC state change failed");
            }
        }
        else
        {
            Debug.LogError("❌ Failed to create DogNPC component");
        }
        
        // Clean up test objects
        DestroyImmediate(clientObj);
        DestroyImmediate(dogObj);
        
        Debug.Log("NPC Creation Test Complete");
    }
    
    [ContextMenu("Show System Requirements")]
    public void ShowSystemRequirements()
    {
        Debug.Log("=== DOG HANDOVER SYSTEM REQUIREMENTS ===");
        Debug.Log("For the handover system to work, you need:");
        Debug.Log("1. GameManager with PlayerController, UIManager, and TimeSystem assigned");
        Debug.Log("2. A scene with Canvas for UI elements");
        Debug.Log("3. Player GameObject with PlayerController component");
        Debug.Log("4. (Optional) HandoverManager will be created automatically if needed");
        Debug.Log("5. (Optional) DialogueUI will be created automatically if needed");
        Debug.Log("");
        Debug.Log("To test the system:");
        Debug.Log("1. Use HandoverDemo component to set up a test scenario");
        Debug.Log("2. Or manually create ClientNPC and DogNPC GameObjects");
        Debug.Log("3. Ensure the appointment time matches the current game time");
        Debug.Log("=========================================");
    }
}