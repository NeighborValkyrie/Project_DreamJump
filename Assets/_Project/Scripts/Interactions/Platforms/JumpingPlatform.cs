using UnityEngine;

namespace PlatformerGame.Interactions.Platforms
{
    /// <summary>
    /// 점프 플랫폼 (점프력 증가)
    /// </summary>
    public class JumpingPlatform : MonoBehaviour, Interfaces.IPlatformerGameEffect
    {
        [Header("Jump Settings")]
        [SerializeField] private float jumpForceMultiplier = 2f;

        [Header("Effects")]
        [SerializeField] private string jumpSFX = "Jump";

        public void ApplyEffect(GameObject target)
        {
            var movement = target.GetComponent<Core.Player.PlayerMovement>();
            if (movement != null)
            {
                movement.SetSpeedMultiplier(jumpForceMultiplier);

                // SFX 재생
                if (Systems.Audio.AudioManager.Instance != null)
                {
                    Systems.Audio.AudioManager.Instance.Play(jumpSFX);
                }
            }
        }

        public void RemoveEffect(GameObject target)
        {
            var movement = target.GetComponent<Core.Player.PlayerMovement>();
            if (movement != null)
            {
                movement.SetSpeedMultiplier(1f);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                ApplyEffect(collision.gameObject);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                RemoveEffect(collision.gameObject);
            }
        }
    }
}