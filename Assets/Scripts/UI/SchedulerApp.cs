using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TennisCoachCho.Core;
using TennisCoachCho.Data;
using TennisCoachCho.Utilities;
using TMPro;

namespace TennisCoachCho.UI
{
    public class SchedulerApp : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Transform scheduleListParent;
        [SerializeField] private GameObject scheduleItemPrefab;
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI noScheduleText;
        [SerializeField] private TextMeshProUGUI currentTimeText;
        
        private List<GameObject> scheduleItems = new List<GameObject>();
        
        private void Awake()
        {
            DebugLogger.LogSeparator("SCHEDULER APP AWAKE");
            DebugLogger.LogSchedulerApp("SchedulerApp.Awake() called!");
            DebugLogger.LogGameObject(gameObject, "SchedulerApp in Awake");
            DebugLogger.LogSchedulerApp($"scheduleListParent is null: {scheduleListParent == null}");
            
            // Check if ScrollView needs to be created or fixed
            bool needsScrollViewCreation = scheduleListParent == null;
            bool needsScrollViewFix = false;
            
            if (scheduleListParent != null)
            {
                RectTransform contentRect = scheduleListParent as RectTransform;
                if (contentRect != null && contentRect.rect.width == 0)
                {
                    DebugLogger.LogSchedulerApp("ScrollView exists but has zero width - needs fixing!");
                    needsScrollViewFix = true;
                }
            }
            
            if (needsScrollViewCreation || needsScrollViewFix)
            {
                CreateScrollViewStructure();
            }
        }
        
        public void Initialize()
        {
            DebugLogger.LogSeparator("SCHEDULER APP INITIALIZE");
            DebugLogger.LogSchedulerApp("SchedulerApp.Initialize() called!");
            DebugLogger.LogGameObject(gameObject, "SchedulerApp in Initialize");
            
            if (backButton != null)
            {
                backButton.onClick.AddListener(GoBack);
                DebugLogger.LogSchedulerApp("Back button listener added");
            }
            else
            {
                DebugLogger.LogSchedulerApp("Back button is NULL!");
            }
                
            RefreshSchedule();
        }
        
        private void OnEnable()
        {
            DebugLogger.LogSeparator("SCHEDULER APP ON ENABLE");
            DebugLogger.LogSchedulerApp("SchedulerApp.OnEnable() called!");
            DebugLogger.LogGameObject(gameObject, "SchedulerApp in OnEnable");
            DebugLogger.LogSchedulerApp($"scheduleListParent is null: {scheduleListParent == null}");
            
            // Check if ScrollView needs to be created or fixed
            bool needsScrollViewCreation = scheduleListParent == null;
            bool needsScrollViewFix = false;
            
            if (scheduleListParent != null)
            {
                RectTransform contentRect = scheduleListParent as RectTransform;
                if (contentRect != null && contentRect.rect.width == 0)
                {
                    DebugLogger.LogSchedulerApp("ScrollView exists but has zero width in OnEnable - needs fixing!");
                    needsScrollViewFix = true;
                }
            }
            
            if (needsScrollViewCreation || needsScrollViewFix)
            {
                CreateScrollViewStructure();
            }
        }
        
        public void RefreshSchedule()
        {
            DebugLogger.LogSeparator("REFRESH SCHEDULE");
            DebugLogger.LogSchedulerApp("SchedulerApp.RefreshSchedule() called!");
            DebugLogger.LogSchedulerApp($"scheduleListParent is null: {scheduleListParent == null}");
            DebugLogger.LogSchedulerApp($"scheduleItemPrefab is null: {scheduleItemPrefab == null}");
            DebugLogger.LogSchedulerApp($"currentTimeText is null: {currentTimeText == null}");
            DebugLogger.LogSchedulerApp($"noScheduleText is null: {noScheduleText == null}");
            
            if (scheduleListParent != null)
            {
                DebugLogger.LogGameObject(scheduleListParent.gameObject, "scheduleListParent");
                DebugLogger.LogRectTransform((RectTransform)scheduleListParent, "scheduleListParent RectTransform");
            }
            
            UpdateCurrentTime();
            ClearScheduleList();
            PopulateScheduleList();
        }
        
        private void UpdateCurrentTime()
        {
            if (currentTimeText != null && GameManager.Instance?.TimeSystem != null)
            {
                var currentTime = GameManager.Instance.TimeSystem.CurrentTime;
                currentTimeText.text = $"Current Time: {currentTime.GetTimeString()} - {currentTime.GetDateString()}";
            }
        }
        
        private void ClearScheduleList()
        {
            foreach (var item in scheduleItems)
            {
                if (item != null)
                    Destroy(item);
            }
            scheduleItems.Clear();
        }
        
