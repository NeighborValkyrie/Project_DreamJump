using UnityEngine;
using System.Collections;

public class BuffItem_Speed : MonoBehaviour
{
    [Header("Buff Settings")]
    public float duration = 5f;          // 버프 지속시간
    public float speedMultiplier = 1.5f; // 이동속도 배율
    public GameObject pickupVFX;
    public AudioClip pickupSFX;
    public float sfxVolume = 1f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var buffManager = BuffManager.Instance;
        if (!buffManager) return;

        // 버프 적용
        buffManager.StartCoroutine(ApplySpeedBuff(other.gameObject));

        if (pickupVFX) Instantiate(pickupVFX, transform.position, Quaternion.identity);
        if (pickupSFX) AudioSource.PlayClipAtPoint(pickupSFX, transform.position, sfxVolume);

        gameObject.SetActive(false); // 아이템 비활성화
    }

    IEnumerator ApplySpeedBuff(GameObject player)
    {
        var motor = player.GetComponent<TraversalPro.CharacterRun>();
        if (!motor)
        {
            Debug.LogWarning("[BuffItem_Speed] CharacterRun 컴포넌트를 찾을 수 없습니다.");
            yield break;
        }

        float originalSpeed = motor.runSpeed;
        float originalSprint = motor.sprintSpeed;

        // 버프 시작
        
        motor.runSpeed *= speedMultiplier;
        motor.sprintSpeed *= speedMultiplier;
        Debug.Log($"[BuffItem_Speed] 속도 버프 시작! 현재 배율: {speedMultiplier}x (지속 {duration:F1}초)");

        yield return new WaitForSeconds(duration);

        // 버프 종료
        
        motor.runSpeed = originalSpeed;
        motor.sprintSpeed = originalSprint;
        Debug.Log("[BuffItem_Speed] 속도 버프 종료 — 이동속도 원상복귀 완료!");
    }
}
