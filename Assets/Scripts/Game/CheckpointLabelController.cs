// CheckpointLabelController.cs
using UnityEngine;

public class CheckpointLabelController : MonoBehaviour
{
    public enum ShowMode
    {
        Always,          // 항상 표시
        WhenPlayerNear,  // 플레이어가 일정 거리 이내일 때만
        UntilActivated,  // 체크포인트 찍기 전까지 표시, 찍으면 숨김
        OnActivateFlash  // 찍는 순간에만 잠깐 표시
    }

    [Header("Mode")]
    public ShowMode mode = ShowMode.WhenPlayerNear;

    [Header("Near Settings")]
    public string playerTag = "Player";
    public float showRadius = 8f;
    public float hideRadius = 9.5f;

    [Header("Flash Settings")]
    public float flashSeconds = 1.2f;

    [Header("Optional")]
    public CanvasGroup group;         // 없으면 SetActive 사용
    public bool billboardToCamera = true;

    Transform player;
    float flashTimer;

    void Awake()
    {
        if (!group) group = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        // 카메라 향하도록 빌보드
        if (billboardToCamera && Camera.main)
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        switch (mode)
        {
            case ShowMode.Always:
                SetVisible(true);
                break;

            case ShowMode.WhenPlayerNear:
                UpdateNearMode();
                break;

            case ShowMode.UntilActivated:
                // 활성화 전에는 Checkpoint에서 SetActive(true) 상태로 유지
                // OnActivated()에서 끄기만 수행
                break;

            case ShowMode.OnActivateFlash:
                if (flashTimer > 0f)
                {
                    flashTimer -= Time.unscaledDeltaTime;
                    SetVisible(true);
                }
                else
                {
                    SetVisible(false);
                }
                break;
        }
    }

    void UpdateNearMode()
    {
        if (!player)
        {
            var go = GameObject.FindGameObjectWithTag(playerTag);
            if (go) player = go.transform;
        }
        if (!player)
        {
            SetVisible(false);
            return;
        }

        float d = Vector3.Distance(player.position, transform.position);
        bool showNow;

        if (d <= showRadius) showNow = true;
        else if (d >= hideRadius) showNow = false;
        else showNow = IsVisible(); // 경계 구간에서는 이전 상태 유지(깜빡임 방지)

        SetVisible(showNow);
    }

    bool IsVisible()
    {
        if (group) return group.alpha > 0.5f;
        return gameObject.activeSelf;
    }

    void SetVisible(bool on)
    {
        if (group)
        {
            float target = on ? 1f : 0f;
            group.alpha = Mathf.MoveTowards(group.alpha, target, Time.unscaledDeltaTime * 8f);
            group.blocksRaycasts = false;
        }
        else
        {
            if (gameObject.activeSelf != on)
                gameObject.SetActive(on);
        }
    }

    // Checkpoint에서 호출
    public void OnActivated()
    {
        if (mode == ShowMode.UntilActivated)
        {
            SetVisible(false);
        }
        else if (mode == ShowMode.OnActivateFlash)
        {
            flashTimer = Mathf.Max(flashTimer, flashSeconds);
        }
    }
}
