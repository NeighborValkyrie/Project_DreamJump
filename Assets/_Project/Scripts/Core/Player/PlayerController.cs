using UnityEngine;

namespace PlatformerGame.Core.Player
{
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerAnimation))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        private PlayerMovement movement;
        private PlayerAnimation animation;

        [Header("Input")]
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
            if (PlatformerGame.Systems.Events.GameEventManager.Instance != null)
            {
                PlatformerGame.Systems.Events.GameEventManager.Instance.OnPlayerInputStateChanged += SetInputEnabled;
            }

            UnityEngine.Camera mainCam = UnityEngine.Camera.main;
            if (mainCam != null && movement != null)
            {
                movement.SetCameraTransform(mainCam.transform);
            }
        }

        private void OnDestroy()
        {
            if (PlatformerGame.Systems.Events.GameEventManager.Instance != null)
            {
                PlatformerGame.Systems.Events.GameEventManager.Instance.OnPlayerInputStateChanged -= SetInputEnabled;
            }
        }

        private void Update()
        {
            if (!inputEnabled) return;

            HandleMovement();
            HandleJump();
            HandleInteraction();
        }

        private void HandleMovement()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            movement.SetMoveInput(new Vector3(h, 0f, v).normalized);
        }

        private void HandleJump()
        {
            if (Input.GetButtonDown("Jump"))
            {
                movement.Jump();
            }
        }

        private void HandleInteraction()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange, interactableLayer);
                foreach (var hit in hits)
                {
                    var interactable = hit.GetComponent<PlatformerGame.Interactions.Interfaces.IInteractable>();
                    if (interactable != null)
                    {
                        interactable.Interact(gameObject);
                        break;
                    }
                }
            }
        }

        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
            if (!enabled && movement != null)
            {
                movement.SetMoveInput(Vector3.zero);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}