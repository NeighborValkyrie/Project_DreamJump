using UnityEngine;
using TraversalPro;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Platform_SoftJump_Always_NoPatch_TP : MonoBehaviour
{
    [Range(0f, 1f)] public float upwardScaleOnJump = 0.5f; // 점프 직후 즉시 감쇠
    public float followWindow = 0.10f;                     // 이후 짧은 추가 감쇠
    [Range(0f, 1f)] public float followFactor = 0.8f;       // 추가 감쇠 비율

    CharacterMotor motor;
    Jump jump;
    double lastSeenJumpTime = double.MinValue;

    void Awake() => GetComponent<Collider>().isTrigger = true;

    void OnTriggerEnter(Collider other)
    {
        motor = other.GetComponentInParent<CharacterMotor>();
        jump = other.GetComponentInParent<Jump>();
    }

    void OnTriggerStay(Collider other)
    {
        if (!motor || !jump) return;

        // 트리거 안에서 "방금 점프" 했으면 즉시 감쇠
        if (jump.LastJumpTime > lastSeenJumpTime)
        {
            lastSeenJumpTime = jump.LastJumpTime;
            var rb = motor.Rigidbody;
            var v = rb.velocity;
            if (v.y > 0f) rb.velocity = new Vector3(v.x, v.y * upwardScaleOnJump, v.z);

            StopAllCoroutines();
            StartCoroutine(FollowClamp(rb));
        }
    }

    void OnTriggerExit(Collider _)
    {
        motor = null; jump = null; // 즉시 해제
    }

    IEnumerator FollowClamp(Rigidbody rb)
    {
        float t = 0f;
        while (t < followWindow && rb != null)
        {
            var v = rb.velocity;
            if (v.y > 0f) rb.velocity = new Vector3(v.x, v.y * followFactor, v.z);
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
