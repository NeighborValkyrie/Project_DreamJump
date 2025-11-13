using UnityEngine;
using System.Collections.Generic;

namespace PlatformerGame.Systems.Audio
{
    /// <summary>
    /// 게임 전체 오디오 관리
    /// 배경음악(BGM)과 효과음(SFX)을 관리합니다.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Background Music")]
        [SerializeField] private AudioClip titleBGM;
        [SerializeField] private AudioClip gameBGM;
        [SerializeField] private AudioClip storyBGM;

        [Header("Volume Settings")]
        [SerializeField, Range(0f, 1f)] private float bgmVolume = 0.7f;
        [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 1f;

        private Dictionary<string, AudioClip> soundEffects = new Dictionary<string, AudioClip>();
        private bool isFading = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SetupAudioSources();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void SetupAudioSources()
        {
            // BGM AudioSource 설정
            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
            }
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            bgmSource.volume = bgmVolume;

            // SFX AudioSource 설정
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume;
        }

        #region BGM Control

        /// <summary>
        /// 배경음악 재생
        /// </summary>
        public void PlayBGM(AudioClip clip, bool fade = true)
        {
            if (clip == null)
            {
                Debug.LogWarning("[AudioManager] 재생할 BGM이 없습니다.");
                return;
            }

            if (bgmSource.clip == clip && bgmSource.isPlaying)
            {
                return; // 이미 같은 음악이 재생 중
            }

            if (fade)
            {
                StartCoroutine(FadeAndPlayBGM(clip));
            }
            else
            {
                bgmSource.clip = clip;
                bgmSource.Play();
            }
        }

        /// <summary>
        /// 씬별 배경음악 재생
        /// </summary>
        public void PlayBGMForScene(string sceneName, bool fade = true)
        {
            AudioClip clipToPlay = null;

            switch (sceneName)
            {
                case "01_TitleScene":
                    clipToPlay = titleBGM;
                    break;
                case "02_MainGame":
                    clipToPlay = gameBGM;
                    break;
                case "03_StoryScene":
                    clipToPlay = storyBGM;
                    break;
                default:
                    Debug.LogWarning($"[AudioManager] '{sceneName}' 씬에 대한 BGM이 설정되지 않았습니다.");
                    break;
            }

            if (clipToPlay != null)
            {
                PlayBGM(clipToPlay, fade);
            }
        }

        /// <summary>
        /// 배경음악 정지
        /// </summary>
        public void StopBGM(bool fade = true)
        {
            if (fade)
            {
                StartCoroutine(FadeOutBGM());
            }
            else
            {
                bgmSource.Stop();
            }
        }

        /// <summary>
        /// 배경음악 일시정지
        /// </summary>
        public void PauseBGM()
        {
            if (bgmSource.isPlaying)
            {
                bgmSource.Pause();
            }
        }

        /// <summary>
        /// 배경음악 재개
        /// </summary>
        public void UnpauseBGM()
        {
            if (!bgmSource.isPlaying)
            {
                bgmSource.UnPause();
            }
        }

        #endregion

        #region SFX Control

        /// <summary>
        /// 효과음 재생
        /// </summary>
        public void PlaySFX(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("[AudioManager] 재생할 SFX가 없습니다.");
                return;
            }

            sfxSource.PlayOneShot(clip, sfxVolume);
        }

        /// <summary>
        /// 등록된 효과음 재생
        /// </summary>
        public void PlaySFX(string soundName)
        {
            if (soundEffects.ContainsKey(soundName))
            {
                PlaySFX(soundEffects[soundName]);
            }
            else
            {
                Debug.LogWarning($"[AudioManager] '{soundName}' 효과음이 등록되지 않았습니다.");
            }
        }

        /// <summary>
        /// 효과음 등록
        /// </summary>
        public void RegisterSFX(string name, AudioClip clip)
        {
            if (!soundEffects.ContainsKey(name))
            {
                soundEffects.Add(name, clip);
            }
            else
            {
                soundEffects[name] = clip;
            }
        }

        #endregion

        #region Volume Control

        /// <summary>
        /// BGM 볼륨 설정
        /// </summary>
        public void SetBGMVolume(float volume)
        {
            bgmVolume = Mathf.Clamp01(volume);
            bgmSource.volume = bgmVolume;

            PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        }

        /// <summary>
        /// SFX 볼륨 설정
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            sfxSource.volume = sfxVolume;

            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        }

        /// <summary>
        /// 저장된 볼륨 로드
        /// </summary>
        public void LoadVolumeSettings()
        {
            bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

            bgmSource.volume = bgmVolume;
            sfxSource.volume = sfxVolume;
        }

        #endregion

        #region Fade Effects

        private System.Collections.IEnumerator FadeAndPlayBGM(AudioClip newClip)
        {
            if (isFading) yield break;
            isFading = true;

            // 페이드 아웃
            float startVolume = bgmSource.volume;
            float elapsed = 0f;

            while (elapsed < fadeDuration / 2)
            {
                elapsed += Time.unscaledDeltaTime;
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (fadeDuration / 2));
                yield return null;
            }

            // 새 음악으로 교체
            bgmSource.clip = newClip;
            bgmSource.Play();

            // 페이드 인
            elapsed = 0f;
            while (elapsed < fadeDuration / 2)
            {
                elapsed += Time.unscaledDeltaTime;
                bgmSource.volume = Mathf.Lerp(0f, bgmVolume, elapsed / (fadeDuration / 2));
                yield return null;
            }

            bgmSource.volume = bgmVolume;
            isFading = false;
        }

        private System.Collections.IEnumerator FadeOutBGM()
        {
            if (isFading) yield break;
            isFading = true;

            float startVolume = bgmSource.volume;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
                yield return null;
            }

            bgmSource.Stop();
            bgmSource.volume = bgmVolume;
            isFading = false;
        }

        #endregion

        public float GetBGMVolume() => bgmVolume;
        public float GetSFXVolume() => sfxVolume;
        public bool IsBGMPlaying() => bgmSource.isPlaying;
    }
}
