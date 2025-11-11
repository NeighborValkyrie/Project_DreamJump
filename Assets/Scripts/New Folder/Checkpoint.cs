// Checkpoint.cs (업데이트 버전)
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [Header("Respawn Point")]
    [Tooltip("리스폰 위치/회전. 비우면 자기 Transform 사용")]
    public Transform spawnPoint;

    [Header("Activation FX")]
    public ParticleSystem vfxPrefab;  // 선택: 파티클 프리팹
    public AudioClip sfx;             // 선택: 사운드 클립
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Popup UI")]
    public bool showPopup = true;
    public string popupMessage = "체크포인트!";

    [Header("Marker (선택: 활성화 시 감추기)")]
    public GameObject markerToHide;
    public bool hideMarkerOnActivate = true;

    [Header("Behavior")]
    public bool onlyOnce = true;      // 한 번 찍으면 다시 안 뜨게

    bool activated;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        if (!spawnPoint) spawnPoint = transform;

        // 시작 시 마커는 보이도록
        if (markerToHide) markerToHide.SetActive(true);
        activated = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // 자식 콜라이더 대응: 루트로 올라가 Player 태그 확인
        var root = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        if (!root.CompareTag("Player")) return;
        if (onlyOnce && activated) return;

        // 체크포인트 등록
        if (RespawnManager.Instance) RespawnManager.Instance.SetCheckpoint(spawnPoint);

        // FX
        if (vfxPrefab) Instantiate(vfxPrefab, spawnPoint.position, spawnPoint.rotation);
        if (sfx) AudioSource.PlayClipAtPoint(sfx, spawnPoint.position, sfxVolume);

        // UI
        if (showPopup) CheckpointPopupUI.Show(popupMessage);

        // 마커 숨김
        if (hideMarkerOnActivate && markerToHide) markerToHide.SetActive(false);

        activated = true;
    }
}
