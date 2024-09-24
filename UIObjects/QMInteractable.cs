namespace BTKUILib.UIObjects
{

    /// <summary>
    /// This object is the base class for button and toggles, UniqueName must be set to allow element to be pinned to Quick Access
    /// </summary>
    public class QMInteractable : QMUIElement
    {
        /// <summary>
        /// Returns if this element can be pinned or not
        /// </summary>
        public bool CanBePinned { get; private set; }

        internal QMInteractable(string uniqueName, string modName, string rootPage)
        {
            if (uniqueName == null) return;

            UUID = $"{modName}+{rootPage}+{uniqueName}";
            CanBePinned = true;

            UserInterface.Interactables.Add(UUID, this);
        }

        internal virtual void OnInteraction(bool? toggle = null)
        {

        }

        internal override void DeleteInternal(bool tabChange = false)
        {
            base.DeleteInternal(tabChange);

            UserInterface.Interactables.Remove(UUID);
        }
    }
}