using UnityEngine;
using TraversalPro;
using ithappy;

[RequireComponent(typeof(Collider))] // 콜라이더는 여전히 필요합니다.
[RequireComponent(typeof(BlendShapeAnimator))]
public class Platform_JumpPad_TP : MonoBehaviour
{
    public Vector3 localImpulse = new Vector3(0, 12f, 0);
    public bool resetDownwardY = true;

    private BlendShapeAnimator blendShapeAnimator;

    void Awake()
    {
        // 1. [제거] GetComponent<Collider>().isTrigger = true;
        //    -> 이 줄을 삭제하거나 false로 설정해야 물리적 충돌이 일어납니다.

        blendShapeAnimator = GetComponent<BlendShapeAnimator>();
    }

    // 2. [변경] OnTriggerEnter -> OnCollisionEnter
    //    파라미터도 Collider other -> Collision collision 으로 변경됩니다.
    void OnCollisionEnter(Collision collision)
    {
        // 3. [변경] other -> collision.collider
        //    충돌 정보(collision)에서 실제 충돌한 상대방(collider)을 가져옵니다.
        if (!TPPlatformUtils.TryGetMotor(collision.collider, out var motor)) return;

        // --- 이하는 동일합니다 ---

        // 애니메이션 재생 명령
        if (blendShapeAnimator != null)
        {
            blendShapeAnimator.PlayBounceAnimation();
        }

        // (기존 점프 물리 코드)
        Vector3 impulse = transform.TransformDirection(localImpulse);
        var rb = motor.Rigidbody;

        Vector3 v = rb.velocity;
        if (resetDownwardY && v.y < 0f) v.y = 0f;
        v += impulse;

        rb.velocity = v;
    }
}