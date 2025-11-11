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

        [Header("Mouse")]
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float minVerticalAngle = -20f;
        [SerializeField] private float maxVerticalAngle = 60f;
        [SerializeField] private bool invertY = false;

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
        private float currentRotationY = 20f;
        private float targetDistance;
        private Vector3 velocity = Vector3.zero;

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

            targetDistance = distance;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

                if (invertY) mouseY = -mouseY;

                currentRotationX += mouseX;
                currentRotationY = Mathf.Clamp(currentRotationY - mouseY, minVerticalAngle, maxVerticalAngle);
            }

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
        public void SetMouseSensitivity(float sensitivity) => mouseSensitivity = Mathf.Clamp(sensitivity, 0.1f, 5f);
        public void ResetRotation()
        {
            currentRotationX = 0f;
            currentRotationY = 20f;
        }
    }
}