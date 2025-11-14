using UnityEngine;
using UnityEngine.InputSystem;
using static TraversalPro.Utility;

namespace TraversalPro
{
    /// <summary>
    /// Smoothly rotates and clamps the character's view according to user input.
    /// </summary>
    [AddComponentMenu("Traversal Pro/Camera/View Control")]
    public class ViewControl : MonoBehaviour
    {
        [Tooltip("The angle range in degrees that the character's view can rotate up and down.")]
        public Range verticalDegrees = new(-85, 85);

        [Tooltip("The sensitivity for pointer input deltas (usually mouse or touchscreen).")]
        public Vector2 pointerInputMultiplier = new(.15f, .12f);

        [Tooltip("The sensitivity for joystick input speeds (usually gamepad).")]
        public Vector2 joystickInputMultiplier = new(250, 200);

        [Tooltip("Approximately how long in seconds it will take for the view to match rotation input.")]
        [Min(.001f)] public float smoothTime = .03f;

        Vector2 currentJoystickInput;
        Vector3 degrees;
        Vector3 _degreesGoal;
        Vector3 degreesVelocity;

        [Header("Sensitivity Settings")]
        [SerializeField] string sensitivityKey = "MouseSensitivity";  /*[변경가능_감도저장키]*/
        [SerializeField] float basePointerX = .15f;                   /*[변경가능_기본마우스X감도]*/
        [SerializeField] float basePointerY = .12f;                   /*[변경가능_기본마우스Y감도]*/

        /// <summary>
        /// The euler angles in degrees in world space this view is rotating towards.
        /// </summary>
        public Vector3 DegreesGoal
        {
            get => _degreesGoal;
            set
            {
                _degreesGoal = value;
                _degreesGoal.x = Mathf.Clamp(_degreesGoal.x, -verticalDegrees.max, -verticalDegrees.min);
            }
        }

        void OnEnable()
        {
            currentJoystickInput = default;
            degrees = EulerZXY(transform.rotation);
            DegreesGoal = degrees;
            degreesVelocity = default;

            // 🔹 PlayerPrefs에서 감도 값만 읽어서 적용
            float savedSensitivity = PlayerPrefs.GetFloat(sensitivityKey, 5.0f);  /*[변경가능_기본감도값]*/
            ApplySensitivity(savedSensitivity);
        }

        void Update()
        {
            if (Time.timeScale == 0f) return; // 일시정지 중에는 시야 회전 멈추기

            Vector2 joystickOffset = currentJoystickInput * joystickInputMultiplier * Time.deltaTime;
            DegreesGoal += new Vector3(-joystickOffset.y, joystickOffset.x, 0);
            degrees = Vector3.SmoothDamp(degrees, DegreesGoal, ref degreesVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(degrees);
        }

        /// <summary>
        /// Immediately rotate the view. This is generally used to apply mouse input to rotate the view.
        /// </summary>
        public void PerformDeltaInput(Vector2 input)
        {
            input *= pointerInputMultiplier;
            DegreesGoal += new Vector3(-input.y, input.x, 0);
        }

        /// <summary>
        /// Immediately rotate the view using Unity's InputSystem. This is generally used to apply mouse input.
        /// </summary>
        public void DeltaInput(InputAction.CallbackContext context)
        {
            if (!isActiveAndEnabled) return;
            Vector2 input = context.ReadValue<Vector2>();
            PerformDeltaInput(input);
        }

        /// <summary>
        /// Set the view rotation using Unity's InputSystem. This is generally used to apply gamepad joystick input.
        /// </summary>
        public void VelocityInput(InputAction.CallbackContext context)
        {
            if (!isActiveAndEnabled) return;
            Vector2 input = context.ReadValue<Vector2>();
            currentJoystickInput = Vector2.ClampMagnitude(input, 1);
        }

        // 🔹 감도 슬라이더 값(예: 1~10)을 실제 마우스 감도로 변환
        void ApplySensitivity(float value)
        {
            // 기본값 5.0을 1배로 보고 비례 스케일
            float scale = value / 5f;        /*[변경가능_슬라이더기준값]*/
            scale = Mathf.Max(0.1f, scale); /*[변경가능_최소배율]*/

            pointerInputMultiplier = new Vector2(
                basePointerX * scale,
                basePointerY * scale
            );
        }
    }
}
