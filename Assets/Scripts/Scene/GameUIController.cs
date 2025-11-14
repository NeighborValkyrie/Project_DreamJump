using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour
{
    [Header("Game UI Elements")]
    [SerializeField] private GameObject pauseMenuPanel;
    
    [Header("Pause Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button titleButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    
    [Header("Settings")]
    [SerializeField] private GameObject settingsPanel;
    
    [Header("Scene Names")]
    [SerializeField] private string titleSceneName = "Title";
    [SerializeField] private string currentSceneName;
    
    private bool isPaused = false;
    
    // 이벤트 - 다른 스크립트에서 구독할 수 있음
    public delegate void GameEvent();
    public static event GameEvent OnPlayerRespawnRequested;
    
    private void Start()
    {
        // 현재 씬 이름 저장
        currentSceneName = SceneManager.GetActiveScene().name;
        
        // 초기 UI 설정
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        // 버튼 이벤트 연결
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (titleButton != null) titleButton.onClick.AddListener(ReturnToTitle);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
    }
    
    private void Update()
    {
        // ESC 키로 일시정지 메뉴 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }
    
    // ========================================
    // 일시정지 메뉴
    // ========================================
    
    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(isPaused);
        }
        
        Time.timeScale = isPaused ? 0f : 1f;
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;   /*[변경가능_일시정즘_커서상태]*/
            Cursor.visible = true;                    /*[변경가능_일시정지_커서표시]*/
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; /*[변경가능_플레이중_커서상태]*/
            Cursor.visible = false;                   /*[변경가능_플레이중_커서표시]*/
        }
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked; /*[변경가능_플레이중_커서상태]*/
        Cursor.visible = false;                   /*[변경가능_플레이중_커서표시]*/
    }
    
    public void RestartGame()
    {
        isPaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked; /*[변경가능_플레이중_커서상태]*/
        Cursor.visible = false;                   /*[변경가능_플레이중_커서표시]*/
        OnPlayerRespawnRequested?.Invoke();
    }
    
    public void ReturnToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(titleSceneName);
    }
    
    // ========================================
    // 설정
    // ========================================
    
    public void OpenSettings()
    {
        // 게임 메뉴(일시정지 패널)는 끄고
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // 설정 패널만 켜기
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        // 설정 패널 끄고
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // 일시정지 메뉴 다시 보이게
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}