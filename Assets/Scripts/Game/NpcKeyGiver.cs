// NpcKeyGiver.cs

using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Collider))]
public class NpcKeyGiver : MonoBehaviour
{
    [Header("Dialogue")]
    public string speakerName = "NPC";

    [TextArea(2, 4)]
    public string[] notEnoughLines;       // 코인 부족할 때 대사

    [TextArea(2, 4)]
    public string[] giveKeyLines;         // 키 줄 때 대사

    [TextArea(2, 4)]
    public string[] afterGivenLines;      // 이미 키 받은 뒤 다시 말 걸 때 대사

    [Header("Cost / Reward")]
    public int costCoins = 50;            // 필요한 코인 개수
    public bool consumeCoins = true;      // true면 코인 50개 차감, false면 그냥 조건만 체크

    [Header("Prompt UI")]
    public GameObject talkPromptUI;       // "E키를 눌러 대화" 같은 UI
    public string playerTag = "Player";
    public KeyCode useKeyFallback = KeyCode.E;

    bool playerInRange;
    bool hasGivenKey = false;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        if (talkPromptUI)
            talkPromptUI.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return;

        // 일시정지 / 대화 중 / 막 끝난 프레임에는 동작 X
        if (GameUIController.Instance != null && GameUIController.Instance.IsPaused) return;
        if (DialogueManager.Instance != null &&
            DialogueManager.Instance.IsBlockingNpcInputThisFrame) return;

        bool pressed = false;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            pressed = true;
#else
        if (Input.GetKeyDown(useKeyFallback))
            pressed = true;
#endif

        if (pressed)
        {
            TryTalk();
        }
    }

    void TryTalk()
    {
        if (!DialogueManager.Instance)
        {
            Debug.LogWarning("[NpcKeyGiver] DialogueManager 인스턴스가 없습니다.");
            return;
        }
        if (!CollectibleManager.Instance)
        {
            Debug.LogWarning("[NpcKeyGiver] CollectibleManager 인스턴스가 없습니다.");
            return;
        }

        // 혹시 다른 경로로 호출될 수도 있으니, 여기서도 한 번 더 방어
        if (DialogueManager.Instance.IsBlockingNpcInputThisFrame)
        {
            return;
        }

        string[] linesToUse = null;

        if (!hasGivenKey)
        {
            int coins = CollectibleManager.Instance.GetCoin();

            if (coins >= costCoins)
            {
                // 코인 충분 → 키 지급
                if (consumeCoins)
                    CollectibleManager.Instance.AddCoin(-costCoins); // 코인 50개 사용

                CollectibleManager.Instance.AddKey(1); // 키 1개 추가
                hasGivenKey = true;

                linesToUse = (giveKeyLines != null && giveKeyLines.Length > 0)
                    ? giveKeyLines
                    : new[] { "열쇠를 줄게. 이걸로 문을 열 수 있어." };
            }
            else
            {
                // 코인 부족
                linesToUse = (notEnoughLines != null && notEnoughLines.Length > 0)
                    ? notEnoughLines
                    : new[] { $"코인이 {costCoins}개 있어야 열쇠를 줄 수 있어." };
            }
        }
        else
        {
            // 이미 키 준 이후 대사
            if (afterGivenLines != null && afterGivenLines.Length > 0)
                linesToUse = afterGivenLines;
            else
                linesToUse = new[] { "이미 열쇠 줬잖아. 문으로 가 봐!" };
        }

        if (talkPromptUI)
            talkPromptUI.SetActive(false);

        DialogueManager.Instance.StartDialogue(speakerName, linesToUse, OnDialogueEnd);
    }

    void OnDialogueEnd()
    {
        // 대화 끝나고 아직 범위 안에 있으면 프롬프트 다시 보여주기
        if (playerInRange && talkPromptUI)
            talkPromptUI.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        var root = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        if (!root.CompareTag(playerTag)) return;

        playerInRange = true;
        if (talkPromptUI &&
            (DialogueManager.Instance == null || !DialogueManager.Instance.IsDialogueActive))
            talkPromptUI.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        var root = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        if (!root.CompareTag(playerTag)) return;

        playerInRange = false;
        if (talkPromptUI)
            talkPromptUI.SetActive(false);
    }
}
