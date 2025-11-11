using UnityEngine;
using System.Collections;

public class BuffItem_Invincible : MonoBehaviour
{
    [Header("Buff Settings")]
    public float duration = 5f; // 무적 지속 시간 (초)
    public GameObject pickupVFX;
    public AudioClip pickupSFX;
    public float sfxVolume = 1f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var inv = other.GetComponent<PlayerInvincibility>();
        if (!inv)
        {
            Debug.LogWarning("[BuffItem_Invincible] PlayerInvincibility 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        // 이펙트/사운드
        if (pickupVFX) Instantiate(pickupVFX, transform.position, Quaternion.identity);
        if (pickupSFX) AudioSource.PlayClipAtPoint(pickupSFX, transform.position, sfxVolume);

        // 버프 적용
        inv.StartCoroutine(ApplyInvincibleBuff(inv));

        gameObject.SetActive(false); // 아이템 비활성화
    }

    IEnumerator ApplyInvincibleBuff(PlayerInvincibility inv)
    {
        inv.StartInvincibility(duration); // 기존 무적 함수에 시간 전달
        Debug.Log($"[BuffItem_Invincible] 무적 버프 시작! 지속시간: {duration:F1}초");

        yield return new WaitForSeconds(duration);

        // PlayerInvincibility 스크립트 내부에서 자동으로 종료되더라도
        // 디버그로 명시적으로 표시해 줌
        Debug.Log("[BuffItem_Invincible] 무적 버프 종료 — 피해 판정 다시 활성화됨");
    }
}