        private void PopulateScheduleList()
        {
            DebugLogger.LogSeparator("POPULATE SCHEDULE LIST");
            DebugLogger.LogSchedulerApp("SchedulerApp.PopulateScheduleList() called!");
            
            if (GameManager.Instance?.AppointmentManager == null) 
            {
                DebugLogger.LogSchedulerApp("GameManager or AppointmentManager is null!");
                DebugLogger.LogSchedulerApp($"GameManager.Instance is null: {GameManager.Instance == null}");
                if (GameManager.Instance != null)
                    DebugLogger.LogSchedulerApp($"AppointmentManager is null: {GameManager.Instance.AppointmentManager == null}");
                return;
            }
            
            var acceptedAppointments = GameManager.Instance.AppointmentManager.AcceptedAppointments;
            DebugLogger.LogSchedulerApp($"Found {acceptedAppointments.Count} accepted appointments");
            
            if (acceptedAppointments.Count == 0)
            {
                DebugLogger.LogSchedulerApp("No accepted appointments, showing noScheduleText");
                if (noScheduleText != null)
                {
                    noScheduleText.gameObject.SetActive(true);
                    DebugLogger.LogGameObject(noScheduleText.gameObject, "noScheduleText activated");
                }
                else
                {
                    DebugLogger.LogSchedulerApp("noScheduleText is NULL!");
                }
                return;
            }
            
            if (noScheduleText != null)
                noScheduleText.gameObject.SetActive(false);
                
            // Sort appointments by time
            var sortedAppointments = new List<AppointmentData>(acceptedAppointments);
            sortedAppointments.Sort((a, b) => 
            {
                int timeA = a.scheduledHour * 60 + a.scheduledMinute;
                int timeB = b.scheduledHour * 60 + b.scheduledMinute;
                return timeA.CompareTo(timeB);
            });
            
            DebugLogger.LogSchedulerApp($"Creating {sortedAppointments.Count} schedule items...");
            foreach (var appointment in sortedAppointments)
            {
                DebugLogger.LogAppointmentData(appointment);
                CreateScheduleItem(appointment);
            }
        }
        
        private void CreateScheduleItem(AppointmentData appointment)
        {
            DebugLogger.LogSeparator($"CREATE SCHEDULE ITEM: {appointment.clientName}");
            DebugLogger.LogSchedulerApp($"CreateScheduleItem called for {appointment.clientName}");
            DebugLogger.LogSchedulerApp($"scheduleItemPrefab is null: {scheduleItemPrefab == null}");
            DebugLogger.LogSchedulerApp($"scheduleListParent is null: {scheduleListParent == null}");
            
            // Try to find AppointmentItem prefab if not assigned
            if (scheduleItemPrefab == null)
            {
                DebugLogger.LogSchedulerApp("scheduleItemPrefab is null, trying to find AppointmentItem prefab...");
                // You can assign this manually in the Inspector, or we can try to find it
                var appointmentItemPrefab = Resources.Load<GameObject>("AppointmentItem");
                if (appointmentItemPrefab != null)
                {
                    scheduleItemPrefab = appointmentItemPrefab;
                    DebugLogger.LogSchedulerApp("Found AppointmentItem prefab in Resources!");
                }
            }
            
            if (scheduleItemPrefab == null || scheduleListParent == null) 
            {
                DebugLogger.LogError("Cannot create schedule item - prefab or parent is null!");
                DebugLogger.LogError("Please assign the scheduleItemPrefab in the SchedulerApp inspector!");
                return;
            }
            
            GameObject item = Instantiate(scheduleItemPrefab, scheduleListParent);
            scheduleItems.Add(item);
            
            DebugLogger.LogGameObject(item, $"Created schedule item for {appointment.clientName}");
            DebugLogger.LogRectTransform((RectTransform)item.transform, "Schedule item RectTransform");
            
            // Check parent hierarchy
            Transform current = item.transform;
            int level = 0;
            while (current != null && level < 5)
            {
                DebugLogger.LogGameObject(current.gameObject, $"Parent level {level}");
                current = current.parent;
                level++;
            }
            
            // Setup item data
            var itemComponent = item.GetComponent<ScheduleItem>();
            if (itemComponent != null)
            {
                itemComponent.Setup(appointment);
                DebugLogger.LogSchedulerApp("Used ScheduleItem component setup");
            }
            else
            {
                // Fallback: setup with basic UI components
                SetupBasicScheduleItem(item, appointment);
                DebugLogger.LogSchedulerApp("Used basic setup fallback");
            }
        }
        
