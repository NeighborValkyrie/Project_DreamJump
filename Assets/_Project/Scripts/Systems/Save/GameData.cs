using UnityEngine;
using System.Collections.Generic;

namespace PlatformerGame.Systems.Save
{
    /// <summary>
    /// 저장 데이터 구조
    /// v7.0: 확장 및 개선
    /// </summary>
    [System.Serializable]
    public class GameData
    {
        // 플레이어 데이터
        public Vector3 playerPosition;
        public string currentScene;

        // 체크포인트
        public string checkpointID;

        // 인벤토리
        public List<Inventory.InventoryItem> inventoryItems;

        // 퀘스트 (향후 확장)
        public List<string> completedQuestIDs;

        // 게임 설정
        public float masterVolume;
        public int qualityLevel;
        public bool isFullscreen;

        // 게임 통계 (향후 확장)
        public float playTime;
        public int totalCoinsCollected;
        public int totalDeaths;

        public GameData()
        {
            playerPosition = Vector3.zero;
            currentScene = "02_MainGame";
            checkpointID = "";
            inventoryItems = new List<Inventory.InventoryItem>();
            completedQuestIDs = new List<string>();
            masterVolume = 0.8f;
            qualityLevel = 2;
            isFullscreen = true;
            playTime = 0f;
            totalCoinsCollected = 0;
            totalDeaths = 0;
        }
    }
}