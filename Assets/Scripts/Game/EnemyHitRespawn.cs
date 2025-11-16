using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class EnemyHitRespawn : MonoBehaviour
{
    [Header("Settings")]
    public string playerTag = "Player";              /*[변경가능_플레이어태그]*/
    public float respawnDelay = 0.2f;                /*[변경가능_리스폰딜레이]*/
    public GameObject hitEffect;                     /*[변경가능_피격이펙트프리팹]*/

    [Header("Audio")]
    public AudioClip hitVoiceClip;                   /*[변경가능_피격음성클립]*/
    public float hitVoiceVolume = 1f;                /*[변경가능_피격음성볼륨]*/
    public AudioSource audioSource;                  /*[변경가능_재생에쓸오디오소스]*/

    [Header("Coin Penalty")]
    public bool useCoinPenalty = true;               /*[변경가능_코인패널티사용]*/
    public int coinPenalty = 5;                      /*[변경가능_피격시차감코인수]*/

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
            // (선택) 쉴드 전용 사운드도 나중에 추가 가능
            return;
        }

        // 3️⃣ 피격 음성 재생
        PlayHitVoice(player.transform.position);

        // 4️⃣ 피격 이펙트 출력
        if (hitEffect)
            Instantiate(hitEffect, player.transform.position, Quaternion.identity);

        // 5️⃣ 코인 패널티 적용 (있으면)
        ApplyCoinPenalty();

        // 6️⃣ 리스폰 코루틴 실행
        StartCoroutine(CoRespawn(player));
    }

    void ApplyCoinPenalty()
    {
        if (!useCoinPenalty) return;
        if (!CollectibleManager.Instance) return;
        if (coinPenalty <= 0) return;

        // CollectibleManager는 음수 delta 허용 + 0이하 클램프하도록 수정된 상태여야 함
        // AddCoin(-5) → 현재 코인에서 5만큼 감소 (최소 0)
        CollectibleManager.Instance.AddCoin(-coinPenalty);
    }

    void PlayHitVoice(Vector3 position)
    {
        if (!hitVoiceClip) return;

        // audioSource를 연결해놨으면 그걸 사용
        if (audioSource)
        {
            audioSource.PlayOneShot(hitVoiceClip, hitVoiceVolume);
        }
        else
        {
            // 없으면 해당 위치에서 3D로 한 번 재생
            AudioSource.PlayClipAtPoint(hitVoiceClip, position, hitVoiceVolume);
        }
    }

    IEnumerator CoRespawn(GameObject player)
    {
        yield return new WaitForSeconds(respawnDelay);
        RespawnManager.Instance.Respawn(player);
        // Respawn 내부에서 무적 1초 적용됨
    }
}
