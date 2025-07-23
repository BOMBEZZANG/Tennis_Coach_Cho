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
            if (other.CompareTag("Location"))
            {
                LocationTrigger location = other.GetComponent<LocationTrigger>();
                if (location != null)
                {
                    location.OnPlayerEnter();
                }
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Location"))
            {
                LocationTrigger location = other.GetComponent<LocationTrigger>();
                if (location != null)
                {
                    location.OnPlayerExit();
                }
            }
        }
    }
}