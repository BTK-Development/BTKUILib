using System;
using System.IO;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using BTKUILib.UIObjects;
using BTKUILib.UIObjects.Objects;
using System.Linq;
using ABI_RC.Core.UI.UIRework.Managers;
using UnityEngine;

namespace BTKUILib
{
    /// <summary>
    /// This class contains core utilities and some static pages that are used in the QuickMenu
    /// </summary>
    public static class QuickMenuAPI
    {
        /// <summary>
        /// Called when the Cohtml menu is regenerated
        /// </summary>
        public static Action<CVR_MenuManager> OnMenuRegenerate;

        /// <summary>
        /// Called after BTKUILib has finished generating all menu components and BTKUIReady is set
        /// </summary>
        public static Action<CVR_MenuManager> OnMenuGenerated;

        /// <summary>
        /// Called when a user leaves the instance, passes the complete CVRPlayerEntity object. Some data may be nulled as the player is leaving
        /// </summary>
        public static Action<CVRPlayerEntity> UserLeave;

        /// <summary>
        /// Called when a user joins the instance, passes the complete CVRPlayerEntity object
        /// </summary>
        public static Action<CVRPlayerEntity> UserJoin;

        /// <summary>
        /// Fires when a tab change occurs, this includes when the tab is already focused.
        /// First parameter is the target tab, second is the last tab.
        /// </summary>
        public static Action<string, string> OnTabChange;

        /// <summary>
        /// Called when the user is disconnected from a CVR instance
        /// </summary>
        public static Action OnWorldLeave;

        /// <summary>
        /// Called when a user is selected in the quick menu, passes the username and user ID
        /// </summary>
        public static Action<string, string> OnPlayerSelected;

        /// <summary>
        /// Called when a user is selected in the playerlist, passes that players CVRPlayerEntity
        /// </summary>
        public static Action<UIPlayerObject> OnPlayerEntitySelected;

        /// <summary>
        /// Called when a page change occurs, passes the new target page and the previous page
        /// </summary>
        public static Action<string, string> OnOpenedPage;

        /// <summary>
        /// Called when back is used, passes the target page and the previous page
        /// </summary>
        public static Action<string, string> OnBackAction;

        /// <summary>
        /// Last selected player's username from the User Select menu
        /// </summary>
        public static string SelectedPlayerName;

        /// <summary>
        /// Last selected player's user ID from the User Select menu
        /// </summary>
        public static string SelectedPlayerID;

        /// <summary>
        /// Last selected player's CVRPlayerEntity from the PlayerList page
        /// </summary>
        public static UIPlayerObject SelectedPlayerEntity => new(ABI_RC.Systems.UI.UILib.QuickMenuAPI.SelectedPlayerEntity);

        /// <summary>
        /// Player select page for setting up functions that should be used in the context of a user
        /// </summary>
        public static Page PlayerSelectPage
        {
            get
            {
                _playerListPageAdapter ??= new Page(ABI_RC.Systems.UI.UILib.QuickMenuAPI.PlayerSelectPage);
                return _playerListPageAdapter;
            }
        }

        /// <summary>
        /// Contains the currently opened page element ID
        /// </summary>
        public static string CurrentPageID => ABI_RC.Systems.UI.UILib.QuickMenuAPI.CurrentPageID;

        /// <summary>
        /// Creates or returns a basic Misc tab page for use by mods not requiring a full tab
        /// </summary>
        public static Page MiscTabPage
        {
            get
            {
                _miscTabPageAdapter ??= new Page(ABI_RC.Systems.UI.UILib.QuickMenuAPI.MiscTabPage);
                return _miscTabPageAdapter;
            }
        }

        private static Page _playerListPageAdapter;
        private static Page _miscTabPageAdapter;

        #region Utility Functions

        /// <summary>
        /// Injects your custom CSS Style into UILib, this will automatically be reapplied during a menu reload
        /// </summary>
        /// <param name="cssData"></param>
        public static void InjectCSSStyle(string cssData) =>
            ABI_RC.Systems.UI.UILib.QuickMenuAPI.InjectCSSStyle(cssData);

