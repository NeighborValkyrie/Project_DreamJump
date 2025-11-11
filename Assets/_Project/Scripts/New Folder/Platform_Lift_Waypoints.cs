// Platform_Lift_Waypoints.cs
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Platform_Lift_Waypoints : MonoBehaviour
{
    public enum Mode { Loop, PingPong, OneShot }

    [Header("Waypoints (월드 위치 사용)")]
    public Transform[] stops;               // 2개 이상 권장

    [Header("Motion")]
    [Min(0)] public float speed = 2.5f;     // m/s
    [Min(0)] public float dwell = 0.6f;     // 정차 시간
    public Mode mode = Mode.PingPong;
    public bool startAtLast = false;

    Rigidbody rb;
    int i;          // 현재 목표 인덱스
    int step = 1;   // 진행 방향 (1/-1)
    float wait;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (stops == null || stops.Length < 1) { UnityEngine.Debug.LogWarning("Lift: stops 미설정"); return; }
        i = startAtLast ? 0 : 1;            // 첫 이동 목표
        step = startAtLast ? -1 : 1;
    }

    void FixedUpdate()
    {
        if (stops == null || stops.Length == 0) return;

        if (wait > 0f) { wait -= Time.fixedDeltaTime; return; }

        Vector3 target = stops[i].position;
        Vector3 pos = rb.position;
        Vector3 to = target - pos;

        float dist = to.magnitude;
        float move = speed * Time.fixedDeltaTime;

        if (dist <= move + 1e-5f)
        {
            rb.MovePosition(target);
            wait = dwell;

            // 다음 정지점 결정
            if (mode == Mode.Loop)
            {
                i = (i + 1) % stops.Length;
            }
            else if (mode == Mode.PingPong)
            {
                if (i == stops.Length - 1) step = -1;
                else if (i == 0) step = 1;
                i += step;
            }
            else // OneShot
            {
                if (i < stops.Length - 1) i++;
            }
        }
        else
        {
            rb.MovePosition(pos + to.normalized * move);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (stops == null || stops.Length == 0) return;
        Gizmos.color = Color.yellow;
        for (int k=0; k<stops.Length; k++)
        {
            if (!stops[k]) continue;
            Gizmos.DrawSphere(stops[k].position, 0.08f);
            if (k < stops.Length - 1) Gizmos.DrawLine(stops[k].position, stops[k+1].position);
        }
        if (mode == Mode.Loop && stops.Length > 1)
            Gizmos.DrawLine(stops[^1].position, stops[0].position);
    }
#endif
}