        private void SetupBasicScheduleItem(GameObject item, AppointmentData appointment)
        {
            DebugLogger.LogSeparator("BASIC SCHEDULE ITEM SETUP");
            DebugLogger.LogAppointmentData(appointment);
            
            // Try TextMeshPro components first (more common in modern Unity projects)
            var tmpTexts = item.GetComponentsInChildren<TextMeshProUGUI>();
            var texts = item.GetComponentsInChildren<Text>();
            
            DebugLogger.LogSchedulerApp($"Found {tmpTexts.Length} TextMeshProUGUI components");
            DebugLogger.LogSchedulerApp($"Found {texts.Length} Text components");
            
            // Use TextMeshPro if available, otherwise fallback to Text
            if (tmpTexts.Length >= 4)
            {
                DebugLogger.LogSchedulerApp("Using TextMeshProUGUI components");
                tmpTexts[0].text = appointment.clientName;
                tmpTexts[1].text = appointment.GetTimeString();
                tmpTexts[2].text = appointment.location;
                
                // Show status and rewards
                string status = appointment.isCompleted ? "Completed" : 
                               IsUpcoming(appointment) ? "Upcoming" : "Scheduled";
                tmpTexts[3].text = $"${appointment.cashReward} | {appointment.playerExpReward} Player XP | {appointment.specialistExpReward} {appointment.primaryField} XP";
                
                // If there's a 5th text component, use it for status
                if (tmpTexts.Length >= 5)
                {
                    tmpTexts[4].text = status;
                    
                    // Color code based on status
                    Color statusColor = appointment.isCompleted ? Color.green :
                                       IsUpcoming(appointment) ? Color.yellow : Color.white;
                    tmpTexts[4].color = statusColor;
                }
                
                DebugLogger.LogSchedulerApp($"Set client name: {tmpTexts[0].text}");
                DebugLogger.LogSchedulerApp($"Set time: {tmpTexts[1].text}");
                DebugLogger.LogSchedulerApp($"Set location: {tmpTexts[2].text}");
                DebugLogger.LogSchedulerApp($"Set rewards: {tmpTexts[3].text}");
            }
            else if (texts.Length >= 4)
            {
                DebugLogger.LogSchedulerApp("Using Text components");
                texts[0].text = appointment.clientName;
                texts[1].text = appointment.GetTimeString();
                texts[2].text = appointment.location;
                
                // Show status
                string status = appointment.isCompleted ? "Completed" : 
                               IsUpcoming(appointment) ? "Upcoming" : "Scheduled";
                texts[3].text = status;
                
                // Color code based on status
                Color statusColor = appointment.isCompleted ? Color.green :
                                   IsUpcoming(appointment) ? Color.yellow : Color.white;
                texts[3].color = statusColor;
                
                DebugLogger.LogSchedulerApp($"Set client name: {texts[0].text}");
                DebugLogger.LogSchedulerApp($"Set time: {texts[1].text}");
                DebugLogger.LogSchedulerApp($"Set location: {texts[2].text}");
                DebugLogger.LogSchedulerApp($"Set status: {texts[3].text}");
            }
            else
            {
                DebugLogger.LogError($"AppointmentItem prefab doesn't have enough text components! TMPro: {tmpTexts.Length}, Text: {texts.Length}");
                
                // Log all available text components for debugging
                for (int i = 0; i < tmpTexts.Length; i++)
                {
                    DebugLogger.LogSchedulerApp($"TMPro[{i}]: {tmpTexts[i].name} - current text: '{tmpTexts[i].text}'");
                }
                for (int i = 0; i < texts.Length; i++)
                {
                    DebugLogger.LogSchedulerApp($"Text[{i}]: {texts[i].name} - current text: '{texts[i].text}'");
                }
            }
        }
        
        private bool IsUpcoming(AppointmentData appointment)
        {
            if (GameManager.Instance?.TimeSystem == null) return false;
            
            var currentTime = GameManager.Instance.TimeSystem.CurrentTime;
            int currentMinutes = currentTime.hour * 60 + currentTime.minute;
            int appointmentMinutes = appointment.scheduledHour * 60 + appointment.scheduledMinute;
            
            // Consider appointment "upcoming" if it's within the next 30 minutes
            return appointmentMinutes > currentMinutes && appointmentMinutes - currentMinutes <= 30;
        }
        
        private void GoBack()
        {
            var smartphoneUI = GetComponentInParent<SmartphoneUI>();
            if (smartphoneUI != null)
            {
                smartphoneUI.ShowAppMenu();
            }
        }
        
