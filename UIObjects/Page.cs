using System.Collections.Generic;

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

            return category;
        }
    }
}