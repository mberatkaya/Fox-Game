using UnityEngine;

namespace TilkiMacera
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public Transform cameraTransform;
        public float walkSpeed = 4f;
        public float sprintSpeed = 6.5f;
        public float jumpHeight = 1.1f;
        public float gravity = -20f;
        public float turnSmoothTime = 0.08f;
        public float worldRadius = 46f;

        private CharacterController characterController;
        private float verticalVelocity;
        private float turnSmoothVelocity;
        private bool canMove = true;

        public bool CanMove => canMove;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            if (!canMove)
            {
                characterController.Move(Vector3.up * gravity * Time.deltaTime);
                return;
            }

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 input = new Vector3(horizontal, 0f, vertical).normalized;

            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            if (Input.GetButtonDown("Jump") && characterController.isGrounded)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            if (input.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg;
                if (cameraTransform != null)
                {
                    targetAngle += cameraTransform.eulerAngles.y;
                }

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
                characterController.Move(moveDirection.normalized * speed * Time.deltaTime);
            }

            verticalVelocity += gravity * Time.deltaTime;
            characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
            ClampToWorld();
        }

        public void SetCanMove(bool value)
        {
            canMove = value;
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !value;
        }

        private void ClampToWorld()
        {
            Vector3 position = transform.position;
            Vector2 flat = new Vector2(position.x, position.z);
            if (flat.magnitude <= worldRadius)
            {
                return;
            }

            Vector2 clamped = flat.normalized * worldRadius;
            transform.position = new Vector3(clamped.x, position.y, clamped.y);
        }
    }
}
