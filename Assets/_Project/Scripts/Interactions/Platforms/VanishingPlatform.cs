using UnityEngine;
using System.Collections;

namespace PlatformerGame.Interactions.Platforms
{
    /// <summary>
    /// 사라지는 플랫폼
    /// </summary>
    public class VanishingPlatform : MonoBehaviour
    {
        [Header("Vanish Settings")]
        [SerializeField] private float vanishDelay = 1f;
        [SerializeField] private float respawnDelay = 3f;

        private bool isVanishing = false;
        private Renderer platformRenderer;
        private Collider platformCollider;

        private void Awake()
        {
            platformRenderer = GetComponent<Renderer>();
            platformCollider = GetComponent<Collider>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player") && !isVanishing)
            {
                StartCoroutine(VanishRoutine());
            }
        }

        private IEnumerator VanishRoutine()
        {
            isVanishing = true;

            yield return new WaitForSeconds(vanishDelay);

            // 플랫폼 비활성화
            if (platformRenderer != null)
                platformRenderer.enabled = false;

            if (platformCollider != null)
                platformCollider.enabled = false;

            yield return new WaitForSeconds(respawnDelay);

            // 플랫폼 재활성화
            if (platformRenderer != null)
                platformRenderer.enabled = true;

            if (platformCollider != null)
                platformCollider.enabled = true;

            isVanishing = false;
        }
    }
}