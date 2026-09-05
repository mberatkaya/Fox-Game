namespace TilkiMacera
{
    public interface IInteractable
    {
        string InteractionText { get; }
        void Interact(PlayerInteractor interactor);
    }
}
