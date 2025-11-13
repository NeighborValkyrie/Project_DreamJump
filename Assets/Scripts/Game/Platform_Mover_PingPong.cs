// Platform_Mover_PingPong.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Platform_Mover_PingPong : MonoBehaviour
{
    [Header("Path")]
    public Transform endPoint;              // 선택: 지정 없으면 localOffset 사용
    public Vector3 localOffset = new Vector3(0, 0, 5);

    [Header("Motion")]
    [Min(0)] public float speed = 2f;       // m/s
    [Min(0)] public float waitAtEnds = 0.5f;
    public bool startAtEnd = false;

    Rigidbody rb;
    Vector3 A, B;
    float t;          // 0..1
    int dir = 1;      // 1 또는 -1
    float wait;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        A = transform.position;
        B = endPoint ? endPoint.position : transform.TransformPoint(localOffset);

        t = startAtEnd ? 1f : 0f;
        dir = startAtEnd ? -1 : 1;
    }

    void FixedUpdate()
    {
        if (wait > 0f) { wait -= Time.fixedDeltaTime; MoveTo(Lerp()); return; }

        float dist = Vector3.Distance(A, B);
        if (dist < 1e-4f) return;

        float step = speed * Time.fixedDeltaTime / dist;
        t += dir * step;

        if (t >= 1f) { t = 1f; dir = -1; wait = waitAtEnds; }
        else if (t <= 0f) { t = 0f; dir = 1; wait = waitAtEnds; }

        MoveTo(Lerp());
    }

    Vector3 Lerp() => Vector3.Lerp(A, B, t);
    void MoveTo(Vector3 p) => rb.MovePosition(p);

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Vector3 a = Application.isPlaying ? A : transform.position;
        Vector3 b = endPoint ? endPoint.position : transform.TransformPoint(localOffset);
        Gizmos.color = Color.cyan; Gizmos.DrawLine(a, b); Gizmos.DrawSphere(a, 0.08f); Gizmos.DrawSphere(b, 0.08f);
    }
#endif
}
