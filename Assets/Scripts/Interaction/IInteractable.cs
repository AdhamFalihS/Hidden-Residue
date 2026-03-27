namespace HiddenResidue.Interaction
{
    public interface IInteractable
    {
        void Interact();
        bool CanInteract { get; }
        string InteractPrompt { get; }
    }
}