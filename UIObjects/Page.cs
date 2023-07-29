using System.Collections.Generic;
using ABI_RC.Core.InteractionSystem;
using BTKUILib.UIObjects.Components;

namespace BTKUILib.UIObjects
{
    /// <summary>
    /// This object represents a page that exists in Cohtml
    /// </summary>
    public class Page : QMUIElement
    {
        /// <summary>
        /// Get or set the menu title displayed at the very top of the QM, will update on the fly
        /// </summary>
        public string MenuTitle
        {
            get => _menuTitle;
            set
            {
                _menuTitle = value;
                if(!Protected)
                    QuickMenuAPI.UpdateMenuTitle(_menuTitle, _menuSubtitle);
            }
        }

        /// <summary>
        /// Get or set the menu subtitle displayed at the very top of the QM, will update on the fly
        /// </summary>
        public string MenuSubtitle
        {
            get => _menuSubtitle;
            set
            {
                _menuSubtitle = value;
                if(!Protected)
                    QuickMenuAPI.UpdateMenuTitle(_menuTitle, _menuSubtitle);
            }
        }

        /// <summary>
        /// Reference to the button that opens this subpage
        /// </summary>
        public Button SubpageButton
        {
            get => _subpageButton;
            internal set => _subpageButton = value;
        }

        internal bool RootPage;
        internal string PageName = "MainPage";
        internal readonly string ModName;
        internal List<QMUIElement> PageElements = new();
        internal bool InPlayerlist = false;
        private Button _subpageButton;

        private string _menuSubtitle;
        private string _tabIcon;
        private string _menuTitle;
        private Category _category;
        private string _tabID;

        /// <summary>
        /// Create a new page object, this will automatically be created within Cohtml when it is ready
        /// </summary>
        /// <param name="modName">Name of your mod, you can use this to have multiple mods use the same root tab</param>
        /// <param name="pageName">Name of the page, this isn't visible anywhere</param>
        /// <param name="rootPage">Sets if this page should also generate a tab</param>
        /// <param name="tabIcon">Icon to be displayed on the tab</param>
        /// <param name="category">Only set if this page was created from a category</param>
        public Page(string modName, string pageName, bool rootPage = false, string tabIcon = null, Category category = null)
        {
            if(!rootPage)
                PageName = pageName;
            ModName = modName;
            RootPage = rootPage;
            _tabIcon = tabIcon;
            _category = category;

            if(!rootPage)
            {
                ElementID = $"btkUI-{UIUtils.GetCleanString(modName)}-{UIUtils.GetCleanString(pageName)}";
            }
            else
            {
                ElementID = $"btkUI-{UIUtils.GetCleanString(modName)}-MainPage";
                _tabID = $"btkUI-Tab-{UIUtils.GetCleanString(modName)}";
            }
            
            if (rootPage) 
                UserInterface.Instance.RegisterRootPage(this);
        }

        /// <summary>
        /// Internal use only, maps this page element to an existing element in the menu
        /// </summary>
        /// <param name="elementID">ElementID matching the existing element</param>
        internal Page(string elementID)
        {
            Protected = true;
            ModName = "BTKUILib";
            UserInterface.RootPages.Add(this);
            ElementID = elementID;
        }

        /// <summary>
        /// Opens this page in Cohtml
        /// </summary>
        public void OpenPage()
        {
            if (!UIUtils.IsQMReady() || !IsGenerated) return;
            
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkPushPage", ElementID);
        }

        /// <summary>
        /// Add a new category (row) to this page
        /// </summary>
        /// <param name="categoryName">Name of the category, displayed at the top</param>
        /// <returns>A newly created category</returns>
        public Category AddCategory(string categoryName)
        {
            var category = new Category(categoryName, this);
            PageElements.Add(category);

            if (UIUtils.IsQMReady()) 
                category.GenerateCohtml();

            return category;
        }
        
        /// <summary>
        /// Add a new category to this page, for use on Protected pages only
        /// </summary>
        /// <param name="categoryName">Name of the category, displayed at the top</param>
        /// <param name="modName">Name of the mod creating the category, should match other usages</param>
        /// <returns>A newly created category</returns>
        public Category AddCategory(string categoryName, string modName)
        {
            if(!Protected)
                BTKUILib.Log.Warning("You should not be using AddCategory(categoryName, modName) on your created pages! This is only intended for special protected pages! (PlayerSelectPage and Misc page)");
            
            var category = new Category(categoryName, this, true, modName);
            PageElements.Add(category);

            if (UIUtils.IsQMReady()) 
                category.GenerateCohtml();

            return category;
        }

