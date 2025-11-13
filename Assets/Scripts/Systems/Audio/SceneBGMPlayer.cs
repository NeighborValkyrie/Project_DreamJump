using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlatformerGame.Systems.Audio
{
    /// <summary>
    /// 씬 로드 시 자동으로 배경음악 재생
    /// 각 씬에 배치하여 사용합니다.
    /// </summary>
    public class SceneBGMPlayer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool playOnStart = true;
        [SerializeField] private bool fadeIn = true;

        private void Start()
        {
            if (playOnStart)
            {
                PlayBGMForCurrentScene();
            }
        }

        /// <summary>
        /// 현재 씬의 배경음악 재생
        /// </summary>
        public void PlayBGMForCurrentScene()
        {
            if (AudioManager.Instance == null)
            {
                Debug.LogWarning("[SceneBGMPlayer] AudioManager가 없습니다!");
                return;
            }

            string currentScene = SceneManager.GetActiveScene().name;
            AudioManager.Instance.PlayBGMForScene(currentScene, fadeIn);

            Debug.Log($"[SceneBGMPlayer] {currentScene} 씬의 BGM 재생");
        }
    }
}
