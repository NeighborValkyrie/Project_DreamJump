using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class PlayerInvincibility : MonoBehaviour
{
    [Header("Settings")]
    public float defaultDuration = 1f;     // 리스폰 기본 무적 시간
    public float blinkInterval = 0.15f;    // 깜빡임 속도 (초 단위)

    [Header("References")]
    public Renderer[] renderers;           // 비워두면 자동 수집됨

    public bool IsInvincible { get; private set; }

    Coroutine invRoutine;

    void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(true);
    }

    public void StartInvincibility() => StartInvincibility(defaultDuration);

    public void StartInvincibility(float duration)
    {
        if (invRoutine != null)
        {
            StopCoroutine(invRoutine);
            SetVisible(true); // 이전 상태 복원
        }
        invRoutine = StartCoroutine(CoInvincible(duration));
    }

    IEnumerator CoInvincible(float duration)
    {
        IsInvincible = true;
        Debug.Log($"[PlayerInvincibility] 무적 시작 ({duration:F1}초)");

        float timer = 0f;
        bool visible = true;

        while (timer < duration)
        {
            visible = !visible;
            SetVisible(visible);
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        SetVisible(true);
        IsInvincible = false;
        invRoutine = null;
        Debug.Log("[PlayerInvincibility] 무적 종료");
    }

    void SetVisible(bool on)
    {
        foreach (var r in renderers)
            if (r) r.enabled = on;
    }

    void OnDisable()
    {
        // 비활성화 시 깜빡임 중단 및 복구
        SetVisible(true);
    }
}
