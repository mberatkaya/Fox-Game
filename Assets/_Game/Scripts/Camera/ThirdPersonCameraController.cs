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
        private bool externalControl;
        private bool lookInputLocked;

        public Transform Target => target;
        public bool IsExternalControlActive => externalControl;
        public bool IsLookInputLocked => lookInputLocked;

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
            if (target == null || externalControl)
            {
                return;
            }

            float deltaTime = Time.deltaTime;
            Vector2 look = lookInputLocked ? Vector2.zero : lookAction?.ReadValue<Vector2>() ?? Vector2.zero;
            bool mouseLook = lookAction?.activeControl?.device is Mouse;
            float sensitivity = mouseLook ? mouseSensitivity : gamepadSensitivity * deltaTime;

            yaw += look.x * sensitivity;
            pitch = Mathf.Clamp(pitch - look.y * sensitivity, minPitch, maxPitch);

            ApplyFollow(deltaTime, false);
        }

        public void SnapToTarget()
        {
            if (target == null)
            {
                return;
            }

            yaw = target.eulerAngles.y;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            ApplyFollow(0f, true);
        }

        private void ApplyFollow(float deltaTime, bool snap)
        {
            Vector3 targetPosition = target.position + targetOffset;
            smoothedTargetPosition = snap
                ? targetPosition
                : Vector3.Lerp(
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

        public void SetExternalControl(bool enabled)
        {
            if (externalControl == enabled)
            {
                return;
            }

            externalControl = enabled;
            if (!externalControl)
            {
                ResumeFromCurrentTransform();
            }
        }

        public void SetLookInputLocked(bool locked)
        {
            lookInputLocked = locked;
        }

        public void ResumeFromCurrentTransform()
        {
            if (target != null)
            {
                smoothedTargetPosition = target.position + targetOffset;
            }

            Vector3 eulerAngles = transform.rotation.eulerAngles;
            yaw = eulerAngles.y;
            pitch = NormalizePitch(eulerAngles.x);
        }

        private float NormalizePitch(float rawPitch)
        {
            float normalized = rawPitch > 180f ? rawPitch - 360f : rawPitch;
            return Mathf.Clamp(normalized, minPitch, maxPitch);
        }

        private float ResolveCameraDistance(Vector3 origin, Vector3 direction)
        {
            RaycastHit[] hits = Physics.SphereCastAll(
                origin,
                collisionRadius,
                direction,
                distance,
                obstructionLayers,
                QueryTriggerInteraction.Ignore);

            float closestDistance = float.PositiveInfinity;
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.collider == null || IsTargetCollider(hit.collider))
                {
                    continue;
                }

                closestDistance = Mathf.Min(closestDistance, hit.distance);
            }

            if (!float.IsPositiveInfinity(closestDistance))
            {
                return Mathf.Clamp(closestDistance - collisionRadius, minDistance, distance);
            }

            return distance;
        }

        private bool IsTargetCollider(Collider candidate)
        {
            return target != null
                && candidate != null
                && candidate.transform.root == target.root;
        }
    }
}
