using UnityEngine;

public class EnemyConeSight : MonoBehaviour
{
    [Header("References")]
    public Transform eyes;                  // [변경가능] 눈 기준 Transform (비면 자기 자신)
    public Transform player;                // [변경가능] 플레이어 Transform

    [Header("Sight")]
    [Range(0.5f, 100f)] public float viewRange = 12f;   // [변경가능]
    [Range(1f, 180f)] public float viewAngle = 70f;    // [변경가능]
    public LayerMask playerMask;                         // [변경가능] Player 레이어
    public LayerMask obstacleMask;                       // [변경가능] 가림막 레이어

    public bool CanSeePlayer { get; private set; }

    void Reset()
    {
        eyes = transform;
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    void Update()
    {
        CanSeePlayer = false;
        if (!player || !eyes) return;

        Vector3 origin = eyes.position;
        Vector3 to = player.position - origin;
        float dist = to.magnitude;
        if (dist > viewRange) return;

        to.y = 0f; // 수평면 기준
        Vector3 fwd = eyes.forward; fwd.y = 0f;

        float half = viewAngle * 0.5f;
        if (Vector3.Angle(fwd, to) > half) return;

        // 가림막 체크
        if (Physics.Raycast(origin, (player.position - origin).normalized, out RaycastHit hit, viewRange, playerMask | obstacleMask))
        {
            if (((1 << hit.collider.gameObject.layer) & playerMask) != 0)
                CanSeePlayer = true;
        }
    }
}
