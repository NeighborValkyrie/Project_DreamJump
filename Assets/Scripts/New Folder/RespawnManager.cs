using System.Diagnostics;
using UnityEngine;
using TraversalPro;
// using System;  // 쓰고 싶으면 추가, 아니면 System.Action 전체 수식 사용

public class RespawnManager : MonoBehaviour
{
    // ★ 클래스 맨 위(필드 영역)에 "정적 이벤트"로 선언하세요. (메서드 안 X)
    public static event System.Action OnRespawn;

    public static RespawnManager Instance { get; private set; }

    [SerializeField] private Transform defaultCheckpoint;
    public Transform CurrentCheckpoint { get; private set; }

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (!CurrentCheckpoint) CurrentCheckpoint = defaultCheckpoint;
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        if (checkpoint) CurrentCheckpoint = checkpoint;
    }

    public void Respawn(GameObject player)
    {
        if (!player) return;

        Transform cp = CurrentCheckpoint ? CurrentCheckpoint : defaultCheckpoint;
        if (!cp) { UnityEngine.Debug.LogWarning("RespawnManager: 체크포인트가 설정되지 않았습니다."); return; }

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

        var motor = player.GetComponent<CharacterMotor>();
        if (motor != null)
        {
            motor.LocalVelocityGoal = Vector3.zero;
            motor.MaxLocalSpeed = Mathf.Max(motor.MaxLocalSpeed, 5f);
        }

        // ★ 여기서 이벤트 발사
        OnRespawn?.Invoke();
    }
}
