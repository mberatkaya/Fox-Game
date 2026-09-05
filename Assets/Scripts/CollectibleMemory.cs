using UnityEngine;

namespace TilkiMacera
{
    public class CollectibleMemory : MonoBehaviour
    {
        public string memoryId = "memory";
        public string title = "Ani";
        [TextArea(2, 4)] public string message = "Kucuk ve guzel bir ani.";
        public float bobHeight = 0.2f;
        public float spinSpeed = 45f;

        private Vector3 startPosition;

        private void Start()
        {
            startPosition = transform.position;
            if (QuestManager.Instance.HasCollectedMemory(memoryId))
            {
                gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
            transform.position = startPosition + Vector3.up * (Mathf.Sin(Time.time * 2.2f) * bobHeight);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponentInParent<PlayerController>())
            {
                return;
            }

            if (QuestManager.Instance.TryCollectMemory(memoryId, title, message))
            {
                gameObject.SetActive(false);
            }
        }
    }
}
