// CollectibleCounterTMP.cs (bulletproof)
using UnityEngine;
using TMPro;
using System.Collections;

public class CollectibleCounterTMP : MonoBehaviour
{
    [Header("TMP Texts")]
    public TMP_Text coinText, starText, keyText;

    [Header("Format")]
    public string coinFormat = "코인: {0}";
    public string starFormat = "별: {0}";
    public string keyFormat = "키: {0}";

    [Header("Animation")]
    public bool animate = true;
    public float animDuration = 0.2f;

    int dCoin, dStar, dKey;
    Coroutine animCo;

    CollectibleManager bound;   // 현재 구독 중인 매니저

    void OnEnable() { TryBindManager(true); }
    void OnDisable() { Unbind(); }

    void Update()
    {
        // 실행 순서/씬 전환으로 매니저가 나중에 생겨도 자동 구독
        if (!bound && CollectibleManager.Instance) TryBindManager(false);
    }

    void TryBindManager(bool instantRefresh)
    {
        var inst = CollectibleManager.Instance;
        if (!inst) return;
        bound = inst;
        bound.OnChanged += Refresh;
        Refresh(instantRefresh);
    }

    void Unbind()
    {
        if (bound) bound.OnChanged -= Refresh;
        bound = null;
    }

    void Refresh() => Refresh(false);

    void Refresh(bool instant)
    {
        if (!bound) return;

        if (!animate || instant)
        {
            dCoin = bound.GetCoin();
            dStar = bound.GetStar();
            dKey = bound.GetKey();

            if (coinText) coinText.text = string.Format(coinFormat, dCoin);
            if (starText) starText.text = string.Format(starFormat, dStar);
            if (keyText) keyText.text = string.Format(keyFormat, dKey);
            return;
        }

        int tCoin = bound.GetCoin();
        int tStar = bound.GetStar();
        int tKey = bound.GetKey();

        if (animCo != null) StopCoroutine(animCo);
        animCo = StartCoroutine(AnimateTo(tCoin, tStar, tKey));
    }

    IEnumerator AnimateTo(int tCoin, int tStar, int tKey)
    {
        int sCoin = dCoin, sStar = dStar, sKey = dKey;
        float t = 0f, dur = Mathf.Max(0.0001f, animDuration);

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / dur);

            int c = Mathf.RoundToInt(Mathf.Lerp(sCoin, tCoin, k));
            int s = Mathf.RoundToInt(Mathf.Lerp(sStar, tStar, k));
            int ky = Mathf.RoundToInt(Mathf.Lerp(sKey, tKey, k));

            if (coinText) coinText.text = string.Format(coinFormat, c);
            if (starText) starText.text = string.Format(starFormat, s);
            if (keyText) keyText.text = string.Format(keyFormat, ky);

            yield return null;
        }
        dCoin = tCoin; dStar = tStar; dKey = tKey;
        animCo = null;
    }
}
