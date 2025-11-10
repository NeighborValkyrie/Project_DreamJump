using UnityEngine;
using System.IO;
using System.Threading.Tasks;

namespace PlatformerGame.Systems.Save
{
    /// <summary>
    /// 비동기 저장/로드 시스템
    /// v7.0: 개선 및 확장
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        [Header("Save Settings")]
        [SerializeField] private string saveFileName = "savegame.json";

        private string saveFilePath;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
                Debug.Log($"[SaveManager] 저장 경로: {saveFilePath}");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public async void SaveGameAsync()
        {
            await Task.Run(() => SaveGame());
        }

        public async void LoadGameAsync()
        {
            await Task.Run(() => LoadGame());
        }

        private void SaveGame()
        {
            GameData data = GatherGameData();

            string json = JsonUtility.ToJson(data, true);

            try
            {
                File.WriteAllText(saveFilePath, json);
                Debug.Log("[SaveManager] 게임 저장 완료");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveManager] 저장 실패: {e.Message}");
            }
        }

        private void LoadGame()
        {
            if (!File.Exists(saveFilePath))
            {
                Debug.LogWarning("[SaveManager] 저장 파일이 없습니다.");
                return;
            }

            try
            {
                string json = File.ReadAllText(saveFilePath);
                GameData data = JsonUtility.FromJson<GameData>(json);

                ApplyGameData(data);

                Debug.Log("[SaveManager] 게임 로드 완료");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveManager] 로드 실패: {e.Message}");
            }
        }

        private GameData GatherGameData()
        {
            GameData data = new GameData();

            // 플레이어 위치
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                data.playerPosition = player.transform.position;
            }

            // 현재 씬
            data.currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            // 인벤토리
            if (Inventory.InventoryManager.Instance != null)
            {
                data.inventoryItems = Inventory.InventoryManager.Instance.GetItemList();
            }

            // 체크포인트
            if (Respawn.RespawnManager.Instance != null)
            {
                data.checkpointID = Respawn.RespawnManager.Instance.GetLastCheckpointID();
            }

            // 게임 설정
            if (Game.GameSettings.Instance != null)
            {
                data.masterVolume = Game.GameSettings.Instance.GetMasterVolume();
                data.qualityLevel = Game.GameSettings.Instance.GetQualityLevel();
                data.isFullscreen = Game.GameSettings.Instance.IsFullscreen();
            }

            return data;
        }

        private void ApplyGameData(GameData data)
        {
            // 인벤토리 로드
            if (Inventory.InventoryManager.Instance != null)
            {
                Inventory.InventoryManager.Instance.LoadItems(data.inventoryItems);
            }

            // 체크포인트 로드
            if (Respawn.RespawnManager.Instance != null)
            {
                Respawn.RespawnManager.Instance.SetLastCheckpoint(data.checkpointID);
            }

            // 게임 설정 로드
            if (Game.GameSettings.Instance != null)
            {
                Game.GameSettings.Instance.SetMasterVolume(data.masterVolume);
                Game.GameSettings.Instance.SetQualityLevel(data.qualityLevel);
                Game.GameSettings.Instance.SetFullscreen(data.isFullscreen);
            }

            // 플레이어 위치는 씬 로드 후 적용
        }

        public void NewGame()
        {
            // 인벤토리 초기화
            if (Inventory.InventoryManager.Instance != null)
            {
                Inventory.InventoryManager.Instance.ClearInventory();
            }

            // 체크포인트 초기화
            if (Respawn.RespawnManager.Instance != null)
            {
                Respawn.RespawnManager.Instance.ResetCheckpoints();
            }

            Debug.Log("[SaveManager] 새 게임 시작");
        }

        public bool SaveFileExists()
        {
            return File.Exists(saveFilePath);
        }

        public void DeleteSaveFile()
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
                Debug.Log("[SaveManager] 저장 파일 삭제 완료");
            }
        }
    }
}