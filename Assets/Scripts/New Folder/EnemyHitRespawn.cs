using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class EnemyHitRespawn : MonoBehaviour
{
    [Header("Settings")]
    public string playerTag = "Player";
    public float respawnDelay = 0.2f;
    public GameObject hitEffect;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        HandleHit(other.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(playerTag)) return;
        HandleHit(collision.gameObject);
    }

    void HandleHit(GameObject player)
    {
        if (!RespawnManager.Instance) return;

        // 1️⃣ 무적 상태라면 무시
        var inv = player.GetComponent<PlayerInvincibility>();
        if (inv && inv.IsInvincible) return;

        // 2️⃣ 쉴드 버프 활성화되어 있으면 1회 방어
        if (BuffManager.Instance && BuffManager.Instance.ConsumeShield())
        {
            // (선택) 쉴드 파티클 이펙트 추가 가능
            return;
        }

        // 3️⃣ 피격 이펙트 출력
        if (hitEffect)
            Instantiate(hitEffect, player.transform.position, Quaternion.identity);

        // 4️⃣ 리스폰 코루틴 실행
        StartCoroutine(CoRespawn(player));
    }

    IEnumerator CoRespawn(GameObject player)
    {
        yield return new WaitForSeconds(respawnDelay);
        RespawnManager.Instance.Respawn(player);
        // Respawn 내부에서 무적 1초 적용됨
    }
}
