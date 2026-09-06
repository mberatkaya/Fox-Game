using UnityEngine;

namespace TilkiOyunu.Foundation
{
    public sealed class CardMatchingStart : MonoBehaviour, IInteractable
    {
        [SerializeField] private CardMatchingController controller;
        [SerializeField] private string interactionLabel = "Oyunu Başlat";

        public string InteractionLabel => interactionLabel;

        public bool CanInteract(InteractionContext context)
        {
            return controller != null && controller.CanStartRun;
        }

        public void Interact(InteractionContext context)
        {
            if (CanInteract(context))
            {
                controller.Open();
            }
        }
    }
}
