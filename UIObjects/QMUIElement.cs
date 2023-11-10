using System;
using System.Collections.Generic;

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
        /// Reference to the parent QMUIElement this element is a child of
        /// Root pages will be null, so will global custom elements
        /// </summary>
        public QMUIElement Parent { get; internal set; }

        /// <summary>
        /// Returns the root page of this element by walking up the parents
        /// </summary>
        public QMUIElement RootPage
        {
            get
            {
                if (_cachedRootPage != null)
                    return _cachedRootPage;

                _cachedRootPage = this;

                while (_cachedRootPage.Parent != null)
                {
                    _cachedRootPage = _cachedRootPage.Parent;
                }

                return _cachedRootPage;
            }
        }

        public bool IsVisible
        {
            get
            {
                return RootPage != null ? RootPage._visible : _visible;
            }
            internal set
            {
                if(RootPage == this)
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

                var target = ElementID;

                //Disabling page disables the subpage button if it's applicable
                if (thisType == typeof(Page))
                {
                    var page = (Page)this;
                    if(page.SubpageButton != null)
                        page.SubpageButton.Disabled = value;
                    return;
                }

                _disabled = value;

                if (!UIUtils.IsQMReady()) return;
                UIUtils.GetInternalView().TriggerEvent("btkSetDisabled", target, value);
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
        private QMUIElement _cachedRootPage;

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

        internal virtual void DeleteInternal(bool tabChange = false)
        {
            if(!tabChange)
                UserInterface.QMElements.Remove(this);

            IsGenerated = false;

            //Recursively delete sub elements that need special handling
            foreach (var element in SubElements.ToArray())
            {
                element.IsGenerated = false;

                switch (element)
                {
                    case Category:
                    case Page:
                        element.DeleteInternal(tabChange);
                        break;
                }
            }

            if (!UIUtils.IsQMReady()) return;
            UIUtils.GetInternalView().TriggerEvent("btkDeleteElement", ElementID);
        }

        /// <summary>
        /// Used to generate the cohtml side of this element, expected to be overriden
        /// </summary>
        internal virtual void GenerateCohtml()
        {
            if (!UIUtils.IsQMReady()) return;
            UIUtils.GetInternalView().TriggerEvent("btkSetDisabled", ElementID, _disabled);
        }
    }
}