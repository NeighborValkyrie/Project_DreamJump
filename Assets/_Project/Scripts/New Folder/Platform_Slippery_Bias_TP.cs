using UnityEngine;
using TraversalPro;

[RequireComponent(typeof(Collider))]
public class Platform_Slippery_TP_Event : MonoBehaviour
{
    [Header("조향/가속 설정")]
    [Range(0f, 1f)] public float accelScale = 0.4f;     // 가속(조향력) 축소 비율
    public float minAccel = 6f;                         // 너무 미끄러워도 이 값 이하로는 안 내려감
    public float turnRateDegPerSec = 180f;              // 초당 최대 회전각(속도 벡터가 입력 방향으로 도는 속도)
    [Range(0f, 1f)] public float inputInfluence = 0.55f; // 입력 영향(0=안먹음, 1=최대)

    [Header("속도 유지감")]
    [Range(0f, 1f)] public float speedFloor = 0.6f;      // 목표 속도 하한( MaxLocalSpeed * floor )
    public bool projectOnGround = true;                  // 지면 법선에 직교한 수평 투영만 사용

    CharacterMotor motor;
    float originalAccel;
    bool touching;

    void Awake() => GetComponent<Collider>().isTrigger = true;

    void OnDisable()
    {
        if (motor != null) motor.Moving -= OnMotorMoving;
        motor = null; touching = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (touching) return;
        var m = other.GetComponentInParent<CharacterMotor>();
        if (!m) return;

        motor = m;
        originalAccel = motor.AccelerationGoal;
        motor.Moving += OnMotorMoving;
        touching = true;
    }

    void OnTriggerExit(Collider other)
    {
        var m = other.GetComponentInParent<CharacterMotor>();
        if (!m || m != motor) return;

        motor.Moving -= OnMotorMoving;
        motor.AccelerationGoal = originalAccel; // 복구
        motor = null; touching = false;
    }

    void OnMotorMoving(ICharacterMotor _)
    {
        if (motor == null) return;
        if (!motor.IsGrounded) return; // 공중에선 미끄럼 영향 X

        // 1) 가속(조향력) 축소—하지만 완전히 0은 아님
        float targetAccel = Mathf.Max(originalAccel * accelScale, minAccel);
        motor.AccelerationGoal = targetAccel;

        // 2) 현재 지면 상대 수평 속도(tangent) 추출
        Vector3 tangent = motor.LocalVelocity;  // 지면 상대 속도
        if (projectOnGround && motor.Ground.Collider != null)
            tangent = Vector3.ProjectOnPlane(tangent, motor.Ground.Normal);
        tangent.y = 0f;

        // 3) 입력 방향(수평) 계산
        Vector3 wishDir = motor.MoveInput;
        if (projectOnGround && motor.Ground.Collider != null)
            wishDir = Vector3.ProjectOnPlane(wishDir, motor.Ground.Normal);
        wishDir.y = 0f;

        // 입력이 거의 없으면 현재 진행방향 유지
        if (wishDir.sqrMagnitude < 0.0001f)
            wishDir = (tangent.sqrMagnitude > 0.0001f) ? tangent.normalized : transform.forward;

        // 4) 속도 벡터를 입력 방향으로 "천천히 회전"
        float maxRadiansThisFrame = Mathf.Deg2Rad * turnRateDegPerSec * Time.deltaTime * inputInfluence;
        Vector3 currDir = (tangent.sqrMagnitude > 0.0001f) ? tangent.normalized : wishDir;
        Vector3 newDir = Vector3.RotateTowards(currDir, wishDir.normalized, maxRadiansThisFrame, 0f);

        // 5) 목표 속도 크기 유지(너무 느려지면 약간의 바닥 추진감)
        float targetSpeed = Mathf.Max(tangent.magnitude, motor.MaxLocalSpeed * speedFloor);

        // 6) 최종 목표: 방향만 조금 틀어진 상태로 유지
        Vector3 goal = newDir * targetSpeed;
        motor.LocalVelocityGoal = goal;
    }
}
