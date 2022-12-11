using ABI_RC.Core.InteractionSystem;

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
                
            }
        }

        private string _categoryName;

        internal Category(string categoryName, Page page)
        {
            
        }
        
        private void UpdateCategoryName()
        {
            
        }
    }
}