// Platform_Rotator.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Platform_Rotator : MonoBehaviour
{
    public enum RotMode { Continuous, Oscillate }

    [Header("Axis")]
    public Vector3 axis = Vector3.up;       // 회전축
    public bool worldSpaceAxis = true;

    [Header("Rotation")]
    public RotMode mode = RotMode.Continuous;
    public float degreesPerSecond = 90f;

    [Header("Oscillate")]
    public float minAngle = -45f;           // 기준 회전으로부터 상대각
    public float maxAngle = 45f;
    public float startAngle = 0f;

    Rigidbody rb;
    Quaternion baseRot;
    float angle;    // 현재 상대각
    int dir = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        baseRot = transform.rotation;
        angle = Mathf.Clamp(startAngle, minAngle, maxAngle);
    }

    void FixedUpdate()
    {
        Vector3 ax = axis.sqrMagnitude < 1e-6f ? Vector3.up : axis.normalized;

        if (mode == RotMode.Continuous)
        {
            float step = degreesPerSecond * Time.fixedDeltaTime;
            Quaternion dq = Quaternion.AngleAxis(step, worldSpaceAxis ? ax : transform.TransformDirection(ax));
            rb.MoveRotation(dq * rb.rotation);
        }
        else // Oscillate
        {
            angle += dir * degreesPerSecond * Time.fixedDeltaTime;
            if (angle >= maxAngle) { angle = maxAngle; dir = -1; }
            else if (angle <= minAngle) { angle = minAngle; dir = 1; }

            Quaternion q = worldSpaceAxis
                ? Quaternion.AngleAxis(angle, ax) * baseRot
                : baseRot * Quaternion.AngleAxis(angle, ax);

            rb.MoveRotation(q);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Vector3 p = Application.isPlaying ? rb.position : transform.position;
        Vector3 ax = (axis.sqrMagnitude < 1e-6f ? Vector3.up : axis.normalized) * 1.0f;
        Gizmos.DrawLine(p - ax, p + ax);
        Gizmos.DrawSphere(p + ax, 0.06f);
        Gizmos.DrawSphere(p - ax, 0.06f);
    }
#endif
}
