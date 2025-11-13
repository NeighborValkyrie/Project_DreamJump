using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TitleUIController : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button quitButton;
    
    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject controlsPanel;
    
    [Header("Scene Names")]
    [SerializeField] private string firstLevelScene = "Stage1";
    
    private GameObject currentActivePanel;
    
    private void Start()
    {
        // 버튼 이벤트 연결
        if (startButton != null) startButton.onClick.AddListener(StartGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (controlsButton != null) controlsButton.onClick.AddListener(OpenControls);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
        
        // 패널 초기화
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
    }
    
    // 게임 시작 - Stage1 씬으로 전환
    public void StartGame()
    {
        SceneManager.LoadScene(firstLevelScene);
    }
    
    // 설정 패널 열기
    public void OpenSettings()
    {
        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
        }
        
        settingsPanel.SetActive(true);
        currentActivePanel = settingsPanel;
    }
    
    // 조작방법 패널 열기
    public void OpenControls()
    {
        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
        }
        
        controlsPanel.SetActive(true);
        currentActivePanel = controlsPanel;
    }
    
    // 열려있는 패널 닫기
    public void ClosePanel()
    {
        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
            currentActivePanel = null;
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
}