using UnityEngine;

namespace PlatformerGame.Core.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float baseMoveSpeed = 7f;
        [SerializeField] private float rotationSpeed = 500f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 15f;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float jumpMultiplier = 1f;
        [SerializeField] private float gravity = 20f;

        [Header("Ground Check")]
        [SerializeField] private float groundCheckDistance = 0.3f;
        [SerializeField] private float groundCheckRadius = 0.4f;
        [SerializeField] private LayerMask groundLayer = -1;

        [Header("Camera")]
        [SerializeField] private Transform cameraTransform;

        private Rigidbody rb;
        private CapsuleCollider capsule;
        private Vector3 moveDirection;
        private Vector3 currentVelocity;
        private bool isGrounded;
        private float currentSpeedMultiplier = 1f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            capsule = GetComponent<CapsuleCollider>();
            
            // 중요: Drag를 0으로!
            rb.mass = 1f;
            rb.drag = 0f;  // ← 이게 핵심!
            rb.angularDrag = 0.05f;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            if (cameraTransform == null)
            {
                UnityEngine.Camera mainCam = UnityEngine.Camera.main;
                if (mainCam != null) cameraTransform = mainCam.transform;
            }
        }

        private void FixedUpdate()
        {
            CheckGroundStatus();
            ApplyMovement();
            ApplyRotation();
            ApplyGravity();
        }

        private void CheckGroundStatus()
        {
            isGrounded = false;

            float bottomY = 0f;
            if (capsule != null)
            {
                bottomY = capsule.center.y - (capsule.height * 0.5f) + capsule.radius;
            }

            Vector3 checkPosition = transform.position + Vector3.up * bottomY;

            RaycastHit hit;
            if (Physics.SphereCast(
                checkPosition + Vector3.up * 0.1f,
                groundCheckRadius,
                Vector3.down,
                out hit,
                groundCheckDistance + 0.1f,
                groundLayer))
            {
                if (hit.collider.gameObject != gameObject)
                {
                    isGrounded = true;
                }
            }

            if (!isGrounded)
            {
                if (Physics.Raycast(checkPosition, Vector3.down, out hit, groundCheckDistance + 0.2f, groundLayer))
                {
                    if (hit.collider.gameObject != gameObject)
                    {
                        isGrounded = true;
                    }
                }
            }
        }

        private void ApplyMovement()
        {
            if (moveDirection.magnitude < 0.1f)
            {
                currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
                if (currentVelocity.magnitude < 0.01f) currentVelocity = Vector3.zero;
            }
            else
            {
                Vector3 cameraRelative = GetCameraRelativeDirection(moveDirection);
                Vector3 targetVelocity = cameraRelative * baseMoveSpeed * currentSpeedMultiplier;
                currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            }

            rb.velocity = new Vector3(currentVelocity.x, rb.velocity.y, currentVelocity.z);
        }

        private void ApplyRotation()
        {
            if (moveDirection.magnitude > 0.1f && currentVelocity.magnitude > 1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(currentVelocity);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                );
            }
        }

        private void ApplyGravity()
        {
            if (!isGrounded && rb.velocity.y < 0)
            {
                rb.velocity += Vector3.down * gravity * Time.fixedDeltaTime;
            }
        }

        private Vector3 GetCameraRelativeDirection(Vector3 input)
        {
            if (cameraTransform == null) return input;

            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            return (forward * input.z + right * input.x).normalized;
        }

        public void SetMoveDirection(Vector3 direction) => moveDirection = direction;
        public void SetMoveInput(Vector3 input) => moveDirection = input;

        public void Jump()
        {
            if (isGrounded)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce * jumpMultiplier, rb.velocity.z);

                if (PlatformerGame.Systems.Events.GameEventManager.Instance != null)
                {
                    PlatformerGame.Systems.Events.GameEventManager.Instance.TriggerPlayerJumped();
                }
            }
        }

        public void Teleport(Vector3 position)
        {
            transform.position = position;
            rb.velocity = Vector3.zero;
            currentVelocity = Vector3.zero;
        }

        public void SetSpeedMultiplier(float multiplier) => currentSpeedMultiplier = multiplier;
        public void SetCameraTransform(Transform cam) => cameraTransform = cam;
        public bool IsGrounded() => isGrounded;
        public float GetMoveSpeed() => currentVelocity.magnitude;

        private void OnDrawGizmosSelected()
        {
            float bottomY = 0f;
            CapsuleCollider col = GetComponent<CapsuleCollider>();
            if (col != null)
            {
                bottomY = col.center.y - (col.height * 0.5f) + col.radius;
            }

            Vector3 checkPosition = transform.position + Vector3.up * bottomY;

            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
            Gizmos.DrawLine(checkPosition, checkPosition + Vector3.down * groundCheckDistance);
        }
    }
}