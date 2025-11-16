
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class DialogueManager : MonoBehaviour
{
    bool skipFirstNextInput = false;
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject dialoguePanel;        // 전체 대화 패널
    public TMP_Text speakerNameText;        // 화자 이름 (옵션)
    public TMP_Text dialogueBodyText;       // 본문 대사

    [Header("Navigation Buttons")]
    public Button prevButton;               // 이전 대사 버튼 (←)
    public Button nextButton;               // 다음 대사 버튼 (→)
    public Button closeButton;              // 닫기(X) 버튼

    [Header("Keyboard Input")]
    public KeyCode nextKeyFallback = KeyCode.E;        // E키: 다음 대사
    public KeyCode closeKeyFallback = KeyCode.Escape;  // ESC: 종료

    string[] currentLines;
    int currentIndex;
    System.Action onDialogueEnd;

    bool isDialogueActive;
    public bool IsDialogueActive => isDialogueActive;

    // ★ 이 프레임에 막 대화가 끝났는지 기록
    int lastEndFrame = -1;
    public bool IsBlockingNpcInputThisFrame => isDialogueActive || Time.frameCount == lastEndFrame;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (dialoguePanel) dialoguePanel.SetActive(false);
    }

    void Start()
    {
        // 버튼 클릭 이벤트 연결
        if (nextButton) nextButton.onClick.AddListener(GoNext);
        if (prevButton) prevButton.onClick.AddListener(GoPrev);
        if (closeButton) closeButton.onClick.AddListener(EndDialogue);

        UpdateNavButtons();
    }

    void Update()
    {
        if (!isDialogueActive) return;

        bool nextPressed = false;
        bool closePressed = false;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
                nextPressed = true;
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                closePressed = true;
        }
#else
        if (Input.GetKeyDown(nextKeyFallback))
            nextPressed = true;
        if (Input.GetKeyDown(closeKeyFallback))
            closePressed = true;
#endif

        // ★ 대화 막 시작한 프레임: E 입력은 이미 사용했으니 한 번 무시
        if (skipFirstNextInput)
        {
            skipFirstNextInput = false;

            // 혹시 같은 프레임에 esc를 같이 눌렀다면 그건 인정
            if (closePressed)
                EndDialogue();

            return;
        }

        if (closePressed)
        {
            EndDialogue();
            return;
        }

        if (nextPressed)
        {
            GoNext();
        }
    }

    // -------------------------
    // 외부에서 호출할 시작 함수
    // -------------------------
    public void StartDialogue(string speakerName, string[] lines, System.Action onEnd = null)
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("[DialogueManager] 시작하려는 대사가 비어있습니다.");
            return;
        }

        currentLines = lines;
        currentIndex = 0;
        onDialogueEnd = onEnd;

        if (speakerNameText)
            speakerNameText.text = speakerName ?? "";

        // ★ 먼저 대화 활성화 플래그 세팅
        isDialogueActive = true;

        // ★ 키보드 E 첫 입력은 '시작용'이므로 한 번 무시
        skipFirstNextInput = true;

        // 이제 첫 줄 표시 + 버튼 상태 갱신
        ShowCurrentLine();

        // UI 켜기
        if (dialoguePanel) dialoguePanel.SetActive(true);

        // 게임 일시정지 + 커서 활성화
        PauseGameForDialogue();
    }

    // -------------------------
    // 내부: 대사 표시 / 인덱스 이동
    // -------------------------
    void ShowCurrentLine()
    {
        if (currentLines == null || currentLines.Length == 0)
            return;

        currentIndex = Mathf.Clamp(currentIndex, 0, currentLines.Length - 1);
        if (dialogueBodyText)
            dialogueBodyText.text = currentLines[currentIndex];

        UpdateNavButtons();
    }

    void GoNext()
    {
        Step(+1);
    }

    void GoPrev()
    {
        Step(-1);
    }

    void Step(int delta)
    {
        if (currentLines == null || currentLines.Length == 0)
        {
            EndDialogue();
            return;
        }

        int lastIndex = currentLines.Length - 1;

        // 마지막 대사에서 "다음"을 누르면 종료
        if (delta > 0 && currentIndex >= lastIndex)
        {
            EndDialogue();
            return;
        }

        int newIndex = Mathf.Clamp(currentIndex + delta, 0, lastIndex);

        if (newIndex != currentIndex)
        {
            currentIndex = newIndex;
            ShowCurrentLine();
        }
        else
        {
            // 맨 앞에서 이전 누르면 아무 변화 없음
            // 맨 뒤에서 다음 누르면 위 if에서 이미 종료 처리
        }
    }

    void UpdateNavButtons()
    {
        if (currentLines == null || currentLines.Length == 0)
        {
            if (prevButton) prevButton.interactable = false;
            if (nextButton) nextButton.interactable = false;
            return;
        }

        int lastIndex = currentLines.Length - 1;

        if (prevButton)
            prevButton.interactable = currentIndex > 0;

        if (nextButton)
            nextButton.interactable = true;  // 항상 클릭 가능 (마지막 줄에서 클릭하면 EndDialogue 호출)
    }

    // -------------------------
    // 종료 처리
    // -------------------------
    public void EndDialogue()
    {
        if (!isDialogueActive) return;

        isDialogueActive = false;
        lastEndFrame = Time.frameCount;   // ★ 이 프레임에는 NPC가 다시 시작 못 하게

        // UI 끄기
        if (dialoguePanel) dialoguePanel.SetActive(false);

        // 게임 재개 + 커서 잠금
        ResumeGameFromDialogue();

        // 콜백 호출
        onDialogueEnd?.Invoke();
        onDialogueEnd = null;

        currentLines = null;
        currentIndex = 0;
        UpdateNavButtons();
    }

    // -------------------------
    // 게임 정지 / 재개 & 커서 처리
    // -------------------------
    void PauseGameForDialogue()
    {
        Time.timeScale = 0f;

        // 대화 중에는 마우스 커서 활성화 + 잠금 해제
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void ResumeGameFromDialogue()
    {
        // 게임 다시 재생
        Time.timeScale = 1f;

        // 어떤 상황이든, 대화 끝나면 항상 커서 잠금 + 숨김
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
