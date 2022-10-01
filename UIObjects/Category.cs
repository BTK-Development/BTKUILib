using ABI_RC.Core.InteractionSystem;

namespace BTKUILib.UIObjects
{
    /// <summary>
    /// This act as Bootstrap rows within Cohtml
    /// </summary>
    public class Category
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

        private void UpdateCategoryName()
        {
            
        }
    }
}