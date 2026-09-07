using UnityEngine;

namespace TilkiOyunu.Foundation
{
    [RequireComponent(typeof(Animator))]
    public sealed class FoxAnimationDriver : MonoBehaviour
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int GroundedHash = Animator.StringToHash("Grounded");
        private static readonly int VerticalVelocityHash = Animator.StringToHash("VerticalVelocity");

        [SerializeField] private FoxController controller;
        [SerializeField] private Animator animator;
        [SerializeField, Min(0.01f)] private float runSpeed = 5.6f;
        [SerializeField, Min(0.01f)] private float dampSeconds = 0.12f;

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
        }
    }
}
