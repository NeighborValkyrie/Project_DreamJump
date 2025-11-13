using UnityEngine;
using UnityEngine.UI;
using PlatformerGame.Systems.Audio;

namespace PlatformerGame.Systems.UI
{
    /// <summary>
    /// 설정 패널 UI 관리
    /// 볼륨 조절 등의 기능을 제공합니다.
    /// v7.1: 수정됨 - 네임스페이스 접근 수정
    /// </summary>
    public class SettingsPanel : MonoBehaviour
    {
        [Header("Volume Sliders")]
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        [Header("Volume Text")]
        [SerializeField] private TMPro.TextMeshProUGUI bgmVolumeText;
        [SerializeField] private TMPro.TextMeshProUGUI sfxVolumeText;

        [Header("Test Button")]
        [SerializeField] private Button sfxTestButton;
        [SerializeField] private AudioClip testSFX;

        private void Start()
        {
            SetupSliders();
            LoadVolumeSettings();
        }

        private void SetupSliders()
        {
            // BGM 슬라이더
            if (bgmSlider != null)
            {
                bgmSlider.minValue = 0f;
                bgmSlider.maxValue = 1f;
                bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            }

            // SFX 슬라이더
            if (sfxSlider != null)
            {
                sfxSlider.minValue = 0f;
                sfxSlider.maxValue = 1f;
                sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            }

            // 테스트 버튼
            if (sfxTestButton != null)
            {
                sfxTestButton.onClick.AddListener(TestSFX);
            }
        }

        private void LoadVolumeSettings()
        {
            if (AudioManager.Instance == null) return;

            // BGM 볼륨 로드
            if (bgmSlider != null)
            {
                bgmSlider.value = AudioManager.Instance.GetBGMVolume();
            }

            // SFX 볼륨 로드
            if (sfxSlider != null)
            {
                sfxSlider.value = AudioManager.Instance.GetSFXVolume();
            }
        }

        private void OnBGMVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetBGMVolume(value);
            }

            // 텍스트 업데이트
            if (bgmVolumeText != null)
            {
                bgmVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
            }
        }

        private void OnSFXVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSFXVolume(value);
            }

            // 텍스트 업데이트
            if (sfxVolumeText != null)
            {
                sfxVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
            }
        }

        private void TestSFX()
        {
            if (AudioManager.Instance != null && testSFX != null)
            {
                AudioManager.Instance.PlaySFX(testSFX);
            }
        }
    }
}
