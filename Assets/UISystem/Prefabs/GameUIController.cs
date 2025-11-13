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
    
    [Header("Guide Message Animation")]
    [SerializeField] private float fadeInDuration = 0.12f;
    [SerializeField] private float holdDuration = 0.80f;
    [SerializeField] private float fadeOutDuration = 0.25f;
    [SerializeField] private Vector2 moveOffset = new Vector2(0f, 40f);
    
    private bool isPaused = false;
    private Coroutine guideMessageCoroutine;
    private RectTransform guideMessageRect;
    private CanvasGroup guideMessageCanvasGroup;
    private Vector2 guideMessageBasePosition;
    
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
        InitializeGuideMessage();
        
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
            Debug.Log("[GameUIController] ESC 키 감지!");
            TogglePauseMenu();
        }
    }
    
    // ========================================
    // 가이드 메시지 시스템
    // ========================================
    
    /// <summary>
    /// 가이드 메시지 초기화 (CanvasGroup 자동 추가)
    /// </summary>
    private void InitializeGuideMessage()
    {
        if (guideMessagePanel != null)
        {
            guideMessagePanel.SetActive(false);
            
            // RectTransform 가져오기
            guideMessageRect = guideMessagePanel.GetComponent<RectTransform>();
            if (guideMessageRect != null)
            {
                guideMessageBasePosition = guideMessageRect.anchoredPosition;
            }
            
            // CanvasGroup 가져오거나 추가
            guideMessageCanvasGroup = guideMessagePanel.GetComponent<CanvasGroup>();
            if (guideMessageCanvasGroup == null)
            {
                guideMessageCanvasGroup = guideMessagePanel.AddComponent<CanvasGroup>();
                Debug.Log("[GameUIController] CanvasGroup을 guideMessagePanel에 자동으로 추가했습니다.");
            }
            
            // 초기 상태 설정
            guideMessageCanvasGroup.alpha = 0f;
            guideMessageCanvasGroup.interactable = false;
            guideMessageCanvasGroup.blocksRaycasts = false;
        }
    }
    
    /// <summary>
    /// 가이드 메시지 표시 (체크포인트 스타일 애니메이션)
    /// </summary>
    public void ShowGuideMessage(string message, float duration = 0f)
    {
        if (string.IsNullOrEmpty(message))
        {
            Debug.LogWarning("[GameUIController] 메시지가 비어있습니다.");
            return;
        }
        
        if (guideMessagePanel == null || guideMessageText == null)
        {
            Debug.LogError("[GameUIController] Guide Message Panel 또는 Text가 할당되지 않았습니다!");
            return;
        }
        
        // 기존 코루틴 정지
        if (guideMessageCoroutine != null)
        {
            StopCoroutine(guideMessageCoroutine);
        }
        
        // 메시지 설정
        guideMessageText.text = message;
        
        // 애니메이션 시작
        guideMessageCoroutine = StartCoroutine(ShowGuideMessageCoroutine());
    }
    
    /// <summary>
    /// 가이드 메시지 애니메이션 코루틴 (체크포인트 스타일)
    /// </summary>
    private System.Collections.IEnumerator ShowGuideMessageCoroutine()
    {
        // 패널 활성화
        guideMessagePanel.SetActive(true);
        
        // 초기 위치 설정
        if (guideMessageRect != null)
        {
            guideMessageRect.anchoredPosition = guideMessageBasePosition;
        }
        
        if (guideMessageCanvasGroup != null)
        {
            guideMessageCanvasGroup.alpha = 0f;
            guideMessageCanvasGroup.interactable = false;
            guideMessageCanvasGroup.blocksRaycasts = false;
        }
        
        float elapsedTime;
        
        // ===== Fade In =====
        elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // 일시정지 중에도 작동!
            float progress = Mathf.Clamp01(elapsedTime / Mathf.Max(0.0001f, fadeInDuration));
            
            if (guideMessageCanvasGroup != null)
            {
                guideMessageCanvasGroup.alpha = progress;
            }
            
            if (guideMessageRect != null)
            {
                guideMessageRect.anchoredPosition = guideMessageBasePosition + moveOffset * progress * 0.5f;
            }
            
            yield return null;
        }
        
        // ===== Hold =====
        if (guideMessageCanvasGroup != null)
        {
            guideMessageCanvasGroup.alpha = 1f;
        }
        
        if (guideMessageRect != null)
        {
            guideMessageRect.anchoredPosition = guideMessageBasePosition + moveOffset * 0.5f;
        }
        
        elapsedTime = 0f;
        while (elapsedTime < holdDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        
        // ===== Fade Out =====
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = 1f - Mathf.Clamp01(elapsedTime / Mathf.Max(0.0001f, fadeOutDuration));
            
            if (guideMessageCanvasGroup != null)
            {
                guideMessageCanvasGroup.alpha = progress;
            }
            
            if (guideMessageRect != null)
            {
                guideMessageRect.anchoredPosition = guideMessageBasePosition + moveOffset * (0.5f + (1f - progress) * 0.5f);
            }
            
            yield return null;
        }
        
        // 정리
        if (guideMessageCanvasGroup != null)
        {
            guideMessageCanvasGroup.alpha = 0f;
            guideMessageCanvasGroup.interactable = false;
            guideMessageCanvasGroup.blocksRaycasts = false;
        }
        
        if (guideMessageRect != null)
        {
            guideMessageRect.anchoredPosition = guideMessageBasePosition;
        }
        
        guideMessagePanel.SetActive(false);
        guideMessageCoroutine = null;
    }
    
    /// <summary>
    /// 체크포인트 메시지 표시 (ShowGuideMessage와 동일)
    /// </summary>
    public void ShowCheckpointMessage(string message)
    {
        ShowGuideMessage(message);
    }
    
    // ========================================
    // 일시정지 메뉴
    // ========================================
    
    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        
        Debug.Log($"[GameUIController] isPaused = {isPaused}");
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(isPaused);
            Debug.Log($"[GameUIController] pauseMenuPanel 활성화: {isPaused}");
        }
        else
        {
            Debug.LogError("[GameUIController] pauseMenuPanel이 null입니다!");
        }
        
        // 시간 조절
        Time.timeScale = isPaused ? 0f : 1f;
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    
    public void RestartGame()
    {
        isPaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        
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
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }
    
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
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
