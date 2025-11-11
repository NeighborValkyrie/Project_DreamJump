using UnityEngine;

namespace PlatformerGame.Systems.Events
{
    /// <summary>
    /// 전역 이벤트 허브 - 모든 시스템 간 통신
    /// v7.0: 씬 전환 및 인벤토리 이벤트 추가
    /// </summary>
    public class GameEventManager : MonoBehaviour
    {
        public static GameEventManager Instance { get; private set; }

        // 게임 상태 이벤트
        public event System.Action<GameState> OnGameStateChanged;
        public event System.Action OnGamePaused;
        public event System.Action OnGameResumed;

        // 씬 전환 이벤트
        public event System.Action<string> OnSceneLoadStarted;
        public event System.Action<string> OnSceneLoadCompleted;

        // 플레이어 이벤트
        public event System.Action<bool> OnPlayerInputStateChanged;
        public event System.Action OnPlayerJumped;
        public event System.Action<Vector3> OnPlayerDied;
        public event System.Action<Vector3, string> OnPlayerRespawned;

        // 아이템 이벤트
        public event System.Action<string, int> OnItemCollected;
        public event System.Action<string, int> OnItemUsed;

        // 인벤토리 이벤트
        public event System.Action OnInventoryOpened;
        public event System.Action OnInventoryClosed;

        // 체크포인트 이벤트
        public event System.Action<Vector3, string> OnCheckpointReached;

        // UI 이벤트
        public event System.Action<string> OnNotificationRequested;

        // 대화 이벤트
        public event System.Action OnDialogueStarted;
        public event System.Action OnDialogueEnded;

        // 퀘스트 이벤트
        public event System.Action<string> OnQuestStarted;
        public event System.Action<string> OnQuestCompleted;

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

        // 게임 상태 트리거
        public void TriggerGameStateChanged(GameState newState)
        {
            OnGameStateChanged?.Invoke(newState);
        }

        public void TriggerGamePaused()
        {
            OnGamePaused?.Invoke();
        }

        public void TriggerGameResumed()
        {
            OnGameResumed?.Invoke();
        }

        // 씬 전환 트리거
        public void TriggerSceneLoadStarted(string sceneName)
        {
            OnSceneLoadStarted?.Invoke(sceneName);
        }

        public void TriggerSceneLoadCompleted(string sceneName)
        {
            OnSceneLoadCompleted?.Invoke(sceneName);
        }

        // 플레이어 트리거
        public void TriggerPlayerInputStateChanged(bool enabled)
        {
            OnPlayerInputStateChanged?.Invoke(enabled);
        }

        public void TriggerPlayerJumped()
        {
            OnPlayerJumped?.Invoke();
        }

        public void TriggerPlayerDied(Vector3 deathPosition)
        {
            OnPlayerDied?.Invoke(deathPosition);
        }

        public void TriggerPlayerRespawned(Vector3 respawnPosition, string checkpointID)
        {
            OnPlayerRespawned?.Invoke(respawnPosition, checkpointID);
        }

        // 아이템 트리거
        public void TriggerItemCollected(string itemID, int quantity)
        {
            OnItemCollected?.Invoke(itemID, quantity);
        }

        public void TriggerItemUsed(string itemID, int quantity)
        {
            OnItemUsed?.Invoke(itemID, quantity);
        }

        // 인벤토리 트리거
        public void TriggerInventoryOpened()
        {
            OnInventoryOpened?.Invoke();
        }

        public void TriggerInventoryClosed()
        {
            OnInventoryClosed?.Invoke();
        }

        // 체크포인트 트리거
        public void TriggerCheckpointReached(Vector3 position, string checkpointID)
        {
            OnCheckpointReached?.Invoke(position, checkpointID);
        }

        // UI 트리거
        public void TriggerNotificationRequested(string message)
        {
            OnNotificationRequested?.Invoke(message);
        }

        // 대화 트리거
        public void TriggerDialogueStarted()
        {
            OnDialogueStarted?.Invoke();
        }

        public void TriggerDialogueEnded()
        {
            OnDialogueEnded?.Invoke();
        }

        // 퀘스트 트리거
        public void TriggerQuestStarted(string questID)
        {
            OnQuestStarted?.Invoke(questID);
        }

        public void TriggerQuestCompleted(string questID)
        {
            OnQuestCompleted?.Invoke(questID);
        }
    }

    /// <summary>
    /// 게임 상태 열거형
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory
    }
}