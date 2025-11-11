using UnityEngine;
using System.Collections.Generic;

public class PlayerHideController : MonoBehaviour
{
    [Header("=== [TUNE] 입력 ===")]
    [SerializeField] KeyCode hideToggleKey = KeyCode.E;         // [변경가능] 숨기/나오기 키: [KEY_HIDE]
    [SerializeField] LayerMask hiddenLayer = 0;                  // [변경가능] 숨김시 적용할 레이어: [LAYER_HIDDEN]
    [SerializeField] bool hideRenderers = true;                  // [변경가능] 렌더러 가시성 끄기
    [SerializeField] bool disableCollisions = false;             // [변경가능] 콜라이더 끄기(들락날락하는 오브젝트면 비추)
    [SerializeField] MonoBehaviour[] toDisableWhileHidden;       // [변경가능] 숨는 동안 비활성화할 이동/입력 스크립트들

    [Header("=== [TUNE] 연출 ===")]
    [SerializeField] float snapIntoSpotSpeed = 12f;              // [변경가능] 숨는 지점으로 끌려가는 속도: [SNAP_SPEED]
    [SerializeField] float exitOffsetForward = 0.6f;             // [변경가능] 나오며 앞쪽으로 밀어낼 거리: [EXIT_OFFSET]

    public bool IsHidden { get; private set; }
    public static bool AnyPlayerHidden;                          // 적 시야에서 참고

    Transform currentSpot;
    int originalLayer;
    List<Renderer> cachedRenderers = new List<Renderer>();
    List<Collider> cachedColliders = new List<Collider>();

    void Awake()
    {
        originalLayer = gameObject.layer;
        GetComponentsInChildren(true, cachedRenderers);
        GetComponentsInChildren(true, cachedColliders);
    }

    void Update()
    {
        // 숨은 상태에서 '나오기' 단축키 허용
        if (IsHidden && Input.GetKeyDown(hideToggleKey))
            ExitHide();
    }

    public void EnterHide(Transform spotAnchor)
    {
        if (IsHidden) return;
        IsHidden = true; AnyPlayerHidden = true;
        currentSpot = spotAnchor;

        // 이동/입력 스크립트 비활
        foreach (var m in toDisableWhileHidden)
            if (m) m.enabled = false;

        // 레이어 전환
        if (hiddenLayer != 0) gameObject.layer = LayerMaskToLayerIndex(hiddenLayer);

        // 렌더러 끄기
        if (hideRenderers)
            foreach (var r in cachedRenderers) if (r) r.enabled = false;

        // 콜라이더 끄기(선택)
        if (disableCollisions)
            foreach (var c in cachedColliders) if (c) c.enabled = false;

        // 스냅 이동 코루틴
        StopAllCoroutines();
        StartCoroutine(SnapToSpot());
    }

    System.Collections.IEnumerator SnapToSpot()
    {
        // 살짝 끌려들어가는 연출
        while (IsHidden && currentSpot)
        {
            transform.position = Vector3.Lerp(transform.position, currentSpot.position, Time.deltaTime * snapIntoSpotSpeed);
            yield return null;
        }
    }

    public void ExitHide()
    {
        if (!IsHidden) return;
        IsHidden = false; AnyPlayerHidden = false;

        // 이동/입력 복구
        foreach (var m in toDisableWhileHidden)
            if (m) m.enabled = true;

        // 레이어 복구
        gameObject.layer = originalLayer;

        // 렌더러 복구
        if (hideRenderers)
            foreach (var r in cachedRenderers) if (r) r.enabled = true;

        // 콜라이더 복구
        if (disableCollisions)
            foreach (var c in cachedColliders) if (c) c.enabled = true;

        // 스팟 앞쪽으로 살짝 밀어주기
        if (currentSpot)
        {
            Vector3 forward = currentSpot.forward;
            transform.position = currentSpot.position + forward * exitOffsetForward;
        }
        currentSpot = null;
    }

    // HiddenLayer를 인덱스로 환산(에디터에서 "Hidden" 레이어를 만들어두면 편함)
    int LayerMaskToLayerIndex(LayerMask mask)
    {
        int layer = mask.value;
        for (int i = 0; i < 32; i++)
        {
            if (layer == (1 << i)) return i;
        }
        return gameObject.layer; // 실패 시 원래 레이어 유지
    }
}
