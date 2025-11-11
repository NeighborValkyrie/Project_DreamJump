// Platform_Glass_TP_Fix.cs
using System.Collections;
using System.Diagnostics;
using TraversalPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Platform_Glass_TP_Fix : MonoBehaviour
{
    [Header("Config")]
    public float breakAfter = 1.2f;
    public GameObject shatterVFX;          // [선택]
    public AudioClip breakSFX;              // [선택]
    public bool destroyObject = false;      // ★ 자동 재생성 사용 시 false 권장
    public float disableDelay = 0.0f;       // VFX/SFX 재생 시간 확보용

    [Header("References")]
    public Collider solidCollider;          // 부모의 '비-트리거' 콜라이더
    public Collider[] extraSolidColliders;  // 추가 콜라이더들(있으면)
    public Renderer[] renderersToHide;      // 숨길 렌더러(있으면)

    [Header("Auto Respawn")]
    public bool autoRespawn = true;         // 깨진 뒤 자동 재생성
    public float autoRespawnDelay = 3.0f;   // 재생성 지연(초)
    public bool resetOnPlayerRespawn = true;// 플레이어 리스폰 시 즉시 복구

    float stayTimer;
    bool broken;
    Collider sensor;
    Coroutine respawnRoutine;

    void Awake()
    {
        sensor = GetComponent<Collider>();
        sensor.isTrigger = true;
        if (!solidCollider)
            UnityEngine.Debug.LogWarning("[Glass] solidCollider를 할당하세요 (부모의 비-트리거 콜라이더).");
    }

    void OnEnable()
    {
        if (resetOnPlayerRespawn) RespawnManager.OnRespawn += HandleRespawn;
    }

    void OnDisable()
    {
        if (resetOnPlayerRespawn) RespawnManager.OnRespawn -= HandleRespawn;
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.GetComponentInParent<CharacterMotor>()) return;
        if (broken) return;

        stayTimer += Time.deltaTime;
        if (stayTimer >= breakAfter) BreakNow();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.GetComponentInParent<CharacterMotor>()) return;
        stayTimer = 0f;
    }

    void BreakNow()
    {
        if (broken) return;
        broken = true;

        if (shatterVFX) Instantiate(shatterVFX, transform.position, transform.rotation);
        if (breakSFX) AudioSource.PlayClipAtPoint(breakSFX, transform.position, 1f);

        // 충돌/센서/비주얼 비활성화
        if (solidCollider) solidCollider.enabled = false;
        if (extraSolidColliders != null) foreach (var col in extraSolidColliders) if (col) col.enabled = false;
        if (sensor) sensor.enabled = false;
        if (renderersToHide != null) foreach (var r in renderersToHide) if (r) r.enabled = false;

        if (destroyObject)
        {
            // 파괴 모드: 자동 재생성을 이 스크립트에서 못함(오브젝트가 사라지므로)
            // → GlassRespawnSpawner 사용 필요
            StartCoroutine(DestroyAfter(disableDelay));
            return;
        }

        // 자동 재생성
        if (autoRespawn)
        {
            if (respawnRoutine != null) StopCoroutine(respawnRoutine);
            respawnRoutine = StartCoroutine(AutoRespawnAfter(autoRespawnDelay));
        }
    }

    IEnumerator AutoRespawnAfter(float delay)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        ResetGlass();
    }

    void HandleRespawn()
    {
        // 플레이어 리스폰 시 즉시 복구
        if (respawnRoutine != null) { StopCoroutine(respawnRoutine); respawnRoutine = null; }
        if (!destroyObject) ResetGlass();
        // destroyObject==true라면 Spawner가 새로 생성해야 함
    }

    public void ResetGlass()
    {
        stayTimer = 0f;
        broken = false;

        if (solidCollider) solidCollider.enabled = true;
        if (extraSolidColliders != null) foreach (var col in extraSolidColliders) if (col) col.enabled = true;

        if (sensor)
        {
            sensor.enabled = true;
            sensor.isTrigger = true;
        }

        if (renderersToHide != null) foreach (var r in renderersToHide) if (r) r.enabled = true;

        respawnRoutine = null;
    }

    IEnumerator DestroyAfter(float t)
    {
        if (t > 0) yield return new WaitForSeconds(t);
        Destroy(transform.root.gameObject);
    }
}
