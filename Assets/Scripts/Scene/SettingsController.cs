using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [Header("Settings Controls")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeText;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityText;
    [SerializeField] private Button closeButton;
    
    // 설정 키 이름
    private const string VolumeKey = "MasterVolume";
    private const string SensitivityKey = "MouseSensitivity";
    
    private void Start()
    {
        // 저장된 설정 불러오기
        LoadSettings();
        
        // 슬라이더 이벤트 연결
        if (volumeSlider != null) volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        if (sensitivitySlider != null) sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        if (closeButton != null) closeButton.onClick.AddListener(CloseSettings);
    }
    
    // 설정 불러오기
    private void LoadSettings()
    {
        // PlayerPrefs에서 설정 불러오기
        float volume = PlayerPrefs.GetFloat(VolumeKey, 0.75f);
        float sensitivity = PlayerPrefs.GetFloat(SensitivityKey, 5.0f);
        
        // 슬라이더 값 설정
        if (volumeSlider != null) volumeSlider.value = volume;
        if (sensitivitySlider != null) sensitivitySlider.value = sensitivity;
        
        // 텍스트 업데이트
        UpdateVolumeText(volume);
        UpdateSensitivityText(sensitivity);
        
        // 오디오 설정 적용
        AudioListener.volume = volume;
    }
    
    // 볼륨 변경 시 호출
    private void OnVolumeChanged(float value)
    {
        // 텍스트 업데이트
        UpdateVolumeText(value);
        
        // 오디오 볼륨 설정
        AudioListener.volume = value;
        
        // 설정 저장
        PlayerPrefs.SetFloat(VolumeKey, value);
    }
    
    // 마우스 감도 변경 시 호출
    private void OnSensitivityChanged(float value)
    {
        // 텍스트 업데이트
        UpdateSensitivityText(value);
        
        // 설정 저장
        PlayerPrefs.SetFloat(SensitivityKey, value);
        
        // 다른 스크립트에서 이 값을 가져다 쓸 수 있도록 PlayerPrefs에 저장만 함
    }
    
    // 볼륨 텍스트 업데이트
    private void UpdateVolumeText(float value)
    {
        if (volumeText != null)
        {
            volumeText.text = $"볼륨: {Mathf.Round(value * 10)}%";
        }
    }
    
    // 감도 텍스트 업데이트
    private void UpdateSensitivityText(float value)
    {
        if (sensitivityText != null)
        {
            sensitivityText.text = $"마우스 감도: {value:F1}";
        }
    }
    
    // 설정 패널 닫기
    public void CloseSettings()
    {
        // 설정 저장
        PlayerPrefs.Save();
        
        // 상위 오브젝트가 TitleUI인 경우 TitleUIController 호출
        TitleUIController titleController = GetComponentInParent<TitleUIController>();
        if (titleController != null)
        {
            titleController.ClosePanel();
        }
        else
        {
            // 상위 오브젝트가 없는 경우 직접 비활성화
            gameObject.SetActive(false);
        }
    }
}