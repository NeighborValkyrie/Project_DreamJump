using UnityEngine;

namespace PlatformerGame.Systems.Respawn
{
    /// <summary>
    /// 리스폰 시스템 관리
    /// </summary>
    public class RespawnManager : MonoBehaviour
    {
        public static RespawnManager Instance { get; private set; }

        [Header("Respawn Settings")]
        [SerializeField] private Vector3 defaultRespawnPosition = Vector3.zero;

        private Vector3 lastCheckpointPosition;
        private string lastCheckpointID = "";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                lastCheckpointPosition = defaultRespawnPosition;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // 체크포인트 이벤트 구독
            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.OnCheckpointReached += OnCheckpointReached;
            }
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.OnCheckpointReached -= OnCheckpointReached;
            }
        }

        private void OnCheckpointReached(Vector3 position, string checkpointID)
        {
            lastCheckpointPosition = position;
            lastCheckpointID = checkpointID;

            Debug.Log($"[RespawnManager] 체크포인트 업데이트: {checkpointID}");
        }

        public void RespawnPlayer(GameObject player)
        {
            if (player == null) return;

            var movement = player.GetComponent<Core.Player.PlayerMovement>();
            if (movement != null)
            {
                movement.Teleport(lastCheckpointPosition);
            }
            else
            {
                player.transform.position = lastCheckpointPosition;
            }

            // 이벤트 발생
            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.TriggerPlayerRespawned(lastCheckpointPosition, lastCheckpointID);
            }

            Debug.Log($"[RespawnManager] 플레이어 리스폰: {lastCheckpointPosition}");
        }

        public void SetLastCheckpoint(string checkpointID)
        {
            lastCheckpointID = checkpointID;
        }

        public string GetLastCheckpointID() => lastCheckpointID;

        public void ResetCheckpoints()
        {
            lastCheckpointPosition = defaultRespawnPosition;
            lastCheckpointID = "";
        }
    }
}