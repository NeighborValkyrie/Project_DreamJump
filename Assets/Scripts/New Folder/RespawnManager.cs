using UnityEngine;
using TraversalPro;

public class RespawnManager : MonoBehaviour
{
    public static event System.Action OnRespawn;

    public static RespawnManager Instance { get; private set; }

    [SerializeField] private Transform defaultCheckpoint;
    public Transform CurrentCheckpoint { get; private set; }

    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (!CurrentCheckpoint)
            CurrentCheckpoint = defaultCheckpoint;
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        if (checkpoint)
            CurrentCheckpoint = checkpoint;
    }

    public void Respawn(GameObject player)
    {
        if (!player) return;

        Transform cp = CurrentCheckpoint ? CurrentCheckpoint : defaultCheckpoint;
        if (!cp)
        {
            Debug.LogWarning("RespawnManager: 체크포인트가 설정되지 않았습니다.");
            return;
        }

        // 위치 리셋
        var rb = player.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = cp.position;
            rb.rotation = cp.rotation;
        }
        else
        {
            player.transform.SetPositionAndRotation(cp.position, cp.rotation);
        }

        // 캐릭터 모터 리셋 (TraversalPro)
        var motor = player.GetComponent<CharacterMotor>();
        if (motor != null)
        {
            motor.LocalVelocityGoal = Vector3.zero;
            motor.MaxLocalSpeed = Mathf.Max(motor.MaxLocalSpeed, 5f);
        }

        // 💥 무적 시작
        var inv = player.GetComponent<PlayerInvincibility>();
        if (inv)
            inv.StartInvincibility();

        // 📢 리스폰 이벤트 브로드캐스트
        OnRespawn?.Invoke();
    }
}