        /// <summary>
        /// Get the MelonLoader prefs tab page for a specific mod, fetched by identifier
        /// </summary>
        /// <param name="prefsIdentifier">Identifier used for the mods MelonPreferences (MelonPreferences_Category.Identifier)</param>
        /// <returns>The created ML prefs page containing the SubpageButton element</returns>
        public static Page GetMLPrefsPageByIdentifier(string prefsIdentifier)
        {
            return !BTKUILib.Instance.MLPrefsPages.ContainsKey(prefsIdentifier) ? null : BTKUILib.Instance.MLPrefsPages[prefsIdentifier];
        }

        /// <summary>
        /// Prepares icons for usage by dropping them in the correct folder
        ///
        /// Icons should be 256x256 in size to avoid issues with CSS, they also need to be PNGs
        /// </summary>
        /// <param name="modName">Your mod name, this should be the same as your pages</param>
        /// <param name="iconName">Name of the icon to be saved</param>
        /// <param name="resourceStream">Stream containing your image data</param>
        public static void PrepareIcon(string modName, string iconName, Stream resourceStream) => ABI_RC.Systems.UI.UILib.QuickMenuAPI.PrepareIcon(modName, iconName, resourceStream);

        /// <summary>
        /// Check if an icon was prepared already
        /// </summary>
        /// <param name="modName">Your mod name, this should be the same as your pages</param>
        /// <param name="iconName">Name of the icon you're checking for</param>
        /// <returns></returns>
        public static bool DoesIconExist(string modName, string iconName) => ABI_RC.Systems.UI.UILib.QuickMenuAPI.DoesIconExist(modName, iconName);

        /// <summary>
        /// Shows a yes/no confirmation dialog with actions
        /// </summary>
        /// <param name="title">Sets the top title of the dialog window</param>
        /// <param name="content">Sets the main content of the dialog window</param>
        /// <param name="onNo">No/Cancel button action</param>
        /// <param name="onYes">Yes/Confirm button action (Optional)</param>
        /// <param name="yesText">Yes/Confirm button text (Optional, defaults to Yes)</param>
        /// <param name="noText">No/Cancel button text (Optional, defaults to No)</param>
        public static void ShowConfirm(string title, string content, Action onYes, Action onNo = null, string yesText = "Yes", string noText = "No") => ABI_RC.Systems.UI.UILib.QuickMenuAPI.ShowConfirm(title, content, onYes, onNo, yesText, noText);

        /// <summary>
        /// Shows a basic notice dialog with an OK button
        /// </summary>
        /// <param name="title">Sets the top title of the dialog window</param>
        /// <param name="content">Sets the main content of the dialog window</param>
        /// <param name="onOK">Action to be fired upon clicking the OK/Close button</param>
        /// <param name="okText">OK/Close button text</param>
        public static void ShowNotice(string title, string content, Action onOK = null, string okText = "OK") => ABI_RC.Systems.UI.UILib.QuickMenuAPI.ShowNotice(title, content, onOK, okText);

        /// <summary>
        /// Opens the number input page, currently limited to 0-9999
        /// </summary>
        /// <param name="name">Sets the text displayed at the top of the page</param>
        /// <param name="input">Initial number input</param>
        /// <param name="onCompleted">Action to be fired when saving the input</param>
        public static void OpenNumberInput(string name, float input, Action<float> onCompleted) => ABI_RC.Systems.UI.UILib.QuickMenuAPI.OpenNumberInput(name, input, onCompleted);

        /// <summary>
        /// Opens the multiselection page
        /// </summary>
        /// <param name="multiSelection">Generated and populated MultiSelection object to populate the multiselection page</param>
        public static void OpenMultiSelect(MultiSelection multiSelection) => ABI_RC.Systems.UI.UILib.QuickMenuAPI.OpenMultiSelect(multiSelection.InternalMultiSelect);

