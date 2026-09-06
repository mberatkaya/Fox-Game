namespace TilkiOyunu.Foundation
{
    public sealed class LightPathStart : UnityEngine.MonoBehaviour, IInteractable
    {
        [UnityEngine.SerializeField] private LightPathController controller;
        [UnityEngine.SerializeField] private string interactionLabel = "Işık Yolunu Başlat";

        public string InteractionLabel => interactionLabel;

        public bool CanInteract(InteractionContext context)
        {
            return controller != null && controller.CanStartRun;
        }

        public void Interact(InteractionContext context)
        {
            if (CanInteract(context))
            {
                controller.StartRun();
            }
        }
    }
}
