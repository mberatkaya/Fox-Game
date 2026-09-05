namespace TilkiOyunu.Foundation
{
    public interface IInteractable
    {
        string InteractionLabel { get; }
        bool CanInteract(InteractionContext context);
        void Interact(InteractionContext context);
    }
}
