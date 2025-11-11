// CharacterWallSlideJump_TP.cs
using UnityEngine;
using TraversalPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[DefaultExecutionOrder(-1200)]
public class CharacterWallSlideJump_TP : MonoBehaviour
{
    [Header("Refs")]
    public CharacterMotor motor;

    [Header("Wall Check")]
    [Min(0)] public float checkDistance = 0.6f;
    [Min(0)] public float checkRadius = 0.25f;
    public LayerMask wallMask;                 // 비워두면 motor.layerMask 사용
    [Range(0f, 1f)] public float requiredIntoWallDot = 0.3f; // 벽을 '누르는' 정도

    [Header("Slide")]
    [Min(0)] public float slideMaxDownSpeed = 4f;  // 최대 하강 속도
    [Min(0)] public float slideFriction = 8f;  // 하강 감속 세기

    [Header("Wall Jump")]
    [Min(0)] public float jumpUpStrength = 8f;
    [Min(0)] public float jumpAwayStrength = 5f;
    [Min(0)] public float regrabBlockTime = 0.15f; // 점프 직후 재접착 방지

    bool isSliding;
    Vector3 slideNormal;
    float regrabTimer;

    void Reset() { motor = GetComponent<CharacterMotor>(); }
    void OnEnable() { if (!motor) motor = GetComponent<CharacterMotor>(); if (motor) motor.Moving += OnMotorMoving; }
    void OnDisable() { if (motor) motor.Moving -= OnMotorMoving; }
    void Update() { if (regrabTimer > 0) regrabTimer -= Time.deltaTime; }

    void OnMotorMoving(ICharacterMotor _)
    {
        if (motor.IsGrounded) { isSliding = false; return; }
        if (regrabTimer > 0f) { isSliding = false; return; }

        var mask = wallMask.value == 0 ? motor.layerMask : wallMask;
        Vector3 origin = motor.Center;
        Vector3 moveDirWorld = GetWorldMoveDir();

        if (SphereHit(origin, moveDirWorld, checkRadius, checkDistance, mask, out RaycastHit hit))
        {
            Vector3 wallN = hit.normal;
            float into = Vector3.Dot(-moveDirWorld, wallN); // 벽 쪽으로 미는 정도
            if (into >= requiredIntoWallDot)
            {
                isSliding = true;
                slideNormal = wallN;

                // 하강 속도 캡
                Vector3 v = motor.Rigidbody.GetVelocity();
                if (v.y < -slideMaxDownSpeed)
                {
                    v.y = Mathf.MoveTowards(v.y, -slideMaxDownSpeed, slideFriction * Time.deltaTime);
                    motor.Rigidbody.SetVelocity(v);
                }

                // 벽 점프
                if (JumpPressedThisFrame())
                {
                    Vector3 jump = slideNormal * jumpAwayStrength + Vector3.up * jumpUpStrength;
                    motor.LocalVelocityGoal += jump;                                     // ★ 월드 가산
                    motor.MaxLocalSpeed = Mathf.Max(motor.MaxLocalSpeed, motor.LocalVelocityGoal.magnitude + 2f);
                    isSliding = false;
                    regrabTimer = regrabBlockTime;
                }
                return;
            }
        }
        isSliding = false;
    }

    bool SphereHit(Vector3 origin, Vector3 dir, float radius, float dist, LayerMask mask, out RaycastHit hit)
    {
        if (Physics.SphereCast(origin, Mathf.Max(0.01f, radius), dir, out hit, Mathf.Max(0.01f, dist), mask, QueryTriggerInteraction.Ignore))
            return true;
        // 보조 방향(좌/우/정/역)
        Vector3[] dirs = { transform.right, -transform.right, transform.forward, -transform.forward };
        foreach (var d in dirs)
            if (Physics.SphereCast(origin, radius, d, out hit, dist, mask, QueryTriggerInteraction.Ignore)) return true;
        return false;
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

    bool JumpPressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Space);
#endif
    }
}
