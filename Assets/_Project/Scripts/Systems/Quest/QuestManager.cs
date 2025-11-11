using UnityEngine;
using System.Collections.Generic;

namespace PlatformerGame.Systems.Quest
{
    /// <summary>
    /// 퀘스트 시스템 관리
    /// v7.0: 기존 코드 유지
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Quests")]
        [SerializeField] private List<Quest> availableQuests = new List<Quest>();

        private List<Quest> activeQuests = new List<Quest>();
        private List<Quest> completedQuests = new List<Quest>();

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
        }

        public void StartQuest(Quest quest)
        {
            if (activeQuests.Contains(quest))
            {
                Debug.LogWarning($"[QuestManager] 퀘스트 '{quest.questName}'는 이미 진행 중입니다.");
                return;
            }

            activeQuests.Add(quest);
            quest.isActive = true;

            // 이벤트 발생
            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.TriggerQuestStarted(quest.questID);
            }

            Debug.Log($"[QuestManager] 퀘스트 시작: {quest.questName}");
        }

        public void CompleteQuest(Quest quest)
        {
            if (!activeQuests.Contains(quest))
            {
                Debug.LogWarning($"[QuestManager] 퀘스트 '{quest.questName}'가 활성화되지 않았습니다.");
                return;
            }

            activeQuests.Remove(quest);
            completedQuests.Add(quest);
            quest.isActive = false;
            quest.isCompleted = true;

            // 이벤트 발생
            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.TriggerQuestCompleted(quest.questID);
            }

            Debug.Log($"[QuestManager] 퀘스트 완료: {quest.questName}");
        }

        public Quest GetQuestByID(string questID)
        {
            return availableQuests.Find(q => q.questID == questID);
        }

        public List<Quest> GetActiveQuests()
        {
            return activeQuests;
        }

        public List<Quest> GetCompletedQuests()
        {
            return completedQuests;
        }
    }
}