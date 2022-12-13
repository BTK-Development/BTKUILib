namespace BTKUILib.UIObjects;

public class QMInteractable : QMUIElement
{
    internal QMInteractable()
    {
        UserInterface.Interactables.Add(UUID, this);
    }

    internal virtual void OnInteraction(bool? toggle = null)
    {
        
    }
}