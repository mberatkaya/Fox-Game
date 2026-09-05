using UnityEngine;

namespace TilkiMacera
{
    public class LightPathNode : MonoBehaviour
    {
        public int sequenceIndex;
        public Renderer targetRenderer;
        public Color activeColor = new Color(1f, 0.82f, 0.25f);
        public Color inactiveColor = new Color(0.35f, 0.45f, 0.75f);
        public Color completedColor = new Color(0.35f, 1f, 0.55f);

        private bool isActive;
        private bool isCompleted;

        private void Awake()
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<Renderer>();
            }
        }

        public void SetState(bool active, bool completed)
        {
            isActive = active;
            isCompleted = completed;
            if (targetRenderer != null)
            {
                Color color = completed ? completedColor : active ? activeColor : inactiveColor;
                targetRenderer.material.color = color;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isActive && other.GetComponentInParent<PlayerController>())
            {
                MinigameManager.Instance.TouchLightNode(this);
            }
        }
    }
}
