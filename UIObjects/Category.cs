using ABI_RC.Core.InteractionSystem;
using BTKUILib.UIObjects.Components;

namespace BTKUILib.UIObjects
{
    /// <summary>
    /// This act as category with header and row within Cohtml
    /// </summary>
    public class Category : QMUIElement
    {
        /// <summary>
        /// Category name, will update on the fly
        /// </summary>
        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                UpdateCategoryName();
            }
        }

        internal string ModName => _modName ?? LinkedPage.ModName;

        internal readonly Page LinkedPage;
        private string _categoryName;
        private readonly string _modName;
        private bool _showHeader = false;

        internal Category(string categoryName, Page page, bool showHeader = true, string modName = null)
        {
            _categoryName = categoryName;
            LinkedPage = page;
            _showHeader = showHeader;
            _modName = modName;
            
            ElementID = "btkUI-Row-" + UUID;
        }
        
        /// <summary>
        /// Creates a simple button
        /// </summary>
        /// <param name="buttonText">Text to be displayed on the button</param>
        /// <param name="buttonIcon">Icon for the button</param>
        /// <param name="buttonTooltip">Tooltip to be displayed when hovering on the button</param>
        /// <returns></returns>
        public Button AddButton(string buttonText, string buttonIcon, string buttonTooltip)
        {
            return AddButton(buttonText, buttonIcon, buttonTooltip, ButtonStyle.TextWithIcon);
        }

        /// <summary>
        /// Creates a simple button
        /// </summary>
        /// <param name="buttonText">Text to be displayed on the button</param>
        /// <param name="buttonIcon">Icon for the button</param>
        /// <param name="buttonTooltip">Tooltip to be displayed when hovering on the button</param>
        /// <param name="style">Sets the button style, this cannot be changed after creation!</param>
        /// <returns></returns>
        public Button AddButton(string buttonText, string buttonIcon, string buttonTooltip, ButtonStyle style)
        {
            var button = new Button(buttonText, buttonIcon, buttonTooltip, this, style);
            SubElements.Add(button);
            
            if(UIUtils.IsQMReady())
                button.GenerateCohtml();

            return button;
        }

        /// <summary>
        /// Simple toggle element
        /// </summary>
        /// <param name="toggleText">Text to be displayed on toggle</param>
        /// <param name="toggleTooltip">Tooltip to be displayed when hovering on the toggle</param>
        /// <param name="state">Initial state of the toggle</param>
        /// <returns>Newly created toggle object</returns>
        public ToggleButton AddToggle(string toggleText, string toggleTooltip, bool state)
        {
            var toggle = new ToggleButton(toggleText, toggleTooltip, state, this);
            SubElements.Add(toggle);
            
            if(UIUtils.IsQMReady())
                toggle.GenerateCohtml();

            return toggle;
        }

        /// <summary>
        /// Create a new subpage as well as the button required to open it
        /// </summary>
        /// <param name="pageName">Name of the new page, this will appear at the top of the page</param>
        /// <param name="pageIcon">Icon to be used on the button</param>
        /// <param name="pageTooltip">Tooltip to be displayed when hovering on the button</param>
        /// <param name="modName">Mod name, this should be the same as your root page</param>
        /// <returns>Newly created page object with SubpageButton set to the created button</returns>
        public Page AddPage(string pageName, string pageIcon, string pageTooltip, string modName)
        {
            var page = new Page(modName, pageName);
            SubElements.Add(page);

            if (modName == "BTKUILib" && LinkedPage.ElementID == "btkUI-PlayerSelectPage")
            {
                page.InPlayerlist = true;
            }

            var pageButton = new Button(pageName, pageIcon, pageTooltip, this);
            SubElements.Add(pageButton);
            pageButton.OnPress += () =>
            {
                page.OpenPage();
            };

            if (UIUtils.IsQMReady())
            {
                page.GenerateCohtml();
                pageButton.GenerateCohtml();
            }

            page.SubpageButton = pageButton;

            return page;
        }

        /// <inheritdoc />
        public override void Delete()
        {
            //Delete the row header with the row
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkDeleteElement", ElementID + "-HeaderRoot");
            
            base.Delete();
            if (Protected) return;
            LinkedPage.SubElements.Remove(this);
        }

        /// <summary>
        /// Deletes all children of this category
        /// </summary>
        public void ClearChildren()
        {
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkClearChildren", ElementID);
        }

        internal override void GenerateCohtml()
        {
            if (!UIUtils.IsQMReady()) return;

            if(!IsGenerated)
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkCreateRow", LinkedPage.ElementID, UUID, _showHeader ? _categoryName : null);
            
            foreach(var element in SubElements)
                element.GenerateCohtml();
            
            base.GenerateCohtml();

            IsGenerated = true;
        }
        
        private void UpdateCategoryName()
        {
            if (!BTKUILib.Instance.IsOnMainThread())
            {
                BTKUILib.Instance.MainThreadQueue.Enqueue(UpdateCategoryName);
                return;
            }
            
            if (!UIUtils.IsQMReady()) return;
            
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkUpdateText", $"btkUI-Row-{UUID}-HeaderText", _categoryName);
        }
    }
}