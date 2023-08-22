using System;
using System.Collections.Generic;
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
        /// Disabled will block input and gray out the element it is set on
        /// </summary>
        public bool Disabled
        {
            get => _disabled;
            set
            {
                var thisType = GetType();

                //Don't allow pages to be disabled
                if (thisType == typeof(Page)) return;

                _disabled = value;

                if (!UIUtils.IsQMReady()) return;
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkSetDisabled", ElementID, value);
            }
        }

        /// <summary>
        /// Set to prevent changes to some elements (Internal use)
        /// </summary>
        internal bool Protected;

        /// <summary>
        /// This list contains elements that are children of this element (categories/pages)
        /// </summary>
        internal List<QMUIElement> SubElements = new();

        private bool _disabled;

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

            DeleteInternal();
        }

        internal virtual void DeleteInternal()
        {
            UserInterface.QMElements.Remove(this);

            //Recursively delete sub elements that need special handling
            foreach (var element in SubElements)
            {
                switch (element)
                {
                    case Category:
                    case Page:
                        element.DeleteInternal();
                        break;
                }
            }

            if (!UIUtils.IsQMReady()) return;
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkDeleteElement", ElementID);
        }

        /// <summary>
        /// Used to generate the cohtml side of this element, expected to be overriden
        /// </summary>
        internal virtual void GenerateCohtml()
        {
            if (!UIUtils.IsQMReady()) return;
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkSetDisabled", ElementID, _disabled);
        }
    }
}