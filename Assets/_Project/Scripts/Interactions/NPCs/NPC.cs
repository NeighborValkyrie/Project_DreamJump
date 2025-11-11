using UnityEngine;

namespace PlatformerGame.Interactions.NPCs
{
    public class NPC : MonoBehaviour, Interfaces.IInteractable
    {
        [Header("NPC Settings")]
        [SerializeField] private string npcName = "마을 주민";

        [Header("Outline (Optional)")]
        [SerializeField] private bool useOutline = false;
        [SerializeField] private Color outlineColor = Color.yellow;
        [SerializeField] private float outlineWidth = 5f;

        [Header("Dialogue")]
        [SerializeField] private Systems.Dialogue.DialogueManager.DialogueLine[] dialogueLines;

        [Header("Quest")]
        [SerializeField] private string questID = "";

        [Header("Detection")]
        [SerializeField] private float detectionRadius = 3f;

        private Renderer[] renderers;
        private MaterialPropertyBlock propertyBlock;
        private bool isPlayerNearby;

        private void Awake()
        {
            if (useOutline)
            {
                renderers = GetComponentsInChildren<Renderer>();
                propertyBlock = new MaterialPropertyBlock();
            }
        }

        private void Update()
        {
            CheckPlayerDistance();
        }

        private void CheckPlayerDistance()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            float distance = Vector3.Distance(transform.position, player.transform.position);
            bool wasNearby = isPlayerNearby;
            isPlayerNearby = distance <= detectionRadius;

            if (isPlayerNearby != wasNearby)
            {
                if (isPlayerNearby)
                {
                    if (useOutline) ShowOutline(true);
                    ShowInteractionUI(true);
                }
                else
                {
                    if (useOutline) ShowOutline(false);
                    ShowInteractionUI(false);
                }
            }
        }

        private void ShowOutline(bool show)
        {
            if (renderers == null || renderers.Length == 0) return;

            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;

                renderer.GetPropertyBlock(propertyBlock);
                
                if (show)
                {
                    propertyBlock.SetColor("_OutlineColor", outlineColor);
                    propertyBlock.SetFloat("_OutlineWidth", outlineWidth);
                }
                else
                {
                    propertyBlock.SetFloat("_OutlineWidth", 0f);
                }
                
                renderer.SetPropertyBlock(propertyBlock);
            }
        }

        private void ShowInteractionUI(bool show)
        {
            if (Systems.UI.UIManager.Instance != null)
            {
                if (show)
                {
                    Systems.UI.UIManager.Instance.ShowInteractionPrompt(GetInteractionPrompt());
                }
                else
                {
                    Systems.UI.UIManager.Instance.HideInteractionPrompt();
                }
            }
        }

        public void Interact(GameObject interactor)
        {
            if (Systems.Dialogue.DialogueManager.Instance != null && dialogueLines.Length > 0)
            {
                Systems.Dialogue.DialogueManager.Instance.StartDialogue(dialogueLines);
            }

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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}