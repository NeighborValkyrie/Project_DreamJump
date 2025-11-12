// DoubleJumpExtension.cs (호환용)
using UnityEngine;

public class DoubleJumpExtension : MonoBehaviour
{
    public TraversalPro.Jump jump; // [변경가능] 수동 할당 가능

    void Awake() { if (!jump) jump = GetComponent<TraversalPro.Jump>(); }

    public void ActivateDoubleJump(float duration)
    {
        if (!jump) { Debug.LogWarning("[DoubleJumpExtension] Jump가 없습니다."); return; }
        jump.EnableDoubleJump(duration);
    }
}
