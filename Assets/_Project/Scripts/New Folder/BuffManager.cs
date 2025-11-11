using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance { get; private set; }

    bool hasShield;
    float shieldTimer;

    void Awake()
    {
        Instance = this;
    }

    public void AddShield(float duration)
    {
        hasShield = true;
        shieldTimer = duration;
        Debug.Log($"[BUFF] ΩØµÂ ON ({duration}√ )");
    }

    void Update()
    {
        if (hasShield)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0f)
            {
                hasShield = false;
                Debug.Log("[BUFF] ΩØµÂ OFF");
            }
        }
    }

    public bool ConsumeShield()
    {
        if (!hasShield) return false;
        hasShield = false;
        Debug.Log("[BUFF] ΩØµÂ∞° ¿˚ √Êµπ∑Œ ªÁ∂Û¡¸ (1»∏ πÊæÓ)");
        return true;
    }
}
