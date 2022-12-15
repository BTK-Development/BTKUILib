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

        public static void DeleteElement(QMUIElement element)
        {
            if (!UIUtils.IsQMReady()) return;
            
        }

        #endregion

        #region Update Functions

        internal static void UpdateMenuTitle(string title, string subtitle)
        {
            if (!BTKUILib.Instance.IsOnMainThread())
            {
                BTKUILib.Instance.MainThreadQueue.Enqueue(() =>
                {
                    UpdateMenuTitle(title, subtitle);
                });
                return;
            }
            
            if (!UIUtils.IsQMReady()) return;
            
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkUpdateTitle", title, subtitle);
        }

        #endregion
        
        #region Utility Functions
        
        public static void ShowConfirm(string title, string content, string yesText = "Yes", Action onYes = null, string noText = "No", Action onNo = null)
        {
            if (!UIUtils.IsQMReady()) return;
            
            ConfirmYes = onYes;
            ConfirmNo = onNo;
            
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkShowConfirm", title, content, yesText, noText);
        }
        
        public static void ShowNotice(string title, string content, string okText = "OK", Action onOK = null)
        {
            if (!UIUtils.IsQMReady()) return;
            
            NoticeOk = onOK;
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkShowNotice", title, content, okText);
        }
        
        public static void OpenNumberInput(string name, float input, Action<float> onCompleted)
        {
            if (!UIUtils.IsQMReady()) return;
            
            NumberInputComplete = onCompleted;
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkOpenNumberInput", name, input);
        }
        
        public static void OpenMultiSelect(MultiSelection multiSelection)
        {
            UserInterface.Instance.SelectedMultiSelect = multiSelection;
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkOpenMultiSelect", multiSelection.Name, multiSelection.Options, multiSelection.SelectedOption);
        }
        
        #endregion
    }
}