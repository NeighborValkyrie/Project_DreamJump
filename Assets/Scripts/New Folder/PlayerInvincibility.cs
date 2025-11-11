using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInvincibility : MonoBehaviour
{
    [Header("=== [TUNE] 무적 ===")]
    public float invincibleTime = 1f;                         // [변경가능] 무적 시간(초)
    public string[] ignoreLayerNames = { "Enemy", "Hazard" }; // [변경가능] 무시할 상대 레이어들

    [Header("=== [옵션] 시각효과 ===")]
    public Renderer[] blinkRenderers;                         // [변경가능] 깜빡일 렌더러(비우면 자동)
    public float blinkInterval = 0.15f;                       // [변경가능]
    public bool useBlink = true;                              // [변경가능]

    int playerLayer;
    int[] ignoreLayers;                   // 캐시
    bool isInvincible;
    readonly List<(int, int)> activeIgnores = new(); // (playerLayer, otherLayer) 기록

    void Awake()
    {
        playerLayer = gameObject.layer;

        // 문자열 → 레이어 인덱스로 변환
        var layers = new List<int>();
        foreach (var name in ignoreLayerNames)
        {
            int id = LayerMask.NameToLayer(name);
            if (id >= 0) layers.Add(id);
            else Debug.LogWarning($"[Invincibility] 레이어 '{name}'를 찾을 수 없습니다. Project Settings > Tags and Layers 확인.");
        }
        ignoreLayers = layers.ToArray();

        if ((blinkRenderers == null || blinkRenderers.Length == 0))
            blinkRenderers = GetComponentsInChildren<Renderer>();
    }

    public bool IsInvincible => isInvincible;

    public void StartInvincibility()
    {
        if (!isInvincible) StartCoroutine(InvincibleRoutine());
    }

    IEnumerator InvincibleRoutine()
    {
        isInvincible = true;

        // 1) 플레이어 레이어 vs 적/함정 레이어 충돌 무시
        activeIgnores.Clear();
        foreach (var other in ignoreLayers)
        {
            if (!Physics.GetIgnoreLayerCollision(playerLayer, other))
            {
                Physics.IgnoreLayerCollision(playerLayer, other, true);
                activeIgnores.Add((playerLayer, other));
            }
        }

        // 2) 깜빡이기(콜라이더는 그대로라 떨어지지 않음)
        float t = 0f;
        bool visible = true;
        while (t < invincibleTime)
        {
            if (useBlink && blinkRenderers != null && blinkRenderers.Length > 0)
            {
                visible = !visible;
                foreach (var r in blinkRenderers) if (r) r.enabled = visible;
            }
            yield return new WaitForSeconds(blinkInterval);
            t += blinkInterval;
        }

        // 3) 복구
        foreach (var (pl, other) in activeIgnores)
            Physics.IgnoreLayerCollision(pl, other, false);
        activeIgnores.Clear();

        if (blinkRenderers != null)
            foreach (var r in blinkRenderers) if (r) r.enabled = true;

        isInvincible = false;
    }
}
