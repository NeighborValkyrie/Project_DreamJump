// CollectibleItem.cs
using System.Diagnostics;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public enum Type { Coin, Star, Key }

    [Header("Type & Amount")]
    public Type type = Type.Coin;
    public int amount = 1;

    [Header("Effects (optional)")]
    public GameObject pickupVFX;
    public AudioClip pickupSFX;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Respawn Options")]
    public bool autoRespawn = false;
    public float respawnDelay = 3f;        // 자동 재생성 지연
    public bool resetOnPlayerRespawn = false; // 플레이어 리스폰 시 즉시 복구

    [Header("Visuals")]
    public Renderer[] renderersToToggle;   // 비워두면 자동 수집
    public Collider pickupCollider;       // 비워두면 GetComponent<Collider>()

    // 내부 상태
    bool collected;
    Vector3 startPos; Quaternion startRot;

    void Awake()
    {
        if (!pickupCollider) pickupCollider = GetComponent<Collider>();
        if (pickupCollider) pickupCollider.isTrigger = true;

        if (renderersToToggle == null || renderersToToggle.Length == 0)
            renderersToToggle = GetComponentsInChildren<Renderer>(true);

        startPos = transform.position; startRot = transform.rotation;
    }

    void OnEnable()
    {
        // RespawnManager 이벤트(있으면) 구독
        var rm = FindObjectOfType<RespawnManager>();
        if (rm && resetOnPlayerRespawn) RespawnManager.OnRespawn += HandleRespawn;
    }

    void OnDisable()
    {
        if (resetOnPlayerRespawn) RespawnManager.OnRespawn -= HandleRespawn;
    }

    void OnTriggerEnter(Collider other)
    {
        var root = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        if (!root.CompareTag("Player")) return;
        if (collected) return;

        // 카운트 증가
        var mgr = CollectibleManager.Instance;
        if (!mgr) { UnityEngine.Debug.LogWarning("[Collectible] CollectibleManager가 없습니다."); return; }

        switch (type)
        {
            case Type.Coin: mgr.AddCoin(amount); break;
            case Type.Star: mgr.AddStar(amount); break;
            case Type.Key: mgr.AddKey(amount); break;
        }

        // 이펙트
        if (pickupVFX) Instantiate(pickupVFX, transform.position, transform.rotation);
        if (pickupSFX) AudioSource.PlayClipAtPoint(pickupSFX, transform.position, sfxVolume);

        // 가리기
        SetVisible(false);
        collected = true;

        // 자동 재생성
        if (autoRespawn) StartCoroutine(AutoRespawnAfter(respawnDelay));
    }

    System.Collections.IEnumerator AutoRespawnAfter(float t)
    {
        if (t > 0) yield return new WaitForSecondsRealtime(t);
        ResetItem();
    }

    void HandleRespawn()
    {
        if (resetOnPlayerRespawn) ResetItem();
    }

    void ResetItem()
    {
        collected = false;
        transform.SetPositionAndRotation(startPos, startRot);
        SetVisible(true);
    }

    void SetVisible(bool on)
    {
        if (pickupCollider) pickupCollider.enabled = on;
        if (renderersToToggle != null)
            foreach (var r in renderersToToggle) if (r) r.enabled = on;
    }
}
