// CheckpointPopupUI.cs
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

[DisallowMultipleComponent]
public class CheckpointPopupUI : MonoBehaviour
{
    public static CheckpointPopupUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private CanvasGroup group;     // 알파/클릭 차단 제어
    [SerializeField] private UnityEngine.UI.Text label;            // 메시지 표시(없으면 생략 가능)

    [Header("Animation")]
    [Min(0)] public float fadeIn = 0.12f;
    [Min(0)] public float hold = 0.80f;
    [Min(0)] public float fadeOut = 0.25f;
    public Vector2 moveOffset = new Vector2(0f, 40f); // 살짝 위로 뜨는 효과

    RectTransform rt;
    Vector2 baseAnchoredPos;
    Coroutine playCo;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        rt = GetComponent<RectTransform>();
        if (rt) baseAnchoredPos = rt.anchoredPosition;
        if (group) { group.alpha = 0f; group.interactable = false; group.blocksRaycasts = false; }
    }

    public static void Show(string msg = "체크포인트!")
    {
        if (Instance) Instance.Play(msg);
    }

    public void Play(string msg)
    {
        if (label && !string.IsNullOrEmpty(msg)) label.text = msg;
        if (playCo != null) StopCoroutine(playCo);
        playCo = StartCoroutine(PlayRoutine());
    }

    System.Collections.IEnumerator PlayRoutine()
    {
        float t;

        // 시작 상태
        if (rt) rt.anchoredPosition = baseAnchoredPos;
        if (group) { group.alpha = 0f; group.interactable = false; group.blocksRaycasts = false; }

        // Fade In
        t = 0f;
        while (t < fadeIn)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / Mathf.Max(0.0001f, fadeIn));
            if (group) group.alpha = k;
            if (rt) rt.anchoredPosition = baseAnchoredPos + moveOffset * k * 0.5f;
            yield return null;
        }

        // Hold
        if (group) group.alpha = 1f;
        if (rt) rt.anchoredPosition = baseAnchoredPos + moveOffset * 0.5f;
        t = 0f;
        while (t < hold) { t += Time.unscaledDeltaTime; yield return null; }

        // Fade Out
        t = 0f;
        while (t < fadeOut)
        {
            t += Time.unscaledDeltaTime;
            float k = 1f - Mathf.Clamp01(t / Mathf.Max(0.0001f, fadeOut));
            if (group) group.alpha = k;
            if (rt) rt.anchoredPosition = baseAnchoredPos + moveOffset * (0.5f + (1f - k) * 0.5f);
            yield return null;
        }

        if (group) { group.alpha = 0f; group.interactable = false; group.blocksRaycasts = false; }
        if (rt) rt.anchoredPosition = baseAnchoredPos;
        playCo = null;
    }
}
