using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TennisCoachCho.Core;
using TennisCoachCho.Data;

namespace TennisCoachCho.MiniGames
{
    public enum HitResult
    {
        Miss,
        Bad,
        Good,
        Perfect
    }
    
    [System.Serializable]
    public class RhythmNote
    {
        public float timeToHit;
        public bool hasBeenHit;
        public HitResult result;
        
        public RhythmNote(float time)
        {
            timeToHit = time;
            hasBeenHit = false;
            result = HitResult.Miss;
        }
    }
    
    public class RhythmMiniGame : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private float songDuration = 30f;
        [SerializeField] private int totalNotes = 20;
        [SerializeField] private KeyCode hitKey = KeyCode.Space;
        
        [Header("Timing Windows (in seconds)")]
        [SerializeField] private float perfectWindow = 0.1f;
        [SerializeField] private float goodWindow = 0.2f;
        [SerializeField] private float badWindow = 0.3f;
        
        [Header("UI References")]
        [SerializeField] private Slider progressBar;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text feedbackText;
        [SerializeField] private GameObject hitZone;
        [SerializeField] private GameObject noteIndicator;
        [SerializeField] private RectTransform noteSpawnPoint;
        [SerializeField] private RectTransform hitPoint;
        
        private List<RhythmNote> notes;
        private List<GameObject> noteObjects;
        private AppointmentData currentAppointment;
        private float gameStartTime;
        private bool isPlaying;
        private int score;
        private int perfectHits;
        private int goodHits;
        private int badHits;
        private int missedHits;
        
        private void Awake()
        {
            notes = new List<RhythmNote>();
            noteObjects = new List<GameObject>();
        }
        
        public void StartMiniGame(AppointmentData appointment)
        {
            currentAppointment = appointment;
            SetupGame();
            StartCoroutine(PlayGame());
        }
        
        private void SetupGame()
        {
            // DOG COACH SYSTEM: Apply Handling skill bonus to hit zone size  
            float handlingBonus = GameManager.Instance?.ProgressionManager?.GetHandlingBonus() ?? 1f;
            ApplyHandlingBonus(handlingBonus);
            
            // Generate notes
            GenerateNotes();
            
            // Reset UI
            score = 0;
            perfectHits = goodHits = badHits = missedHits = 0;
            UpdateUI();
            
            if (feedbackText != null)
                feedbackText.text = "Get Ready!";
        }
        
        private void ApplyHandlingBonus(float bonus)
        {
            if (hitZone != null)
            {
                Vector3 scale = hitZone.transform.localScale;
                scale.x *= bonus;
                hitZone.transform.localScale = scale;
            }
            
            // Adjust timing windows
            perfectWindow *= bonus;
            goodWindow *= bonus;
        }
        
        private void GenerateNotes()
        {
            notes.Clear();
            float interval = songDuration / totalNotes;
            
            for (int i = 0; i < totalNotes; i++)
            {
                float noteTime = (i + 1) * interval;
                // Add some randomness to make it more interesting
                noteTime += Random.Range(-interval * 0.1f, interval * 0.1f);
                notes.Add(new RhythmNote(noteTime));
            }
            
            // Sort notes by time
            notes.Sort((a, b) => a.timeToHit.CompareTo(b.timeToHit));
        }
        
        private IEnumerator PlayGame()
        {
            isPlaying = true;
            gameStartTime = Time.time;
            
            if (feedbackText != null)
            {
                feedbackText.text = "GO!";
                yield return new WaitForSeconds(1f);
                feedbackText.text = "";
            }
            
            StartCoroutine(SpawnNotes());
            
            while (Time.time - gameStartTime < songDuration)
            {
                HandleInput();
                UpdateGame();
                yield return null;
            }
            
            // Game finished
            isPlaying = false;
            EndGame();
        }
        
        private IEnumerator SpawnNotes()
        {
            float noteSpawnDelay = 2f; // Notes appear 2 seconds before they need to be hit
            
            foreach (var note in notes)
            {
                float spawnTime = note.timeToHit - noteSpawnDelay;
                
                while (Time.time - gameStartTime < spawnTime)
                {
                    yield return null;
                }
                
                if (isPlaying)
                {
                    SpawnNoteVisual(note);
                }
            }
        }
        
        private void SpawnNoteVisual(RhythmNote note)
        {
            if (noteIndicator != null && noteSpawnPoint != null)
            {
                GameObject noteObj = Instantiate(noteIndicator, noteSpawnPoint.position, Quaternion.identity, transform);
                noteObjects.Add(noteObj);
                
                // Start moving the note towards the hit point
                StartCoroutine(MoveNote(noteObj, note));
            }
        }
        
