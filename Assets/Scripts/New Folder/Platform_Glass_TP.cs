// Platform_Glass_TP_Fix.cs
using System.Collections;
using TraversalPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Platform_Glass_TP_Fix : MonoBehaviour
{
    public enum BreakTrigger { WhileStanding, AfterFirstStep, AfterStepOff } // [변경가능]

    [Header("Config")]
    public BreakTrigger breakTrigger = BreakTrigger.AfterFirstStep; // [변경가능] 기본: 밟은 직후 타이머
    public float breakAfter = 1.2f;                                  // [변경가능]
    public GameObject shatterVFX;                                    // [선택]
    public AudioClip breakSFX;                                       // [선택]
    public bool destroyObject = false;                               // 자동 재생성 쓰려면 false 권장
    public float disableDelay = 0.0f;                                // VFX/SFX 재생 여유

    [Header("References")]
    public Collider solidCollider;           // 부모의 '비-트리거' 콜라이더
    public Collider[] extraSolidColliders;   // 추가 콜라이더들
    public Renderer[] renderersToHide;       // 숨길 렌더러

    [Header("Auto Respawn")]
    public bool autoRespawn = true;          // 깨진 뒤 자동 재생성
    public float autoRespawnDelay = 3.0f;    // 재생성 지연
    public bool resetOnPlayerRespawn = true; // 플레이어 리스폰 시 즉시 복구

    float stayTimer;
    bool broken;
    bool armed;                               // 타이머 무장 여부(중복 방지)
    Collider sensor;
    Coroutine respawnRoutine;
    Coroutine breakSchedule;                  // 예약 코루틴 핸들

    void Awake()
    {
        sensor = GetComponent<Collider>();
        sensor.isTrigger = true;
        if (!solidCollider)
            Debug.LogWarning("[Glass] solidCollider를 할당하세요 (부모의 비-트리거 콜라이더).");
    }

    void OnEnable()
    {
        if (resetOnPlayerRespawn) RespawnManager.OnRespawn += HandleRespawn;
    }

    void OnDisable()
    {
        if (resetOnPlayerRespawn) RespawnManager.OnRespawn -= HandleRespawn;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponentInParent<CharacterMotor>()) return;
        if (broken) return;

        // ✅ 밟은 '즉시' 타이머 시작
        if (breakTrigger == BreakTrigger.AfterFirstStep && !armed)
        {
            ArmAndSchedule();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.GetComponentInParent<CharacterMotor>()) return;
        if (broken) return;

        // (구버전 동작) 밟고 '올라서 있는 동안' 누적
        if (breakTrigger == BreakTrigger.WhileStanding)
        {
            stayTimer += Time.deltaTime;
            if (stayTimer >= breakAfter) BreakNow();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.GetComponentInParent<CharacterMotor>()) return;

        // WhileStanding 모드일 때만 누적 초기화
        if (breakTrigger == BreakTrigger.WhileStanding)
            stayTimer = 0f;

        // ✅ 발을 뗀 '이후' 타이머 시작
        if (breakTrigger == BreakTrigger.AfterStepOff && !broken && !armed)
        {
            ArmAndSchedule();
        }
    }

    void ArmAndSchedule()
    {
        armed = true;
        if (breakSchedule != null) StopCoroutine(breakSchedule);
        breakSchedule = StartCoroutine(ScheduleBreak(breakAfter));
    }

    IEnumerator ScheduleBreak(float delay)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        BreakNow();
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
        if (breakSchedule != null) { StopCoroutine(breakSchedule); breakSchedule = null; }
        if (!destroyObject) ResetGlass();
    }

    public void ResetGlass()
    {
        stayTimer = 0f;
        broken = false;
        armed = false;
        breakSchedule = null;

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
