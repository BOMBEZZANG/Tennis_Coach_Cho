using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TennisCoachCho.Core;
using TennisCoachCho.Data;
using TMPro;
namespace TennisCoachCho.UI
{
    public class LessonBoardApp : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Transform appointmentListParent;
        [SerializeField] private GameObject appointmentItemPrefab;
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI noAppointmentsText;
        
        private List<GameObject> appointmentItems = new List<GameObject>();
        
        public void Initialize()
        {
            if (backButton != null)
                backButton.onClick.AddListener(GoBack);
                
            RefreshAppointments();
        }
        
        public void RefreshAppointments()
        {
            ClearAppointmentList();
            PopulateAppointmentList();
        }
        
        private void ClearAppointmentList()
        {
            foreach (var item in appointmentItems)
            {
                if (item != null)
                    Destroy(item);
            }
            appointmentItems.Clear();
        }
        
        private void PopulateAppointmentList()
        {
            if (GameManager.Instance?.AppointmentManager == null) return;
            
            var availableAppointments = GameManager.Instance.AppointmentManager.AvailableAppointments;
            
            if (availableAppointments.Count == 0)
            {
                if (noAppointmentsText != null)
                    noAppointmentsText.gameObject.SetActive(true);
                return;
            }
            
            if (noAppointmentsText != null)
                noAppointmentsText.gameObject.SetActive(false);
                
            // Fix ScrollRect sizing before populating
            FixScrollRectSizing();
                
            foreach (var appointment in availableAppointments)
            {
                CreateAppointmentItem(appointment);
            }
        }
        
        private void FixScrollRectSizing()
        {
            var scrollRect = appointmentListParent.GetComponentInParent<UnityEngine.UI.ScrollRect>();
            if (scrollRect != null)
            {
                // Fix Viewport size to properly fill the ScrollRect area
                if (scrollRect.viewport != null)
                {
                    var viewportRect = scrollRect.viewport;
                    viewportRect.anchorMin = Vector2.zero;
                    viewportRect.anchorMax = Vector2.one;
                    viewportRect.sizeDelta = Vector2.zero;
                    viewportRect.anchoredPosition = Vector2.zero;
                    
                    // CRITICAL: Disable the Mask component that was clipping content
                    var mask = scrollRect.viewport.GetComponent<UnityEngine.UI.Mask>();
                    if (mask != null)
                    {
                        mask.enabled = false;
                    }
                }
                
                // Fix Content size for proper scrolling
                if (scrollRect.content != null)
                {
                    var contentRect = scrollRect.content;
                    contentRect.anchorMin = new Vector2(0, 1);
                    contentRect.anchorMax = new Vector2(1, 1);
                    contentRect.sizeDelta = new Vector2(0, 300);
                    contentRect.anchoredPosition = new Vector2(0, 0);
                }
            }
        }
        
        private void CreateAppointmentItem(AppointmentData appointment)
        {
            if (appointmentItemPrefab == null || appointmentListParent == null) return;
            
            GameObject item = Instantiate(appointmentItemPrefab, appointmentListParent);
            appointmentItems.Add(item);
            
            // Setup item data
            var itemComponent = item.GetComponent<AppointmentItem>();
            if (itemComponent != null)
            {
                itemComponent.Setup(appointment, OnAppointmentAccepted);
            }
            else
            {
                // Fallback: setup with basic UI components
                SetupBasicAppointmentItem(item, appointment);
            }
        }
        
        private void SetupBasicAppointmentItem(GameObject item, AppointmentData appointment)
        {
            // Find UI components in the item
            var texts = item.GetComponentsInChildren<Text>();
            var button = item.GetComponentInChildren<Button>();
            
            if (texts.Length >= 4)
            {
                texts[0].text = appointment.clientName;
                texts[1].text = appointment.GetTimeString();
                texts[2].text = appointment.location;
                texts[3].text = $"${appointment.cashReward} | {appointment.playerExpReward} Player XP | {appointment.specialistExpReward} {appointment.primaryField} XP";
            }
            
            if (button != null)
            {
                button.onClick.AddListener(() => OnAppointmentAccepted(appointment));
            }
        }
        
        private void OnAppointmentAccepted(AppointmentData appointment)
        {
            if (GameManager.Instance?.AppointmentManager != null)
            {
                bool success = GameManager.Instance.AppointmentManager.AcceptAppointment(appointment);
                if (success)
                {
                    RefreshAppointments();
                    
                    // Show confirmation message
                    ShowAcceptedMessage(appointment);
                }
            }
        }
        
        private void ShowAcceptedMessage(AppointmentData appointment)
        {
            // For now, just refresh. In a full implementation, you might show a popup
            Debug.Log($"Accepted appointment with {appointment.clientName} at {appointment.GetTimeString()}");
        }
        
        private void GoBack()
        {
            var smartphoneUI = GetComponentInParent<SmartphoneUI>();
            if (smartphoneUI != null)
            {
                smartphoneUI.ShowAppMenu();
            }
        }
    }
}