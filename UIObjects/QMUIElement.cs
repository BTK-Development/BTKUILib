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
        /// Root page a given element is a child of, can be null in some cases
        /// mainly for root pages and for global custom elements
        /// </summary>
        public Page RootPage;

        public bool IsVisible
        {
            get
            {
                if (RootPage != null)
                    return RootPage.IsVisible && IsGenerated;

                return _visible && IsGenerated;
            }
            internal set
            {
                if(RootPage == null)
                    _visible = value;
            }
        }

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
                UIUtils.GetInternalView().TriggerEvent("btkSetDisabled", ElementID, value);
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
        private bool _visible;

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
            UIUtils.GetInternalView().TriggerEvent("btkDeleteElement", ElementID);
        }

        /// <summary>
        /// Used to generate the cohtml side of this element, expected to be overriden
        /// </summary>
        internal virtual void GenerateCohtml(Page rootPage)
        {
            RootPage = rootPage;

            if (!UIUtils.IsQMReady()) return;
            UIUtils.GetInternalView().TriggerEvent("btkSetDisabled", ElementID, _disabled);
        }
    }
}