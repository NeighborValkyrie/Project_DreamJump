// Checkpoint.cs (������Ʈ ����)
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [Header("Respawn Point")]
    [Tooltip("������ ��ġ/ȸ��. ���� �ڱ� Transform ���")]
    public Transform spawnPoint;

    [Header("Activation FX")]
    public ParticleSystem vfxPrefab;  // ����: ��ƼŬ ������
    public AudioClip sfx;             // ����: ���� Ŭ��
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Popup UI")]
    public bool showPopup = true;
    public string popupMessage = "üũ����Ʈ!";

    [Header("Marker (����: Ȱ��ȭ �� ���߱�)")]
    public GameObject markerToHide;
    public bool hideMarkerOnActivate = true;

    [Header("Behavior")]
    public bool onlyOnce = true;      // �� �� ������ �ٽ� �� �߰�

    bool activated;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        if (!spawnPoint) spawnPoint = transform;

        // ���� �� ��Ŀ�� ���̵���
        if (markerToHide) markerToHide.SetActive(true);
        activated = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // �ڽ� �ݶ��̴� ����: ��Ʈ�� �ö� Player �±� Ȯ��
        var root = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        if (!root.CompareTag("Player")) return;
        if (onlyOnce && activated) return;

        // üũ����Ʈ ���
        if (RespawnManager.Instance) RespawnManager.Instance.SetCheckpoint(spawnPoint);

        // FX
        if (vfxPrefab) Instantiate(vfxPrefab, spawnPoint.position, spawnPoint.rotation);
        if (sfx) AudioSource.PlayClipAtPoint(sfx, spawnPoint.position, sfxVolume);

        // UI
        if (showPopup) CheckpointPopupUI.Show(popupMessage);

        // ��Ŀ ����
        if (hideMarkerOnActivate && markerToHide) markerToHide.SetActive(false);

        activated = true;
    }
}
