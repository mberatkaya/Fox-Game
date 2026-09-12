using System;
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
        [SerializeField] private LayerMask obstacleLayers = ~0;
        [SerializeField, Min(0.005f)] private float obstacleSkin = 0.035f;
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
        private bool obstacleConstrainedThisFrame;
        private Vector3 lastObstacleNormal;

        private const float PostJumpGroundLockSeconds = 0.12f;
        private const int MaxObstacleSlideIterations = 2;
        private const float MaxBlockingObstacleNormalY = 0.55f;

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
            Vector3 frameMotion = ResolveObstacleMotion(frameVelocity * deltaTime);
            CollisionFlags collisionFlags = characterController.Move(frameMotion);
            if (obstacleConstrainedThisFrame)
            {
                horizontalVelocity = Vector3.ProjectOnPlane(horizontalVelocity, lastObstacleNormal);
                if (horizontalVelocity.sqrMagnitude < 0.01f)
                {
                    horizontalVelocity = Vector3.zero;
                }
            }
            else if ((collisionFlags & CollisionFlags.Sides) != 0)
            {
                horizontalVelocity = Vector3.zero;
            }

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

        private Vector3 ResolveObstacleMotion(Vector3 motion)
        {
            obstacleConstrainedThisFrame = false;
            lastObstacleNormal = Vector3.up;

            Vector3 horizontalMotion = new(motion.x, 0f, motion.z);
            if (horizontalMotion.sqrMagnitude <= 0.000001f || characterController == null || !characterController.enabled)
            {
                return motion;
            }

            Vector3 verticalMotion = Vector3.up * motion.y;
            Vector3 resolvedHorizontal = ResolveHorizontalObstacleMotion(horizontalMotion, Vector3.zero, 0);
            return resolvedHorizontal + verticalMotion;
        }

        private Vector3 ResolveHorizontalObstacleMotion(Vector3 desiredMotion, Vector3 capsuleOffset, int iteration)
        {
            if (iteration >= MaxObstacleSlideIterations || desiredMotion.sqrMagnitude <= 0.000001f)
            {
                return Vector3.zero;
            }

            if (!TryFindBlockingObstacle(capsuleOffset, desiredMotion, out RaycastHit hit))
            {
                return desiredMotion;
            }

            obstacleConstrainedThisFrame = true;
            lastObstacleNormal = hit.normal;

            float distance = desiredMotion.magnitude;
            Vector3 direction = desiredMotion / distance;
            float allowedDistance = Mathf.Max(0f, hit.distance - obstacleSkin);
            Vector3 allowedMotion = direction * allowedDistance;
            Vector3 remainingMotion = desiredMotion - allowedMotion;
            Vector3 slideMotion = Vector3.ProjectOnPlane(remainingMotion, hit.normal);
            slideMotion.y = 0f;

            if (Vector3.Dot(slideMotion, desiredMotion) <= 0f || slideMotion.sqrMagnitude <= 0.000001f)
            {
                return allowedMotion;
            }

            return allowedMotion + ResolveHorizontalObstacleMotion(slideMotion, capsuleOffset + allowedMotion, iteration + 1);
        }

        private bool TryFindBlockingObstacle(Vector3 capsuleOffset, Vector3 motion, out RaycastHit blockingHit)
        {
            blockingHit = default;
            if (!TryGetCharacterCapsule(capsuleOffset, out Vector3 point1, out Vector3 point2, out float radius))
            {
                return false;
            }

            float distance = motion.magnitude;
            if (distance <= 0.0001f)
            {
                return false;
            }

            RaycastHit[] hits = Physics.CapsuleCastAll(
                point1,
                point2,
                radius,
                motion / distance,
                distance + obstacleSkin,
                obstacleLayers,
                QueryTriggerInteraction.Ignore);

            if (hits.Length == 0)
            {
                return false;
            }

            Array.Sort(hits, static (left, right) => left.distance.CompareTo(right.distance));
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.collider == null || IsSelfCollider(hit.collider) || hit.normal.y > MaxBlockingObstacleNormalY)
                {
                    continue;
                }

                blockingHit = hit;
                return true;
            }

            return false;
        }

        private bool TryGetCharacterCapsule(Vector3 offset, out Vector3 point1, out Vector3 point2, out float radius)
        {
            point1 = Vector3.zero;
            point2 = Vector3.zero;
            radius = 0f;
            if (characterController == null)
            {
                return false;
            }

            radius = Mathf.Max(0.01f, characterController.radius - obstacleSkin);
            float height = Mathf.Max(characterController.height, radius * 2f);
            float segmentHalfHeight = Mathf.Max(0f, (height * 0.5f) - radius);
            Vector3 center = transform.TransformPoint(characterController.center) + offset;
            Vector3 up = transform.up;
            point1 = center + up * segmentHalfHeight;
            point2 = center - up * segmentHalfHeight;
            return true;
        }

        private bool IsSelfCollider(Collider candidate)
        {
            return candidate == characterController || candidate.transform.IsChildOf(transform);
        }
    }
}
