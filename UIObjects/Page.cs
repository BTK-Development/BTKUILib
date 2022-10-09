using System.Collections.Generic;

namespace BTKUILib.UIObjects
{
    public class Page : QMUIElement
    {
        public string PageName;
        public readonly string ModName;
        public List<Category> PageCategories = new();

        internal bool RootPage;

        public Page(string modName, string pageName)
        {
            PageName = pageName;
            ModName = modName;
            RootPage = false;

            ElementID = $"btkUI-{UIUtils.GetCleanName(modName)}-{UIUtils.GetCleanName(pageName)}";
        }

        internal Page(string modName, string pageName, bool rootPage)
        {
            PageName = pageName;
            ModName = modName;
            RootPage = rootPage;
            
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