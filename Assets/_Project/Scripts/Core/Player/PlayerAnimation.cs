using UnityEngine;

namespace PlatformerGame.Core.Player
{
    /// <summary>
    /// 플레이어 애니메이션 제어
    /// v7.0: 자식 오브젝트에서 Animator 자동 탐색
    /// </summary>
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator animator;
        private PlayerMovement movement;

        // 애니메이터 파라미터 해시 (성능 최적화)
        private int speedHash;
        private int isGroundedHash;
        private int isJumpingHash;
        private int isFallingHash;

        private void Awake()
        {
            movement = GetComponent<PlayerMovement>();

            // 자식 오브젝트에서 Animator 찾기
            animator = GetComponentInChildren<Animator>();

            if (animator == null)
            {
                Debug.LogError("[PlayerAnimation] Animator를 자식 오브젝트에서 찾을 수 없습니다!");
                enabled = false;
                return;
            }

            // 해시 캐싱
            speedHash = Animator.StringToHash("Speed");
            isGroundedHash = Animator.StringToHash("IsGrounded");
            isJumpingHash = Animator.StringToHash("IsJumping");
            isFallingHash = Animator.StringToHash("IsFalling");
        }

        private void Update()
        {
            UpdateAnimationParameters();
        }

        private void UpdateAnimationParameters()
        {
            float speed = movement.GetMoveSpeed();
            bool grounded = movement.IsGrounded();

            animator.SetFloat(speedHash, speed);
            animator.SetBool(isGroundedHash, grounded);

            // 점프/낙하 판단
            float verticalVelocity = GetComponent<Rigidbody>().velocity.y;
            animator.SetBool(isJumpingHash, verticalVelocity > 0.1f && !grounded);
            animator.SetBool(isFallingHash, verticalVelocity < -0.1f && !grounded);
        }
    }
}