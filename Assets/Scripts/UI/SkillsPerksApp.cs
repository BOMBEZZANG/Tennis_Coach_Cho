using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TennisCoachCho.Core;
using TennisCoachCho.Data;
using TennisCoachCho.Progression;
using TennisCoachCho.Utilities;
using TMPro;

namespace TennisCoachCho.UI
{
    public class SkillsPerksApp : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject headerPanel;
        
        [Header("Tab Buttons")]
        [SerializeField] private Button skillsTabButton;
        [SerializeField] private Button perksTabButton;
        [SerializeField] private Button backButton;
        
        [Header("Tab Panels")]
        [SerializeField] private GameObject skillsPanel;
        [SerializeField] private GameObject perksPanel;
        
        [Header("Specialist Skills UI - Dog Coach System")]
        [SerializeField] private Transform skillsListParent;
        [SerializeField] private GameObject skillItemPrefab;
        [SerializeField] private TextMeshProUGUI playerLevelText;
        [SerializeField] private TextMeshProUGUI staminaText;
        
        [Header("Perks UI")]
        [SerializeField] private Transform perksListParent;
        [SerializeField] private GameObject perkItemPrefab;
        [SerializeField] private TextMeshProUGUI perkPointsText;
        
        private List<GameObject> skillItems = new List<GameObject>();
        private List<GameObject> perkItems = new List<GameObject>();
        private bool isSkillsTabActive = true;
        
        public void Initialize()
        {
            DebugLogger.LogSkillsPerks("SkillsPerksApp Initialize() called");
            
            if (skillsTabButton != null)
                skillsTabButton.onClick.AddListener(() => SwitchTab(true));
            if (perksTabButton != null)
                perksTabButton.onClick.AddListener(() => SwitchTab(false));
            if (backButton != null)
                backButton.onClick.AddListener(GoBack);
                
            // Fix ScrollRect sizing issues based on Problem_History.txt
            FixScrollRectSizing();
                
            // Start with main menu visible
            ShowMainMenu();
            
            DebugLogger.LogSkillsPerks("SkillsPerksApp Initialize() completed");
        }
        
        public void RefreshData()
        {
            UpdatePointsDisplay();
            
            if (isSkillsTabActive)
                RefreshSkillsTab();
            else
                RefreshPerksTab();
        }
        
        private void SwitchTab(bool showSkills)
        {
            DebugLogger.LogSkillsPerks($"SwitchTab called - showSkills: {showSkills}");
            
            isSkillsTabActive = showSkills;
            
            // Hide header when showing content panels
            if (headerPanel != null)
                headerPanel.SetActive(false);
            
            if (skillsPanel != null)
                skillsPanel.SetActive(showSkills);
            if (perksPanel != null)
                perksPanel.SetActive(!showSkills);
                
            // Update tab button visuals
            UpdateTabButtons();
            
            RefreshData();
        }
        
        private void UpdateTabButtons()
        {
            Color activeColor = new Color(0.8f, 0.8f, 1f);
            Color inactiveColor = Color.white;
            
            if (skillsTabButton != null)
            {
                var colors = skillsTabButton.colors;
                colors.normalColor = isSkillsTabActive ? activeColor : inactiveColor;
                skillsTabButton.colors = colors;
            }
            
            if (perksTabButton != null)
            {
                var colors = perksTabButton.colors;
                colors.normalColor = !isSkillsTabActive ? activeColor : inactiveColor;
                perksTabButton.colors = colors;
            }
        }
        
        private void UpdatePointsDisplay()
        {
            if (GameManager.Instance?.ProgressionManager != null)
            {
                var stats = GameManager.Instance.ProgressionManager.PlayerStats;
                
                // DOG COACH SYSTEM: Display player level and stamina instead of skill points
                if (playerLevelText != null)
                    playerLevelText.text = $"Player Level: {stats.playerLevel} (XP: {stats.playerExp}/{stats.playerExpToNext})";
                if (staminaText != null)
                    staminaText.text = $"Stamina: {stats.currentStamina}/{stats.maxStamina}";
                if (perkPointsText != null)
                    perkPointsText.text = $"Perk Points: {stats.perkPoints}";
            }
        }
        
        private void RefreshSkillsTab()
        {
            ClearSkillsList();
            PopulateSkillsList();
        }
        
        private void RefreshPerksTab()
        {
            ClearPerksList();
            PopulatePerksList();
        }
        
