using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TennisCoachCho.Core;
using TennisCoachCho.Data;

namespace TennisCoachCho.UI
{
    public class SkillsPerksApp : MonoBehaviour
    {
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
        [SerializeField] private Text skillPointsText;
        
        [Header("Perks UI")]
        [SerializeField] private Transform perksListParent;
        [SerializeField] private GameObject perkItemPrefab;
        [SerializeField] private Text perkPointsText;
        
        private List<GameObject> skillItems = new List<GameObject>();
        private List<GameObject> perkItems = new List<GameObject>();
        private bool isSkillsTabActive = true;
        
        public void Initialize()
        {
            if (skillsTabButton != null)
                skillsTabButton.onClick.AddListener(() => SwitchTab(true));
            if (perksTabButton != null)
                perksTabButton.onClick.AddListener(() => SwitchTab(false));
            if (backButton != null)
                backButton.onClick.AddListener(GoBack);
                
            SwitchTab(true); // Start with skills tab
            RefreshData();
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
            isSkillsTabActive = showSkills;
            
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
            if (GameManager.Instance?.ProgressionManager == null) return;
            
            var gameData = GameManager.Instance.ProgressionManager.PlayerStats;
            // Access skill tree from GameManager
            var skillTree = FindObjectOfType<GameData>()?.coachingSkillTree;
            
            if (skillTree != null)
            {
                foreach (var skill in skillTree)
                {
                    CreateSkillItem(skill);
                }
            }
        }
        
        private void PopulatePerksList()
        {
            if (GameManager.Instance?.ProgressionManager == null) return;
            
            var gameData = FindObjectOfType<GameData>();
            if (gameData?.availablePerks != null)
            {
                foreach (var perk in gameData.availablePerks)
                {
                    CreatePerkItem(perk);
                }
            }
        }
        
        private void CreateSkillItem(SkillTreeNode skill)
        {
            if (skillItemPrefab == null || skillsListParent == null) return;
            
            GameObject item = Instantiate(skillItemPrefab, skillsListParent);
            skillItems.Add(item);
            
            var itemComponent = item.GetComponent<SkillItem>();
            if (itemComponent != null)
            {
                itemComponent.Setup(skill, OnSkillUpgrade);
            }
            else
            {
                SetupBasicSkillItem(item, skill);
            }
        }
        
        private void CreatePerkItem(PerkData perk)
        {
            if (perkItemPrefab == null || perksListParent == null) return;
            
            GameObject item = Instantiate(perkItemPrefab, perksListParent);
            perkItems.Add(item);
            
            var itemComponent = item.GetComponent<PerkItem>();
            if (itemComponent != null)
            {
                itemComponent.Setup(perk, OnPerkUnlock);
            }
            else
            {
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
            var smartphoneUI = GetComponentInParent<SmartphoneUI>();
            if (smartphoneUI != null)
            {
                smartphoneUI.ShowAppMenu();
            }
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