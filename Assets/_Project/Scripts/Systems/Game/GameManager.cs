using UnityEngine;

namespace PlatformerGame.Systems.Game
{
    /// <summary>
    /// 게임 상태 및 흐름 제어
    /// v7.0: 새로 추가됨
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private Events.GameState currentGameState = Events.GameState.MainMenu;

        private bool isPaused = false;

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

        private void Start()
        {
            // 초기 상태 설정
            SetGameState(Events.GameState.MainMenu);
        }

        private void Update()
        {
            // ESC 키로 일시정지 토글 (Playing 상태일 때만)
            if (Input.GetKeyDown(KeyCode.Escape) && currentGameState == Events.GameState.Playing)
            {
                if (isPaused)
                    ResumeGame();
                else
                    PauseGame();
            }
        }

        public void SetGameState(Events.GameState newState)
        {
            currentGameState = newState;

            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.TriggerGameStateChanged(newState);
            }

            Debug.Log($"[GameManager] 게임 상태 변경: {newState}");
        }

        public void PauseGame()
        {
            if (isPaused) return;

            isPaused = true;
            Time.timeScale = 0f;

            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.TriggerGamePaused();
            }

            Debug.Log("[GameManager] 게임 일시정지");
        }

        public void ResumeGame()
        {
            if (!isPaused) return;

            isPaused = false;
            Time.timeScale = 1f;

            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.TriggerGameResumed();
            }

            Debug.Log("[GameManager] 게임 재개");
        }

        public void StartNewGame()
        {
            // 저장 데이터 초기화
            if (Save.SaveManager.Instance != null)
            {
                Save.SaveManager.Instance.NewGame();
            }

            // 메인게임 씬으로 전환
            if (Scene.SceneController.Instance != null)
            {
                Scene.SceneController.Instance.LoadScene("02_MainGame");
            }

            SetGameState(Events.GameState.Playing);
        }

        public void GameOver()
        {
            SetGameState(Events.GameState.GameOver);
            PauseGame();
        }

        public void Victory()
        {
            SetGameState(Events.GameState.Victory);
            PauseGame();
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        public Events.GameState GetCurrentState() => currentGameState;
        public bool IsPaused() => isPaused;
    }
}