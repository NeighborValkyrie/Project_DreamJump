using UnityEngine;
using UnityEngine.InputSystem;
using TraversalPro;

[RequireComponent(typeof(Jump))]
public class DoubleJumpExtension : MonoBehaviour
{
    public int extraJumps = 1;
    private Jump jump;
    private int jumpsLeft;
    private bool isActive = false;

    void Awake()
    {
        jump = GetComponent<Jump>();
    }

    void Update()
    {
        if (!isActive) return;

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryDoubleJump();
        }
    }

    public void TryDoubleJump()
    {
        if (jump.CharacterMotor.IsGrounded)
        {
            jumpsLeft = extraJumps;
            return;
        }

        if (jumpsLeft > 0)
        {
            jumpsLeft--;
            jump.PerformJump();
            Debug.Log("[BUFF] Double Jump 사용!");
        }
    }

    public void ActivateDoubleJump(float time)
    {
        StopAllCoroutines();
        StartCoroutine(TempActivate(time));
    }

    System.Collections.IEnumerator TempActivate(float time)
    {
        isActive = true;
        jumpsLeft = extraJumps;
        Debug.Log($"[BUFF] Double Jump 활성 (지속시간: {time}초)");
        yield return new WaitForSeconds(time);
        isActive = false;
        Debug.Log("[BUFF] Double Jump 비활성화됨");
    }
}
