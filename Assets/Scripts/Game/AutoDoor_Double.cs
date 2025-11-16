using System.Diagnostics;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Collider))]
public class AutoDoor_Double_Target : MonoBehaviour
{
    [Header("문 Transform")]
    public Transform leftDoor;          // 실제 왼쪽 문
    public Transform rightDoor;         // 실제 오른쪽 문

    [Header("열렸을 때 위치(Target)")]
    public Transform leftOpenTarget;    // 왼쪽 문이 완전히 열렸을 때 자리
    public Transform rightOpenTarget;   // 오른쪽 문이 완전히 열렸을 때 자리

    [Header("동작 설정")]
    public float moveSpeed = 4f;        // 열리고 닫히는 속도
    public string playerTag = "Player"; // 플레이어 태그 이름

    [Header("오디오 설정")]
    public AudioClip openDoorClip;      // 문 열릴 때 재생할 사운드
    public float openDoorVolume = 1f;   // 사운드 볼륨
    public AudioSource audioSource;     // 재생에 사용할 AudioSource
    public float minInterval = 0.3f;    // 연속 재생 방지 최소 간격(초)

    [Header("Key Lock (옵션)")]
    public bool requireKey = false;     // true면 키 있어야 문 해제
    public int requiredKeys = 1;        // 필요한 키 개수
    public bool consumeKeyOnUse = true; // true면 문 해제 시 키 소모

    [Header("UI")]
    public GameObject openPromptUI;     // 문 근처: "E키를 눌러 문을 여세요"
    public GameObject lockedHintUI;     // 키 없을 때: "열쇠가 필요합니다"
    public float lockedHintDuration = 1.5f;
    public KeyCode useKeyFallback = KeyCode.E;

    Vector3 _leftClosedLocal;
    Vector3 _rightClosedLocal;
    Vector3 _leftOpenLocal;
    Vector3 _rightOpenLocal;

    bool _playerInRange = false;

    bool _isUnlocked = false;       // 키로 잠금 해제되었는지
    bool _isOpenNow = false;
    bool _prevIsOpenNow = false;
    float _lastOpenSoundTime = -999f;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        if (!leftDoor || !rightDoor || !leftOpenTarget || !rightOpenTarget)
        {
            UnityEngine.Debug.LogWarning("AutoDoor_Double_Target : 문/타겟 Transform을 모두 지정해야 합니다.");
            enabled = false;
            return;
        }

        // 닫힌 위치 = 시작 localPosition
        _leftClosedLocal = leftDoor.localPosition;
        _rightClosedLocal = rightDoor.localPosition;

        // 열린 위치 = 타겟의 localPosition
        _leftOpenLocal = leftOpenTarget.localPosition;
        _rightOpenLocal = rightOpenTarget.localPosition;

        if (openPromptUI) openPromptUI.SetActive(false);
        if (lockedHintUI) lockedHintUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        _playerInRange = true;

        // 문 앞에 들어오면 "E키를 눌러 문을 여세요" 표시
        if (requireKey && !_isUnlocked)
        {
            if (openPromptUI) openPromptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        _playerInRange = false;

        // 범위 벗어나면 UI들 끄기
        if (openPromptUI) openPromptUI.SetActive(false);
        if (lockedHintUI) lockedHintUI.SetActive(false);
    }

    void Update()
    {
        // 1) 키 잠금 해제 입력 처리 (문 앞에 있을 때만)
        if (requireKey && !_isUnlocked && _playerInRange)
        {
            // (선택) 일시정지/대화 중엔 상호작용 막고 싶으면 여기서 체크
            // if (GameUIController.Instance != null && GameUIController.Instance.IsPaused) return;
            // if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive) return;

            bool pressed = false;
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                pressed = true;
#else
            if (Input.GetKeyDown(useKeyFallback))
                pressed = true;
#endif

            if (pressed)
                TryUnlockByKey();
        }

        // 2) 문 열림/닫힘 상태 결정
        bool shouldOpen = _playerInRange && (!requireKey || _isUnlocked);

        _isOpenNow = shouldOpen;

        // 상태 변화: 닫힘(false) → 열림(true)로 바뀌는 그 프레임에만 사운드
        if (!_prevIsOpenNow && _isOpenNow)
        {
            TryPlayOpenSound();
        }
        _prevIsOpenNow = _isOpenNow;

        Vector3 targetL = shouldOpen ? _leftOpenLocal : _leftClosedLocal;
        Vector3 targetR = shouldOpen ? _rightOpenLocal : _rightClosedLocal;

        float t = moveSpeed * Time.deltaTime;

        leftDoor.localPosition = Vector3.Lerp(leftDoor.localPosition, targetL, t);
        rightDoor.localPosition = Vector3.Lerp(rightDoor.localPosition, targetR, t);
    }

    void TryUnlockByKey()
    {
        if (!CollectibleManager.Instance)
        {
            UnityEngine.Debug.LogWarning("[AutoDoor_Double_Target] CollectibleManager 인스턴스가 없습니다.");
            return;
        }

        int keyCount = CollectibleManager.Instance.GetKey();

        if (keyCount >= requiredKeys)
        {
            // 🔓 키 충분 → 잠금 해제
            if (consumeKeyOnUse && requiredKeys > 0)
                CollectibleManager.Instance.AddKey(-requiredKeys);

            _isUnlocked = true;

            // 안내 UI들 끄기
            if (openPromptUI) openPromptUI.SetActive(false);
            if (lockedHintUI) lockedHintUI.SetActive(false);
        }
        else
        {
            // ❌ 키 부족 → 힌트 표시
            if (lockedHintUI)
            {
                lockedHintUI.SetActive(true);
                CancelInvoke(nameof(HideLockedHint));
                Invoke(nameof(HideLockedHint), lockedHintDuration);
            }
        }
    }

    void HideLockedHint()
    {
        if (lockedHintUI) lockedHintUI.SetActive(false);
    }

    void TryPlayOpenSound()
    {
        if (!openDoorClip) return;

        // 연속 재생 방지 쿨타임 체크
        if (Time.time - _lastOpenSoundTime < minInterval) return;

        if (audioSource)
            audioSource.PlayOneShot(openDoorClip, openDoorVolume);
        else
            AudioSource.PlayClipAtPoint(openDoorClip, transform.position, openDoorVolume);

        _lastOpenSoundTime = Time.time;
    }
}