        private void ClearSkillsList()
        {
            foreach (var item in skillItems)
            {
                if (item != null)
                    Destroy(item);
            }
            skillItems.Clear();
        }
        
        private void ClearPerksList()
        {
            foreach (var item in perkItems)
            {
                if (item != null)
                    Destroy(item);
            }
            perkItems.Clear();
        }
        
        private void PopulateSkillsList()
        {
            DebugLogger.LogSkillsPerks("PopulateSkillsList called - DOG COACH SYSTEM");
            
            if (GameManager.Instance?.ProgressionManager == null) 
            {
                DebugLogger.LogSkillsPerks("ERROR: GameManager.Instance or ProgressionManager is null!");
                return;
            }
            
            var progressionManager = GameManager.Instance.ProgressionManager;
            var gameData = progressionManager.GameDataRef;
            var specialistSkills = gameData?.specialistSkills;
            
            DebugLogger.LogSkillsPerks($"GameData found: {gameData != null}");
            DebugLogger.LogSkillsPerks($"Specialist Skills count: {specialistSkills?.Count ?? 0}");
            
            if (specialistSkills != null && specialistSkills.Count > 0)
            {
                foreach (var skill in specialistSkills)
                {
                    DebugLogger.LogSkillsPerks($"Creating specialist skill item: {skill.GetFieldDisplayName()}");
                    CreateSpecialistSkillItem(skill);
                }
            }
            else
            {
                DebugLogger.LogSkillsPerks("WARNING: No specialist skills found!");
            }
        }
        
        private void PopulatePerksList()
        {
            DebugLogger.LogSkillsPerks("PopulatePerksList called - DOG COACH SYSTEM");
            
            if (GameManager.Instance?.ProgressionManager == null) 
            {
                DebugLogger.LogSkillsPerks("ERROR: GameManager.Instance or ProgressionManager is null!");
                return;
            }
            
            var progressionManager = GameManager.Instance.ProgressionManager;
            var gameData = progressionManager.GameDataRef;
            DebugLogger.LogSkillsPerks($"GameData found: {gameData != null}");
            DebugLogger.LogSkillsPerks($"AllPerkTrees count: {gameData?.allPerkTrees?.Count ?? 0}");
            
            if (gameData?.allPerkTrees != null && gameData.allPerkTrees.Count > 0)
            {
                foreach (var perk in gameData.allPerkTrees)
                {
                    DebugLogger.LogSkillsPerks($"Creating perk item: {perk.nodeName}");
                    CreatePerkItem(perk);
                }
            }
            else
            {
                DebugLogger.LogSkillsPerks("WARNING: No perks found in available perks!");
            }
        }
        
        private void CreateSpecialistSkillItem(SpecialistSkillData skill)
        {
            DebugLogger.LogSkillsPerks($"CreateSpecialistSkillItem called for: {skill?.GetFieldDisplayName()}");
            DebugLogger.LogSkillsPerks($"skillItemPrefab is null: {skillItemPrefab == null}");
            DebugLogger.LogSkillsPerks($"skillsListParent is null: {skillsListParent == null}");
            
            if (skillItemPrefab == null || skillsListParent == null) 
            {
                DebugLogger.LogSkillsPerks("ERROR: Cannot create specialist skill item - prefab or parent is null!");
                return;
            }
            
            GameObject item = Instantiate(skillItemPrefab, skillsListParent);
            skillItems.Add(item);
            
            DebugLogger.LogSkillsPerks($"Specialist skill item instantiated: {item.name}");
            
            // DOG COACH SYSTEM: Setup specialist skill display (no upgrades, just info)
            SetupBasicSpecialistSkillItem(item, skill);
        }
        
        private void CreatePerkItem(PerkTreeNode perk)
        {
            DebugLogger.LogSkillsPerks($"CreatePerkItem called for: {perk?.nodeName}");
            DebugLogger.LogSkillsPerks($"perkItemPrefab is null: {perkItemPrefab == null}");
            DebugLogger.LogSkillsPerks($"perksListParent is null: {perksListParent == null}");
            
            if (perkItemPrefab == null || perksListParent == null) 
            {
                DebugLogger.LogSkillsPerks("ERROR: Cannot create perk item - prefab or parent is null!");
                return;
            }
            
            GameObject item = Instantiate(perkItemPrefab, perksListParent);
            perkItems.Add(item);
            
            DebugLogger.LogSkillsPerks($"Perk item instantiated: {item.name}");
            DebugLogger.LogGameObject(item, "PerkItem created");
            
            var itemComponent = item.GetComponent<PerkItem>();
            if (itemComponent != null)
            {
                DebugLogger.LogSkillsPerks($"Using PerkItem component for {perk.nodeName}");
                itemComponent.Setup(perk, OnPerkUnlock);
            }
            else
            {
                DebugLogger.LogSkillsPerks($"Using basic setup for {perk.nodeName}");
                SetupBasicPerkItem(item, perk);
            }
        }
        
