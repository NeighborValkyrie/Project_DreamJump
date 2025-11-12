// SlipperySurfaceHandler_TP.cs
using UnityEngine;
using TraversalPro;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterMotor))]
public class SlipperySurfaceHandler_TP : MonoBehaviour
{
    CharacterMotor motor;

    SlipperySurface currentSurface;   // 지금 밟는 표면
    float cachedAccel;                // 진입 시점 AccelerationGoal 백업
    bool hasCached;

    void Awake()
    {
        motor = GetComponent<CharacterMotor>();
    }

    void OnEnable()
    {
        motor.Moving += OnMotorMoving;
    }

    void OnDisable()
    {
        motor.Moving -= OnMotorMoving;
        // 떠날 때 복구 못하고 꺼질 수 있으니, 가능하면 복구
        if (hasCached) motor.AccelerationGoal = cachedAccel;
        hasCached = false;
        currentSurface = null;
    }

    void OnMotorMoving(ICharacterMotor _)
    {
        // 공중에선 적용 X
        if (!motor.IsGrounded)
        {
            LeaveSurfaceIfNeeded();
            return;
        }

        // 현재 밟는 지면의 콜라이더에서 SlipperySurface 탐색
        var gcol = motor.Ground.Collider;
        SlipperySurface surface = null;
        if (gcol)
        {
            // 콜라이더에 직접 붙었거나, 부모에 붙었을 수도 있으니 GetComponentInParent 사용
            surface = gcol.GetComponentInParent<SlipperySurface>();
        }

        // 표면 변경 감지 (진입/이탈)
        if (surface != currentSurface)
        {
            if (currentSurface != null) // 표면 떠남 → 가속 복구
            {
                if (hasCached) motor.AccelerationGoal = cachedAccel;
                hasCached = false;
            }
            currentSurface = surface;
            if (currentSurface != null) // 표면 진입 → 가속 백업
            {
                cachedAccel = motor.AccelerationGoal;
                hasCached = true;
            }
        }

        // 미끄럼 표면이 아니라면 아무것도 안 함(원래 시스템/버프가 컨트롤)
        if (currentSurface == null) return;

        ApplySlip(currentSurface);
    }

    void LeaveSurfaceIfNeeded()
    {
        if (currentSurface != null)
        {
            if (hasCached) motor.AccelerationGoal = cachedAccel;
            hasCached = false;
            currentSurface = null;
        }
    }

    void ApplySlip(SlipperySurface s)
    {
        // 1) 가속(조향력) 축소—완전 0은 방지
        float targetAccel = Mathf.Max(cachedAccel * s.accelScale, s.minAccel);
        motor.AccelerationGoal = targetAccel;

        // 2) 지면 상대 수평 속도 벡터
        Vector3 tangent = motor.LocalVelocity;
        if (s.projectOnGround && motor.Ground.Collider != null)
            tangent = Vector3.ProjectOnPlane(tangent, motor.Ground.Normal);
        tangent.y = 0f;

        // 3) 입력 방향(수평) 계산
        Vector3 wishDir = motor.MoveInput;
        if (s.projectOnGround && motor.Ground.Collider != null)
            wishDir = Vector3.ProjectOnPlane(wishDir, motor.Ground.Normal);
        wishDir.y = 0f;

        if (wishDir.sqrMagnitude < 1e-4f)
            wishDir = (tangent.sqrMagnitude > 1e-4f) ? tangent.normalized : transform.forward;

        // 4) 속도 방향을 입력 방향으로 천천히 회전
        float maxRad = Mathf.Deg2Rad * s.turnRateDegPerSec * Time.deltaTime * s.inputInfluence;
        Vector3 currDir = (tangent.sqrMagnitude > 1e-4f) ? tangent.normalized : wishDir;
        Vector3 newDir = Vector3.RotateTowards(currDir, wishDir.normalized, maxRad, 0f);

        // 5) 너무 느려지지 않도록 바닥 속도 유지
        float targetSpeed = Mathf.Max(tangent.magnitude, motor.MaxLocalSpeed * s.speedFloor);

        // 6) 목표 속도 갱신
        motor.LocalVelocityGoal = newDir * targetSpeed;
    }
}