        /// <summary>
        /// Creates the ScrollView structure programmatically if it doesn't exist
        /// Based on the working LessonBoardApp configuration from Problem_History.txt
        /// </summary>
        private void CreateScrollViewStructure()
        {
            DebugLogger.LogSeparator("CREATE SCROLLVIEW STRUCTURE");
            DebugLogger.LogSchedulerApp("Creating ScrollView structure for SchedulerApp...");
            DebugLogger.LogGameObject(gameObject, "SchedulerApp before ScrollView creation");
            
            // Clear any existing ScrollView structure first
            Transform existingScrollView = transform.Find("ScheduleScrollView");
            if (existingScrollView != null)
            {
                DebugLogger.LogSchedulerApp("Found existing ScrollView, destroying it...");
                DestroyImmediate(existingScrollView.gameObject);
            }
            
            // Create main ScrollView GameObject
            GameObject scrollViewObj = new GameObject("ScheduleScrollView");
            scrollViewObj.transform.SetParent(transform);
            
            // Add required components
            RectTransform scrollRect = scrollViewObj.AddComponent<RectTransform>();
            scrollViewObj.AddComponent<CanvasRenderer>();
            
            // Add and configure Image (disabled like in LessonBoardApp)
            Image image = scrollViewObj.AddComponent<Image>();
            image.enabled = false;
            
            // Add ScrollRect component
            ScrollRect scrollRectComponent = scrollViewObj.AddComponent<ScrollRect>();
            
            // Configure ScrollView RectTransform (matching LessonBoardApp configuration)
            scrollRect.anchorMin = new Vector2(0.5f, 0.5f);
            scrollRect.anchorMax = new Vector2(0.5f, 0.5f);
            scrollRect.anchoredPosition = new Vector2(92, -318);
            scrollRect.sizeDelta = new Vector2(400, 400);
            scrollRect.pivot = new Vector2(0.5f, 0.5f);
            
            // Create Viewport
            GameObject viewportObj = new GameObject("Viewport");
            viewportObj.transform.SetParent(scrollViewObj.transform);
            
            RectTransform viewportRect = viewportObj.AddComponent<RectTransform>();
            Mask mask = viewportObj.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            Image viewportImage = viewportObj.AddComponent<Image>();
            viewportImage.color = Color.white;
            
            // Configure Viewport RectTransform - CRITICAL FIX from Problem_History.txt
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = Vector2.zero;
            viewportRect.anchoredPosition = Vector2.zero;
            viewportRect.pivot = new Vector2(0f, 1f);
            
            DebugLogger.LogSchedulerApp("Viewport configuration:");
            DebugLogger.LogRectTransform(viewportRect, "Viewport after configuration");
            
            // Create Content
            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(viewportObj.transform);
            
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            
            // Add VerticalLayoutGroup for automatic item arrangement
            VerticalLayoutGroup layoutGroup = contentObj.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false; // Allow dynamic height
            layoutGroup.spacing = 10f;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);
            
