// Platform_JumpPad_TP.cs
using UnityEngine;
using TraversalPro;

[RequireComponent(typeof(Collider))]
public class Platform_JumpPad_TP : MonoBehaviour
{
    public Vector3 localImpulse = new Vector3(0, 12f, 0); // [변경가능]
    public bool resetDownwardY = true; // 하강 중이면 0으로 올려줌

    void Awake() => GetComponent<Collider>().isTrigger = true;

    void OnTriggerEnter(Collider other)
    {
        if (!TPPlatformUtils.TryGetMotor(other, out var motor)) return;

        Vector3 impulse = transform.TransformDirection(localImpulse);
        var rb = motor.Rigidbody;

        Vector3 v = rb.velocity;
        if (resetDownwardY && v.y < 0f) v.y = 0f;
        v += impulse;

        rb.velocity = v; // 한 프레임에 확 세팅
    }
}
