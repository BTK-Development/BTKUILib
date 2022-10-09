using System;
using System.Collections.Generic;
using ABI_RC.Core.InteractionSystem;
using BTKUILib.UIObjects;
using BTKUILib.UIObjects.Components;
using BTKUILib.UIObjects.Objects;
using cohtml;

namespace BTKUILib
{
    public class QuickMenuAPI
    {
        //Main Actions
        public static Action<CVR_MenuManager> OnMenuRegenerate;
        public static Action<string, string> OnOpenedPage;
        public static Action<string, string> OnBackAction;
        
        //Menu variables and in use objects
        public static MultiSelection SelectedMultiSelect;
        
        //Internal actions for utility functions
        internal static Action NoticeOk;
        internal static Action ConfirmYes;
        internal static Action ConfirmNo;
        internal static Action<float> NumberInputComplete;

        //Basic functionality handlers
        private static Dictionary<string, Action> _buttonHandlers = new();
        private static Dictionary<string, Action<bool>> _toggleHandlers = new();

        #region Creation/Deletion Functions

        public static void DeleteElement(string elementID)
        {
            
        }

        /// <summary>
        /// Creates a new root page in the QuickMenu
        /// </summary>
        /// <param name="modName">The name of the mod creating a root page</param>
        /// <param name="pageName">The name of the page, will be used as a header</param>
        /// <returns></returns>
        public static Page CreateRootPage(string modName, string pageName)
        {
            var page = new Page(modName, pageName, true);
            return page;
        }

        #endregion

        #region Update Functions

        public static void SetToggleState(ToggleButton toggle, bool state)
        {
            
        }

        #endregion
        
        #region Utility Functions
        
        public static void ShowConfirm(string title, string content, string yesText = "Yes", Action onYes = null, string noText = "No", Action onNo = null)
        {
            ConfirmYes = onYes;
            ConfirmNo = onNo;
            
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkShowConfirm", title, content, yesText, noText);
        }
        
        public static void ShowNotice(string title, string content, string okText = "OK", Action onOK = null)
        {
            NoticeOk = onOK;
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkShowNotice", title, content, okText);
        }
        
        public static void OpenNumberInput(string name, float input, Action<float> onCompleted)
        {
            NumberInputComplete = onCompleted;
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkOpenNumberInput", name, input);
        }
        
        #endregion
    }
}