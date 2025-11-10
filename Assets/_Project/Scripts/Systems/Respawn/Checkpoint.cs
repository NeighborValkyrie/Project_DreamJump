using UnityEngine;

namespace PlatformerGame.Systems.Respawn
{
    /// <summary>
    /// 체크포인트
    /// </summary>
    public class Checkpoint : MonoBehaviour
    {
        [Header("Checkpoint Settings")]
        [SerializeField] private string checkpointID = "checkpoint_01";

        [Header("Effects")]
        [SerializeField] private string activateSFX = "Checkpoint";
        [SerializeField] private GameObject activateEffect;

        private bool isActivated = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !isActivated)
            {
                ActivateCheckpoint();
            }
        }

        private void ActivateCheckpoint()
        {
            isActivated = true;

            // 체크포인트 이벤트 발생
            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.TriggerCheckpointReached(transform.position, checkpointID);
            }

            // SFX 재생
            if (Systems.Audio.AudioManager.Instance != null)
            {
                Systems.Audio.AudioManager.Instance.Play(activateSFX);
            }

            // 활성화 이펙트
            if (activateEffect != null)
            {
                if (Systems.Pool.ObjectPoolManager.Instance != null)
                {
                    Systems.Pool.ObjectPoolManager.Instance.SpawnFromPool(
                        "CheckpointEffect",
                        transform.position,
                        Quaternion.identity
                    );
                }
                else
                {
                    Instantiate(activateEffect, transform.position, Quaternion.identity);
                }
            }

            Debug.Log($"[Checkpoint] 체크포인트 활성화: {checkpointID}");
        }

        public string GetCheckpointID() => checkpointID;
    }
}