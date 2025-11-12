using UnityEngine;

/// <summary>
/// 플레이어가 근처에 오면 자동으로 양쪽으로 열리고,
/// 떠나면 자동으로 닫히는 슬라이드 도어 + 여닫을 때 SFX 재생.
/// </summary>
[RequireComponent(typeof(Collider))]
public class SlidingDoorAuto : MonoBehaviour
{
    [Header("=== [TUNE] 참조 ===")]
    [SerializeField] Transform leftDoor;   // 왼쪽 문 패널
    [SerializeField] Transform rightDoor;  // 오른쪽 문 패널

    [Header("=== [TUNE] 동작 ===")]
    [SerializeField] string playerTag = "Player";  // 플레이어 태그
    [SerializeField] float openDistance = 1.5f;    // 한쪽으로 미는 거리
    [SerializeField] float openSpeed = 3f;         // 여는 속도
    [SerializeField] float closeSpeed = 3f;        // 닫는 속도
    [SerializeField] float autoCloseDelay = 0.5f;  // 나간 뒤 닫히기까지 딜레이

    [Header("=== [TUNE] SFX ===")]
    public AudioSource audioSource;               // 없으면 자동 생성
    public AudioClip openSfx;                     // 열릴 때
    public AudioClip closeSfx;                    // 닫힐 때
    [Range(0f, 1f)] public float openVolume = 1f;
    [Range(0f, 1f)] public float closeVolume = 1f;

    Vector3 leftClosedPos, rightClosedPos;
    Vector3 leftOpenPos, rightOpenPos;

    bool isOpenTarget;       // 열려라 / 닫혀라 목표 상태
    float lastLeaveTime;     // 마지막으로 플레이어가 나간 시각
    int insideCount;         // 트리거 안 플레이어 수 (보통 0/1)

    Collider trigger;

    void Awake()
    {
        trigger = GetComponent<Collider>();
        trigger.isTrigger = true; // 감지용이라 반드시 트리거

        if (!leftDoor || !rightDoor)
        {
            Debug.LogWarning("[SlidingDoorAuto] leftDoor / rightDoor를 할당해 주세요.", this);
        }
        else
        {
            // 현재 로컬 위치 = "닫힌 상태"
            leftClosedPos = leftDoor.localPosition;
            rightClosedPos = rightDoor.localPosition;

            // 로컬 X 좌표를 기준으로 바깥 방향으로 미는 벡터 계산
            float signL = Mathf.Sign(leftClosedPos.x == 0 ? -1f : leftClosedPos.x);
            float signR = Mathf.Sign(rightClosedPos.x == 0 ? 1f : rightClosedPos.x);

            leftOpenPos = leftClosedPos + Vector3.right * openDistance * signL;
            rightOpenPos = rightClosedPos + Vector3.right * openDistance * signR;
        }

        // 오디오 소스 자동 준비
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
            if (!audioSource)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }

        // 3D 문 같은 느낌: 필요하면 취향대로 조절
        audioSource.spatialBlend = 1f;     // 3D 사운드
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    void Update()
    {
        if (!leftDoor || !rightDoor) return;

        // 플레이어가 아무도 없고, 열려 있는 상태이면 → 딜레이 후 닫힘
        if (insideCount == 0 && isOpenTarget)
        {
            if (Time.time - lastLeaveTime >= autoCloseDelay)
            {
                SetOpenTarget(false); // 여기서 "닫혀라" + 닫힘 SFX
            }
        }

        // 목표 상태에 따라 문 위치 보간
        Vector3 targetL = isOpenTarget ? leftOpenPos : leftClosedPos;
        Vector3 targetR = isOpenTarget ? rightOpenPos : rightClosedPos;
        float speed = isOpenTarget ? openSpeed : closeSpeed;

        leftDoor.localPosition = Vector3.MoveTowards(leftDoor.localPosition, targetL, speed * Time.deltaTime);
        rightDoor.localPosition = Vector3.MoveTowards(rightDoor.localPosition, targetR, speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        insideCount++;
        // 열릴 방향으로 상태 전환 + 열림 SFX
        SetOpenTarget(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        insideCount = Mathf.Max(insideCount - 1, 0);
        if (insideCount == 0)
        {
            // 바로 닫지 않고, 딜레이 카운트만 시작
            lastLeaveTime = Time.time;
        }
    }

    // ★ 여기서 "열림/닫힘 상태 전환" + SFX 재생을 한 번에 처리
    void SetOpenTarget(bool open)
    {
        if (isOpenTarget == open) return; // 상태 변동 없으면 무시

        isOpenTarget = open;

        if (!audioSource) return;

        if (open)
        {
            if (openSfx) audioSource.PlayOneShot(openSfx, openVolume);
        }
        else
        {
            if (closeSfx) audioSource.PlayOneShot(closeSfx, closeVolume);
        }
    }
}