            // Add ContentSizeFitter for dynamic sizing
            ContentSizeFitter sizeFitter = contentObj.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            // Configure Content RectTransform - CRITICAL FIX from Problem_History.txt
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);  // This gives it full width!
            contentRect.sizeDelta = new Vector2(0f, 300f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.pivot = new Vector2(0f, 1f);
            
            DebugLogger.LogSchedulerApp("BEFORE Content anchoring fix:");
            DebugLogger.LogRectTransform(contentRect, "Content before fix");
            
            // Force refresh the RectTransform
            contentRect.ForceUpdateRectTransforms();
            
            DebugLogger.LogSchedulerApp("AFTER Content anchoring fix:");
            DebugLogger.LogRectTransform(contentRect, "Content after fix");
            
            // Create Vertical Scrollbar
            GameObject verticalScrollbar = new GameObject("Scrollbar Vertical");
            verticalScrollbar.transform.SetParent(scrollViewObj.transform);
            
            RectTransform verticalScrollbarRect = verticalScrollbar.AddComponent<RectTransform>();
            verticalScrollbar.AddComponent<CanvasRenderer>();
            Image verticalScrollbarImage = verticalScrollbar.AddComponent<Image>();
            Scrollbar verticalScrollbarComponent = verticalScrollbar.AddComponent<Scrollbar>();
            
            // Configure vertical scrollbar
            verticalScrollbarComponent.direction = Scrollbar.Direction.TopToBottom;
            verticalScrollbarRect.anchorMin = new Vector2(1f, 0f);
            verticalScrollbarRect.anchorMax = new Vector2(1f, 1f);
            verticalScrollbarRect.sizeDelta = new Vector2(20f, 0f);
            verticalScrollbarRect.anchoredPosition = Vector2.zero;
            
            // Create Horizontal Scrollbar (disabled but present)
            GameObject horizontalScrollbar = new GameObject("Scrollbar Horizontal");
            horizontalScrollbar.transform.SetParent(scrollViewObj.transform);
            
            RectTransform horizontalScrollbarRect = horizontalScrollbar.AddComponent<RectTransform>();
            horizontalScrollbar.AddComponent<CanvasRenderer>();
            Image horizontalScrollbarImage = horizontalScrollbar.AddComponent<Image>();
            Scrollbar horizontalScrollbarComponent = horizontalScrollbar.AddComponent<Scrollbar>();
            
            // Configure horizontal scrollbar
            horizontalScrollbarComponent.direction = Scrollbar.Direction.LeftToRight;
            horizontalScrollbarRect.anchorMin = new Vector2(0f, 0f);
            horizontalScrollbarRect.anchorMax = new Vector2(1f, 0f);
            horizontalScrollbarRect.sizeDelta = new Vector2(0f, 20f);
            horizontalScrollbarRect.anchoredPosition = Vector2.zero;
            
            // Configure ScrollRect component (matching LessonBoardApp)
            scrollRectComponent.content = contentRect;
            scrollRectComponent.viewport = viewportRect;
            scrollRectComponent.horizontalScrollbar = horizontalScrollbarComponent;
            scrollRectComponent.verticalScrollbar = verticalScrollbarComponent;
            
            scrollRectComponent.horizontal = false;
            scrollRectComponent.vertical = true;
            scrollRectComponent.movementType = ScrollRect.MovementType.Elastic;
            scrollRectComponent.elasticity = 0.1f;
            scrollRectComponent.inertia = true;
            scrollRectComponent.decelerationRate = 0.135f;
            scrollRectComponent.scrollSensitivity = 1f;
            
            // Configure scrollbar visibility (Auto Hide And Expand Viewport)
            scrollRectComponent.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRectComponent.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRectComponent.horizontalScrollbarSpacing = -3f;
            scrollRectComponent.verticalScrollbarSpacing = -3f;
            
            // Set the scheduleListParent reference to the Content GameObject
            scheduleListParent = contentRect.transform;
            
            DebugLogger.LogSchedulerApp("ScrollView structure created successfully!");
            DebugLogger.LogGameObject(scrollViewObj, "Created ScrollView");
            DebugLogger.LogGameObject(viewportObj, "Created Viewport");
            DebugLogger.LogGameObject(contentObj, "Created Content");
            DebugLogger.LogRectTransform(contentRect, "Content RectTransform after creation");
            DebugLogger.LogSchedulerApp("scheduleListParent assigned to Content GameObject");
        }
    }
    
    [System.Serializable]
    public class ScheduleItem : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Text clientNameText;
        [SerializeField] private Text timeText;
        [SerializeField] private Text locationText;
        [SerializeField] private Text statusText;
        [SerializeField] private Image backgroundImage;
        
        public void Setup(AppointmentData appointment)
        {
            if (clientNameText != null)
                clientNameText.text = appointment.clientName;
            if (timeText != null)
                timeText.text = appointment.GetTimeString();
            if (locationText != null)
                locationText.text = appointment.location;
                
            UpdateStatus(appointment);
        }
        
        private void UpdateStatus(AppointmentData appointment)
        {
            string status;
            Color backgroundColor = Color.white;
            Color textColor = Color.black;
            
            if (appointment.isCompleted)
            {
                status = "Completed";
                backgroundColor = new Color(0.8f, 1f, 0.8f); // Light green
                textColor = Color.green;
            }
            else if (IsUpcoming(appointment))
            {
                status = "Upcoming";
                backgroundColor = new Color(1f, 1f, 0.8f); // Light yellow
                textColor = new Color(1f, 0.5f, 0f); // Orange color
            }
            else
            {
                status = "Scheduled";
                backgroundColor = Color.white;
                textColor = Color.black;
            }
            
            if (statusText != null)
            {
                statusText.text = status;
                statusText.color = textColor;
            }
            
            if (backgroundImage != null)
                backgroundImage.color = backgroundColor;
        }
        
        private bool IsUpcoming(AppointmentData appointment)
        {
            if (GameManager.Instance?.TimeSystem == null) return false;
            
            var currentTime = GameManager.Instance.TimeSystem.CurrentTime;
            int currentMinutes = currentTime.hour * 60 + currentTime.minute;
            int appointmentMinutes = appointment.scheduledHour * 60 + appointment.scheduledMinute;
            
            return appointmentMinutes > currentMinutes && appointmentMinutes - currentMinutes <= 30;
        }
    }
}