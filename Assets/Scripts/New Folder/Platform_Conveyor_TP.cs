// Platform_Conveyor_TP_CM_Fix.cs
using UnityEngine;
using TraversalPro;

[RequireComponent(typeof(Collider))]
[DefaultExecutionOrder(-1500)] // CharacterRun(-2000) 뒤, CharacterMotor(-1000) 앞
public class Platform_Conveyor_TP_CM_Fix : MonoBehaviour
{
    public enum FlowMode { Vector, Downhill }   // [변경가능: 필요 없으면 Vector만 쓰기]
    public FlowMode flow = FlowMode.Vector;
    public enum SpaceMode { World, Local }

    [Header("컨베이어 흐름")]
    public SpaceMode space = SpaceMode.Local;
    public Vector3 direction = Vector3.back;
    [Min(0)] public float speed = 4f;
    public bool horizontalOnly = true;
    public bool projectOnGround = true;

    [Header("동작 옵션")]
    public bool pushWhenIdle = true;
    public bool opposeOnlyWhenMovingForward = false;
    public float maxSpeedPadding = 1.5f;

    [Header("적용 범위")]
    public Transform platformRoot;          // 실제 바닥(비-트리거) 콜라이더가 달린 루트
    public bool requireSameGround = true;   // 같은 바닥일 때만 적용할지

    private CharacterMotor motor;

    void Awake()
    {
        if (!platformRoot) platformRoot = transform.parent ? transform.parent : transform;

        var col = GetComponent<Collider>();
        col.isTrigger = true;

        // 트리거 신뢰성 확보: kinematic RB 보장
        var rb = GetComponent<Rigidbody>();
        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        TryBind(other);
    }

    void OnTriggerStay(Collider other)  // ★ 첫 프레임 겹침/활성 순서 문제 해결
    {
        if (!motor) TryBind(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (!motor) return;
        var m = other.GetComponentInParent<CharacterMotor>();
        if (m == motor) motor = null;
    }

    void FixedUpdate()
    {
        if (!motor || !motor.IsGrounded) return;

        // 같은 바닥만 적용할지 선택
        if (requireSameGround)
        {
            var groundCol = motor.Ground.Collider;
            if (!groundCol) return;
            bool onThis = (groundCol.transform == platformRoot) || groundCol.transform.IsChildOf(platformRoot);
            if (!onThis) return;
        }

        // 1) 월드 기준 컨베이어 벡터
        Vector3 dir = direction.sqrMagnitude > 1e-6f ? direction.normalized : Vector3.zero;
        Vector3 worldFlow = (space == SpaceMode.Local ? transform.TransformDirection(dir) : dir) * speed;
        if (projectOnGround && motor.Ground.Collider) worldFlow = Vector3.ProjectOnPlane(worldFlow, motor.Ground.Normal);
        if (horizontalOnly) worldFlow.y = 0f;
        if (worldFlow.sqrMagnitude < 1e-6f) return;

        // 2) 입력/방향 조건
        Vector3 moveInput = motor.MoveInput;
        bool hasInput = moveInput.sqrMagnitude > 1e-6f;
        if (!pushWhenIdle && !hasInput) return;

        if (opposeOnlyWhenMovingForward && hasInput)
        {
            var run = motor.GetComponent<CharacterRun>();
            Transform view = run ? run.view : motor.transform;
            Quaternion yaw = Quaternion.Euler(0f, view.eulerAngles.y, 0f);
            Vector3 worldMoveDir = (yaw * moveInput).normalized;
            if (Vector3.Dot(worldFlow.normalized, worldMoveDir) >= 0f) return;
        }

       
        // 3) 목표속도 보정(월드 벡터 가산)
        motor.LocalVelocityGoal += worldFlow;

        // 4) 상한 여유
        motor.MaxLocalSpeed = Mathf.Max(motor.MaxLocalSpeed, motor.LocalVelocityGoal.magnitude + maxSpeedPadding);

        // // 디버그 레이(필요하면 주석 해제)
        // UnityEngine.Debug.DrawRay(motor.transform.position + Vector3.up * 0.5f, worldFlow, Color.cyan, Time.fixedDeltaTime, false);
    }

    void TryBind(Collider other)
    {
        if (motor) return;
        var m = other.GetComponentInParent<CharacterMotor>();
        if (m) motor = m;
    }
#if UNITY_EDITOR
private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.cyan;
    Vector3 dir = direction.sqrMagnitude > 1e-6f ? direction.normalized : Vector3.forward;
    Vector3 worldDir = (space == SpaceMode.Local ? transform.TransformDirection(dir) : dir);
    Vector3 p = transform.position + Vector3.up * 0.05f;
    Gizmos.DrawLine(p, p + worldDir);
    Gizmos.DrawSphere(p + worldDir, 0.05f);
}
#endif

}
