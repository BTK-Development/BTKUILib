using System.IO;
using System.Reflection;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Networking;
using ABI_RC.Core.Networking.IO.Self;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Systems.GameEventSystem;
using ABI_RC.Systems.UI.UILib.Features;
using ABI_RC.Systems.UI.UILib.UIObjects;
using MelonLoader;
using UnityEngine;

namespace BTKUILib
{
    /// <summary>
    /// Basic utilities used within the UI
    /// </summary>
    public static class UIUtils
    {
        private static FieldInfo _qmuiElementProtected = typeof(QMUIElement).GetField("Protected", BindingFlags.Instance | BindingFlags.NonPublic);
        private static FieldInfo _internalUILibSettingsCat = typeof(QuickMenuSettings).GetField("UISettingsMainCategory", BindingFlags.Static | BindingFlags.NonPublic);
        
        /// <summary>
        /// Check if the CVR_MenuManager view is ready
        /// </summary>
        /// <returns>True if view is ready, false if it's not</returns>
        public static bool IsQMReady() => CVR_MenuManager.Instance.IsReady;
        
        /// <summary>
        /// Clean non alphanumeric characters from a given string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Cleaned string</returns>
        public static string GetCleanString(string input) => ABI_RC.Systems.UI.UILib.UIUtils.GetCleanString(input);
        
        /// <summary>
        /// Get stream from an EmbeddedResource with a given name
        /// </summary>
        /// <param name="iconName"></param>
        /// <returns></returns>
        public static Stream GetIconStream(string iconName)
        {
            var melon = MelonUtils.GetMelonFromStackTrace();

            string assemblyName = melon.MelonAssembly.Assembly.GetName().Name;
            return melon.MelonAssembly.Assembly.GetManifestResourceStream($"{assemblyName}.Resources.{iconName}");
        }

        /// <summary>
        /// Gets the private Animator from PuppetMaster
        /// </summary>
        /// <param name="pm">Target puppet master</param>
        /// <returns>Private avatar animator</returns>
        public static Animator GetAvatarAnimator(PuppetMaster pm) => pm.Animator;

        /// <summary>
        /// Gets the username of the local user
        /// </summary>
        /// <returns>Local users username</returns>
        public static string GetSelfUsername()
        {
            return AuthManager.Username;
        }
        
        internal static void SetProtected(this UIObjects.QMUIElement qmuiElement, bool value)
        {
            _qmuiElementProtected.SetValue(qmuiElement.InternalElement, value);
        }

        internal static Category GetInternalSettingsPage()
        {
            return (Category)_internalUILibSettingsCat.GetValue(null);
        }
    }
}