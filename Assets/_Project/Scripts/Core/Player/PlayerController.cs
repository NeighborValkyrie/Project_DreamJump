using UnityEngine;

namespace PlatformerGame.Core.Player
{
    /// <summary>
    /// 플레이어 입력 처리 및 상호작용 제어
    /// </summary>
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerAnimation))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        private PlayerMovement movement;
        private PlayerAnimation animation;

        [Header("Input Settings")]
        [SerializeField] private bool inputEnabled = true;

        [Header("Interaction")]
        [SerializeField] private float interactionRange = 2f;
        [SerializeField] private LayerMask interactableLayer;

        private void Awake()
        {
            movement = GetComponent<PlayerMovement>();
            animation = GetComponent<PlayerAnimation>();
        }

        private void Start()
        {
            // 입력 상태 변경 이벤트 구독
            if (PlatformerGame.Systems.Events.GameEventManager.Instance != null)
            {
                PlatformerGame.Systems.Events.GameEventManager.Instance.OnPlayerInputStateChanged += SetInputEnabled;
            }
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            if (PlatformerGame.Systems.Events.GameEventManager.Instance != null)
            {
                PlatformerGame.Systems.Events.GameEventManager.Instance.OnPlayerInputStateChanged -= SetInputEnabled;
            }
        }

        private void Update()
        {
            if (!inputEnabled) return;

            HandleMovementInput();
            HandleJumpInput();
            HandleInteractionInput();
        }

        private void HandleMovementInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
            movement.SetMoveDirection(moveDirection);
        }

        private void HandleJumpInput()
        {
            if (Input.GetButtonDown("Jump"))
            {
                movement.Jump();
            }
        }

        private void HandleInteractionInput()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryInteract();
            }
        }

        private void TryInteract()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange, interactableLayer);

            foreach (var hitCollider in hitColliders)
            {
                var interactable = hitCollider.GetComponent<PlatformerGame.Interactions.Interfaces.IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(gameObject);
                    break;
                }
            }
        }

        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}