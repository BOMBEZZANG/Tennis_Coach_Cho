using UnityEngine;

namespace TennisCoachCho.Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private Transform target; // Player to follow
        [SerializeField] private float followSpeed = 2f;
        [SerializeField] private float lookAheadDistance = 1.5f;
        
        [Header("Camera Bounds (Optional)")]
        [SerializeField] private bool useBounds = false;
        [SerializeField] private float minX = -10f;
        [SerializeField] private float maxX = 10f;
        [SerializeField] private float minY = -5f;
        [SerializeField] private float maxY = 5f;
        
        [Header("Smoothing Settings")]
        [SerializeField] private bool useSmoothing = true;
        [SerializeField] private float smoothTime = 0.3f;
        
        [Header("Debug")]
        [SerializeField] private bool debugMode = false;
        
        private Vector3 velocity = Vector3.zero;
        private Vector3 lastTargetPosition;
        private Camera cam;
        
        private void Awake()
        {
            cam = GetComponent<Camera>();
            if (cam == null)
            {
                Debug.LogError("[CameraController] No Camera component found!");
            }
        }
        
        private void Start()
        {
            // Find player if not assigned
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                    Debug.Log($"[CameraController] Auto-found player: {player.name}");
                }
                else
                {
                    Debug.LogError("[CameraController] No target assigned and no GameObject with 'Player' tag found!");
                }
            }
            
            if (target != null)
            {
                lastTargetPosition = target.position;
                
                // Set initial camera position to target (without smooth transition)
                Vector3 initialPos = GetDesiredPosition();
                transform.position = new Vector3(initialPos.x, initialPos.y, transform.position.z);
            }
        }
        
        private void LateUpdate()
        {
            if (target == null) return;
            
            FollowTarget();
        }
        
        private void FollowTarget()
        {
            Vector3 desiredPosition = GetDesiredPosition();
            
            if (useSmoothing)
            {
                // Smooth movement
                Vector3 targetPosition = new Vector3(desiredPosition.x, desiredPosition.y, transform.position.z);
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            }
            else
            {
                // Direct movement
                transform.position = new Vector3(desiredPosition.x, desiredPosition.y, transform.position.z);
            }
            
            if (debugMode)
            {
                Debug.Log($"[CameraController] Camera pos: {transform.position}, Target pos: {target.position}");
            }
        }
        
        private Vector3 GetDesiredPosition()
        {
            Vector3 targetPos = target.position;
            
            // Add look-ahead based on movement direction
            Vector3 lookAhead = Vector3.zero;
            if (lookAheadDistance > 0)
            {
                Vector3 targetMovement = targetPos - lastTargetPosition;
                if (targetMovement.magnitude > 0.01f) // Only apply look-ahead if actually moving
                {
                    lookAhead = targetMovement.normalized * lookAheadDistance;
                }
            }
            
            Vector3 desiredPos = targetPos + lookAhead;
            
            // Apply camera bounds if enabled
            if (useBounds)
            {
                desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);
                desiredPos.y = Mathf.Clamp(desiredPos.y, minY, maxY);
            }
            
            lastTargetPosition = targetPos;
            return desiredPos;
        }
        
        // Public methods for runtime control
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            Debug.Log($"[CameraController] Target set to: {newTarget?.name ?? "null"}");
        }
        
        public void SetFollowSpeed(float speed)
        {
            followSpeed = Mathf.Max(0.1f, speed);
        }
        
        public void SetSmoothTime(float time)
        {
            smoothTime = Mathf.Max(0.01f, time);
        }
        
        public void EnableBounds(float minX, float maxX, float minY, float maxY)
        {
            useBounds = true;
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
        }
        
        public void DisableBounds()
        {
            useBounds = false;
        }
        
        public void TeleportToTarget()
        {
            if (target == null) return;
            
            Vector3 targetPos = GetDesiredPosition();
            transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);
            velocity = Vector3.zero; // Reset smoothing velocity
        }
        
        // Gizmo drawing for scene view
        private void OnDrawGizmosSelected()
        {
            if (useBounds)
            {
                Gizmos.color = Color.yellow;
                Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, transform.position.z);
                Vector3 size = new Vector3(maxX - minX, maxY - minY, 1f);
                Gizmos.DrawWireCube(center, size);
            }
            
            if (target != null && lookAheadDistance > 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(target.position, lookAheadDistance);
            }
        }
    }
}