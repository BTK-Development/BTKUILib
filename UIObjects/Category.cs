using ABI_RC.Core.InteractionSystem;
using cohtml;

namespace BTKUILib.UIObjects
{
    /// <summary>
    /// This act as category with header and row within Cohtml
    /// </summary>
    public class Category : QMUIElement
    {
        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                UpdateCategoryName();
            }
        }

        private string _categoryName;
        private Page _linkedPage;
        private bool _showHeader = false;

        internal Category(string categoryName, Page page, bool showHeader = true)
        {
            _categoryName = categoryName;
            _linkedPage = page;
            _showHeader = showHeader;
            
            page.PageCategories.Add(this);
            ElementID = "btkUI-Row-" + UUID;
        }

        internal void GenerateCohtml()
        {
            if(!IsGenerated)
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkCreateRow", _linkedPage.ElementID, UUID, _showHeader ? _categoryName : null);

            IsGenerated = true;
        }
        
        private void UpdateCategoryName()
        {
            if (!UIUtils.IsQMReady()) return;
            
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkUpdateText", $"btkUI-Row-HeaderText-{UUID}", _categoryName);
        }
    }
}