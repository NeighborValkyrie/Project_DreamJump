using UnityEngine;
using UnityEngine.UI;

namespace PlatformerGame.Systems.UI
{
    /// <summary>
    /// 타이틀 화면 UI 관리
    /// v7.0: 새로 추가됨
    /// </summary>
    public class TitleUIManager : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [Header("Panels")]
        [SerializeField] private GameObject settingsPanel;

        private void Start()
        {
            // 버튼 이벤트 연결
            if (newGameButton != null)
                newGameButton.onClick.AddListener(OnNewGame);

            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinue);

            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnOpenSettings);

            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuit);

            // 저장 파일 존재 여부에 따라 "이어하기" 버튼 활성화
            UpdateContinueButton();

            // 타이틀 BGM 재생
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.StopAllMusic();
                Audio.AudioManager.Instance.Play("TitleBGM");
            }

            // 설정 패널 숨기기
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        private void UpdateContinueButton()
        {
            if (continueButton != null && Save.SaveManager.Instance != null)
            {
                bool hasSaveFile = Save.SaveManager.Instance.SaveFileExists();
                continueButton.interactable = hasSaveFile;
            }
        }

        public void OnNewGame()
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.Play("UIClick");
            }

            if (Game.GameManager.Instance != null)
            {
                Game.GameManager.Instance.StartNewGame();
            }
        }

        public void OnContinue()
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.Play("UIClick");
            }

            // 저장 파일 로드
            if (Save.SaveManager.Instance != null)
            {
                Save.SaveManager.Instance.LoadGameAsync();
            }

            // 메인게임 씬으로 전환
            if (Scene.SceneController.Instance != null)
            {
                Scene.SceneController.Instance.LoadScene("02_Stage1");
            }

            if (Game.GameManager.Instance != null)
            {
                Game.GameManager.Instance.SetGameState(Events.GameState.Playing);
            }
        }

        public void OnOpenSettings()
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.Play("UIClick");
            }

            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
            }
        }

        public void OnCloseSettings()
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.Play("UIClick");
            }

            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        public void OnQuit()
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.Play("UIClick");
            }

            if (Game.GameManager.Instance != null)
            {
                Game.GameManager.Instance.QuitGame();
            }
        }
    }
}