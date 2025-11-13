using UnityEngine;
using UnityEngine.UI;
using PlatformerGame.Systems.Scene;
using PlatformerGame.Systems.Audio;
using PlatformerGame.Systems.Events;

namespace PlatformerGame.Systems.UI
{
    /// <summary>
    /// 게임 씬 일시정지 및 설정 UI 관리
    /// ESC 키로 일시정지하고 설정 패널을 엽니다.
    /// v7.1: BGM 일시정지 기능 및 GameEventManager 연동 추가
    /// </summary>
    public class GamePauseUIManager : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Pause Panel Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button titleButton;
        [SerializeField] private Button quitButton;

        [Header("Settings Panel Buttons")]
        [SerializeField] private Button closeSettingsButton;

        [Header("Settings")]
        [SerializeField] private string titleSceneName = "01_TitleScene";
        [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

        private bool isPaused = false;

        private void Start()
        {
            SetupButtons();
            HidePausePanel();
            HideSettingsPanel();
        }

        private void Update()
        {
            // ESC 키로 일시정지 토글
            if (Input.GetKeyDown(pauseKey))
            {
                if (isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        private void SetupButtons()
        {
            // 계속하기 버튼
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(Resume);
            }

            // 설정 버튼
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OpenSettings);
            }

            // 타이틀로 버튼
            if (titleButton != null)
            {
                titleButton.onClick.AddListener(GoToTitle);
            }

            // 종료 버튼
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(QuitGame);
            }

            // 설정 닫기 버튼
            if (closeSettingsButton != null)
            {
                closeSettingsButton.onClick.AddListener(CloseSettings);
            }
        }

        /// <summary>
        /// 게임 일시정지
        /// </summary>
        public void Pause()
        {
            if (isPaused) return;

            isPaused = true;
            Time.timeScale = 0f;
            ShowPausePanel();

            // BGM 일시정지
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PauseBGM();
            }

            // 이벤트 발생
            if (GameEventManager.Instance != null)
            {
                GameEventManager.Instance.TriggerGamePaused();
            }

            Debug.Log("[GamePauseUIManager] 게임 일시정지");
        }

        /// <summary>
        /// 게임 재개
        /// </summary>
        public void Resume()
        {
            if (!isPaused) return;

            isPaused = false;
            Time.timeScale = 1f;
            HidePausePanel();
            HideSettingsPanel();

            // BGM 재개
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.UnpauseBGM();
            }

            // 이벤트 발생
            if (GameEventManager.Instance != null)
            {
                GameEventManager.Instance.TriggerGameResumed();
            }

            Debug.Log("[GamePauseUIManager] 게임 재개");
        }

        /// <summary>
        /// 설정 패널 열기
        /// </summary>
        private void OpenSettings()
        {
            HidePausePanel();
            ShowSettingsPanel();

            Debug.Log("[GamePauseUIManager] 설정 열기");
        }

        /// <summary>
        /// 설정 패널 닫기
        /// </summary>
        private void CloseSettings()
        {
            HideSettingsPanel();
            ShowPausePanel();

            Debug.Log("[GamePauseUIManager] 설정 닫기");
        }

        /// <summary>
        /// 타이틀로 돌아가기
        /// </summary>
        private void GoToTitle()
        {
            // 게임 재개 (TimeScale 복원)
            Time.timeScale = 1f;
            isPaused = false;

            Debug.Log("[GamePauseUIManager] 타이틀로 이동");

            if (SceneController.Instance != null)
            {
                SceneController.Instance.LoadScene(titleSceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(titleSceneName);
            }
        }

        /// <summary>
        /// 게임 종료
        /// </summary>
        private void QuitGame()
        {
            Debug.Log("[GamePauseUIManager] 게임 종료");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        private void ShowPausePanel()
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }
        }

        private void HidePausePanel()
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }

        private void ShowSettingsPanel()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
            }
        }

        private void HideSettingsPanel()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        public bool IsPaused() => isPaused;
    }
}
