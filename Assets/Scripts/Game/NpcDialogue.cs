// NpcDialogue.cs
using System.Diagnostics;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Collider))]
public class NpcDialogue : MonoBehaviour
{
    [Header("Dialogue Data")]
    public string speakerName = "NPC";
    [TextArea(2, 4)]
    public string[] lines;            // Inspector에서 대사들 입력

    [Header("Prompt UI")]
    public GameObject talkPromptUI;   // "E키를 눌러 대화하기" 같은 UI (월드/스크린 상관 없음)

    [Header("Settings")]
    public string playerTag = "Player";
    public KeyCode useKeyFallback = KeyCode.E;

    bool playerInRange;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true; // 근접 감지용 트리거
        if (talkPromptUI) talkPromptUI.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return;

        // ★ 게임이 일시정지면 대화 시작 금지
        if (GameUIController.Instance != null && GameUIController.Instance.IsPaused)
            return;

        // 이미 다른 대화 진행 중이면 무시
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            return;

        bool pressed = false;
#if ENABLE_INPUT_SYSTEM
    if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        pressed = true;
#else
        if (Input.GetKeyDown(useKeyFallback))
            pressed = true;
#endif

        if (pressed)
            StartDialogue();
    }

    void StartDialogue()
    {
        if (DialogueManager.Instance == null)
        {
            UnityEngine.Debug.LogWarning("[NpcDialogue] DialogueManager 인스턴스가 없습니다.");
            return;
        }

        if (lines == null || lines.Length == 0)
        {
            UnityEngine.Debug.LogWarning("[NpcDialogue] 대사(lines)가 비어 있습니다.", this);
            return;
        }

        DialogueManager.Instance.StartDialogue(speakerName, lines);

        // 대화창이 켜졌으니 프롬프트는 숨김
        if (talkPromptUI) talkPromptUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        var root = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        if (!root.CompareTag(playerTag)) return;

        playerInRange = true;
        if (talkPromptUI) talkPromptUI.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        var root = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        if (!root.CompareTag(playerTag)) return;

        playerInRange = false;
        if (talkPromptUI) talkPromptUI.SetActive(false);
    }
}
