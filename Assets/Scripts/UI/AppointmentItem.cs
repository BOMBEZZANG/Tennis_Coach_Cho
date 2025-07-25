using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TennisCoachCho.Data;

namespace TennisCoachCho.UI
{
    public class AppointmentItem : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI clientNameText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI locationText;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private Button acceptButton;
        
        private AppointmentData appointmentData;
        private System.Action<AppointmentData> onAcceptCallback;
        
        public void Setup(AppointmentData appointment, System.Action<AppointmentData> onAccept)
        {
            appointmentData = appointment;
            onAcceptCallback = onAccept;
            
            UpdateUI();
            
            if (acceptButton != null)
            {
                acceptButton.onClick.RemoveAllListeners();
                acceptButton.onClick.AddListener(AcceptAppointment);
            }
            else
            {
                Debug.LogError("acceptButton is null!");
            }
        }
        
        private void UpdateUI()
        {
            if (appointmentData == null) 
            {
                Debug.LogError("AppointmentData is null in UpdateUI!");
                return;
            }
            
            if (clientNameText != null)
                clientNameText.text = appointmentData.clientName;
            else
                Debug.LogError("clientNameText is null!");
                
            if (timeText != null)
                timeText.text = appointmentData.GetTimeString();
            else
                Debug.LogError("timeText is null!");
                
            if (locationText != null)
                locationText.text = appointmentData.location;
            else
                Debug.LogError("locationText is null!");
                
            if (rewardText != null)
                rewardText.text = $"${appointmentData.cashReward} | {appointmentData.playerExpReward} Player XP | {appointmentData.specialistExpReward} {appointmentData.primaryField} XP";
            else
                Debug.LogError("rewardText is null!");
        }
        
        private void AcceptAppointment()
        {
            if (appointmentData != null && onAcceptCallback != null)
            {
                onAcceptCallback.Invoke(appointmentData);
            }
        }
        
        public void SetInteractable(bool interactable)
        {
            if (acceptButton != null)
                acceptButton.interactable = interactable;
        }
        
        public AppointmentData GetAppointmentData()
        {
            return appointmentData;
        }
    }
}