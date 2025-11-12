using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TraversalPro
{
    [RequireComponent(typeof(ICharacterMotor))]
    [AddComponentMenu("Traversal Pro/Character Physics/Jump")]
    public class Jump : MonoBehaviour, IJump
    {
        [Header("Jump")]
        [Min(0)] public float height = 1f;                 // [변경가능]
        [Min(0)] public float cooldownDuration = .5f;      // [변경가능]
        [Min(0)] public float graceDuration = .1f;         // [변경가능] (코요테 타임)
        [Min(0)] public float delay = .1f;                 // [변경가능] (입력~적용 지연)
        [Min(0)] public float maxGroundForce = 100000;     // [변경가능]

        [Header("Double Jump")]
        public int maxExtraJumps = 0;                      // [변경가능] (버프 시 1)
        int extraJumpsLeft = 0;

        InterfaceRef<ICharacterMotor> characterMotor;
        double jumpRequestTime, jumpForceTime = double.MaxValue, lastGroundedTime;
        const float recentJumpDuration = .2f;
        public double LastJumpTime { get; private set; }

        Rigidbody RB => characterMotor.Value.Rigidbody;

        void Awake()
        {
            var cm = GetComponent<ICharacterMotor>();
            if (cm == null) { Debug.LogError("[Jump] ICharacterMotor가 없습니다."); enabled = false; return; }
            characterMotor.Value = cm;
        }

        void OnEnable()
        {
            LastJumpTime = float.MinValue;
            characterMotor.Value.Moving += CheckForJump;
        }

        void OnDisable()
        {
            characterMotor.Value.Moving -= CheckForJump;
        }

        void CheckForJump(ICharacterMotor _)
        {
            double t = Time.timeAsDouble;

            if (characterMotor.Value.IsGrounded)
            {
                lastGroundedTime = t;
                extraJumpsLeft = Mathf.Max(0, maxExtraJumps);
            }

            if (ShouldJump())
            {
                LastJumpTime = t;
                jumpForceTime = t + delay;
            }

            if (t >= jumpForceTime)
            {
                jumpForceTime = double.MaxValue;
                PerformJump();
            }

            if (t >= LastJumpTime + delay && t <= LastJumpTime + delay + recentJumpDuration)
                characterMotor.Value.IsGrounded = false;
        }

        bool ShouldJump()
        {
            if (jumpRequestTime == 0) return false;
            double t = Time.timeAsDouble;
            if (t < jumpRequestTime) return false;
            if (t > jumpRequestTime + graceDuration) return false;
            if (characterMotor.Value.IsOnSteepSlope) return false;

            bool groundedOrCoyote = characterMotor.Value.IsGrounded || (t < lastGroundedTime + graceDuration);

            // 지상/코요테: 쿨다운 체크
            if (groundedOrCoyote)
            {
                if (jumpRequestTime < LastJumpTime + cooldownDuration) return false;
                return true;
            }

            // 공중: 더블점프 (쿨다운 미적용)
            if (extraJumpsLeft > 0) { extraJumpsLeft--; return true; }

            return false;
        }

        public void PerformJump()
        {
            if (Time.deltaTime <= 0) return;

            Vector3 v = RB.velocity;
            float gy = Physics.gravity.y; // 단순 중력 사용
            float jumpV = CalcJumpSpeed(gy, height);
            float priorY = v.y;

            v.y = jumpV + characterMotor.Value.Ground.PointVelocity.y;
            RB.velocity = v;

            if (characterMotor.Value.Ground.Rigidbody)
            {
                float mass = RB.mass;
                float force = (v.y - priorY) / Time.deltaTime * mass;
                force = Mathf.Clamp(force, 0, maxGroundForce);
                float gravityForce = gy * mass;
                Vector3 groundForce = new Vector3(0, gravityForce - force, 0);
                characterMotor.Value.Ground.Rigidbody.AddForceAtPosition(groundForce, characterMotor.Value.Ground.Point, ForceMode.Force);
            }
        }

        static float CalcJumpSpeed(float gravityY, float heightMeters)
        {
            return Mathf.Sqrt(Mathf.Max(0.0001f, 2f * Mathf.Abs(gravityY) * heightMeters));
        }

        public void RequestJump() => jumpRequestTime = Time.timeAsDouble;

        public void RequestJump(InputAction.CallbackContext context)
        {
            if (!isActiveAndEnabled) return;
            if (context.phase == InputActionPhase.Started) RequestJump();
        }

        // 버프에서 호출
        public void EnableDoubleJump(float duration) { StartCoroutine(DoubleJumpRoutine(duration)); }

        IEnumerator DoubleJumpRoutine(float duration)
        {
            maxExtraJumps = Mathf.Max(1, maxExtraJumps);
            extraJumpsLeft = Mathf.Max(1, extraJumpsLeft);
            Debug.Log($"[BUFF] 더블 점프 시작 ({duration:0.##}s)");
            float end = Time.time + Mathf.Max(0.01f, duration);
            while (Time.time < end) yield return null;
            maxExtraJumps = 0;
            extraJumpsLeft = 0;
            Debug.Log("[BUFF] 더블 점프 종료");
        }
        public Rigidbody Rigidbody => characterMotor.Value.Rigidbody;
    }
}
