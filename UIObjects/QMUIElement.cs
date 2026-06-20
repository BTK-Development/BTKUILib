using System;
using System.Collections.Generic;

namespace BTKUILib.UIObjects
{
    /// <summary>
    /// This object is the base class for all other UI elements
    /// </summary>
    public class QMUIElement
    {
        internal ABI_RC.Systems.UI.UILib.UIObjects.QMUIElement InternalElement
        {
            get => _internalElement;
            set
            {
                if (_internalElement != null)
                    _internalElement.UpdatedQMUIElement -= UpdateQMUIElement;
                
                _internalElement = value;
                
                _internalElement.UpdatedQMUIElement += UpdateQMUIElement;
                UpdateQMUIElement();
            }
        }
        
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
        public QMUIElement Parent 
        {
            get
            {
                _adapterParentElement ??= new QMUIElement(InternalElement.Parent);
                return _adapterParentElement;
            }
        }

        /// <summary>
        /// Returns the root page of this element by walking up the parents
        /// </summary>
        public QMUIElement RootPage
        {
            get
            {
                _adapterRootElement ??= new QMUIElement(InternalElement.RootPage);
                return _adapterRootElement;
            }
        }

        /// <summary>
        /// Returns the visibility state of this objects highest root page element
        /// </summary>
        public bool IsVisible => InternalElement.IsVisible;

        /// <summary>
        /// Hidden entirely hides the target element, if set on a page it'll hide the page button too
        /// </summary>
        public virtual bool Hidden
        {
            get => InternalElement.Hidden;
            set => InternalElement.Hidden = value;
        }

        /// <summary>
        /// Disabled will block input and gray out the element it is set on
        /// </summary>
        public virtual bool Disabled
        {
            get => InternalElement.Disabled;
            set => InternalElement.Disabled = value;
        }

        /// <summary>
        /// Controls the column count for a given object, you can go between 0 and 12
        /// </summary>
        public virtual int? ColumnCount
        {
            get => InternalElement.ColumnCount;
            set
            {
                if (value != null) 
                    InternalElement.ColumnCount = value.Value;
            }
        }

        private QMUIElement _adapterParentElement;
        private QMUIElement _adapterRootElement;
        private ABI_RC.Systems.UI.UILib.UIObjects.QMUIElement _internalElement;

        internal QMUIElement(ABI_RC.Systems.UI.UILib.UIObjects.QMUIElement internalElement)
        {
            InternalElement = internalElement;

            InternalElement.UpdatedQMUIElement += UpdateQMUIElement;
            UpdateQMUIElement();
        }

        internal QMUIElement()
        {
            
        }

        /// <summary>
        /// Deletes this element from the QuickMenu
        /// </summary>
        public virtual void Delete()
        {
            InternalElement.Delete();
        }

        private void UpdateQMUIElement()
        {
            ElementID = InternalElement.ElementID;
            UUID = InternalElement.UUID;
            IsGenerated = InternalElement.IsGenerated;
        }
    }
}