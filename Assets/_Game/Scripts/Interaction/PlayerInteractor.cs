using UnityEngine;
using UnityEngine.InputSystem;

namespace TilkiOyunu.Foundation
{
    public sealed class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private Transform origin;
        [SerializeField] private InteractionPromptUI promptUI;
        [SerializeField, Min(0.1f)] private float range = 2.1f;
        [SerializeField, Range(0f, 180f)] private float maxViewAngle = 95f;
        [SerializeField] private LayerMask targetLayers = ~0;

        private readonly Collider[] hits = new Collider[16];
        private InputActionMap playerMap;
        private InputAction interactAction;
        private IInteractable focused;
        private string bindingLabel = "Interact";
        private bool inputLocked;

        public IInteractable Focused => focused;

        private void Awake()
        {
            ResolveInputActions();
            bindingLabel = BuildBindingLabel();
        }

        private void OnEnable()
        {
            ResolveInputActions();
            playerMap?.Enable();
        }

        private void OnDisable()
        {
            playerMap?.Disable();
            promptUI?.Clear();
            focused = null;
        }

        private void Update()
        {
            if (inputLocked)
            {
                focused = null;
                promptUI?.Clear();
                return;
            }

            focused = FindFocusedInteractable();
            UpdatePrompt();

            if (focused != null && interactAction != null && interactAction.WasPressedThisFrame())
            {
                InteractionContext context = new(gameObject);
                if (focused.CanInteract(context))
                {
                    focused.Interact(context);
                }
            }
        }

        private void ResolveInputActions()
        {
            if (inputActions == null)
            {
                return;
            }

            playerMap = inputActions.FindActionMap(InputActionIds.MapPlayer, false);
            interactAction = playerMap?.FindAction(InputActionIds.Interact, false);
        }

        private IInteractable FindFocusedInteractable()
        {
            Transform queryOrigin = origin != null ? origin : transform;
            InteractionContext context = new(gameObject);
            int count = Physics.OverlapSphereNonAlloc(
                queryOrigin.position,
                range,
                hits,
                targetLayers,
                QueryTriggerInteraction.Collide);

            IInteractable best = null;
            float bestScore = float.NegativeInfinity;

            for (int i = 0; i < count; i++)
            {
                Collider hit = hits[i];
                if (hit == null)
                {
                    continue;
                }

                IInteractable interactable = hit.GetComponentInParent<IInteractable>();
                if (interactable == null || !interactable.CanInteract(context))
                {
                    continue;
                }

                Vector3 toTarget = hit.bounds.center - queryOrigin.position;
                float distance = toTarget.magnitude;
                if (distance <= 0.001f)
                {
                    continue;
                }

                float angle = Vector3.Angle(queryOrigin.forward, toTarget / distance);
                if (angle > maxViewAngle * 0.5f)
                {
                    continue;
                }

                float score = (1f - distance / range) + (1f - angle / Mathf.Max(1f, maxViewAngle));
                if (score > bestScore)
                {
                    bestScore = score;
                    best = interactable;
                }
            }

            return best;
        }

        private void UpdatePrompt()
        {
            if (focused == null)
            {
                promptUI?.Clear();
                return;
            }

            promptUI?.Show(bindingLabel, focused.InteractionLabel);
        }

        private string BuildBindingLabel()
        {
            if (interactAction == null)
            {
                return InputActionIds.Interact;
            }

            string display = interactAction.GetBindingDisplayString(
                InputBinding.MaskByGroup("KeyboardMouse"),
                InputBinding.DisplayStringOptions.DontIncludeInteractions);

            return string.IsNullOrWhiteSpace(display) ? InputActionIds.Interact : display;
        }

        public void SetInputLocked(bool locked)
        {
            inputLocked = locked;
            if (locked)
            {
                focused = null;
                promptUI?.Clear();
            }
        }
    }
}
