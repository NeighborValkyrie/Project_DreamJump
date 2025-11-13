using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour
{
    [Header("Game UI Elements")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject guideMessagePanel;
    [SerializeField] private TextMeshProUGUI guideMessageText;
    
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
        
        // 가이드 메시지 초기화
        if (guideMessagePanel != null)
        {
            guideMessagePanel.SetActive(false);
        }
        
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
    
    // 일시정지 메뉴 토글
    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(isPaused);
        }
        
        // 시간 조절
        Time.timeScale = isPaused ? 0f : 1f;
    }
    
    // 게임 계속하기
    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    
    // 게임 다시하기 (리스폰)
    public void RestartGame()
    {
        // 일시정지 상태 해제
        isPaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        
        // 리스폰 요청 이벤트 발생
        OnPlayerRespawnRequested?.Invoke();
        
        // function.cs가 이 이벤트를 구독하여 처리하도록 함
    }
    
    // 타이틀로 돌아가기
    public void ReturnToTitle()
    {
        // 일시정지 상태 해제
        Time.timeScale = 1f;
        
        // 타이틀 씬으로 로드
        SceneManager.LoadScene(titleSceneName);
    }
    
    // 설정 메뉴 열기
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }
    
    // 설정 메뉴 닫기
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
    
    // 게임 종료
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    // 가이드 메시지 표시
    public void ShowGuideMessage(string message, float duration = 3f)
    {
        StartCoroutine(ShowGuideMessageCoroutine(message, duration));
    }
    
    // 가이드 메시지 코루틴
    private System.Collections.IEnumerator ShowGuideMessageCoroutine(string message, float duration)
    {
        if (guideMessagePanel != null && guideMessageText != null)
        {
            guideMessageText.text = message;
            guideMessagePanel.SetActive(true);
            
            yield return new WaitForSeconds(duration);
            
            guideMessagePanel.SetActive(false);
        }
    }
}