        private void SetupBasicSpecialistSkillItem(GameObject item, SpecialistSkillData skill)
        {
            var texts = item.GetComponentsInChildren<Text>();
            var button = item.GetComponentInChildren<Button>();
            
            if (texts.Length >= 3)
            {
                texts[0].text = skill.GetFieldDisplayName();
                texts[1].text = $"Specialist field progression - gain XP from {skill.GetFieldName()} activities";
                texts[2].text = $"Level: {skill.level} (XP: {skill.exp}/{skill.expToNext})";
            }
            
            // DOG COACH SYSTEM: Specialist skills are not directly upgradeable by player
            // They level up automatically through activities
            if (button != null)
            {
                button.interactable = false;
                var buttonText = button.GetComponentInChildren<Text>();
                if (buttonText != null)
                    buttonText.text = "Auto Level";
            }
        }
        
        private void SetupBasicPerkItem(GameObject item, PerkTreeNode perk)
        {
            var texts = item.GetComponentsInChildren<Text>();
            var button = item.GetComponentInChildren<Button>();
            
            if (texts.Length >= 3)
            {
                texts[0].text = perk.nodeName;
                texts[1].text = perk.description;
                texts[2].text = $"Cost: {perk.perkCost} perk points | Requires: {perk.requiredField} Lv{perk.requiredFieldLevel}";
            }
            
            if (button != null)
            {
                // Check if player can unlock this perk
                var progressionManager = GameManager.Instance?.ProgressionManager;
                bool canUnlock = false;
                if (progressionManager != null)
                {
                    var fieldLevel = progressionManager.GetSpecialistLevel(perk.requiredField);
                    canUnlock = perk.CanUnlock(fieldLevel, progressionManager.PlayerStats.perkPoints);
                }
                
                button.interactable = canUnlock;
                button.onClick.AddListener(() => OnPerkUnlock(perk));
                
                var buttonText = button.GetComponentInChildren<Text>();
                if (buttonText != null)
                    buttonText.text = perk.isUnlocked ? "Unlocked" : "Unlock";
                    
                // Visual feedback for unlocked perks
                if (perk.isUnlocked)
                {
                    var image = item.GetComponent<Image>();
                    if (image != null)
                        image.color = new Color(0.8f, 1f, 0.8f);
                }
            }
        }
        
        private bool HasPerkPoints()
        {
            return GameManager.Instance?.ProgressionManager?.PlayerStats.perkPoints > 0;
        }
        
        // DOG COACH SYSTEM: Removed OnSkillUpgrade - specialist skills auto-level through activities
        
        private void OnPerkUnlock(PerkTreeNode perk)
        {
            if (GameManager.Instance?.ProgressionManager != null)
            {
                bool success = GameManager.Instance.ProgressionManager.UnlockPerk(perk.nodeName);
                if (success)
                {
                    RefreshData();
                }
            }
        }
        
        private void GoBack()
        {
            // If we're showing a panel, go back to header menu
            if (headerPanel != null && !headerPanel.activeInHierarchy)
            {
                ShowMainMenu();
            }
            else
            {
                // If we're on main menu, go back to smartphone menu
                var smartphoneUI = GetComponentInParent<SmartphoneUI>();
                if (smartphoneUI != null)
                {
                    smartphoneUI.ShowAppMenu();
                }
            }
        }
        
        private void ShowMainMenu()
        {
            DebugLogger.LogSkillsPerks("ShowMainMenu called - showing header, hiding panels");
            
            // Show header
            if (headerPanel != null)
                headerPanel.SetActive(true);
                
            // Hide both panels
            if (skillsPanel != null)
                skillsPanel.SetActive(false);
            if (perksPanel != null)
                perksPanel.SetActive(false);
        }
        
        private void FixScrollRectSizing()
        {
            // Fix Skills ScrollRect - based on Problem_History.txt solution
            FixSingleScrollRect(skillsListParent);
            
            // Fix Perks ScrollRect
            FixSingleScrollRect(perksListParent);
        }
        
