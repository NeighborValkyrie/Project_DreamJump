// SlipperySurface.cs
using UnityEngine;

[DisallowMultipleComponent]
public class SlipperySurface : MonoBehaviour
{
    [Header("조향/가속 설정")]
    [Range(0f, 1f)] public float accelScale = 0.4f;    // [변경가능] 가속 축소 비율
    public float minAccel = 6f;                         // [변경가능] 너무 낮아지지 않도록 바닥값
    public float turnRateDegPerSec = 180f;              // [변경가능] 초당 최대 회전각
    [Range(0f, 1f)] public float inputInfluence = 0.55f;// [변경가능] 입력 영향도

    [Header("속도 유지감")]
    [Range(0f, 1f)] public float speedFloor = 0.6f;     // [변경가능] 목표 속도 하한( MaxLocalSpeed * floor )
    public bool projectOnGround = true;                 // [변경가능] 지면 법선 투영 사용

    // 이 스크립트는 "마커+파라미터" 용도이므로 로직 없음
}
