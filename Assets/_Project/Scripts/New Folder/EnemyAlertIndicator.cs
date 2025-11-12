using UnityEngine;

[RequireComponent(typeof(EnemyChaseNav))]
public class EnemyAlertIndicator : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite questionSprite;       // 평소 (?)
    public Sprite exclamationSprite;    // 추격 (!)

    [Header("References")]
    public SpriteRenderer indicatorRenderer;
    public EnemyChaseNav chase;         // 그대로 사용
    public EnemyConeSight sight;        // 보조 판정용(선택)

    [Header("Settings")]
    public Vector3 offset = new Vector3(0f, 2f, 0f);
    public bool faceCamera = true;
    public int sortingOrder = 50;
    public float scale = 1f;

    void Awake()
    {
        if (!chase) chase = GetComponent<EnemyChaseNav>();
        if (!sight) sight = GetComponent<EnemyConeSight>();

        if (!indicatorRenderer)
        {
            var go = new GameObject("AlertIcon");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = offset;
            indicatorRenderer = go.AddComponent<SpriteRenderer>();
        }

        indicatorRenderer.sortingOrder = sortingOrder;
        indicatorRenderer.transform.localScale = Vector3.one * scale;
    }

    void LateUpdate()
    {
        if (!indicatorRenderer) return;

        // 1순위: FSM 상태로 판정(EnemyChaseNav 수정 불필요)
        bool chasing = (chase && chase.CurrentState == EnemyChaseNav.State.Chase);

        // 2순위: 보조로 시야 체크(선택)
        if (!chasing && sight) chasing = sight.CanSeePlayer;

        indicatorRenderer.sprite = chasing ? exclamationSprite : questionSprite;

        // 머리 위 고정
        indicatorRenderer.transform.localPosition = offset;

        // 빌보드 처리
        if (faceCamera && Camera.main)
        {
            var camFwd = Camera.main.transform.forward;
            indicatorRenderer.transform.rotation = Quaternion.LookRotation(camFwd, Vector3.up);
        }
    }
}
