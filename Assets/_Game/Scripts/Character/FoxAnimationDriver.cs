using UnityEngine;

namespace TilkiOyunu.Foundation
{
    [RequireComponent(typeof(Animator))]
    public sealed class FoxAnimationDriver : MonoBehaviour
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int GroundedHash = Animator.StringToHash("Grounded");
        private static readonly int VerticalVelocityHash = Animator.StringToHash("VerticalVelocity");
        private static readonly int IdleSitHash = Animator.StringToHash("IdleSit");
        private static readonly int IdleBreakHash = Animator.StringToHash("IdleBreak");

        [SerializeField] private FoxController controller;
        [SerializeField] private Animator animator;
        [SerializeField, Min(0.01f)] private float runSpeed = 5.6f;
        [SerializeField, Min(0.01f)] private float dampSeconds = 0.12f;
        [SerializeField, Min(0.5f)] private float secondaryIdleDelay = 6f;
        [SerializeField, Range(0f, 1f)] private float secondaryIdleChance = 0.32f;
        [SerializeField, Min(0.5f)] private float secondaryIdleCooldown = 9f;

        private float idleTimer;
        private float cooldownTimer;
        private int idleSequence;

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            animator.applyRootMotion = false;
        }

        private void Update()
        {
            if (animator == null || controller == null)
            {
                return;
            }

            Vector3 velocity = controller.Velocity;
            float horizontalSpeed = new Vector2(velocity.x, velocity.z).magnitude;
            animator.SetFloat(SpeedHash, Mathf.Clamp01(horizontalSpeed / runSpeed), dampSeconds, Time.deltaTime);
            animator.SetBool(GroundedHash, controller.IsGrounded);
            animator.SetFloat(VerticalVelocityHash, velocity.y, dampSeconds, Time.deltaTime);
            animator.applyRootMotion = false;
            UpdateSecondaryIdle(horizontalSpeed);
        }

        private void UpdateSecondaryIdle(float horizontalSpeed)
        {
            cooldownTimer = Mathf.Max(0f, cooldownTimer - Time.deltaTime);
            bool canIdle = controller.IsGrounded
                && horizontalSpeed < 0.05f
                && controller.MoveInput.sqrMagnitude < 0.001f
                && cooldownTimer <= 0f;

            if (!canIdle)
            {
                idleTimer = 0f;
                return;
            }

            idleTimer += Time.deltaTime;
            if (idleTimer < secondaryIdleDelay)
            {
                return;
            }

            idleTimer = 0f;
            cooldownTimer = secondaryIdleCooldown;
            idleSequence++;
            if (PseudoRandom01(idleSequence) > secondaryIdleChance)
            {
                return;
            }

            animator.SetTrigger((idleSequence & 1) == 0 ? IdleSitHash : IdleBreakHash);
        }

        private static float PseudoRandom01(int value)
        {
            uint x = (uint)(value * 747796405 + 2891336453);
            x = ((x >> ((int)(x >> 28) + 4)) ^ x) * 277803737;
            x = (x >> 22) ^ x;
            return (x & 0x00FFFFFF) / (float)0x01000000;
        }
    }
}
