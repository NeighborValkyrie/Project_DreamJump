using UnityEngine;

namespace PlatformerGame.Interactions.NPCs
{
    /// <summary>
    /// NPC 대화 시스템
    /// v7.0: 퀘스트 연동
    /// </summary>
    public class NPC : MonoBehaviour, Interfaces.IInteractable
    {
        [Header("NPC Settings")]
        [SerializeField] private string npcName = "마을 주민";

        [Header("Dialogue")]
        [SerializeField] private Systems.Dialogue.DialogueManager.DialogueLine[] dialogueLines;

        [Header("Quest")]
        [SerializeField] private string questID = "";

        public void Interact(GameObject interactor)
        {
            // 대화 시작
            if (Systems.Dialogue.DialogueManager.Instance != null && dialogueLines.Length > 0)
            {
                Systems.Dialogue.DialogueManager.Instance.StartDialogue(dialogueLines);
            }

            // 퀘스트 시작 (있는 경우)
            if (!string.IsNullOrEmpty(questID) && Systems.Quest.QuestManager.Instance != null)
            {
                Systems.Quest.Quest quest = Systems.Quest.QuestManager.Instance.GetQuestByID(questID);
                if (quest != null && !quest.isActive && !quest.isCompleted)
                {
                    Systems.Quest.QuestManager.Instance.StartQuest(quest);
                }
            }
        }

        public string GetInteractionPrompt()
        {
            return $"[E] {npcName}와 대화하기";
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // 상호작용 프롬프트 표시
                if (Systems.UI.UIManager.Instance != null)
                {
                    Systems.UI.UIManager.Instance.ShowInteractionPrompt(GetInteractionPrompt());
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // 상호작용 프롬프트 숨기기
                if (Systems.UI.UIManager.Instance != null)
                {
                    Systems.UI.UIManager.Instance.HideInteractionPrompt();
                }
            }
        }
    }
}