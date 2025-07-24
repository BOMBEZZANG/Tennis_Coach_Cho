using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TennisCoachCho.Core;
using TennisCoachCho.Data;
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
        
        [Header("Skills UI")]
        [SerializeField] private Transform skillsListParent;
        [SerializeField] private GameObject skillItemPrefab;
        [SerializeField] private TextMeshProUGUI skillPointsText;
        
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
                
                if (skillPointsText != null)
                    skillPointsText.text = $"Skill Points: {stats.skillPoints}";
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
            DebugLogger.LogSkillsPerks("PopulateSkillsList called");
            
            if (GameManager.Instance?.ProgressionManager == null) 
            {
                DebugLogger.LogSkillsPerks("ERROR: GameManager.Instance or ProgressionManager is null!");
                return;
            }
            
            var progressionManager = GameManager.Instance.ProgressionManager;
            var gameData = progressionManager.GameData;
            var skillTree = gameData?.coachingSkillTree;
            
            DebugLogger.LogSkillsPerks($"GameData found: {gameData != null}");
            DebugLogger.LogSkillsPerks($"SkillTree count: {skillTree?.Count ?? 0}");
            
            if (skillTree != null && skillTree.Count > 0)
            {
                foreach (var skill in skillTree)
                {
                    DebugLogger.LogSkillsPerks($"Creating skill item: {skill.nodeName}");
                    DebugLogger.LogSkillData(skill);
                    CreateSkillItem(skill);
                }
            }
            else
            {
                DebugLogger.LogSkillsPerks("WARNING: No skills found in skill tree!");
            }
        }
        
        private void PopulatePerksList()
        {
            DebugLogger.LogSkillsPerks("PopulatePerksList called");
            
            if (GameManager.Instance?.ProgressionManager == null) 
            {
                DebugLogger.LogSkillsPerks("ERROR: GameManager.Instance or ProgressionManager is null!");
                return;
            }
            
            var progressionManager = GameManager.Instance.ProgressionManager;
            var gameData = progressionManager.GameData;
            DebugLogger.LogSkillsPerks($"GameData found: {gameData != null}");
            DebugLogger.LogSkillsPerks($"AvailablePerks count: {gameData?.availablePerks?.Count ?? 0}");
            
            if (gameData?.availablePerks != null && gameData.availablePerks.Count > 0)
            {
                foreach (var perk in gameData.availablePerks)
                {
                    DebugLogger.LogSkillsPerks($"Creating perk item: {perk.perkName}");
                    DebugLogger.LogPerkData(perk);
                    CreatePerkItem(perk);
                }
            }
            else
            {
                DebugLogger.LogSkillsPerks("WARNING: No perks found in available perks!");
            }
        }
        
        private void CreateSkillItem(SkillTreeNode skill)
        {
            DebugLogger.LogSkillsPerks($"CreateSkillItem called for: {skill?.nodeName}");
            DebugLogger.LogSkillsPerks($"skillItemPrefab is null: {skillItemPrefab == null}");
            DebugLogger.LogSkillsPerks($"skillsListParent is null: {skillsListParent == null}");
            
            if (skillItemPrefab == null || skillsListParent == null) 
            {
                DebugLogger.LogSkillsPerks("ERROR: Cannot create skill item - prefab or parent is null!");
                return;
            }
            
            GameObject item = Instantiate(skillItemPrefab, skillsListParent);
            skillItems.Add(item);
            
            DebugLogger.LogSkillsPerks($"Skill item instantiated: {item.name}");
            DebugLogger.LogGameObject(item, "SkillItem created");
            
            var itemComponent = item.GetComponent<SkillItem>();
            if (itemComponent != null)
            {
                DebugLogger.LogSkillsPerks($"Using SkillItem component for {skill.nodeName}");
                itemComponent.Setup(skill, OnSkillUpgrade);
            }
            else
            {
                DebugLogger.LogSkillsPerks($"Using basic setup for {skill.nodeName}");
                SetupBasicSkillItem(item, skill);
            }
        }
        
        private void CreatePerkItem(PerkData perk)
        {
            DebugLogger.LogSkillsPerks($"CreatePerkItem called for: {perk?.perkName}");
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
                DebugLogger.LogSkillsPerks($"Using PerkItem component for {perk.perkName}");
                itemComponent.Setup(perk, OnPerkUnlock);
            }
            else
            {
                DebugLogger.LogSkillsPerks($"Using basic setup for {perk.perkName}");
                SetupBasicPerkItem(item, perk);
            }
        }
        
        private void SetupBasicSkillItem(GameObject item, SkillTreeNode skill)
        {
            var texts = item.GetComponentsInChildren<Text>();
            var button = item.GetComponentInChildren<Button>();
            
            if (texts.Length >= 3)
            {
                texts[0].text = skill.nodeName;
                texts[1].text = skill.description;
                texts[2].text = $"Level: {skill.level}/{skill.maxLevel}";
            }
            
            if (button != null)
            {
                button.interactable = skill.CanUpgrade() && HasSkillPoints();
                button.onClick.AddListener(() => OnSkillUpgrade(skill));
                
                var buttonText = button.GetComponentInChildren<Text>();
                if (buttonText != null)
                    buttonText.text = skill.CanUpgrade() ? "Upgrade" : "Max";
            }
        }
        
        private void SetupBasicPerkItem(GameObject item, PerkData perk)
        {
            var texts = item.GetComponentsInChildren<Text>();
            var button = item.GetComponentInChildren<Button>();
            
            if (texts.Length >= 2)
            {
                texts[0].text = perk.perkName;
                texts[1].text = perk.description;
            }
            
            if (button != null)
            {
                button.interactable = !perk.isUnlocked && HasPerkPoints();
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
        
        private bool HasSkillPoints()
        {
            return GameManager.Instance?.ProgressionManager?.PlayerStats.skillPoints > 0;
        }
        
        private bool HasPerkPoints()
        {
            return GameManager.Instance?.ProgressionManager?.PlayerStats.perkPoints > 0;
        }
        
        private void OnSkillUpgrade(SkillTreeNode skill)
        {
            if (GameManager.Instance?.ProgressionManager != null)
            {
                bool success = GameManager.Instance.ProgressionManager.UpgradeSkill(skill.nodeName);
                if (success)
                {
                    RefreshData();
                }
            }
        }
        
        private void OnPerkUnlock(PerkData perk)
        {
            if (GameManager.Instance?.ProgressionManager != null)
            {
                bool success = GameManager.Instance.ProgressionManager.UnlockPerk(perk.perkName);
                if (success)
                {
                    RefreshData();
                }
            }
        }
        
        private void GoBack()
        {
            // If we're showing a panel, go back to header menu
            if (!headerPanel.activeInHierarchy)
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
        
        private SkillTreeNode skillData;
        private System.Action<SkillTreeNode> onUpgradeCallback;
        
        public void Setup(SkillTreeNode skill, System.Action<SkillTreeNode> onUpgrade)
        {
            skillData = skill;
            onUpgradeCallback = onUpgrade;
            
            if (skillNameText != null)
                skillNameText.text = skill.nodeName;
            if (descriptionText != null)
                descriptionText.text = skill.description;
            if (levelText != null)
                levelText.text = $"Level: {skill.level}/{skill.maxLevel}";
                
            if (upgradeButton != null)
            {
                upgradeButton.onClick.AddListener(UpgradeSkill);
                UpdateButtonState();
            }
        }
        
        private void UpdateButtonState()
        {
            if (upgradeButton == null) return;
            
            bool canUpgrade = skillData.CanUpgrade() && HasSkillPoints();
            upgradeButton.interactable = canUpgrade;
            
            var buttonText = upgradeButton.GetComponentInChildren<Text>();
            if (buttonText != null)
                buttonText.text = skillData.CanUpgrade() ? "Upgrade" : "Max";
        }
        
        private bool HasSkillPoints()
        {
            return GameManager.Instance?.ProgressionManager?.PlayerStats.skillPoints > 0;
        }
        
        private void UpgradeSkill()
        {
            onUpgradeCallback?.Invoke(skillData);
        }
    }
    
    public class PerkItem : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Text perkNameText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Button unlockButton;
        [SerializeField] private Image backgroundImage;
        
        private PerkData perkData;
        private System.Action<PerkData> onUnlockCallback;
        
        public void Setup(PerkData perk, System.Action<PerkData> onUnlock)
        {
            perkData = perk;
            onUnlockCallback = onUnlock;
            
            if (perkNameText != null)
                perkNameText.text = perk.perkName;
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
            
            bool canUnlock = !perkData.isUnlocked && HasPerkPoints();
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