using UnityEngine;

namespace TilkiMacera
{
    public class CameraController : MonoBehaviour
    {
        public Transform target;
        public Vector3 targetOffset = new Vector3(0f, 1.4f, 0f);
        public float distance = 6f;
        public float sensitivity = 3f;
        public float minPitch = -20f;
        public float maxPitch = 62f;
        public float followSmooth = 12f;

        private float yaw;
        private float pitch = 22f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (target == null)
            {
                PlayerController player = FindObjectOfType<PlayerController>();
                if (player != null)
                {
                    target = player.transform;
                }
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                yaw += Input.GetAxis("Mouse X") * sensitivity;
                pitch -= Input.GetAxis("Mouse Y") * sensitivity;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            }

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredPosition = target.position + targetOffset - rotation * Vector3.forward * distance;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, 1f - Mathf.Exp(-followSmooth * Time.deltaTime));
            transform.LookAt(target.position + targetOffset);
        }
    }
}
