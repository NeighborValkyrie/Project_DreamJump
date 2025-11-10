using UnityEngine;

namespace PlatformerGame.Systems.Quest
{
    /// <summary>
    /// 퀘스트 데이터 클래스
    /// v7.0: 기존 코드 유지
    /// </summary>
    [System.Serializable]
    public class Quest
    {
        public string questID;
        public string questName;

        [TextArea(3, 10)]
        public string description;

        public bool isActive = false;
        public bool isCompleted = false;

        // 퀘스트 목표
        public int currentProgress = 0;
        public int targetProgress = 1;

        // 보상
        public int coinReward = 0;
        public string itemReward = "";

        public Quest(string id, string name, string desc)
        {
            questID = id;
            questName = name;
            description = desc;
        }
    }
}