using UnityEngine;

public class BoatDisembark : MonoBehaviour
{
    [Header("참조")]
    public Transform player;
    public Transform landingPoint;
    public Transform arrivalStop;

    [Header("설정")]
    public KeyCode interactKey = KeyCode.E;   // /*[변경가능_키]*/
    public float arrivalRadius = 1f;       // /*[변경가능_거리]*/

    [Header("UI")]
    public GameObject promptUI;  // "E 눌러 하선하기" 텍스트나 패널  /*[변경가능_UI객체]*/

    bool hasDisembarked = false;
    bool _uiVisible = false;     // 현재 UI가 켜져 있는지 상태 캐싱

    void Update()
    {
        // 이미 내렸으면 UI도 꺼져 있어야 하고, 더 이상 처리 안 함
        if (hasDisembarked)
        {
            if (_uiVisible) HidePrompt();
            return;
        }

        bool arrived = IsArrived();

        // 도착 상태에 따라 UI On/Off
        if (arrived && !_uiVisible)
        {
            ShowPrompt();
        }
        else if (!arrived && _uiVisible)
        {
            HidePrompt();
        }

        // 도착한 상태에서만 상호작용 키 받기
        if (arrived && Input.GetKeyDown(interactKey))
        {
            TeleportPlayerToLandingPoint();
        }
    }

    bool IsArrived()
    {
        if (!arrivalStop) return false;

        Vector3 boatPos = transform.position;
        Vector3 targetPos = arrivalStop.position;
        float sqrDist = (boatPos - targetPos).sqrMagnitude;

        return sqrDist <= arrivalRadius * arrivalRadius;
    }

    void ShowPrompt()
    {
        if (!promptUI) return;
        promptUI.SetActive(true);
        _uiVisible = true;
    }

    void HidePrompt()
    {
        if (!promptUI) return;
        promptUI.SetActive(false);
        _uiVisible = false;
    }

    void TeleportPlayerToLandingPoint()
    {
        if (!player || !landingPoint) return;
        // 여기서 E 연타 안전장치도 같이 넣을 거야 (아래 2번에서 더 설명)
    }

    void OnDisable()
    {
        // 씬 전환/오브젝트 비활성화 시 UI가 남아있지 않도록
        if (_uiVisible) HidePrompt();
    }
}
