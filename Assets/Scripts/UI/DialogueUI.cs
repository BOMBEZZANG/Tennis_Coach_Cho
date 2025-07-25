using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace TennisCoachCho.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private TextMeshProUGUI confirmButtonText;
        
        [Header("Settings")]
        [SerializeField] private float typingSpeed = 50f; // Characters per second
        
        private Action onConfirmCallback;
        private bool isDialogueActive = false;
        private bool isTyping = false;
        private string fullText = "";
        
        private void Start()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            // Create dialogue UI if not assigned
            if (dialoguePanel == null)
            {
                CreateDialogueUI();
            }
            
            // Hide dialogue initially
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            
            // Setup confirm button
            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(OnConfirmClicked);
            }
            
            Debug.Log("[DialogueUI] Dialogue system initialized");
        }
        
        private void CreateDialogueUI()
        {
            // Create dialogue panel as child of Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[DialogueUI] No Canvas found! Please ensure there's a Canvas in the scene.");
                return;
            }
            
            // Create main dialogue panel
            GameObject panel = new GameObject("DialoguePanel");
            panel.transform.SetParent(canvas.transform, false);
            
            // Add Image component for background
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black
            
            // Set panel size and position
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.1f, 0.1f);
            panelRect.anchorMax = new Vector2(0.9f, 0.4f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            // Create text area
            GameObject textObj = new GameObject("DialogueText");
            textObj.transform.SetParent(panel.transform, false);
            
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "Dialogue text will appear here...";
            text.fontSize = 18;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.TopLeft;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.05f, 0.3f);
            textRect.anchorMax = new Vector2(0.95f, 0.9f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            // Create confirm button
            GameObject buttonObj = new GameObject("ConfirmButton");
            buttonObj.transform.SetParent(panel.transform, false);
            
            Button button = buttonObj.AddComponent<Button>();
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.6f, 0.2f, 1f); // Green button
            
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.7f, 0.05f);
            buttonRect.anchorMax = new Vector2(0.95f, 0.25f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            
            // Create button text
            GameObject buttonTextObj = new GameObject("ButtonText");
            buttonTextObj.transform.SetParent(buttonObj.transform, false);
            
            TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = "Continue";
            buttonText.fontSize = 14;
            buttonText.color = Color.white;
            buttonText.alignment = TextAlignmentOptions.Center;
            
            RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;
            
            // Assign references
            dialoguePanel = panel;
            dialogueText = text;
            confirmButton = button;
            confirmButtonText = buttonText;
            
            Debug.Log("[DialogueUI] Created dialogue UI elements");
        }
        
        public void ShowDialogue(string text, string buttonText, Action onConfirm)
        {
            if (isDialogueActive)
            {
                Debug.LogWarning("[DialogueUI] Dialogue already active!");
                return;
            }
            
            fullText = text;
            onConfirmCallback = onConfirm;
            isDialogueActive = true;
            
            // Show panel
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }
            
            // Set button text
            if (confirmButtonText != null)
            {
                confirmButtonText.text = buttonText;
            }
            
            // Start typing effect
            StartCoroutine(TypeText());
            
            Debug.Log($"[DialogueUI] Showing dialogue: {text}");
        }
        
        private System.Collections.IEnumerator TypeText()
        {
            isTyping = true;
            
            if (dialogueText != null)
            {
                dialogueText.text = "";
                
                for (int i = 0; i <= fullText.Length; i++)
                {
                    dialogueText.text = fullText.Substring(0, i);
                    yield return new WaitForSeconds(1f / typingSpeed);
                }
            }
            
            isTyping = false;
        }
        
        private void Update()
        {
            // Allow space or enter to skip typing or confirm dialogue
            if (isDialogueActive)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                {
                    if (isTyping)
                    {
                        // Skip typing
                        StopCoroutine(TypeText());
                        if (dialogueText != null)
                        {
                            dialogueText.text = fullText;
                        }
                        isTyping = false;
                    }
                    else
                    {
                        // Confirm dialogue
                        OnConfirmClicked();
                    }
                }
            }
        }
        
        private void OnConfirmClicked()
        {
            if (!isDialogueActive) return;
            
            // If still typing, complete the text first
            if (isTyping)
            {
                StopCoroutine(TypeText());
                if (dialogueText != null)
                {
                    dialogueText.text = fullText;
                }
                isTyping = false;
                return;
            }
            
            // Close dialogue
            CloseDialogue();
            
            // Call callback
            onConfirmCallback?.Invoke();
            
            Debug.Log("[DialogueUI] Dialogue confirmed and closed");
        }
        
        public void CloseDialogue()
        {
            if (!isDialogueActive) return;
            
            isDialogueActive = false;
            isTyping = false;
            
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            
            onConfirmCallback = null;
            fullText = "";
            
            Debug.Log("[DialogueUI] Dialogue closed");
        }
        
        public bool IsDialogueActive()
        {
            return isDialogueActive;
        }
        
        // Method to show simple message without button
        public void ShowMessage(string message, float duration = 3f)
        {
            if (isDialogueActive) return;
            
            StartCoroutine(ShowTemporaryMessage(message, duration));
        }
        
        private System.Collections.IEnumerator ShowTemporaryMessage(string message, float duration)
        {
            isDialogueActive = true;
            
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }
            
            if (dialogueText != null)
            {
                dialogueText.text = message;
            }
            
            if (confirmButton != null)
            {
                confirmButton.gameObject.SetActive(false);
            }
            
            yield return new WaitForSeconds(duration);
            
            CloseDialogue();
            
            if (confirmButton != null)
            {
                confirmButton.gameObject.SetActive(true);
            }
        }
    }
}