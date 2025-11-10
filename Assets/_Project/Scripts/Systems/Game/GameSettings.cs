using UnityEngine;

namespace PlatformerGame.Systems.Game
{
    /// <summary>
    /// 게임 설정 관리 (볼륨, 품질, 전체화면)
    /// v7.0: 새로 추가됨
    /// </summary>
    public class GameSettings : MonoBehaviour
    {
        public static GameSettings Instance { get; private set; }

        [Header("Audio Settings")]
        [SerializeField] [Range(0f, 1f)] private float masterVolume = 0.8f;
        [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.7f;
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;

        [Header("Graphics Settings")]
        [SerializeField] private int qualityLevel = 2; // 0=Low, 1=Medium, 2=High
        [SerializeField] private bool isFullscreen = true;

        [Header("Gameplay Settings")]
        [SerializeField] private float mouseSensitivity = 1f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSettings();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            ApplySettings();
        }

        // 볼륨 설정
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            AudioListener.volume = masterVolume;
            SaveSettings();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);

            if (Audio.AudioManager.Instance != null)
            {
                // AudioManager에서 BGM 볼륨 조절
                Audio.AudioManager.Instance.SetMusicVolume(musicVolume);
            }

            SaveSettings();
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);

            if (Audio.AudioManager.Instance != null)
            {
                // AudioManager에서 SFX 볼륨 조절
                Audio.AudioManager.Instance.SetSFXVolume(sfxVolume);
            }

            SaveSettings();
        }

        // 그래픽 설정
        public void SetQualityLevel(int level)
        {
            qualityLevel = Mathf.Clamp(level, 0, QualitySettings.names.Length - 1);
            QualitySettings.SetQualityLevel(qualityLevel);
            SaveSettings();
        }

        public void SetFullscreen(bool fullscreen)
        {
            isFullscreen = fullscreen;
            Screen.fullScreen = isFullscreen;
            SaveSettings();
        }

        // 게임플레이 설정
        public void SetMouseSensitivity(float sensitivity)
        {
            mouseSensitivity = Mathf.Clamp(sensitivity, 0.1f, 2f);
            SaveSettings();
        }

        // 설정 적용
        private void ApplySettings()
        {
            AudioListener.volume = masterVolume;
            QualitySettings.SetQualityLevel(qualityLevel);
            Screen.fullScreen = isFullscreen;

            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.SetMusicVolume(musicVolume);
                Audio.AudioManager.Instance.SetSFXVolume(sfxVolume);
            }
        }

        // 설정 저장/로드
        private void SaveSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.SetInt("QualityLevel", qualityLevel);
            PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
            PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
            PlayerPrefs.Save();
        }

        private void LoadSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            qualityLevel = PlayerPrefs.GetInt("QualityLevel", 2);
            isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        }

        // Getter
        public float GetMasterVolume() => masterVolume;
        public float GetMusicVolume() => musicVolume;
        public float GetSFXVolume() => sfxVolume;
        public int GetQualityLevel() => qualityLevel;
        public bool IsFullscreen() => isFullscreen;
        public float GetMouseSensitivity() => mouseSensitivity;
    }
}