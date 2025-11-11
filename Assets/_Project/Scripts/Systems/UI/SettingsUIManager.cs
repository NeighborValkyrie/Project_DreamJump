using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PlatformerGame.Systems.UI
{
    /// <summary>
    /// 설정 UI 관리
    /// v7.0: 새로 추가됨
    /// </summary>
    public class SettingsUIManager : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        [Header("Graphics Settings")]
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private Toggle fullscreenToggle;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;

        private void OnEnable()
        {
            LoadCurrentSettings();

            // 슬라이더 이벤트 연결
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

            // 드롭다운 이벤트 연결
            if (qualityDropdown != null)
                qualityDropdown.onValueChanged.AddListener(OnQualityChanged);

            // 토글 이벤트 연결
            if (fullscreenToggle != null)
                fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);

            // 닫기 버튼 이벤트 연결
            if (closeButton != null)
                closeButton.onClick.AddListener(OnClose);
        }

        private void OnDisable()
        {
            // 이벤트 구독 해제
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);

            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);

            if (qualityDropdown != null)
                qualityDropdown.onValueChanged.RemoveListener(OnQualityChanged);

            if (fullscreenToggle != null)
                fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenChanged);

            if (closeButton != null)
                closeButton.onClick.RemoveListener(OnClose);
        }

        private void LoadCurrentSettings()
        {
            if (Game.GameSettings.Instance == null) return;

            // 볼륨 설정 로드
            if (masterVolumeSlider != null)
                masterVolumeSlider.value = Game.GameSettings.Instance.GetMasterVolume();

            if (musicVolumeSlider != null)
                musicVolumeSlider.value = Game.GameSettings.Instance.GetMusicVolume();

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.value = Game.GameSettings.Instance.GetSFXVolume();

            // 그래픽 설정 로드
            if (qualityDropdown != null)
                qualityDropdown.value = Game.GameSettings.Instance.GetQualityLevel();

            if (fullscreenToggle != null)
                fullscreenToggle.isOn = Game.GameSettings.Instance.IsFullscreen();
        }

        // 볼륨 변경 이벤트
        private void OnMasterVolumeChanged(float value)
        {
            if (Game.GameSettings.Instance != null)
            {
                Game.GameSettings.Instance.SetMasterVolume(value);
            }
        }

        private void OnMusicVolumeChanged(float value)
        {
            if (Game.GameSettings.Instance != null)
            {
                Game.GameSettings.Instance.SetMusicVolume(value);
            }
        }

        private void OnSFXVolumeChanged(float value)
        {
            if (Game.GameSettings.Instance != null)
            {
                Game.GameSettings.Instance.SetSFXVolume(value);
            }

            // SFX 테스트 재생
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.Play("UIClick");
            }
        }

        // 그래픽 변경 이벤트
        private void OnQualityChanged(int index)
        {
            if (Game.GameSettings.Instance != null)
            {
                Game.GameSettings.Instance.SetQualityLevel(index);
            }
        }

        private void OnFullscreenChanged(bool isFullscreen)
        {
            if (Game.GameSettings.Instance != null)
            {
                Game.GameSettings.Instance.SetFullscreen(isFullscreen);
            }
        }

        // 닫기
        private void OnClose()
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.Play("UIClick");
            }

            gameObject.SetActive(false);
        }
    }
}