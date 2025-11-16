// Checkpoint.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [Header("Respawn Point")]
    [Tooltip("리스폰 위치/회전. 비워두면 이 오브젝트의 Transform 사용")]
    public Transform spawnPoint;

    [Header("Activation FX")]
    [Tooltip("체크포인트 발동 시 재생할 파티클 프리팹")]
    public ParticleSystem vfxPrefab;
    [Tooltip("체크포인트 발동 시 재생할 효과음")]
    public AudioClip sfx;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Popup UI")]
    public bool showPopup = true;
    public string popupMessage = "체크포인트!";

    [Header("Marker (예: 느낌표, 불빛 오브젝트)")]
    [Tooltip("체크포인트를 표시하는 장식 오브젝트(활성화 후 숨기고 싶을 때)")]
    public GameObject markerToHide;
    public bool hideMarkerOnActivate = true;

    [Header("Label (월드 텍스트 / 캔버스)")]
    [Tooltip("위에 '체크포인트' 라고 써 있는 월드 캔버스나 3D Text 루트")]
    public GameObject labelObject;
    [Tooltip("라벨 컨트롤러가 없을 때, 체크포인트 발동 시 라벨을 숨길지 여부")]
    public bool hideLabelOnActivate = true;

    [Header("Behavior")]
    [Tooltip("true면 한 번만 발동, 이후에는 다시 밟아도 무시")]
    public bool onlyOnce = true;

    bool activated;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        if (!spawnPoint)
            spawnPoint = transform;

        // 시작할 때 마커/라벨은 켜진 상태로
        if (markerToHide) markerToHide.SetActive(true);
        if (labelObject) labelObject.SetActive(true);

        activated = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // 리지드바디가 붙어 있으면 루트 오브젝트 기준으로 Tag 확인
        var root = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        if (!root.CompareTag("Player")) return;
        if (onlyOnce && activated) return;

        // ✔ 체크포인트 설정
        if (RespawnManager.Instance)
            RespawnManager.Instance.SetCheckpoint(spawnPoint);

        // ✔ 이펙트
        if (vfxPrefab)
            Instantiate(vfxPrefab, spawnPoint.position, spawnPoint.rotation);

        if (sfx)
            AudioSource.PlayClipAtPoint(sfx, spawnPoint.position, sfxVolume);

        // ✔ 팝업 UI
        if (showPopup)
            CheckpointPopupUI.Show(popupMessage);

        // ✔ 라벨 제어
        if (labelObject)
        {
            var ctrl = labelObject.GetComponent<CheckpointLabelController>();
            if (ctrl != null)
            {
                // 컨트롤러가 있으면 모드에 맞게 처리(깜빡임/숨김 등)
                ctrl.OnActivated();
            }
            else if (hideLabelOnActivate)
            {
                // 컨트롤러가 없으면 그냥 라벨 오브젝트 비활성화
                labelObject.SetActive(false);
            }
        }

        // ✔ 마커(느낌표, 불빛 등) 숨기기
        if (hideMarkerOnActivate && markerToHide)
            markerToHide.SetActive(false);

        activated = true;
    }
}
