using UnityEngine;

/// <summary>
/// 플레이어가 트리거에 들어오면 자동으로 양쪽으로 열리고,
/// 나가면 딜레이 후 닫힘. 이동 방향은 문 루트의 transform.right 기준으로
/// 왼쪽 패널은 -Right, 오른쪽 패널은 +Right 로 고정.
/// (월드 좌표 position 기반 이동)
/// </summary>
[RequireComponent(typeof(Collider))]
public class SlidingDoorAuto : MonoBehaviour
{
    [Header("=== [TUNE] 참조 ===")]
    [SerializeField] Transform leftDoor;     // [변경가능] 왼쪽 문 패널
    [SerializeField] Transform rightDoor;    // [변경가능] 오른쪽 문 패널
    [SerializeField] Transform axisSource;   // [변경가능] 방향 기준(기본: 이 스크립트의 transform)

    [Header("=== [TUNE] 동작 ===")]
    [SerializeField] string playerTag = "Player";   // [변경가능]
    [SerializeField] float openDistance = 1.5f;     // [변경가능] 한쪽으로 미는 거리(미터)
    [SerializeField] float openSpeed = 3f;          // [변경가능] 여는 속도
    [SerializeField] float closeSpeed = 3f;         // [변경가능] 닫는 속도
    [SerializeField] float autoCloseDelay = 0.5f;   // [변경가능] 나간 뒤 닫히기까지 딜레이

    [Header("=== [TUNE] SFX ===")]
    [SerializeField] AudioSource audioSource;       // [선택] 없으면 자동 생성
    [SerializeField] AudioClip openSfx;             // [변경가능] 열릴 때
    [SerializeField] AudioClip closeSfx;            // [변경가능] 닫힐 때
    [Range(0f, 1f)] public float openVolume = 1f;    // [변경가능]
    [Range(0f, 1f)] public float closeVolume = 1f;   // [변경가능]

    // 내부 상태(월드 좌표 기준)
    Vector3 leftClosedPos, rightClosedPos;
    Vector3 leftOpenPos, rightOpenPos;
    Vector3 axisRight; // 기준 Right 벡터

    bool isOpenTarget;
    float lastLeaveTime;
    int insideCount;
    Collider trigger;

    void Awake()
    {
        trigger = GetComponent<Collider>();
        trigger.isTrigger = true;

        if (!axisSource) axisSource = transform; // 방향 기준 없으면 자기 자신
        axisRight = axisSource.right.normalized;

        if (!leftDoor || !rightDoor)
        {
            Debug.LogWarning("[SlidingDoorAuto] leftDoor / rightDoor를 할당하세요.", this);
            enabled = false;
            return;
        }

        // 현재 '닫힌 위치'(월드 좌표)
        leftClosedPos = leftDoor.position;
        rightClosedPos = rightDoor.position;

        // ‘왼쪽 패널은 -Right’, ‘오른쪽 패널은 +Right’로 고정
        leftOpenPos = leftClosedPos + (-axisRight * openDistance);
        rightOpenPos = rightClosedPos + (axisRight * openDistance);

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
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    void Update()
    {
        if (!leftDoor || !rightDoor) return;

        // 아무도 없고 열려있는 상태면 딜레이 후 닫힘
        if (insideCount == 0 && isOpenTarget)
        {
            if (Time.time - lastLeaveTime >= autoCloseDelay)
                SetOpenTarget(false);
        }

        // 목표 상태에 따라 월드 좌표로 문 이동
        Vector3 targetL = isOpenTarget ? leftOpenPos : leftClosedPos;
        Vector3 targetR = isOpenTarget ? rightOpenPos : rightClosedPos;
        float speed = isOpenTarget ? openSpeed : closeSpeed;

        leftDoor.position = Vector3.MoveTowards(leftDoor.position, targetL, speed * Time.deltaTime);
        rightDoor.position = Vector3.MoveTowards(rightDoor.position, targetR, speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        insideCount++;
        SetOpenTarget(true); // 열리기 시작 + 열림 SFX
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        insideCount = Mathf.Max(insideCount - 1, 0);
        if (insideCount == 0) lastLeaveTime = Time.time; // 닫힘 카운트다운 시작
    }

    void SetOpenTarget(bool open)
    {
        if (isOpenTarget == open) return;
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

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        var src = axisSource ? axisSource : transform;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(src.position, src.position + src.right * 1.5f);
        Gizmos.DrawWireSphere(src.position + src.right * 1.5f, 0.05f);
    }
#endif
}
