using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerGame.Systems.Dialogue
{
    /// <summary>
    /// 대화 시스템 관리
    /// v7.0: TimeScale 버그 수정
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [System.Serializable]
        public class DialogueLine
        {
            public string characterName;
            [TextArea(3, 10)]
            public string text;
        }

        [Header("UI References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button continueButton;

        [Header("Typing Settings")]
        [SerializeField] private float typingSpeed = 0.05f;

        private Queue<DialogueLine> dialogueQueue;
        private bool isTyping = false;
        private bool isDialogueActive = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            dialogueQueue = new Queue<DialogueLine>();
        }

        private void Start()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }

            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinueButtonClicked);
            }
        }

        public void StartDialogue(DialogueLine[] lines)
        {
            dialogueQueue.Clear();

            foreach (DialogueLine line in lines)
            {
                dialogueQueue.Enqueue(line);
            }

            isDialogueActive = true;

            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }

            // 입력 비활성화
            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.TriggerPlayerInputStateChanged(false);
                Events.GameEventManager.Instance.TriggerDialogueStarted();
            }

            DisplayNextLine();
        }

        private void DisplayNextLine()
        {
            if (dialogueQueue.Count == 0)
            {
                EndDialogue();
                return;
            }

            DialogueLine line = dialogueQueue.Dequeue();

            if (characterNameText != null)
            {
                characterNameText.text = line.characterName;
            }

            StopAllCoroutines();
            StartCoroutine(TypeLine(line.text));
        }

        private IEnumerator TypeLine(string text)
        {
            isTyping = true;

            if (dialogueText != null)
            {
                dialogueText.text = "";

                foreach (char c in text)
                {
                    dialogueText.text += c;
                    yield return new WaitForSecondsRealtime(typingSpeed); // ⭐ TimeScale 영향 없음
                }
            }

            isTyping = false;
        }

        private void OnContinueButtonClicked()
        {
            if (isTyping)
            {
                StopAllCoroutines();
                isTyping = false;

                // 전체 텍스트 즉시 표시
                if (dialogueText != null && dialogueQueue.Count > 0)
                {
                    dialogueText.text = dialogueQueue.Peek().text;
                }
            }
            else
            {
                DisplayNextLine();
            }
        }

        private void EndDialogue()
        {
            isDialogueActive = false;

            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }

            // 입력 활성화
            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.TriggerPlayerInputStateChanged(true);
                Events.GameEventManager.Instance.TriggerDialogueEnded();
            }
        }

        public bool IsDialogueActive() => isDialogueActive;
    }
}