using UnityEngine;

namespace PlatformerGame.Core.Camera
{
    /// <summary>
    /// SmoothDamp 기반 3인칭 카메라
    /// v7.0: 프레임 독립적 움직임
    /// </summary>
    public class FollowCamera : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform target;

        [Header("Distance Settings")]
        [SerializeField] private float distance = 5f;
        [SerializeField] private float height = 2f;

        [Header("Smoothness")]
        [SerializeField] private float smoothness = 0.1f;

        [Header("Look At Settings")]
        [SerializeField] private Vector3 lookAtOffset = new Vector3(0f, 1f, 0f);

        private Vector3 currentVelocity = Vector3.zero;

        private void Start()
        {
            // 타겟이 할당되지 않은 경우 Player 찾기
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Transform cameraTarget = player.transform.Find("CameraTarget");
                    target = cameraTarget != null ? cameraTarget : player.transform;
                }
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position - target.forward * distance + Vector3.up * height;
            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref currentVelocity,
                smoothness
            );

            transform.LookAt(target.position + lookAtOffset);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}