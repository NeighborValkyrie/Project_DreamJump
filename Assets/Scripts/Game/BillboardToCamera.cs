using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    [Header("Target Camera")]
    [Tooltip("비워두면 Camera.main을 자동 사용")]
    public Camera targetCamera;
    public bool useMainCameraIfNull = true;

    [Header("Rotation Mode")]
    [Tooltip("Y축은 고정하고 수평 방향만 카메라를 바라보게 할지 여부")]
    public bool lockYAxis = true;

    void LateUpdate()
    {
        // 카메라 찾기
        if (!targetCamera)
        {
            if (useMainCameraIfNull && Camera.main)
                targetCamera = Camera.main;
            else
                return;
        }

        Vector3 camPos = targetCamera.transform.position;
        Vector3 dir = transform.position - camPos;

        if (lockYAxis)
            dir.y = 0f; // 라벨이 수평으로만 돌아가게

        if (dir.sqrMagnitude < 0.0001f)
            return;

        // 카메라를 바라보도록 회전
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
