using UnityEngine;
using UnityEngine.InputSystem;

namespace TilkiOyunu.Foundation
{
    [RequireComponent(typeof(Camera))]
    public sealed class ThirdPersonCameraController : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 targetOffset = new(0f, 0.75f, 0f);
        [SerializeField, Min(0.5f)] private float distance = 4.4f;
        [SerializeField, Min(0.1f)] private float minDistance = 1.2f;
        [SerializeField] private float mouseSensitivity = 0.08f;
        [SerializeField] private float gamepadSensitivity = 135f;
        [SerializeField] private float minPitch = -18f;
        [SerializeField] private float maxPitch = 58f;
        [SerializeField, Min(0.1f)] private float followSharpness = 18f;
        [SerializeField, Min(0.01f)] private float collisionRadius = 0.18f;
        [SerializeField] private LayerMask obstructionLayers = ~0;

        private InputActionMap playerMap;
        private InputAction lookAction;
        private Vector3 smoothedTargetPosition;
        private float yaw;
        private float pitch = 18f;

        private void Awake()
        {
            ResolveInputActions();
            if (target != null)
            {
                smoothedTargetPosition = target.position + targetOffset;
                yaw = target.eulerAngles.y;
            }
        }

        private void OnValidate()
        {
            distance = Mathf.Max(minDistance, distance);
            maxPitch = Mathf.Max(minPitch + 1f, maxPitch);
        }

        private void OnEnable()
        {
            ResolveInputActions();
            playerMap?.Enable();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            playerMap?.Disable();
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            float deltaTime = Time.deltaTime;
            Vector2 look = lookAction?.ReadValue<Vector2>() ?? Vector2.zero;
            bool mouseLook = lookAction?.activeControl?.device is Mouse;
            float sensitivity = mouseLook ? mouseSensitivity : gamepadSensitivity * deltaTime;

            yaw += look.x * sensitivity;
            pitch = Mathf.Clamp(pitch - look.y * sensitivity, minPitch, maxPitch);

            Vector3 targetPosition = target.position + targetOffset;
            smoothedTargetPosition = Vector3.Lerp(
                smoothedTargetPosition,
                targetPosition,
                1f - Mathf.Exp(-followSharpness * deltaTime));

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredDirection = rotation * Vector3.back;
            float resolvedDistance = ResolveCameraDistance(smoothedTargetPosition, desiredDirection);
            transform.SetPositionAndRotation(
                smoothedTargetPosition + desiredDirection * resolvedDistance,
                rotation);
        }

        private void ResolveInputActions()
        {
            if (inputActions == null)
            {
                return;
            }

            playerMap = inputActions.FindActionMap(InputActionIds.MapPlayer, false);
            lookAction = playerMap?.FindAction(InputActionIds.Look, false);
        }

        private float ResolveCameraDistance(Vector3 origin, Vector3 direction)
        {
            if (Physics.SphereCast(
                    origin,
                    collisionRadius,
                    direction,
                    out RaycastHit hit,
                    distance,
                    obstructionLayers,
                    QueryTriggerInteraction.Ignore))
            {
                return Mathf.Clamp(hit.distance - collisionRadius, minDistance, distance);
            }

            return distance;
        }
    }
}
