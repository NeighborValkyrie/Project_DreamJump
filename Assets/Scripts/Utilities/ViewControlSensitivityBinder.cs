using UnityEngine;
using TraversalPro;

public class ViewControlSensitivityBinder : MonoBehaviour
{
    [SerializeField] private ViewControl viewControl;  /*[변경가능_ViewControl참조]*/

    void Awake()
    {
        if (!viewControl)
            viewControl = GetComponent<ViewControl>();
    }

    void OnEnable()
    {
        SettingsController.OnSensitivityChangedGlobal += OnSensitivityChanged;
    }

    void OnDisable()
    {
        SettingsController.OnSensitivityChangedGlobal -= OnSensitivityChanged;
    }

    void OnSensitivityChanged(float value)
    {
        if (!viewControl) return;

        // ViewControl 내부의 ApplySensitivity를 public으로 만들어두고 호출
        viewControl.SendMessage("ApplySensitivity", value, SendMessageOptions.DontRequireReceiver);
        // 또는 viewControl.ApplySensitivity(value);  (public 메서드로 열어두면)
    }
}
