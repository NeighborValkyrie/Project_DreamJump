using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [Header("Settings Controls")]
    [SerializeField] private Slider volumeSlider;                 /*[변경가능_볼륨슬라이더]*/
    [SerializeField] private TextMeshProUGUI volumeText;          /*[변경가능_볼륨텍스트]*/
    [SerializeField] private Slider sensitivitySlider;            /*[변경가능_감도슬라이더]*/
    [SerializeField] private TextMeshProUGUI sensitivityText;     /*[변경가능_감도텍스트]*/
    [SerializeField] private Button closeButton;                  /*[변경가능_닫기버튼]*/

    // 설정 키 이름
    public const string VolumeKey = "MasterVolume";               /*[변경가능_볼륨키]*/
    public const string SensitivityKey = "MouseSensitivity";      /*[변경가능_감도키]*/

    // 감도 변경을 다른 스크립트에 알려주는 이벤트
    public static System.Action<float> OnSensitivityChangedGlobal; /*[변경가능_감도이벤트]*/

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
        float volume = PlayerPrefs.GetFloat(VolumeKey, 0.75f);
        float sensitivity = PlayerPrefs.GetFloat(SensitivityKey, 5.0f);

        if (volumeSlider != null) volumeSlider.value = volume;
        if (sensitivitySlider != null) sensitivitySlider.value = sensitivity;

        UpdateVolumeText(volume);
        UpdateSensitivityText(sensitivity);

        // 오디오 설정 적용
        AudioListener.volume = volume;
    }

    // 볼륨 변경 시 호출
    private void OnVolumeChanged(float value)
    {
        UpdateVolumeText(value);
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
    }

    // 마우스 감도 변경 시 호출
    private void OnSensitivityChanged(float value)
    {
        UpdateSensitivityText(value);
        PlayerPrefs.SetFloat(SensitivityKey, value);

        // 감도 변경을 실시간으로 알림 (카메라 스크립트에서 구독)
        OnSensitivityChangedGlobal?.Invoke(value);
    }

    // 볼륨 텍스트 업데이트
    private void UpdateVolumeText(float value)
    {
        if (volumeText != null)
        {
            volumeText.text = $"볼륨: {Mathf.Round(value * 100)}%";  /*[변경가능_표시형식]*/
        }
    }

    // 감도 텍스트 업데이트
    private void UpdateSensitivityText(float value)
    {
        if (sensitivityText != null)
        {
            sensitivityText.text = $"마우스 감도: {value:F1}";       /*[변경가능_표시형식]*/
        }
    }

    // 설정 패널 닫기
    public void CloseSettings()
    {
        PlayerPrefs.Save();

        // 1) 게임 안에서 열리는 설정인지 확인
        GameUIController gameUI = GetComponentInParent<GameUIController>();
        if (gameUI != null)
        {
            gameUI.CloseSettings();   // → 게임용 UI에 위임
            return;
        }

        // 2) 타이틀 화면에서 열리는 설정인지 확인
        TitleUIController titleController = GetComponentInParent<TitleUIController>();
        if (titleController != null)
        {
            titleController.ClosePanel();
            return;
        }

        // 3) 아무 상위 UI도 없으면, 자기 자신만 끔
        gameObject.SetActive(false);
    }
}
