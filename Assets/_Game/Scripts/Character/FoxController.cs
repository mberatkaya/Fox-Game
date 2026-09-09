using UnityEngine;
using UnityEngine.InputSystem;

namespace TilkiOyunu.Foundation
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class FoxController : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform groundProbe;
        [SerializeField] private LayerMask groundLayers = ~0;
        [SerializeField] private FoxMovementSettings movement = new();

        private CharacterController characterController;
        private InputActionMap playerMap;
        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction sprintAction;
        private Vector3 horizontalVelocity;
        private float verticalVelocity;
        private float postJumpGroundLockTimer;
        private bool jumpAvailable = true;
        private bool inputLocked;

        private const float PostJumpGroundLockSeconds = 0.12f;

        public Vector3 Velocity => horizontalVelocity + Vector3.up * verticalVelocity;
        public Vector2 MoveInput { get; private set; }
        public bool IsGrounded { get; private set; }
        public bool IsSprinting { get; private set; }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            movement.Sanitize();
            ResolveInputActions();
        }

        private void OnValidate()
        {
            movement?.Sanitize();
        }

        private void OnEnable()
        {
            ResolveInputActions();
            playerMap?.Enable();
        }

        private void OnDisable()
        {
            playerMap?.Disable();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            IsGrounded = CheckGrounded();
            if (postJumpGroundLockTimer > 0f)
            {
                postJumpGroundLockTimer = Mathf.Max(0f, postJumpGroundLockTimer - deltaTime);
                IsGrounded = false;
            }

            MoveInput = inputLocked ? Vector2.zero : moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
            IsSprinting = IsGrounded && sprintAction != null && sprintAction.IsPressed() && MoveInput.sqrMagnitude > 0.01f;

            if (IsGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = movement.groundedStickVelocity;
            }

            TryStartJump(jumpAction != null && jumpAction.WasPressedThisFrame());

            Vector3 desiredVelocity = BuildCameraRelativeMove(MoveInput) * (IsSprinting ? movement.sprintSpeed : movement.moveSpeed);
            float control = IsGrounded ? 1f : movement.airControl;
            float response = desiredVelocity.sqrMagnitude > horizontalVelocity.sqrMagnitude
                ? movement.acceleration
                : movement.deceleration;

            horizontalVelocity = Vector3.MoveTowards(
                horizontalVelocity,
                desiredVelocity,
                response * control * deltaTime);

            RotateToward(horizontalVelocity, deltaTime);

            verticalVelocity += movement.gravity * deltaTime;
            Vector3 frameVelocity = horizontalVelocity + Vector3.up * verticalVelocity;
            CollisionFlags collisionFlags = characterController.Move(frameVelocity * deltaTime);
            RefreshJumpAvailabilityAfterMove(collisionFlags);
        }

        public void TeleportTo(Transform spawnPoint)
        {
            if (spawnPoint == null)
            {
                return;
            }

            characterController.enabled = false;
            transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            characterController.enabled = true;
            horizontalVelocity = Vector3.zero;
            verticalVelocity = movement.groundedStickVelocity;
            postJumpGroundLockTimer = 0f;
            jumpAvailable = true;
        }

        public void SetInputLocked(bool locked)
        {
            inputLocked = locked;
            if (locked)
            {
                MoveInput = Vector2.zero;
                IsSprinting = false;
                horizontalVelocity = Vector3.zero;
            }
        }

        private void ResolveInputActions()
        {
            if (inputActions == null)
            {
                return;
            }

            playerMap = inputActions.FindActionMap(InputActionIds.MapPlayer, false);
            moveAction = playerMap?.FindAction(InputActionIds.Move, false);
            jumpAction = playerMap?.FindAction(InputActionIds.Jump, false);
            sprintAction = playerMap?.FindAction(InputActionIds.Sprint, false);
        }

        private Vector3 BuildCameraRelativeMove(Vector2 input)
        {
            Vector2 clampedInput = Vector2.ClampMagnitude(input, 1f);
            if (clampedInput.sqrMagnitude < 0.0001f)
            {
                return Vector3.zero;
            }

            Vector3 forward = cameraTransform != null ? cameraTransform.forward : transform.forward;
            Vector3 right = cameraTransform != null ? cameraTransform.right : transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            return (forward * clampedInput.y + right * clampedInput.x).normalized;
        }

        private void RotateToward(Vector3 velocity, float deltaTime)
        {
            Vector3 flatVelocity = new(velocity.x, 0f, velocity.z);
            if (flatVelocity.sqrMagnitude < 0.001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(flatVelocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                1f - Mathf.Exp(-movement.rotationSpeed * deltaTime));
        }

        private bool CheckGrounded()
        {
            if (characterController.isGrounded)
            {
                return true;
            }

            Vector3 probePosition = groundProbe != null
                ? groundProbe.position
                : transform.position + Vector3.down * ((characterController.height * 0.5f) - characterController.radius + 0.04f);

            return Physics.CheckSphere(
                probePosition,
                movement.groundedProbeRadius,
                groundLayers,
                QueryTriggerInteraction.Ignore);
        }

        private bool TryStartJump(bool jumpPressedThisFrame)
        {
            if (inputLocked || !jumpAvailable || !IsGrounded || verticalVelocity > 0f || !jumpPressedThisFrame)
            {
                return false;
            }

            verticalVelocity = Mathf.Sqrt(movement.jumpHeight * -2f * movement.gravity);
            postJumpGroundLockTimer = PostJumpGroundLockSeconds;
            jumpAvailable = false;
            IsGrounded = false;
            return true;
        }

        private void RefreshJumpAvailabilityAfterMove(CollisionFlags collisionFlags)
        {
            if (((collisionFlags & CollisionFlags.Below) == 0 && !characterController.isGrounded) || verticalVelocity > 0f)
            {
                return;
            }

            jumpAvailable = true;
            postJumpGroundLockTimer = 0f;
            IsGrounded = true;
        }
    }
}
