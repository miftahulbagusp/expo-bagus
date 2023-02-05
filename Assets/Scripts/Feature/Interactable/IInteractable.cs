public interface IInteractable
{
    public bool IsInteractable(out string interactionName);
    public void Interact(NonVRPlayerAvatar interactor);
}
