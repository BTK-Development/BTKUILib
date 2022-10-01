using System;
using System.Collections.Generic;
using BTKUILib.UIObjects.Components;
using BTKUILib.UIObjects.Objects;

namespace BTKUILib
{
    public class QuickMenuAPI
    {
        //Main Actions
        public static Action OnMenuRegenerate;
        public static Action<string, string> OnOpenedPage;
        public static Action<string, string> OnBackAction;
        
        //Menu variables and in use objects
        public static MultiSelection SelectedMultiSelect;

        //Basic functionality handlers
        private static Dictionary<string, Action> _buttonHandlers = new();
        private static Dictionary<string, Action<bool>> _toggleHandlers = new();

        #region Creation Functions

        

        #endregion

        #region Update Functions

        public static void SetToggleState(ToggleButton toggle, bool state)
        {
            
        }

        #endregion
    }
}