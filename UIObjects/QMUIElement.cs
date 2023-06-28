using System;
using ABI_RC.Core.InteractionSystem;
using cohtml;

namespace BTKUILib.UIObjects
{
    /// <summary>
    /// This object is the base class for all other UI elements
    /// </summary>
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

        /// <summary>
        /// Set to prevent changes to some elements (Internal use)
        /// </summary>
        internal bool Protected;

        internal QMUIElement()
        {
            UUID = Guid.NewGuid().ToString();
            UserInterface.QMElements.Add(this);
        }

        /// <summary>
        /// Deletes this element from the QuickMenu
        /// </summary>
        public virtual void Delete()
        {
            if (Protected)
            {
                BTKUILib.Log.Error($"You cannot delete a protected element! ElementID: {ElementID}");
                return;
            }

            UserInterface.QMElements.Remove(this);

            if (!UIUtils.IsQMReady()) return;
            UIUtils.GetQMInternalView().TriggerEvent("btkDeleteElement", ElementID);
        }

        internal virtual void DeleteInternal()
        {
            UserInterface.QMElements.Remove(this);

            if (!UIUtils.IsQMReady()) return;
            UIUtils.GetQMInternalView().TriggerEvent("btkDeleteElement", ElementID);
        }

        /// <summary>
        /// Used to generate the cohtml side of this element, expected to be overriden
        /// </summary>
        internal virtual void GenerateCohtml()
        {
            
        }
    }
}