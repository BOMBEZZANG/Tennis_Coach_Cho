using UnityEngine;
using UnityEngine.UI;

namespace TennisCoachCho.UI
{
    /// <summary>
    /// Utility script to programmatically create ScrollView structure for SchedulerApp
    /// Based on the working LessonBoardApp configuration from Problem_History.txt
    /// </summary>
    public class SchedulerAppScrollViewSetup : MonoBehaviour
    {
        [Header("SchedulerApp Reference")]
        [SerializeField] private SchedulerApp schedulerApp;
        
        [Header("Setup Configuration")]
        [SerializeField] private Vector2 scrollViewSize = new Vector2(400, 400);
        [SerializeField] private Vector2 scrollViewPosition = new Vector2(92, -318);
        [SerializeField] private float contentHeight = 300f;
        
        [ContextMenu("Setup ScrollView Structure")]
        public void SetupScrollViewStructure()
        {
            if (schedulerApp == null)
            {
                Debug.LogError("SchedulerApp reference is null!");
                return;
            }
            
            // Clear existing children if any
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            
            // Create main ScrollView GameObject
            GameObject scrollViewObj = CreateScrollViewGameObject();
            
            // Create Viewport
            GameObject viewportObj = CreateViewportGameObject(scrollViewObj.transform);
            
            // Create Content
            GameObject contentObj = CreateContentGameObject(viewportObj.transform);
            
            // Create Scrollbars
            GameObject horizontalScrollbar = CreateHorizontalScrollbar(scrollViewObj.transform);
            GameObject verticalScrollbar = CreateVerticalScrollbar(scrollViewObj.transform);
            
            // Configure ScrollRect component
            ConfigureScrollRect(scrollViewObj, contentObj, viewportObj, horizontalScrollbar, verticalScrollbar);
            
            // Update SchedulerApp reference
            UpdateSchedulerAppReference(contentObj.transform);
            
            Debug.Log("ScrollView structure created successfully for SchedulerApp!");
        }
        
        private GameObject CreateScrollViewGameObject()
        {
            GameObject scrollViewObj = new GameObject("ScheduleScrollView");
            scrollViewObj.transform.SetParent(transform);
            
            // Add required components
            RectTransform rectTransform = scrollViewObj.AddComponent<RectTransform>();
            scrollViewObj.AddComponent<CanvasRenderer>();
            
            // Add and configure Image (disabled like in LessonBoardApp)
            Image image = scrollViewObj.AddComponent<Image>();
            image.enabled = false;
            
            // Add ScrollRect component
            scrollViewObj.AddComponent<ScrollRect>();
            
            // Configure RectTransform (matching LessonBoardApp configuration)
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = scrollViewPosition;
            rectTransform.sizeDelta = scrollViewSize;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            
            return scrollViewObj;
        }
        
        private GameObject CreateViewportGameObject(Transform parent)
        {
            GameObject viewportObj = new GameObject("Viewport");
            viewportObj.transform.SetParent(parent);
            
            // Add required components
            RectTransform rectTransform = viewportObj.AddComponent<RectTransform>();
            
            // Add Mask component for clipping
            Mask mask = viewportObj.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            
            // Add Image component for mask
            Image image = viewportObj.AddComponent<Image>();
            image.color = Color.white;
            
            // Configure RectTransform to stretch and fill ScrollView
            // Fixed configuration from Problem_History.txt
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.pivot = new Vector2(0f, 1f);
            
            return viewportObj;
        }
        
        private GameObject CreateContentGameObject(Transform parent)
        {
            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(parent);
            
            // Add required components
            RectTransform rectTransform = contentObj.AddComponent<RectTransform>();
            
            // Add VerticalLayoutGroup for automatic item arrangement
            VerticalLayoutGroup layoutGroup = contentObj.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = true;
            layoutGroup.spacing = 10f;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);
            
            // Add ContentSizeFitter for dynamic sizing
            ContentSizeFitter sizeFitter = contentObj.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            // Configure RectTransform for top-anchored scrolling
            // Fixed configuration from Problem_History.txt
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.sizeDelta = new Vector2(0f, contentHeight);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.pivot = new Vector2(0f, 1f);
            
            return contentObj;
        }
        
        private GameObject CreateHorizontalScrollbar(Transform parent)
        {
            GameObject scrollbarObj = new GameObject("Scrollbar Horizontal");
            scrollbarObj.transform.SetParent(parent);
            
            // Add required components
            RectTransform rectTransform = scrollbarObj.AddComponent<RectTransform>();
            scrollbarObj.AddComponent<CanvasRenderer>();
            Image image = scrollbarObj.AddComponent<Image>();
            Scrollbar scrollbar = scrollbarObj.AddComponent<Scrollbar>();
            
            // Configure for horizontal scrolling
            scrollbar.direction = Scrollbar.Direction.LeftToRight;
            
            // Position at bottom of ScrollView
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 0f);
            rectTransform.sizeDelta = new Vector2(0f, 20f);
            rectTransform.anchoredPosition = new Vector2(0f, 0f);
            
            return scrollbarObj;
        }
        
        private GameObject CreateVerticalScrollbar(Transform parent)
        {
            GameObject scrollbarObj = new GameObject("Scrollbar Vertical");
            scrollbarObj.transform.SetParent(parent);
            
            // Add required components
            RectTransform rectTransform = scrollbarObj.AddComponent<RectTransform>();
            scrollbarObj.AddComponent<CanvasRenderer>();
            Image image = scrollbarObj.AddComponent<Image>();
            Scrollbar scrollbar = scrollbarObj.AddComponent<Scrollbar>();
            
            // Configure for vertical scrolling
            scrollbar.direction = Scrollbar.Direction.TopToBottom;
            
            // Position at right side of ScrollView
            rectTransform.anchorMin = new Vector2(1f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.sizeDelta = new Vector2(20f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 0f);
            
            return scrollbarObj;
        }
        
        private void ConfigureScrollRect(GameObject scrollViewObj, GameObject contentObj, GameObject viewportObj, 
                                       GameObject horizontalScrollbar, GameObject verticalScrollbar)
        {
            ScrollRect scrollRect = scrollViewObj.GetComponent<ScrollRect>();
            
            // Set references
            scrollRect.content = contentObj.GetComponent<RectTransform>();
            scrollRect.viewport = viewportObj.GetComponent<RectTransform>();
            scrollRect.horizontalScrollbar = horizontalScrollbar.GetComponent<Scrollbar>();
            scrollRect.verticalScrollbar = verticalScrollbar.GetComponent<Scrollbar>();
            
            // Configure scrolling behavior (matching LessonBoardApp)
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
            scrollRect.elasticity = 0.1f;
            scrollRect.inertia = true;
            scrollRect.decelerationRate = 0.135f;
            scrollRect.scrollSensitivity = 1f;
            
            // Configure scrollbar visibility (Auto Hide And Expand Viewport)
            scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.horizontalScrollbarSpacing = -3f;
            scrollRect.verticalScrollbarSpacing = -3f;
        }
        
        private void UpdateSchedulerAppReference(Transform contentTransform)
        {
            // Use reflection to update the private scheduleListParent field
            var field = typeof(SchedulerApp).GetField("scheduleListParent", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                field.SetValue(schedulerApp, contentTransform);
                Debug.Log("Updated SchedulerApp.scheduleListParent reference to Content GameObject");
            }
            else
            {
                Debug.LogError("Could not find scheduleListParent field in SchedulerApp. Update it manually in the inspector.");
            }
        }
    }
}