        private void FixSingleScrollRect(Transform contentParent)
        {
            if (contentParent == null) return;
            
            // Get the ScrollRect components in hierarchy
            var scrollRect = contentParent.GetComponentInParent<ScrollRect>();
            if (scrollRect == null) return;
            
            var viewport = scrollRect.viewport;
            var content = scrollRect.content;
            
            if (viewport != null)
            {
                // Fix Viewport sizing - should stretch to fill ScrollRect
                var viewportRect = viewport.GetComponent<RectTransform>();
                if (viewportRect != null)
                {
                    viewportRect.anchorMin = Vector2.zero;
                    viewportRect.anchorMax = Vector2.one;
                    viewportRect.sizeDelta = Vector2.zero;
                    viewportRect.anchoredPosition = Vector2.zero;
                    
                    // Temporarily disable mask if viewport has zero size
                    var mask = viewport.GetComponent<Mask>();
                    if (mask != null && viewportRect.rect.width <= 0)
                    {
                        mask.enabled = false;
                        // Re-enable after a frame when sizing is fixed
                        StartCoroutine(ReEnableMask(mask));
                    }
                }
            }
            
            if (content != null)
            {
                // Fix Content sizing - top-anchored with proper height
                var contentRect = content.GetComponent<RectTransform>();
                if (contentRect != null)
                {
                    contentRect.anchorMin = new Vector2(0, 1);
                    contentRect.anchorMax = new Vector2(1, 1);
                    contentRect.sizeDelta = new Vector2(0, 300); // Initial height
                    contentRect.anchoredPosition = new Vector2(0, 0);
                }
            }
        }
        
        private System.Collections.IEnumerator ReEnableMask(Mask mask)
        {
            yield return new WaitForEndOfFrame();
            if (mask != null)
                mask.enabled = true;
        }
    }
    
    public class SkillItem : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Text skillNameText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Text levelText;
        [SerializeField] private Button upgradeButton;
        
        private SpecialistSkillData skillData;
        
        public void Setup(SpecialistSkillData skill)
        {
            skillData = skill;
            
            if (skillNameText != null)
                skillNameText.text = skill.GetFieldDisplayName();
            if (descriptionText != null)
                descriptionText.text = $"Specialist field progression - gain XP from {skill.GetFieldName()} activities";
            if (levelText != null)
                levelText.text = $"Level: {skill.level} (XP: {skill.exp}/{skill.expToNext})";
                
            // DOG COACH SYSTEM: Specialist skills auto-level, no manual upgrades
            if (upgradeButton != null)
            {
                upgradeButton.interactable = false;
                var buttonText = upgradeButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                    buttonText.text = "Auto Level";
            }
        }
    }
    
    public class PerkItem : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Text perkNameText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Button unlockButton;
        [SerializeField] private Image backgroundImage;
        
        private PerkTreeNode perkData;
        private System.Action<PerkTreeNode> onUnlockCallback;
        
        public void Setup(PerkTreeNode perk, System.Action<PerkTreeNode> onUnlock)
        {
            perkData = perk;
            onUnlockCallback = onUnlock;
            
            if (perkNameText != null)
                perkNameText.text = perk.nodeName;
            if (descriptionText != null)
                descriptionText.text = perk.description;
                
            if (unlockButton != null)
            {
                unlockButton.onClick.AddListener(UnlockPerk);
                UpdateButtonState();
            }
            
            UpdateVisuals();
        }
        
        private void UpdateButtonState()
        {
            if (unlockButton == null) return;
            
            // DOG COACH SYSTEM: Check requirements
            var progressionManager = GameManager.Instance?.ProgressionManager;
            bool canUnlock = false;
            if (progressionManager != null)
            {
                var fieldLevel = progressionManager.GetSpecialistLevel(perkData.requiredField);
                canUnlock = perkData.CanUnlock(fieldLevel, progressionManager.PlayerStats.perkPoints);
            }
            
            unlockButton.interactable = canUnlock;
            
            var buttonText = unlockButton.GetComponentInChildren<Text>();
            if (buttonText != null)
                buttonText.text = perkData.isUnlocked ? "Unlocked" : "Unlock";
        }
        
        private void UpdateVisuals()
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = perkData.isUnlocked ? 
                    new Color(0.8f, 1f, 0.8f) : Color.white;
            }
        }
        
        private bool HasPerkPoints()
        {
            return GameManager.Instance?.ProgressionManager?.PlayerStats.perkPoints > 0;
        }
        
        private void UnlockPerk()
        {
            onUnlockCallback?.Invoke(perkData);
        }
    }
}