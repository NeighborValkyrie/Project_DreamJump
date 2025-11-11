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
        [SerializeField] private float jumpMultiplier = 1f; // JumpingPlatform 호환
        [SerializeField] private float gravity = 20f;
        [SerializeField] private float jumpBufferTime = 0.2f;
        [SerializeField] private float coyoteTime = 0.15f;

        [Header("Ground Check")]
        [SerializeField] private float groundCheckRadius = 0.3f;
        [SerializeField] private Vector3 groundCheckOffset = new Vector3(0, 0.1f, 0);
        [SerializeField] private float groundAngleTolerance = 0.707f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Camera")]
        [SerializeField] private Transform cameraTransform;

        private Rigidbody rb;
        private Vector3 moveDirection;
        private Vector3 currentVelocity;
        private bool isGrounded;
        private float lastGroundedTime;
        private float lastJumpPressTime;
        private float currentSpeedMultiplier = 1f; // 플랫폼 효과용

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
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
            HandleJumpBuffer();
        }

        private void CheckGroundStatus()
        {
            isGrounded = false;

            // 구체 체크
            if (Physics.CheckSphere(transform.position + groundCheckOffset, groundCheckRadius, groundLayer))
            {
                isGrounded = true;
                lastGroundedTime = Time.time;
            }
            else
            {
                // 레이캐스트로 보정 (경사로 대응)
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
                {
                    float angle = Vector3.Dot(hit.normal, Vector3.up);
                    if (angle >= groundAngleTolerance)
                    {
                        isGrounded = true;
                        lastGroundedTime = Time.time;
                    }
                }
            }
        }

        private void ApplyMovement()
        {
            if (moveDirection.magnitude < 0.1f)
            {
                currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
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
            if (currentVelocity.magnitude > 0.1f)
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

        private void HandleJumpBuffer()
        {
            if (Time.time - lastJumpPressTime < jumpBufferTime &&
                Time.time - lastGroundedTime < coyoteTime)
            {
                PerformJump();
                lastJumpPressTime = -1f;
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

        // 기존 호환성 유지
        public void SetMoveDirection(Vector3 direction)
        {
            moveDirection = direction;
        }

        // 새로운 입력 방식
        public void SetMoveInput(Vector3 input)
        {
            moveDirection = input;
        }

        public void Jump()
        {
            lastJumpPressTime = Time.time;

            if (isGrounded || Time.time - lastGroundedTime < coyoteTime)
            {
                PerformJump();
            }
        }

        private void PerformJump()
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce * jumpMultiplier, rb.velocity.z);
            lastGroundedTime = -1f;

            if (PlatformerGame.Systems.Events.GameEventManager.Instance != null)
            {
                PlatformerGame.Systems.Events.GameEventManager.Instance.TriggerPlayerJumped();
            }
        }

        public void Teleport(Vector3 position)
        {
            transform.position = position;
            rb.velocity = Vector3.zero;
            currentVelocity = Vector3.zero;
        }

        // 플랫폼 효과용 (StickyPlatform, JumpingPlatform)
        public void SetSpeedMultiplier(float multiplier)
        {
            currentSpeedMultiplier = multiplier;
        }

        public void SetCameraTransform(Transform cam)
        {
            cameraTransform = cam;
        }

        // 기존 호환성: PlayerAnimation, UI 등에서 사용
        public bool IsGrounded() => isGrounded;
        public float GetMoveSpeed() => currentVelocity.magnitude;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
        }
    }
}