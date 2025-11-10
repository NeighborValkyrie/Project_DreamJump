// CharacterLedgeGrab_TP.cs
using System.Diagnostics;
using TraversalPro;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[DefaultExecutionOrder(-1200)]
public class CharacterLedgeGrab_TP : MonoBehaviour
{
    [Header("Refs")]
    public CharacterMotor motor;

    [Header("Probe")]
    [Min(0)] public float forwardCheck = 0.6f;   // 전방 벽 탐지 거리
    [Min(0)] public float upCheck = 1.2f;   // 위에서 아래로 탐지 거리
    [Min(0)] public float hangDistance = 0.4f;   // 벽에서 떨어져 매달릴 거리
    [Min(0)] public float handHeight = 0.5f;   // 난간 아래로 손 위치 오프셋
    [Min(0)] public float standForwardOffset = 0.2f; // 올라설 때 벽에서 떨어지는 거리
    [Min(0)] public float regrabBlockTime = 0.2f;
    public LayerMask climbMask;                  // 비우면 motor.layerMask 사용
    public bool debugDraw = false;

    [Header("Gate")]
    public float minDownVelocity = -0.5f;        // 이 속도보다 느리면 매달리기 시작 안함

    bool isHanging;
    Vector3 wallNormal;
    Vector3 ledgeTopPoint;
    Vector3 hangAnchor;
    float regrabTimer;

    void Reset() { motor = GetComponent<CharacterMotor>(); }
    void OnEnable() { if (!motor) motor = GetComponent<CharacterMotor>(); if (motor) motor.Moving += OnMotorMoving; }
    void OnDisable() { if (motor) motor.Moving -= OnMotorMoving; }
    void Update() { if (regrabTimer > 0) regrabTimer -= Time.deltaTime; }

    void OnMotorMoving(ICharacterMotor _)
    {
        if (isHanging)
        {
            HoldHang();
            HandleHangInput();
            return;
        }

        if (motor.IsGrounded) return;
        if (regrabTimer > 0f) return;
        if (motor.Rigidbody.GetVelocity().y > minDownVelocity) return; // 충분히 내려가는 중?

        if (TryFindLedge(out wallNormal, out ledgeTopPoint))
            BeginHang();
    }

    void BeginHang()
    {
        isHanging = true;

        float radius = motor.CapsuleCollider.radius;
        hangAnchor = ledgeTopPoint - wallNormal * (hangDistance + radius) - Vector3.up * handHeight;

        motor.Rigidbody.SetVelocity(Vector3.zero);
        motor.Rigidbody.MovePosition(hangAnchor);
        motor.LocalVelocityGoal = Vector3.zero;
    }

    void HoldHang()
    {
        motor.Rigidbody.SetVelocity(Vector3.zero);
        motor.LocalVelocityGoal = Vector3.zero;
        motor.Rigidbody.MovePosition(Vector3.Lerp(motor.Rigidbody.position, hangAnchor, 0.5f));
    }

    void HandleHangInput()
    {
        if (ClimbPressed())
        {
            float radius = motor.CapsuleCollider.radius;
            Vector3 standPos = ledgeTopPoint + wallNormal * (standForwardOffset + radius);
            standPos.y += 0.02f;

            motor.Rigidbody.MovePosition(standPos);
            motor.Rigidbody.SetVelocity(Vector3.zero);
            ExitHang();
            return;
        }
        if (DropPressed())
        {
            ExitHang();
        }
    }

    void ExitHang()
    {
        isHanging = false;
        regrabTimer = regrabBlockTime;
    }

    bool TryFindLedge(out Vector3 hitNormal, out Vector3 topPoint)
    {
        hitNormal = default; topPoint = default;

        var mask = climbMask.value == 0 ? motor.layerMask : climbMask;

        float radius = motor.CapsuleCollider.radius;
        float height = motor.CapsuleCollider.height;

        // 가슴 높이에서 전방 벽 탐지
        Vector3 chest = motor.Center + Vector3.up * (height * 0.15f);
        Vector3 dir = GetWorldMoveDir();
        if (dir.sqrMagnitude < 1e-6f) dir = transform.forward;

        if (!Physics.SphereCast(chest, radius * 0.9f, dir, out RaycastHit wallHit, forwardCheck, mask, QueryTriggerInteraction.Ignore))
            return false;

        // 벽 위쪽에서 아래로 쏴서 꼭대기(위 면) 찾기
        Vector3 over = wallHit.point + wallHit.normal * 0.05f + Vector3.up * upCheck;
        if (!Physics.Raycast(over, Vector3.down, out RaycastHit topHit, upCheck + 0.5f, mask, QueryTriggerInteraction.Ignore))
            return false;

        // 서 있을 공간 검사(캡슐 겹침 X)
        Vector3 stand = topHit.point + wallHit.normal * (standForwardOffset + radius);
        Vector3 p1 = stand + Vector3.up * (radius);
        Vector3 p2 = stand + Vector3.up * (height - radius);
        bool blocked = Physics.CheckCapsule(p1, p2, radius * 0.95f, mask, QueryTriggerInteraction.Ignore);
        if (blocked) return false;

        hitNormal = wallHit.normal;
        topPoint = topHit.point;

        if (debugDraw)
        {
            UnityEngine.Debug.DrawLine(chest, wallHit.point, Color.cyan, 0.2f);
            UnityEngine.Debug.DrawLine(over, topHit.point, Color.green, 0.2f);
        }
        return true;
    }

    Vector3 GetWorldMoveDir()
    {
        Vector3 input = motor.MoveInput;
        var run = motor.GetComponent<CharacterRun>();
        Transform view = run ? run.view : motor.transform;
        Quaternion yaw = Quaternion.Euler(0f, view.eulerAngles.y, 0f);
        Vector3 world = (yaw * new Vector3(input.x, 0f, input.z));
        return world.sqrMagnitude > 1e-6f ? world.normalized : transform.forward;
    }

    bool ClimbPressed()
    {
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        return k != null && (k.spaceKey.wasPressedThisFrame || k.wKey.wasPressedThisFrame || k.upArrowKey.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
#endif
    }
    bool DropPressed()
    {
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        return k != null && (k.sKey.wasPressedThisFrame || k.downArrowKey.wasPressedThisFrame || k.leftCtrlKey.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftControl);
#endif
    }
}
