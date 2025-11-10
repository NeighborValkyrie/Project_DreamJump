using UnityEngine;

namespace PlatformerGame.Interactions.Platforms
{
    /// <summary>
    /// 끈적이는 플랫폼 (이동 속도 감소)
    /// </summary>
    public class StickyPlatform : MonoBehaviour, Interfaces.IPlatformerGameEffect
    {
        [Header("Sticky Settings")]
        [SerializeField] private float speedMultiplier = 0.5f;

        public void ApplyEffect(GameObject target)
        {
            var movement = target.GetComponent<Core.Player.PlayerMovement>();
            if (movement != null)
            {
                movement.SetSpeedMultiplier(speedMultiplier);
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