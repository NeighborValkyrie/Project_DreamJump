using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    [Header("Video Background")]
    public VideoPlayer videoPlayer;
    public RenderTexture renderTexture;
    public RawImage backgroundVideoImage;

    [Header("Background Music")]
    public AudioSource backgroundMusic;
    public float musicFadeOutDuration = 1f;

    [Header("Video Loop Settings")]
    public bool loopLastSeconds = true;
    public float loopDuration = 2f;

    private double videoLength;
    private double loopStartTime;
    private bool isInLoopMode = false;

    [Header("UI Elements")]
    public Image logoImage;
    public Button startGameButton;
    public Button exitGameButton;

    [Header("Animation Settings")]
    public float logoFadeInDelay = 4f;
    public float logoFadeInDuration = 2f;
    public float buttonFadeInDuration = 1.5f;
    public float buttonFadeDelay = 1f;

    [Header("Fluffy Cloud Animation")]
    public bool enableCloudAnimation = true;
    public float floatSpeed = 1f; // 둥둥 떠다니는 속도
    public float floatRange = 20f; // 위아래 움직임 범위
    public float scaleSpeed = 0.5f; // 크기 변화 속도
    public float scaleRange = 0.05f; // 크기 변화 범위 (0.95 ~ 1.05)

    private CanvasGroup logoCanvasGroup;
    private CanvasGroup startButtonCanvasGroup;
    private CanvasGroup exitButtonCanvasGroup;
    private Vector3 logoOriginalPosition;
    private Vector3 logoOriginalScale;

    void Start()
    {
        // 카메라 배경을 검은색으로 설정
        SetCameraToBlack();

        // 시작시 모든 UI 완전히 숨기기
        HideAllUI();

        // 배경음악 즉시 시작
        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }

        StartCoroutine(DelayedStart());
    }

    void SetCameraToBlack()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }

        if (mainCamera != null)
        {
            // 카메라 배경을 단색(검은색)으로 설정
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = Color.black;
        }
    }

    void HideAllUI()
    {
        // 로고 숨기기
        if (logoImage != null)
        {
            logoImage.gameObject.SetActive(false);
        }

        // 버튼들 숨기기
        if (startGameButton != null)
        {
            startGameButton.gameObject.SetActive(false);
        }

        if (exitGameButton != null)
        {
            exitGameButton.gameObject.SetActive(false);
        }

        // 배경 영상도 처음에는 숨기기
        if (backgroundVideoImage != null)
        {
            backgroundVideoImage.gameObject.SetActive(false);
        }
    }

    System.Collections.IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1f);

        // UI 요소들 다시 활성화
        ShowAllUI();

        SetupVideoBackground();
        SetupUI();
        StartTitleSequence();

        if (startGameButton != null)
        {
            startGameButton.onClick.RemoveAllListeners();
            startGameButton.onClick.AddListener(StartGame);
        }

        if (exitGameButton != null)
        {
            exitGameButton.onClick.RemoveAllListeners();
            exitGameButton.onClick.AddListener(ExitGame);
        }
    }

    void ShowAllUI()
    {
        // 배경 영상 먼저 보이기
        if (backgroundVideoImage != null)
        {
            backgroundVideoImage.gameObject.SetActive(true);
        }

        // 로고 보이기 (하지만 투명 상태)
        if (logoImage != null)
        {
            logoImage.gameObject.SetActive(true);
        }

        // 버튼들 보이기 (하지만 투명 상태)
        if (startGameButton != null)
        {
            startGameButton.gameObject.SetActive(true);
        }

        if (exitGameButton != null)
        {
            exitGameButton.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        // 비디오 루프 체크
        if (videoPlayer != null && videoPlayer.isPlaying && loopLastSeconds && isInLoopMode)
        {
            if (videoPlayer.time >= videoLength - 0.05f)
            {
                videoPlayer.time = loopStartTime;
            }
        }

        // 구름 애니메이션 (로고가 보일 때만)
        if (enableCloudAnimation && logoCanvasGroup != null && logoCanvasGroup.alpha > 0f)
        {
            CloudFloatAnimation();
        }
    }

    void CloudFloatAnimation()
    {
        if (logoImage == null) return;

        float time = Time.time;

        // 둥둥 떠다니는 효과 (Y축)
        float floatOffset = Mathf.Sin(time * floatSpeed) * floatRange;
        logoImage.transform.localPosition = logoOriginalPosition + Vector3.up * floatOffset;

        // 살짝 커졌다 작아지는 효과 (호흡하는 느낌)
        float scaleOffset = Mathf.Sin(time * scaleSpeed) * scaleRange;
        Vector3 newScale = logoOriginalScale + Vector3.one * scaleOffset;
        logoImage.transform.localScale = newScale;
    }

    void SetupVideoBackground()
    {
        if (videoPlayer != null && renderTexture != null && backgroundVideoImage != null)
        {
            renderTexture = new RenderTexture(1920, 1080, 0);
            renderTexture.format = RenderTextureFormat.ARGB32;

            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = renderTexture;
            videoPlayer.isLooping = false;
            videoPlayer.playOnAwake = false;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
            videoPlayer.waitForFirstFrame = true;
            videoPlayer.skipOnDrop = false;

            backgroundVideoImage.texture = renderTexture;

            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.loopPointReached += OnVideoLoopPointReached;

            videoPlayer.Prepare();
        }
    }

    void SetupUI()
    {
        if (logoImage != null)
        {
            logoCanvasGroup = logoImage.GetComponent<CanvasGroup>();
            if (logoCanvasGroup == null)
                logoCanvasGroup = logoImage.gameObject.AddComponent<CanvasGroup>();
            logoCanvasGroup.alpha = 0f;

            // 원래 위치와 크기 저장
            logoOriginalPosition = logoImage.transform.localPosition;
            logoOriginalScale = logoImage.transform.localScale;
        }

        if (startGameButton != null)
        {
            startButtonCanvasGroup = startGameButton.GetComponent<CanvasGroup>();
            if (startButtonCanvasGroup == null)
                startButtonCanvasGroup = startGameButton.gameObject.AddComponent<CanvasGroup>();
            startButtonCanvasGroup.alpha = 0f;
        }

        if (exitGameButton != null)
        {
            exitButtonCanvasGroup = exitGameButton.GetComponent<CanvasGroup>();
            if (exitButtonCanvasGroup == null)
                exitButtonCanvasGroup = exitGameButton.gameObject.AddComponent<CanvasGroup>();
            exitButtonCanvasGroup.alpha = 0f;
        }
    }

    void StartTitleSequence()
    {
        if (logoCanvasGroup != null)
        {
            StartCoroutine(FadeInWithDelay(logoCanvasGroup, logoFadeInDuration, logoFadeInDelay));
        }

        if (startButtonCanvasGroup != null)
        {
            float totalLogoTime = logoFadeInDelay + logoFadeInDuration;
            StartCoroutine(FadeInWithDelay(startButtonCanvasGroup, buttonFadeInDuration, totalLogoTime + buttonFadeDelay));
        }

        if (exitButtonCanvasGroup != null)
        {
            float totalLogoTime = logoFadeInDelay + logoFadeInDuration;
            StartCoroutine(FadeInWithDelay(exitButtonCanvasGroup, buttonFadeInDuration, totalLogoTime + buttonFadeDelay + 0.2f));
        }
    }

    System.Collections.IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    System.Collections.IEnumerator FadeInWithDelay(CanvasGroup canvasGroup, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(FadeIn(canvasGroup, duration));
    }

    public void StartGame()
    {
        // 배경음악 페이드아웃과 함께 씬 전환
        StartCoroutine(StartGameWithMusicFadeOut());
    }

    System.Collections.IEnumerator StartGameWithMusicFadeOut()
    {
        // 배경음악 페이드아웃
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            float startVolume = backgroundMusic.volume;
            float elapsed = 0f;

            while (elapsed < musicFadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / musicFadeOutDuration;
                backgroundMusic.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }

            backgroundMusic.volume = 0f;
            backgroundMusic.Stop();
        }

        // 씬 전환
        SceneManager.LoadScene("Stage 2");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        videoLength = vp.length;
        loopStartTime = videoLength - loopDuration;

        videoPlayer.time = 0;
        StartCoroutine(StartVideoFromBeginning());

        if (loopLastSeconds)
        {
            StartCoroutine(CheckVideoTime());
        }
    }

    System.Collections.IEnumerator StartVideoFromBeginning()
    {
        yield return new WaitForSeconds(0.1f);
        videoPlayer.time = 0;
        videoPlayer.Play();
    }

    void OnVideoLoopPointReached(VideoPlayer vp)
    {
        if (loopLastSeconds && !isInLoopMode)
        {
            isInLoopMode = true;
            videoPlayer.time = loopStartTime;
            videoPlayer.Play();
        }
    }

    System.Collections.IEnumerator CheckVideoTime()
    {
        while (videoPlayer != null && videoPlayer.isPlaying)
        {
            if (!isInLoopMode && videoPlayer.time >= loopStartTime)
            {
                isInLoopMode = true;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
        }

        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.loopPointReached -= OnVideoLoopPointReached;
        }
    }
}