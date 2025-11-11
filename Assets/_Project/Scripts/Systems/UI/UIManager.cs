using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PlatformerGame.Systems.UI
{
    /// <summary>
    /// 메인게임 UI 통합 관리
    /// v7.0: 일시정지 메뉴 추가
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI interactionPrompt;

        [Header("Panels")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private GameObject questPanel;

        [Header("Fade")]
        [SerializeField] private Utilities.FadePanel fadePanel;

        private bool isInventoryOpen = false;
        private bool isPauseMenuOpen = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            HideAllPanels();

            // 이벤트 구독
            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.OnGamePaused += ShowPauseMenu;
                Events.GameEventManager.Instance.OnGameResumed += HidePauseMenu;
                Events.GameEventManager.Instance.OnItemCollected += UpdateCoinDisplay;
            }
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.OnGamePaused -= ShowPauseMenu;
                Events.GameEventManager.Instance.OnGameResumed -= HidePauseMenu;
                Events.GameEventManager.Instance.OnItemCollected -= UpdateCoinDisplay;
            }
        }

        private void Update()
        {
            // 인벤토리 토글 (I 키)
            if (Input.GetKeyDown(KeyCode.I) && !isPauseMenuOpen)
            {
                ToggleInventory();
            }
        }

        // HUD 업데이트
        public void UpdateCoinDisplay(string itemID, int quantity)
        {
            if (itemID == "coin" && coinText != null)
            {
                int currentCoins = Inventory.InventoryManager.Instance.GetItemQuantity("coin");
                coinText.text = $"코인: {currentCoins}";
            }
        }

        public void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            if (healthBar != null)
            {
                healthBar.value = currentHealth / maxHealth;
            }
        }

        public void ShowInteractionPrompt(string text)
        {
            if (interactionPrompt != null)
            {
                interactionPrompt.gameObject.SetActive(true);
                interactionPrompt.text = text;
            }
        }

        public void HideInteractionPrompt()
        {
            if (interactionPrompt != null)
            {
                interactionPrompt.gameObject.SetActive(false);
            }
        }

        // 인벤토리
        public void ToggleInventory()
        {
            isInventoryOpen = !isInventoryOpen;

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(isInventoryOpen);
            }

            if (isInventoryOpen)
            {
                Events.GameEventManager.Instance?.TriggerInventoryOpened();
                Events.GameEventManager.Instance?.TriggerPlayerInputStateChanged(false);
            }
            else
            {
                Events.GameEventManager.Instance?.TriggerInventoryClosed();
                Events.GameEventManager.Instance?.TriggerPlayerInputStateChanged(true);
            }
        }

        // 일시정지 메뉴
        private void ShowPauseMenu()
        {
            isPauseMenuOpen = true;

            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            }

            Events.GameEventManager.Instance?.TriggerPlayerInputStateChanged(false);
        }

        private void HidePauseMenu()
        {
            isPauseMenuOpen = false;

            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }

            Events.GameEventManager.Instance?.TriggerPlayerInputStateChanged(true);
        }

        // 일시정지 메뉴 버튼 이벤트
        public void OnResumeGame()
        {
            if (Game.GameManager.Instance != null)
            {
                Game.GameManager.Instance.ResumeGame();
            }

            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.Play("UIClick");
            }
        }

        public void OnOpenSettings()
        {
            // 설정 패널 열기 (SettingsUIManager 사용)
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.Play("UIClick");
            }
        }

        public void OnBackToTitle()
        {
            // 저장 후 타이틀로
            if (Save.SaveManager.Instance != null)
            {
                Save.SaveManager.Instance.SaveGameAsync();
            }

            if (Game.GameManager.Instance != null)
            {
                Game.GameManager.Instance.ResumeGame(); // Time.timeScale 복원
            }

            if (Scene.SceneController.Instance != null)
            {
                Scene.SceneController.Instance.LoadScene("01_TitleScene");
            }

            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.Play("UIClick");
            }
        }

        public void OnQuitGame()
        {
            if (Game.GameManager.Instance != null)
            {
                Game.GameManager.Instance.QuitGame();
            }
        }

        // 대화 패널
        public void ShowDialoguePanel()
        {
            HideAllPanels();

            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }
        }

        public void HideDialoguePanel()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }

        // 퀘스트 패널
        public void ShowQuestPanel()
        {
            if (questPanel != null)
            {
                questPanel.SetActive(true);
            }
        }

        public void HideQuestPanel()
        {
            if (questPanel != null)
            {
                questPanel.SetActive(false);
            }
        }

        // 전체 패널 숨기기
        private void HideAllPanels()
        {
            if (inventoryPanel != null) inventoryPanel.SetActive(false);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            if (questPanel != null) questPanel.SetActive(false);
        }
    }
}