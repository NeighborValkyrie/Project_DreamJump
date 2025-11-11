using UnityEngine;

public class BuffItem_DoubleJump : MonoBehaviour
{
    public float duration = 5f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var ext = other.GetComponent<DoubleJumpExtension>();
        if (ext)
        {
            ext.ActivateDoubleJump(duration);
            Debug.Log($"[BUFF] Double Jump 활성화! 지속시간 {duration}초");
        }

        Destroy(gameObject);
    }
}
