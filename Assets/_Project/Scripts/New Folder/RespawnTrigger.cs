using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RespawnTrigger : MonoBehaviour
{
    [SerializeField] bool useOnStayBackup = true; // 시작부터 겹침/일시정지 대비

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void Awake()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;

        // 트리거 신뢰성 ↑ : Kinematic Rigidbody 보장
        var rb = GetComponent<Rigidbody>();
        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void OnTriggerEnter(Collider other) => TryRespawn(other);

    void OnTriggerStay(Collider other)
    {
        if (useOnStayBackup) TryRespawn(other);
    }

    void TryRespawn(Collider other)
    {
        var player = GetPlayerRoot(other);
        if (!player) return;

        if (RespawnManager.Instance)
        {
            RespawnManager.Instance.Respawn(player);
        }
        else
        {
            // Fallback: 'Respawn' 태그 위치로 이동
            var rp = GameObject.FindGameObjectWithTag("Respawn");
            if (!rp) return;

            var prb = player.GetComponent<Rigidbody>();
            if (prb)
            {
                prb.velocity = Vector3.zero;
                prb.angularVelocity = Vector3.zero;
                prb.position = rp.transform.position;
                prb.rotation = rp.transform.rotation;
            }
            else
            {
                player.transform.SetPositionAndRotation(rp.transform.position, rp.transform.rotation);
            }
        }
    }

    static GameObject GetPlayerRoot(Collider other)
    {
        // attachedRigidbody 기준으로 루트 잡기(자식 콜라이더 대응)
        var rb = other.attachedRigidbody;
        var go = rb ? rb.gameObject : other.gameObject;

        // 본인 또는 부모에 Player 태그가 있으면 OK
        if (go.CompareTag("Player")) return go;

        var t = other.transform;
        while (t != null)
        {
            if (t.CompareTag("Player")) return t.gameObject;
            t = t.parent;
        }
        return null;
    }
}

