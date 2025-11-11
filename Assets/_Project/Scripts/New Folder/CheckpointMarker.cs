// CheckpointMarker.cs
using UnityEngine;

public class CheckpointMarker : MonoBehaviour
{
    [Header("토글할 루트 (이 오브젝트 비워두면 자기 자신)")]
    public GameObject markerRoot;

    [Header("효과 옵션")]
    public bool billboard = true;
    public bool bobbing = true;
    public float bobAmplitude = 0.15f;
    public float bobSpeed = 2.2f;

    private Vector3 baseLocalPos;

    void Awake()
    {
        if (!markerRoot) markerRoot = gameObject;
        baseLocalPos = transform.localPosition;
    }

    void LateUpdate()
    {
        if (billboard && Camera.main)
        {
            Vector3 toCam = Camera.main.transform.position - transform.position;
            toCam.y = 0f;
            if (toCam.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(-toCam.normalized, Vector3.up);
        }

        if (bobbing)
        {
            float y = Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
            transform.localPosition = baseLocalPos + new Vector3(0f, y, 0f);
        }
    }

    public void Show(bool on)
    {
        if (markerRoot) markerRoot.SetActive(on);
    }
}
