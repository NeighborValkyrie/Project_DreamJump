using UnityEngine;

namespace PlatformerGame.Utilities
{
    /// <summary>
    /// 오브젝트 자동 회전 (아이템용)
    /// v7.0: 새로 추가됨
    /// </summary>
    public class AutoRotate : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 100f, 0f);

        [Header("Bob Settings")]
        [SerializeField] private bool enableBobbing = false;
        [SerializeField] private float bobHeight = 0.5f;
        [SerializeField] private float bobSpeed = 1f;

        private Vector3 startPosition;

        private void Start()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            // 회전
            transform.Rotate(rotationSpeed * Time.deltaTime);

            // 상하 움직임 (선택 사항)
            if (enableBobbing)
            {
                float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
        }
    }
}