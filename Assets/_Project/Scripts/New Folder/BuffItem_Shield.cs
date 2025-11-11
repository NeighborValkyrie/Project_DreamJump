using UnityEngine;

public class BuffItem_Shield : MonoBehaviour
{
    public float duration = 10f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var buff = BuffManager.Instance;
        if (buff)
        {
            buff.AddShield(duration);
            Debug.Log($"[BUFF] Shield 활성화! 지속시간 {duration}초");
        }

        Destroy(gameObject);
    }
}
