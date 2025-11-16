// CollectibleManager.cs
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    [Header("Counts (read-only in Inspector)")]
    [SerializeField] int coin;
    [SerializeField] int star;
    [SerializeField] int key;

    // UI가 구독
    public System.Action OnChanged;

    // 저장 키(원하면 비활성화 가능)
    const string K_COIN = "collect.coin";
    const string K_STAR = "collect.star";
    const string K_KEY = "collect.key";

    [Header("Persistence")]
    public bool usePlayerPrefs = true;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
        Notify();
    }

    public void AddCoin(int amt = 1)
    {
        coin = Mathf.Max(0, coin + amt);   // 🔸 delta를 더한 뒤 0 아래로만 안 떨어지게
        Save();
        Notify();
    }

    public void AddStar(int amt = 1)
    {
        star = Mathf.Max(0, star + amt);
        Save();
        Notify();
    }

    public void AddKey(int amt = 1)
    {
        key = Mathf.Max(0, key + amt);
        Save();
        Notify();
    }
    public int GetCoin() => coin;
    public int GetStar() => star;
    public int GetKey() => key;

    public void ResetAll(bool saveNow = true)
    {
        coin = star = key = 0;
        if (saveNow) Save();
        Notify();
    }

    void Notify() => OnChanged?.Invoke();

    void Save()
    {
        if (!usePlayerPrefs) return;
        PlayerPrefs.SetInt(K_COIN, coin);
        PlayerPrefs.SetInt(K_STAR, star);
        PlayerPrefs.SetInt(K_KEY, key);
    }

    void Load()
    {
        if (!usePlayerPrefs) return;
        coin = PlayerPrefs.GetInt(K_COIN, 0);
        star = PlayerPrefs.GetInt(K_STAR, 0);
        key = PlayerPrefs.GetInt(K_KEY, 0);
    }
}