        /// <summary>
        /// Opens the colour picker panel, you can optionally enable live updating of your callback action.
        /// </summary>
        /// <param name="currentColour">Current colour represented as a HTML colour code</param>
        /// <param name="callback">Callback action to be fired when user saves colour, optionally anytime a slider moves if liveUpdate is true. Returns UnityEngine.Color and string HTML colour code</param>
        /// <param name="liveUpdate">Toggle to set callback to be called anytime a slider moves</param>
        public static void OpenColourPicker(string currentColour, Action<Color, string> callback, bool liveUpdate = false) => ABI_RC.Systems.UI.UILib.QuickMenuAPI.OpenColourPicker(currentColour, callback, liveUpdate);
        /// <summary>
        /// Opens the colour picker panel, you can optionally enable live updating of your callback action.
        /// </summary>
        /// <param name="currentColour">Current colour represented as a UnityEngine color object</param>
        /// <param name="callback">Callback action to be fired when user saves colour, optionally anytime a slider moves if liveUpdate is true. Returns UnityEngine.Color and string HTML colour code</param>
        /// <param name="liveUpdate">Toggle to set callback to be called anytime a slider moves</param>
        public static void OpenColourPicker(Color currentColour, Action<Color, string> callback, bool liveUpdate = false) => ABI_RC.Systems.UI.UILib.QuickMenuAPI.OpenColourPicker(currentColour, callback, liveUpdate);

        /// <summary>
        /// Opens the CVR keyboard
        /// </summary>
        /// <param name="currentValue">Current text in the keyboard</param>
        /// <param name="callback">Action to be called when keyboard text is submitted</param>
        public static void OpenKeyboard(string currentValue, Action<string> callback) => ABI_RC.Systems.UI.UILib.QuickMenuAPI.OpenKeyboard(currentValue, callback);

        /// <summary>
        /// Opens the CVR keyboard, this exposes all functional parameters on the KeyboardManager.ShowKeyboard function
        /// </summary>
        /// <param name="currentText">Current text to pass to the keyboard</param>
        /// <param name="callback">Action to be called when keyboard text is submitted</param>
        /// <param name="placeholder">Placeholder text to show when the text field is empty, can be null</param>
        /// <param name="maxCharacterCount">Max character count allowed on this field</param>
        /// <param name="multiLine">Sets if the keyboard should enable multiline mode</param>
        /// <param name="title">Title to be displayed above the keyboard text field</param>
        public static void OpenKeyboard(string currentText, Action<string> callback, string placeholder, int maxCharacterCount, bool multiLine, string title) => ABI_RC.Systems.UI.UILib.QuickMenuAPI.OpenKeyboard(currentText, callback, placeholder, maxCharacterCount, multiLine, title);

        /// <summary>
        /// Shows an toast alert on the quick menu, stays up for set delay
        /// </summary>
        /// <param name="message">Message to be displayed on the toast</param>
        /// <param name="delay">Delay in seconds before toast is hidden</param>
        public static void ShowAlertToast(string message, int delay = 5) => ABI_RC.Systems.UI.UILib.QuickMenuAPI.ShowAlertToast(message, delay);

        /// <summary>
        /// Calls the back function, moves back 1 page in the breadcrumbs
        /// </summary>
        public static void GoBack() => ABI_RC.Systems.UI.UILib.QuickMenuAPI.GoBack();

        /// <summary>
        /// Forcefully adds a page to the RootPages list, you should only use this if you are doing weird stuff.
        /// For general usage please use the RootPage parameter on the Page constructor!
        /// </summary>
        /// <param name="page">The page to be added to the RootPages list</param>
        public static void AddRootPage(Page page) => ABI_RC.Systems.UI.UILib.QuickMenuAPI.AddRootPage(page.InternalPage);

        /// <summary>
        /// Opens the player settings/action page for the given UserID
        /// </summary>
        /// <param name="userId">Target UserID to open the player settings/action page to</param>
        public static void OpenPlayerListByUserID(string userId) => ABI_RC.Systems.UI.UILib.QuickMenuAPI.OpenPlayerListByUserID(userId);

        /// <summary>
        /// Opens the playerlist in player selection mode
        /// </summary>
        /// <param name="title">Title for the PlayerList while in player selection mode</param>
        /// <param name="callback">Callback to be fired when a player is selected</param>
        public static void OpenPlayerSelector(string title, Action<UIPlayerObject> callback)
        {
            //TODO: wrap UIPlayerObject and handle callback
            
        }

        /// <summary>
        /// Opens the playerlist page
        /// </summary>
        public static void OpenPlayerList() => QuickMenuAPI.OpenPlayerList();

        #endregion
    }
}