        /// <summary>
        /// Create a slider on the page
        /// </summary>
        /// <param name="sliderName">Name of the slider, displayed above the slider</param>
        /// <param name="sliderTooltip">Tooltip displayed when hovering on the slider</param>
        /// <param name="initialValue">Initial value of the slider</param>
        /// <param name="minValue">Minimum value that the slider can slide to</param>
        /// <param name="maxValue">Maximum value the slider can slide to</param>
        /// <returns></returns>
        public SliderFloat AddSlider(string sliderName, string sliderTooltip, float initialValue, float minValue, float maxValue)
        {
            return AddSlider(sliderName, sliderTooltip, initialValue, minValue, maxValue, 2, 0f, false);
        }

        /// <summary>
        /// Create a slider on the page
        /// </summary>
        /// <param name="sliderName">Name of the slider, displayed above the slider</param>
        /// <param name="sliderTooltip">Tooltip displayed when hovering on the slider</param>
        /// <param name="initialValue">Initial value of the slider</param>
        /// <param name="minValue">Minimum value that the slider can slide to</param>
        /// <param name="maxValue">Maximum value the slider can slide to</param>
        /// <param name="decimalPlaces">Set the number of decimal places displayed on the slider</param>
        /// <returns></returns>
        public SliderFloat AddSlider(string sliderName, string sliderTooltip, float initialValue, float minValue, float maxValue, int decimalPlaces)
        {
            return AddSlider(sliderName, sliderTooltip, initialValue, minValue, maxValue, decimalPlaces, 0f, false);
        }

        /// <summary>
        /// Create a slider on the page
        /// </summary>
        /// <param name="sliderName">Name of the slider, displayed above the slider</param>
        /// <param name="sliderTooltip">Tooltip displayed when hovering on the slider</param>
        /// <param name="initialValue">Initial value of the slider</param>
        /// <param name="minValue">Minimum value that the slider can slide to</param>
        /// <param name="maxValue">Maximum value the slider can slide to</param>
        /// <param name="decimalPlaces">Set the number of decimal places displayed on the slider</param>
        /// <param name="defaultValue">Default value for this slider</param>
        /// <param name="allowReset">Allow this slider to be reset using the reset button</param>
        /// <returns></returns>
        public SliderFloat AddSlider(string sliderName, string sliderTooltip, float initialValue, float minValue, float maxValue, int decimalPlaces, float defaultValue, bool allowReset)
        {
            var slider = new SliderFloat(this, sliderName, sliderTooltip, initialValue, minValue, maxValue, decimalPlaces, defaultValue, allowReset);
            PageElements.Add(slider);
            
            if(UIUtils.IsQMReady())
                slider.GenerateCohtml();

            return slider;
        }

        /// <inheritdoc />
        public override void Delete()
        {
            base.Delete();

            if (Protected) return;
            
            if (RootPage)
            {
                UserInterface.RootPages.Remove(this);
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkDeleteElement", _tabID);
            }

            //Remove this page from the category list
            if(_category != null && _category.CategoryElements.Contains(this))
                _category.CategoryElements.Remove(this);
        }
        
        /// <summary>
        /// Deletes all children of this category
        /// </summary>
        public void ClearChildren()
        {
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkClearChildren", ElementID + "-Content");
        }

        internal override void DeleteInternal()
        {
            base.DeleteInternal();
            
            if (RootPage)
            {
                UserInterface.RootPages.Remove(this);
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkDeleteElement", _tabID);
            }

            //Remove this page from the category list
            if(_category != null && _category.CategoryElements.Contains(this))
                _category.CategoryElements.Remove(this);
        }
        
        internal override void GenerateCohtml()
        {
            if(!IsGenerated)
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkCreatePage", PageName, ModName, _tabIcon, ElementID, RootPage, UIUtils.GetCleanString(PageName), InPlayerlist);
            
            IsGenerated = true;
            
            foreach (var category in PageElements)
            {
                category.GenerateCohtml();
            }
        }
    }
}