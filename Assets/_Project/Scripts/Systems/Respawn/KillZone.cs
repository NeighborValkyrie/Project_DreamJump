using UnityEngine;

namespace PlatformerGame.Systems.Respawn
{
    /// <summary>
    /// 죽음 지역 (낙사 등)
    /// </summary>
    public class KillZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                HandlePlayerDeath(other.gameObject);
            }
        }

        private void HandlePlayerDeath(GameObject player)
        {
            // 사망 이벤트 발생
            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.TriggerPlayerDied(player.transform.position);
            }

            // 리스폰
            if (RespawnManager.Instance != null)
            {
                RespawnManager.Instance.RespawnPlayer(player);
            }

            Debug.Log("[KillZone] 플레이어 사망 및 리스폰");
        }
    }
}