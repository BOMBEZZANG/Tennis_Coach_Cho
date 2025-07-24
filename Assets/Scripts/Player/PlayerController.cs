using UnityEngine;
using TennisCoachCho.Core;
using TennisCoachCho.World;

namespace TennisCoachCho.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Transform homePosition;
        [SerializeField] private Transform tennisCourtPosition;
        
        private Rigidbody2D rb;
        private Vector2 movementInput;
        private bool canMove = true;
        
        public Vector3 HomePosition => homePosition.position;
        public Vector3 TennisCourtPosition => tennisCourtPosition.position;
        public bool CanMove => canMove;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody2D>();
                
            rb.gravityScale = 0f;
        }
        
        private void Update()
        {
            if (!canMove) return;
            
            HandleInput();
        }
        
        private void FixedUpdate()
        {
            if (!canMove) return;
            
            MovePlayer();
        }
        
        private void HandleInput()
        {
            movementInput.x = Input.GetAxisRaw("Horizontal");
            movementInput.y = Input.GetAxisRaw("Vertical");
            
            // Normalize diagonal movement
            movementInput = movementInput.normalized;
        }
        
        private void MovePlayer()
        {
            Vector2 movement = movementInput * moveSpeed;
            rb.linearVelocity = movement;
        }
        
        public void SpawnAtHome()
        {
            if (homePosition != null)
            {
                transform.position = homePosition.position;
            }
        }
        
        public void TeleportToLocation(string locationName)
        {
            switch (locationName.ToLower())
            {
                case "home":
                    transform.position = homePosition.position;
                    break;
                case "tennis court":
                case "court":
                    transform.position = tennisCourtPosition.position;
                    break;
            }
        }
        
        public void SetMovementEnabled(bool enabled)
        {
            canMove = enabled;
            if (!enabled)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
        
        public bool IsAtLocation(string locationName)
        {
            Vector3 targetPosition = Vector3.zero;
            
            switch (locationName.ToLower())
            {
                case "home":
                    targetPosition = homePosition.position;
                    break;
                case "tennis court":
                case "court":
                    targetPosition = tennisCourtPosition.position;
                    break;
                default:
                    return false;
            }
            
            float distance = Vector3.Distance(transform.position, targetPosition);
            return distance < 1.5f; // Within 1.5 units of the location
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"[PlayerController] OnTriggerEnter2D - Collided with: {other.name}, Tag: '{other.tag}'");
            Debug.Log($"[PlayerController] GameObject path: {GetGameObjectPath(other.gameObject)}");
            Debug.Log($"[PlayerController] Collider bounds: {other.bounds}");
            Debug.Log($"[PlayerController] Is Trigger: {other.isTrigger}");
            
            // List all components on the collided object for debugging
            var components = other.GetComponents<Component>();
            Debug.Log($"[PlayerController] Components on {other.name}: {string.Join(", ", System.Array.ConvertAll(components, c => c.GetType().Name))}");
            
            if (other.CompareTag("Location"))
            {
                Debug.Log($"[PlayerController] ✅ Found Location tag on: {other.name}");
                LocationTrigger location = other.GetComponent<LocationTrigger>();
                if (location != null)
                {
                    Debug.Log($"[PlayerController] ✅ LocationTrigger found on: {other.name}");
                    Debug.Log($"[PlayerController] LocationTrigger settings - Name: '{location.LocationName}', CanStart: {location.CanStartLessons}");
                    Debug.Log($"[PlayerController] Calling OnPlayerEnter()...");
                    location.OnPlayerEnter();
                }
                else
                {
                    Debug.LogError($"[PlayerController] ❌ No LocationTrigger script found on: {other.name}");
                }
            }
            else
            {
                Debug.LogWarning($"[PlayerController] ❌ Object {other.name} does not have 'Location' tag. Current tag: '{other.tag}'");
                
                // Try to find LocationTrigger anyway for debugging
                LocationTrigger location = other.GetComponent<LocationTrigger>();
                if (location != null)
                {
                    Debug.LogWarning($"[PlayerController] ⚠️ Found LocationTrigger on {other.name} but tag is '{other.tag}' instead of 'Location'");
                    Debug.LogWarning($"[PlayerController] FIX: Change the tag to 'Location' on {other.name}");
                    // Call it anyway for testing
                    location.OnPlayerEnter();
                }
                else
                {
                    Debug.Log($"[PlayerController] No LocationTrigger script found on {other.name} (this is normal for non-location objects)");
                }
            }
        }
        
        private string GetGameObjectPath(GameObject obj)
        {
            string path = obj.name;
            Transform parent = obj.transform.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            return path;
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            Debug.Log($"[PlayerController] OnTriggerExit2D - Exiting: {other.name}, Tag: {other.tag}");
            
            if (other.CompareTag("Location"))
            {
                LocationTrigger location = other.GetComponent<LocationTrigger>();
                if (location != null)
                {
                    Debug.Log($"[PlayerController] Calling OnPlayerExit() for: {other.name}");
                    location.OnPlayerExit();
                }
            }
            else
            {
                // Try to find LocationTrigger anyway for debugging
                LocationTrigger location = other.GetComponent<LocationTrigger>();
                if (location != null)
                {
                    Debug.LogWarning($"[PlayerController] Calling OnPlayerExit() for {other.name} despite wrong tag");
                    location.OnPlayerExit();
                }
            }
        }
    }
}