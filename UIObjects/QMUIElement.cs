namespace BTKUILib.UIObjects
{
    public class QMUIElement
    {
        /// <summary>
        /// ID of the element inside the QuickMenu
        /// </summary>
        public string ElementID;

        /// <summary>
        /// Deletes this element from the QuickMenu
        /// </summary>
        public void Delete()
        {
            QuickMenuAPI.DeleteElement(ElementID);
        }
    }
}