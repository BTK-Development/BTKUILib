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

        /// <summary>
        /// Set when element is generated in Cohtml
        /// </summary>
        public bool IsGenerated;

        public QMUIElement()
        {
            UUID = Guid.NewGuid().ToString();
            UserInterface.QMElements.Add(this);
        }

        /// <summary>
        /// Deletes this element from the QuickMenu
        /// </summary>
        public void Delete()
        {
            QuickMenuAPI.DeleteElement(this);
        }

        /// <summary>
        /// Used to generate the cohtml side of this element, expected to be overriden
        /// </summary>
        internal virtual void GenerateCohtml()
        {
            
        }
    }
}