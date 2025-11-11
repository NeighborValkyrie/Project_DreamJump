using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRespawnInput : MonoBehaviour
{
    [Tooltip("플레이어 오브젝트(비워두면 이 컴포넌트가 달린 오브젝트)")]
    public GameObject player;

    void Awake()
    {
        if (!player) player = gameObject;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (RespawnManager.Instance)
                RespawnManager.Instance.Respawn(player);
        }
    }
}
