using System.Collections.Generic;
using ABI_RC.Core.InteractionSystem;
using cohtml;

namespace BTKUILib.UIObjects
{
    public class Page : QMUIElement
    {
        public string PageName = "MainPage";
        public readonly string ModName;
        public List<Category> PageCategories = new();

        public string MenuTitle
        {
            get => _menuTitle;
            set
            {
                _menuTitle = value;
                QuickMenuAPI.UpdateMenuTitle(_menuTitle, _menuSubtitle);
            }
        }

        private string _menuTitle;
        
        public string MenuSubtitle
        {
            get => _menuSubtitle;
            set
            {
                _menuSubtitle = value;
                QuickMenuAPI.UpdateMenuTitle(_menuTitle, _menuSubtitle);
            }
        }

        private string _menuSubtitle;

        internal bool RootPage;

        public Page(string modName, string pageName, bool rootPage = false)
        {
            if(!rootPage)
                PageName = pageName;
            ModName = modName;
            RootPage = rootPage;
            
            UserInterface.RootPages.Add(this);
            
            ElementID = $"btkUI-{UIUtils.GetCleanName(modName)}-{UIUtils.GetCleanName(pageName)}";
        }

        public void OpenPage()
        {
            
        }

        public Category AddCategory(string categoryName)
        {
            var category = new Category(categoryName, this);
            PageCategories.Add(category);

            if (UIUtils.IsQMReady())
            {
                category.GenerateCohtml();
            }

            return category;
        }

        internal override void GenerateCohtml()
        {
            if(!IsGenerated)
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkCreatePage", PageName, ModName, ElementID, true);
            
            IsGenerated = true;
            
            foreach (var category in PageCategories)
            {
                category.GenerateCohtml();
            }
        }
    }
}