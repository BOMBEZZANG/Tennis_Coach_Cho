using System.Collections;
using UnityEngine;
using TennisCoachCho.Data;
using TennisCoachCho.UI;
using TennisCoachCho.NPCs;

namespace TennisCoachCho.Core
{
    public class HandoverManager : MonoBehaviour
    {
        [Header("Handover Settings")]
        [SerializeField] private float handoverDelay = 1.5f;
        
        [Header("UI References")]
        [SerializeField] private DialogueUI dialogueUI;
        
        private bool isHandoverInProgress = false;
        
        public static HandoverManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public bool CanStartHandover(AppointmentData appointment)
        {
            if (isHandoverInProgress)
            {
                Debug.LogWarning("[HandoverManager] Handover already in progress!");
                return false;
            }
            
            if (appointment == null || !appointment.isAccepted)
            {
                Debug.LogWarning("[HandoverManager] No valid appointment for handover!");
                return false;
            }
            
            // Check if within appointment time window
            var currentTime = GameManager.Instance?.TimeSystem?.CurrentTime;
            if (currentTime == null)
            {
                Debug.LogWarning("[HandoverManager] No time system available!");
                return false;
            }
            
            if (!IsWithinAppointmentWindow(appointment, currentTime.Value))
            {
                Debug.LogWarning("[HandoverManager] Not within appointment time window!");
                return false;
            }
            
            return true;
        }
        
        private bool IsWithinAppointmentWindow(AppointmentData appointment, GameDateTime currentTime)
        {
            // Allow handover within 30 minutes of scheduled time
            int timeDifference = Mathf.Abs((currentTime.hour * 60 + currentTime.minute) - 
                                         (appointment.scheduledHour * 60 + appointment.scheduledMinute));
            return timeDifference <= 30;
        }
        
        public void StartHandover(ClientNPC client, DogNPC dog, AppointmentData appointment)
        {
            if (!CanStartHandover(appointment))
                return;
                
            StartCoroutine(HandoverSequence(client, dog, appointment));
        }
        
        private IEnumerator HandoverSequence(ClientNPC client, DogNPC dog, AppointmentData appointment)
        {
            isHandoverInProgress = true;
            
            Debug.Log("[HandoverManager] 🐕 Starting Dog Handover Sequence");
            Debug.Log("[HandoverManager] Client: " + appointment.clientName + ", Dog: " + appointment.dogName);
            
            // Step 1: Show dialogue
            string dialogueText = "Thanks for coming, " + GetPlayerName() + "! " +
                                "Please take " + appointment.dogName + " for a 'calm walk' in the park. " +
                                "Be careful, he gets excited when he sees birds!";
            
            bool dialogueCompleted = false;
            
            if (dialogueUI != null)
            {
                dialogueUI.ShowDialogue(dialogueText, "Take the leash", () => {
                    dialogueCompleted = true;
                });
                
                // Wait for player to confirm dialogue
                yield return new WaitUntil(() => dialogueCompleted);
            }
            else
            {
                Debug.LogWarning("[HandoverManager] No dialogue UI assigned! Skipping dialogue.");
                yield return new WaitForSeconds(0.5f);
            }
            
            // Step 2: Freeze input and execute handover
            Debug.Log("[HandoverManager] Freezing input for handover...");
            FreezePlayerInput(true);
            
            // Placeholder for SFX
            Debug.Log("SFX_Leash_Click_Plays_Here");
            
            // Step 3: Wait for handover delay (placeholder for animation)
            yield return new WaitForSeconds(handoverDelay);
            
            // Step 4: Execute state changes
            Debug.Log("[HandoverManager] Executing dog state change...");
            
            if (dog != null)
            {
                dog.ChangeState(DogNPC.DogState.Follow);
                dog.SetFollowTarget(GameManager.Instance?.PlayerController?.transform);
            }
            
            if (client != null)
            {
                client.CompleteHandover();
            }
            
            // Step 5: Update quest
            UpdateQuest(appointment);
            
            // Step 6: Unfreeze input
            Debug.Log("[HandoverManager] Unfreezing input...");
            FreezePlayerInput(false);
            
            isHandoverInProgress = false;
            
            Debug.Log("[HandoverManager] ✅ Dog Handover Sequence Complete!");
        }
        
        private void FreezePlayerInput(bool freeze)
        {
            var playerController = GameManager.Instance?.PlayerController;
            if (playerController != null)
            {
                playerController.SetInputEnabled(!freeze);
            }
        }
        
        private string GetPlayerName()
        {
            // TODO: Get actual player name from save system
            return "Coach";
        }
        
        private void UpdateQuest(AppointmentData appointment)
        {
            Debug.Log("[HandoverManager] 🎯 Quest Updated: Go to the Park Walking Trail with " + appointment.dogName + ".");
            
            // TODO: Update quest log UI
            if (GameManager.Instance?.UIManager != null)
            {
                string questUpdate = "[Quest Updated] Go to the Park Walking Trail with " + appointment.dogName + ".";
                GameManager.Instance.UIManager.ShowQuestUpdate(questUpdate);
            }
        }
        
        public bool IsHandoverInProgress()
        {
            return isHandoverInProgress;
        }
    }
}