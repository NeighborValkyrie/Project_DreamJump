using UnityEngine;

namespace PlatformerGame.Core.Camera
{
    public class FollowCamera : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Distance")]
        [SerializeField] private float distance = 5f;
        [SerializeField] private float minDistance = 2f;
        [SerializeField] private float maxDistance = 10f;
        [SerializeField] private float height = 2f;

        [Header("Mouse Follow")]
        [SerializeField] private bool followMouse = true;
        [SerializeField] private float mouseInfluence = 30f; // 마우스 영향력
        [SerializeField] private float mouseSmoothness = 5f;

        [Header("Rotation")]
        [SerializeField] private float defaultVerticalAngle = 15f;
        [SerializeField] private float minVerticalAngle = -20f;
        [SerializeField] private float maxVerticalAngle = 60f;

        [Header("Smoothness")]
        [SerializeField] private float positionDamping = 5f;
        [SerializeField] private float rotationDamping = 10f;

        [Header("Zoom")]
        [SerializeField] private float zoomSpeed = 2f;

        [Header("Collision")]
        [SerializeField] private bool checkCollision = true;
        [SerializeField] private float collisionBuffer = 0.3f;
        [SerializeField] private LayerMask collisionLayers;

        private float currentRotationX = 0f;
        private float currentRotationY;
        private float targetDistance;
        private Vector3 velocity = Vector3.zero;
        private Vector2 mouseOffset = Vector2.zero;

        private void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Transform cameraTarget = player.transform.Find("CameraTarget");
                    target = cameraTarget != null ? cameraTarget : player.transform;
                }
            }

            currentRotationY = defaultVerticalAngle;
            targetDistance = distance;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            HandleMouseInput();
            HandleZoom();
            UpdateCameraPosition();
        }

        private void HandleMouseInput()
        {
            if (followMouse)
            {
                // 화면 중앙 대비 마우스 위치 (-1 ~ 1)
                Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                Vector2 mousePos = Input.mousePosition;
                Vector2 mouseDelta = (mousePos - screenCenter) / screenCenter;

                // 부드럽게 적용
                mouseOffset = Vector2.Lerp(mouseOffset, mouseDelta, mouseSmoothness * Time.deltaTime);

                // 마우스 위치에 따라 카메라 회전
                currentRotationX = target.eulerAngles.y + mouseOffset.x * mouseInfluence;
                currentRotationY = Mathf.Clamp(
                    defaultVerticalAngle - mouseOffset.y * (mouseInfluence * 0.5f),
                    minVerticalAngle,
                    maxVerticalAngle
                );
            }
            else
            {
                // 마우스 팔로우 끄면 플레이어 뒤
                currentRotationX = target.eulerAngles.y;
                currentRotationY = defaultVerticalAngle;
            }

            // ESC로 커서 표시
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            targetDistance = Mathf.Clamp(targetDistance - scroll * zoomSpeed, minDistance, maxDistance);
        }

        private void UpdateCameraPosition()
        {
            Quaternion rotation = Quaternion.Euler(currentRotationY, currentRotationX, 0f);
            Vector3 targetPosition = target.position - rotation * Vector3.forward * targetDistance + Vector3.up * height;

            if (checkCollision)
            {
                RaycastHit hit;
                Vector3 direction = targetPosition - target.position;
                float maxDist = direction.magnitude;

                if (Physics.Raycast(target.position, direction.normalized, out hit, maxDist, collisionLayers))
                {
                    targetPosition = hit.point - direction.normalized * collisionBuffer;
                }
            }

            transform.position = Vector3.Lerp(transform.position, targetPosition, positionDamping * Time.deltaTime);
            
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationDamping * Time.deltaTime);
        }

        public void SetTarget(Transform newTarget) => target = newTarget;
        public void SetFollowMouse(bool enable) => followMouse = enable;
    }
}