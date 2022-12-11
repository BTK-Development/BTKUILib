using System;

namespace BTKUILib.UIObjects
{
    public class QMUIElement
    {
        /// <summary>
        /// ID of the element inside the QuickMenu
        /// </summary>
        public string ElementID;

        /// <summary>
        /// Generated UUID to keep track of events from cohtml related to this element
        /// </summary>
        public string UUID;

        public QMUIElement()
        {
            UUID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Deletes this element from the QuickMenu
        /// </summary>
        public void Delete()
        {
            QuickMenuAPI.DeleteElement(this);
        }
    }
}