using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace PlatformerGame.Systems.Scene
{
    /// <summary>
    /// 씬 전환 및 페이드 효과 관리
    /// v7.0: 새로 추가됨
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        public static SceneController Instance { get; private set; }

        [Header("Fade Settings")]
        [SerializeField] private Utilities.FadePanel fadePanel;
        [SerializeField] private float fadeDuration = 1f;

        private bool isLoading = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // FadePanel을 찾아서 할당 (씬에서 찾기)
            if (fadePanel == null)
            {
                fadePanel = FindObjectOfType<Utilities.FadePanel>();
            }
        }

        public void LoadScene(string sceneName)
        {
            if (isLoading)
            {
                Debug.LogWarning("[SceneController] 이미 씬을 로드 중입니다.");
                return;
            }

            StartCoroutine(LoadSceneAsync(sceneName));
        }

        public void ReloadCurrentScene()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            LoadScene(currentScene);
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            isLoading = true;

            // 이벤트 발생
            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.TriggerSceneLoadStarted(sceneName);
            }

            // 페이드 아웃
            if (fadePanel != null)
            {
                yield return StartCoroutine(fadePanel.FadeOut(fadeDuration));
            }
            else
            {
                yield return new WaitForSeconds(fadeDuration);
            }

            // 씬 로드
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            // 로드 진행도 체크
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }

            asyncLoad.allowSceneActivation = true;

            // 씬 활성화 대기
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // FadePanel을 다시 찾기 (새 씬에서)
            fadePanel = FindObjectOfType<Utilities.FadePanel>();

            // 페이드 인
            if (fadePanel != null)
            {
                yield return StartCoroutine(fadePanel.FadeIn(fadeDuration));
            }
            else
            {
                yield return new WaitForSeconds(fadeDuration);
            }

            // 이벤트 발생
            if (Events.GameEventManager.Instance != null)
            {
                Events.GameEventManager.Instance.TriggerSceneLoadCompleted(sceneName);
            }

            isLoading = false;

            Debug.Log($"[SceneController] 씬 로드 완료: {sceneName}");
        }

        public bool IsLoading() => isLoading;
    }
}