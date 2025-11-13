using UnityEngine;
using UnityEngine.UI;
using PlatformerGame.Systems.Scene;

namespace PlatformerGame.Systems.UI
{
    /// <summary>
    /// 타이틀 씬 UI 관리
    /// 게임 시작, 설정, 종료 버튼 등을 관리합니다.
    /// </summary>
    public class TitleUIManager : MonoBehaviour
    {
        [Header("UI Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [Header("Scene Settings")]
        [SerializeField] private string gameSceneName = "02_MainGame";
        [SerializeField] private string storySceneName = "03_StoryScene"; // 나중에 추가될 스토리 씬

        [Header("Settings")]
        [SerializeField] private bool useStoryScene = false; // 스토리 씬 사용 여부

        private void Start()
        {
            SetupButtons();
        }

        private void SetupButtons()
        {
            // 게임 시작 버튼
            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStartButtonClicked);
            }
            else
            {
                Debug.LogWarning("[TitleUIManager] Start Button이 할당되지 않았습니다!");
            }

            // 설정 버튼
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            }

            // 종료 버튼
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitButtonClicked);
            }
        }

        /// <summary>
        /// 게임 시작 버튼 클릭
        /// </summary>
        private void OnStartButtonClicked()
        {
            Debug.Log("[TitleUIManager] 게임 시작!");

            // 스토리 씬을 사용하는 경우
            if (useStoryScene)
            {
                LoadScene(storySceneName);
            }
            else
            {
                LoadScene(gameSceneName);
            }
        }

        /// <summary>
        /// 설정 버튼 클릭
        /// </summary>
        private void OnSettingsButtonClicked()
        {
            Debug.Log("[TitleUIManager] 설정 열기 - 추후 구현 예정");
            // TODO: 설정 패널 열기
        }

        /// <summary>
        /// 종료 버튼 클릭
        /// </summary>
        private void OnQuitButtonClicked()
        {
            Debug.Log("[TitleUIManager] 게임 종료");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        /// <summary>
        /// 씬 로드
        /// </summary>
        private void LoadScene(string sceneName)
        {
            if (SceneController.Instance != null)
            {
                SceneController.Instance.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("[TitleUIManager] SceneController가 없습니다!");
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            }
        }

        /// <summary>
        /// 스토리 씬 사용 여부 설정 (나중에 추가될 때 사용)
        /// </summary>
        public void SetUseStoryScene(bool use)
        {
            useStoryScene = use;
        }
    }
}
