using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRespawnInput : MonoBehaviour
{
    [Tooltip("플레이어 오브젝트(비워두면 이 컴포넌트가 달린 오브젝트)")]
    public GameObject player;   /*[변경가능_플레이어오브젝트]*/

    void Awake()
    {
        if (!player) player = gameObject;
    }

    private void OnEnable()
    {
        // ✅ UI 쪽 Restart 버튼 이벤트 구독
        GameUIController.OnPlayerRespawnRequested += HandleRespawnRequested;
    }

    private void OnDisable()
    {
        // ✅ 씬 전환/비활성화 시 구독 해제
        GameUIController.OnPlayerRespawnRequested -= HandleRespawnRequested;
    }

    void Update()
    {
        // 키보드 R 입력으로도 리스폰 가능
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            RequestRespawn();
        }
    }

    private void HandleRespawnRequested()
    {
        // UI에서 "Restart" 눌렀을 때 들어오는 콜백
        RequestRespawn();
    }

    private void RequestRespawn()
    {
        if (RespawnManager.Instance)
        {
            RespawnManager.Instance.Respawn(player);
        }
        else
        {
            Debug.LogWarning("[PlayerRespawnInput] RespawnManager.Instance 가 없습니다.");
        }
    }
}
