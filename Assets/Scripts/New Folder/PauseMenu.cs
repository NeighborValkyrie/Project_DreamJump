// Assets/Scripts/New Folder/PauseMenu.cs

using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;  // 새 Input System
#endif

public class PauseMenu : MonoBehaviour
{
    [Header("UI 루트 (보였다/숨겼다 할 패널)")]
    [SerializeField] private GameObject pauseUI;

    [Header("플레이어 (새로하기/리스폰용)")]
    [SerializeField] private GameObject player; // 비워두면 Tag=Player 검색

    [Header("씬 이름")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("플레이 상태 커서")]
    [SerializeField] private bool lockCursorWhenPlaying = true;
    [SerializeField] private CursorLockMode playLockMode = CursorLockMode.Locked;

    private bool isPaused;
    private float prevTimeScale = 1f;
    private CursorLockMode prevLockMode;
    private bool prevCursorVisible;

    private void Awake()
    {
        if (!player)
        {
            var tagged = GameObject.FindGameObjectWithTag("Player");
            if (tagged) player = tagged;
        }

        if (pauseUI) pauseUI.SetActive(false);
        isPaused = false;
        ApplyCursorPlayingState();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (isPaused) Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
#else
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
#endif
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!isPaused && focus)
            ApplyCursorPlayingState();
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;

        prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        AudioListener.pause = true;

        if (pauseUI) pauseUI.SetActive(true);

        // 커서 노출/자유화
        prevLockMode = Cursor.lockState;
        prevCursorVisible = Cursor.visible;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;

        Time.timeScale = prevTimeScale <= 0f ? 1f : prevTimeScale;
        AudioListener.pause = false;

        if (pauseUI) pauseUI.SetActive(false);

        // UI가 사라질 때 커서 잠금/숨김 보장
        if (lockCursorWhenPlaying)
        {
            Cursor.lockState = playLockMode;                 // Locked 권장
            Cursor.visible = (playLockMode == CursorLockMode.None);
        }
        else
        {
            Cursor.lockState = prevLockMode;
            Cursor.visible = prevCursorVisible;
        }
    }

    private void ApplyCursorPlayingState()
    {
        if (lockCursorWhenPlaying)
        {
            Cursor.lockState = playLockMode;
            Cursor.visible = (playLockMode == CursorLockMode.None);
        }
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        if (isPaused) Resume(); // 씬 바뀔 때 정리
    }

    // ===== 버튼용 메서드 =====

    // 계속하기: ESC와 동일(토글) → 결과적으로 Resume
    public void Btn_Continue()
    {
        TogglePause();
    }

    // 새로하기: 현재 체크포인트로 리스폰(RespawnManager 있으면 사용)
    public void Btn_Respawn()
    {
        if (isPaused) Resume();

        var target = player ? player : GameObject.FindGameObjectWithTag("Player");
        if (!target)
        {
            Debug.LogWarning("[PauseMenu] Player가 설정되지 않았습니다.");
            return;
        }

        if (RespawnManager.Instance != null)
        {
            RespawnManager.Instance.Respawn(target);
        }
        else
        {
            // 폴백: Respawn 태그가 있으면 그 위치, 없으면 씬 리로드
            var rp = GameObject.FindGameObjectWithTag("Respawn");
            if (rp)
            {
                var rb = target.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.position = rp.transform.position;
                    rb.rotation = rp.transform.rotation;
                }
                else
                {
                    target.transform.SetPositionAndRotation(rp.transform.position, rp.transform.rotation);
                }
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        ApplyCursorPlayingState();
    }

    // 메인 메뉴로
    public void Btn_MainMenu()
    {
        if (string.IsNullOrEmpty(mainMenuSceneName))
        {
            Debug.LogWarning("[PauseMenu] mainMenuSceneName을 설정하세요.");
            return;
        }

        if (isPaused) { Time.timeScale = 1f; AudioListener.pause = false; }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    // 종료
    public void Btn_Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }
}
