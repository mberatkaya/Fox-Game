using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class TestInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionLabel = "Inspect";
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private Light feedbackLight;
        [SerializeField] private Color idleColor = new(0.95f, 0.72f, 0.24f);
        [SerializeField] private Color activeColor = new(0.35f, 0.95f, 0.62f);

        private bool active;

        public string InteractionLabel => interactionLabel;

        private void Awake()
        {
            ApplyState();
        }

        public bool CanInteract(InteractionContext context)
        {
            return context.Actor != null;
        }

        public void Interact(InteractionContext context)
        {
            active = !active;
            ApplyState();
            AppLog.Info(LogCategory.Input, $"Test interactable toggled by {context.Actor.name}.");
        }

        private void ApplyState()
        {
            if (targetRenderer != null)
            {
                targetRenderer.material.color = active ? activeColor : idleColor;
            }

            if (feedbackLight != null)
            {
                feedbackLight.enabled = active;
            }
        }
    }
}