        private IEnumerator MoveNote(GameObject noteObj, RhythmNote note)
        {
            float startTime = Time.time;
            Vector3 startPos = noteSpawnPoint.position;
            Vector3 endPos = hitPoint.position;
            float moveTime = 2f; // Time to travel from spawn to hit point
            
            while (Time.time - startTime < moveTime && noteObj != null)
            {
                float progress = (Time.time - startTime) / moveTime;
                noteObj.transform.position = Vector3.Lerp(startPos, endPos, progress);
                yield return null;
            }
            
            // Note reached hit point without being hit
            if (noteObj != null && !note.hasBeenHit)
            {
                note.hasBeenHit = true;
                note.result = HitResult.Miss;
                missedHits++;
                ShowFeedback("Miss!");
                Destroy(noteObj);
            }
        }
        
        private void HandleInput()
        {
            if (Input.GetKeyDown(hitKey))
            {
                ProcessHit();
            }
        }
        
        private void ProcessHit()
        {
            float currentTime = Time.time - gameStartTime;
            RhythmNote closestNote = GetClosestUnhitNote(currentTime);
            
            if (closestNote != null)
            {
                float timeDifference = Mathf.Abs(currentTime - closestNote.timeToHit);
                HitResult result = EvaluateHit(timeDifference);
                
                closestNote.hasBeenHit = true;
                closestNote.result = result;
                
                ProcessHitResult(result);
                DestroyClosestNoteObject();
            }
            else
            {
                // No note to hit
                ShowFeedback("Too Early!");
            }
        }
        
        private RhythmNote GetClosestUnhitNote(float currentTime)
        {
            RhythmNote closest = null;
            float closestDistance = float.MaxValue;
            
            foreach (var note in notes)
            {
                if (!note.hasBeenHit)
                {
                    float distance = Mathf.Abs(currentTime - note.timeToHit);
                    if (distance < closestDistance && distance <= badWindow)
                    {
                        closest = note;
                        closestDistance = distance;
                    }
                }
            }
            
            return closest;
        }
        
        private HitResult EvaluateHit(float timeDifference)
        {
            if (timeDifference <= perfectWindow)
                return HitResult.Perfect;
            else if (timeDifference <= goodWindow)
                return HitResult.Good;
            else if (timeDifference <= badWindow)
                return HitResult.Bad;
            else
                return HitResult.Miss;
        }
        
        private void ProcessHitResult(HitResult result)
        {
            switch (result)
            {
                case HitResult.Perfect:
                    perfectHits++;
                    score += 100;
                    ShowFeedback("Perfect!");
                    break;
                case HitResult.Good:
                    goodHits++;
                    score += 75;
                    ShowFeedback("Good!");
                    break;
                case HitResult.Bad:
                    badHits++;
                    score += 25;
                    ShowFeedback("Bad!");
                    break;
                case HitResult.Miss:
                    missedHits++;
                    ShowFeedback("Miss!");
                    break;
            }
            
            UpdateUI();
        }
        
        private void DestroyClosestNoteObject()
        {
            if (noteObjects.Count > 0)
            {
                GameObject noteToDestroy = noteObjects[0];
                noteObjects.RemoveAt(0);
                if (noteToDestroy != null)
                    Destroy(noteToDestroy);
            }
        }
        
        private void ShowFeedback(string text)
        {
            if (feedbackText != null)
            {
                feedbackText.text = text;
                StartCoroutine(ClearFeedbackAfterDelay(1f));
            }
        }
        
        private IEnumerator ClearFeedbackAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (feedbackText != null)
                feedbackText.text = "";
        }
        
        private void UpdateGame()
        {
            float progress = (Time.time - gameStartTime) / songDuration;
            if (progressBar != null)
                progressBar.value = progress;
        }
        
        private void UpdateUI()
        {
            if (scoreText != null)
                scoreText.text = $"Score: {score}";
        }
        
        private void EndGame()
        {
            // Calculate performance score (0.0 to 1.0)
            int totalHits = perfectHits + goodHits + badHits + missedHits;
            float performanceScore = 0f;
            
            if (totalHits > 0)
            {
                performanceScore = (perfectHits * 1.0f + goodHits * 0.75f + badHits * 0.5f) / totalHits;
            }
            
            // Clean up remaining note objects
            foreach (var noteObj in noteObjects)
            {
                if (noteObj != null)
                    Destroy(noteObj);
            }
            noteObjects.Clear();
            
            // Complete the lesson
            if (GameManager.Instance?.AppointmentManager != null && currentAppointment != null)
            {
                GameManager.Instance.AppointmentManager.CompleteLesson(currentAppointment, performanceScore);
            }
            
            // Close mini-game
            gameObject.SetActive(false);
        }
        
        public void QuitMiniGame()
        {
            StopAllCoroutines();
            isPlaying = false;
            
            // Clean up
            foreach (var noteObj in noteObjects)
            {
                if (noteObj != null)
                    Destroy(noteObj);
            }
            noteObjects.Clear();
            
            gameObject.SetActive(false);
        }
    }
}