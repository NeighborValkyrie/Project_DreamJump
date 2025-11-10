using UnityEngine;

namespace PlatformerGame.Core.Player
{
    /// <summary>
    /// Rigidbody 기반 플레이어 이동 및 점프 제어
    /// v7.0: Y축 회전 허용 수정
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float baseMoveSpeed = 7f;
        [SerializeField] private float rotationSpeed = 500f;

        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float jumpMultiplier = 1f;

        [Header("Ground Check")]
        [SerializeField] private float groundAngleTolerance = 0.707f;

        private Rigidbody rb;
        private Vector3 moveDirection = Vector3.zero;
        private bool isGrounded = false;
        private float currentSpeedMultiplier = 1f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            // ⭐ 중요: Y축 회전은 허용, X/Z만 고정
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        private void FixedUpdate()
        {
            CheckGroundStatus();
            ApplyMovement();
            ApplyRotation();
        }

        private void CheckGroundStatus()
        {
            isGrounded = false;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
            {
                float angle = Vector3.Dot(hit.normal, Vector3.up);
                if (angle >= groundAngleTolerance)
                {
                    isGrounded = true;
                }
            }
        }

        private void ApplyMovement()
        {
            if (moveDirection.magnitude >= 0.1f)
            {
                float targetSpeed = baseMoveSpeed * currentSpeedMultiplier;
                Vector3 targetVelocity = moveDirection * targetSpeed;

                rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
            }
        }

        private void ApplyRotation()
        {
            if (moveDirection.magnitude >= 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                );
            }
        }

        public void SetMoveDirection(Vector3 direction)
        {
            moveDirection = direction;
        }

        public void Jump()
        {
            if (isGrounded)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce * jumpMultiplier, rb.velocity.z);

                // 점프 이벤트 발생
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
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            currentSpeedMultiplier = multiplier;
        }

        public bool IsGrounded() => isGrounded;
        public float GetMoveSpeed() => moveDirection.magnitude * baseMoveSpeed;
    }
}