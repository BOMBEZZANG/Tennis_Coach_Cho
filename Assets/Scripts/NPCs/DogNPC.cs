using UnityEngine;
using TennisCoachCho.Core;

namespace TennisCoachCho.NPCs
{
    public class DogNPC : MonoBehaviour
    {
        [Header("Dog Settings")]
        [SerializeField] private string dogName = "Sparky";
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float followDistance = 2f;
        [SerializeField] private float stoppingDistance = 1f;
        
        [Header("Visual Elements")]
        [SerializeField] private SpriteRenderer dogRenderer;
        [SerializeField] private LineRenderer leashRenderer;
        
        [Header("AI Behavior")]
        [SerializeField] private float idleWanderRadius = 3f;
        [SerializeField] private float wanderInterval = 2f;
        
        public enum DogState
        {
            IdleWithOwner,
            Follow,
            Walking // For future park walking mini-game
        }
        
        [SerializeField] private DogState currentState = DogState.IdleWithOwner;
        
        private Transform followTarget;
        private Transform owner;
        private Vector3 idlePosition;
        private Vector3 targetPosition;
        private float nextWanderTime;
        
        public DogState CurrentState => currentState;
        public string DogName => dogName;
        
        private void Start()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            // Set placeholder appearance (Grey-box: simple colored circle)
            if (dogRenderer == null)
            {
                dogRenderer = GetComponent<SpriteRenderer>();
                if (dogRenderer == null)
                {
                    dogRenderer = gameObject.AddComponent<SpriteRenderer>();
                }
            }
            
            SetupPlaceholderAppearance();
            SetupLeashRenderer();
            
            // Set initial idle position
            idlePosition = transform.position;
            targetPosition = idlePosition;
            
            Debug.Log("[DogNPC] " + dogName + " initialized in " + currentState + " state");
        }
        
        private void SetupPlaceholderAppearance()
        {
            // Grey-box implementation: Create simple colored circle
            // Dog = Yellow Circle
            
            // Create a simple circle sprite (placeholder)
            int size = 24;
            Texture2D texture = new Texture2D(size, size);
            Color dogColor = Color.yellow;
            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f - 2f;
            
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Vector2 point = new Vector2(x, y);
                    if (Vector2.Distance(point, center) <= radius)
                    {
                        texture.SetPixel(x, y, dogColor);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }
            texture.Apply();
            
            Sprite dogSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
            dogRenderer.sprite = dogSprite;
            
            Debug.Log($"[DogNPC] Placeholder appearance set: Yellow Circle");
        }
        
        private void SetupLeashRenderer()
        {
            // Setup leash line renderer (initially disabled)
            if (leashRenderer == null)
            {
                leashRenderer = gameObject.AddComponent<LineRenderer>();
            }
            
            leashRenderer.material = new Material(Shader.Find("Sprites/Default"));
            Color brownColor = new Color(0.5f, 0.25f, 0f); // Custom brown color
            leashRenderer.startColor = brownColor;
            leashRenderer.endColor = brownColor;
            leashRenderer.startWidth = 0.1f;
            leashRenderer.endWidth = 0.1f;
            leashRenderer.positionCount = 2;
            leashRenderer.enabled = false;
            
            Debug.Log("[DogNPC] Leash renderer setup complete");
        }
        
        private void Update()
        {
            UpdateBehavior();
            UpdateLeashVisual();
        }
        
        private void UpdateBehavior()
        {
            switch (currentState)
            {
                case DogState.IdleWithOwner:
                    HandleIdleBehavior();
                    break;
                    
                case DogState.Follow:
                    HandleFollowBehavior();
                    break;
                    
                case DogState.Walking:
                    // TODO: Implement walking mini-game behavior
                    break;
            }
        }
        
        private void HandleIdleBehavior()
        {
            // Simple idle wandering around owner/idle position
            if (Time.time >= nextWanderTime)
            {
                // Choose a random point within wander radius
                Vector2 randomDirection = Random.insideUnitCircle * idleWanderRadius;
                targetPosition = idlePosition + new Vector3(randomDirection.x, randomDirection.y, 0);
                
                nextWanderTime = Time.time + wanderInterval + Random.Range(-0.5f, 0.5f);
            }
            
            // Move towards target position
            MoveTowardsTarget(targetPosition, 0.2f);
        }
        
        private void HandleFollowBehavior()
        {
            if (followTarget == null)
            {
                Debug.LogWarning("[DogNPC] " + dogName + " is in Follow state but has no follow target!");
                return;
            }
            
            float distanceToTarget = Vector3.Distance(transform.position, followTarget.position);
            
            // Only move if we're too far from the target
            if (distanceToTarget > followDistance)
            {
                MoveTowardsTarget(followTarget.position, stoppingDistance);
            }
        }
        
        private void MoveTowardsTarget(Vector3 target, float stopDistance)
        {
            Vector3 direction = (target - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target);
            
            if (distance > stopDistance)
            {
                transform.position += direction * moveSpeed * Time.deltaTime;
                
                // Simple facing direction (flip sprite if moving left)
                if (dogRenderer != null)
                {
                    dogRenderer.flipX = direction.x < 0;
                }
            }
        }
        
        private void UpdateLeashVisual()
        {
            if (currentState == DogState.Follow && followTarget != null && leashRenderer != null)
            {
                leashRenderer.enabled = true;
                leashRenderer.SetPosition(0, transform.position);
                leashRenderer.SetPosition(1, followTarget.position);
            }
            else if (leashRenderer != null)
            {
                leashRenderer.enabled = false;
            }
        }
        
        public void ChangeState(DogState newState)
        {
            if (currentState == newState) return;
            
            DogState previousState = currentState;
            currentState = newState;
            
            Debug.Log("[DogNPC] " + dogName + " state changed: " + previousState + " → " + newState);
            
            OnStateChanged(previousState, newState);
        }
        
        private void OnStateChanged(DogState previousState, DogState newState)
        {
            switch (newState)
            {
                case DogState.IdleWithOwner:
                    idlePosition = transform.position;
                    targetPosition = idlePosition;
                    nextWanderTime = Time.time + wanderInterval;
                    break;
                    
                case DogState.Follow:
                    // Leash visual will be handled in UpdateLeashVisual()
                    Debug.Log("SFX_Dog_Leash_Attach_Plays_Here");
                    break;
                    
                case DogState.Walking:
                    // TODO: Initialize walking mini-game state
                    break;
            }
        }
        
        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
            Debug.Log("[DogNPC] " + dogName + " follow target set to: " + (target != null ? target.name : "null"));
        }
        
        public void SetOwner(Transform ownerTransform)
        {
            owner = ownerTransform;
            if (currentState == DogState.IdleWithOwner)
            {
                idlePosition = ownerTransform.position;
            }
        }
        
        public void SetDogName(string name)
        {
            dogName = name;
        }
        
        // Method for future walking mini-game integration
        public void StartWalkingActivity()
        {
            ChangeState(DogState.Walking);
            Debug.Log("[DogNPC] " + dogName + " started walking activity");
        }
        
        public void StopWalkingActivity()
        {
            ChangeState(DogState.Follow);
            Debug.Log("[DogNPC] " + dogName + " stopped walking activity");
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw follow distance
            if (currentState == DogState.Follow && followTarget != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(followTarget.position, followDistance);
            }
            
            // Draw idle wander area
            if (currentState == DogState.IdleWithOwner)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(idlePosition, idleWanderRadius);
            }
        }
    }
}