using UnityEngine;

namespace PlatformerGame.Interactions.Platforms
{
    /// <summary>
    /// 미끄러운 플랫폼 (마찰력 제로)
    /// v7.0: 버그 수정
    /// </summary>
    public class SlipperyPlatform : MonoBehaviour
    {
        [Header("Slippery Settings")]
        [SerializeField] private PhysicMaterial zeroFrictionMaterial;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Collider playerCollider = collision.gameObject.GetComponent<Collider>();
                if (playerCollider != null && zeroFrictionMaterial != null)
                {
                    playerCollider.material = zeroFrictionMaterial;
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Collider playerCollider = collision.gameObject.GetComponent<Collider>();
                if (playerCollider != null)
                {
                    playerCollider.material = null;
                }
            }
        }
    }
}