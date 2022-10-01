using System.Reflection;
using ABI_RC.Core.InteractionSystem;

namespace BTKUILib
{
    public class UIUtils
    {
        private static FieldInfo _qmReady = typeof(CVR_MenuManager).GetField("_quickMenuReady", BindingFlags.Instance | BindingFlags.NonPublic);
        
        public static bool IsQMReady()
        {
            if (CVR_MenuManager.Instance == null)
                return false;

            return (bool)_qmReady.GetValue(CVR_MenuManager.Instance);
        }
    }
}