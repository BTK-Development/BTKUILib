using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.UI;
using cohtml.Net;
using MelonLoader;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BTKUILib
{
    /// <summary>
    /// Basic utilities used within the UI
    /// </summary>
    public static class UIUtils
    {
        private static MD5 _hasher = MD5.Create();
        private static FieldInfo _internalCohtmlView = typeof(CohtmlControlledViewWrapper).GetField("_view", BindingFlags.Instance | BindingFlags.NonPublic);
        private static View _internalViewCache;

        /// <summary>
        /// Check if the CVR_MenuManager view is ready
        /// </summary>
        /// <returns>True if view is ready, false if it's not</returns>
        public static bool IsQMReady()
        {
            if (CVR_MenuManager.Instance == null)
                return false;

            return UserInterface.BTKUIReady;
        }

        /// <summary>
        /// Clean non alphanumeric characters from a given string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Cleaned string</returns>
        public static string GetCleanString(string input)
        {
            return input == null ? null : Regex.Replace(Regex.Replace(input, "<.*?>", string.Empty), @"[^0-9a-zA-Z_]+", string.Empty);
        }

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
        
        internal static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            return CreateMD5(inputBytes);
        }

        internal static string CreateMD5(byte[] bytes)
        {
            byte[] hashBytes = _hasher.ComputeHash(bytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }

            return sb.ToString();
        }

        internal static View GetInternalView()
        {
            if (CVR_MenuManager.Instance == null || CVR_MenuManager.Instance.quickMenu == null) return null;

            if (_internalViewCache == null)
                _internalViewCache = (View)_internalCohtmlView.GetValue(CVR_MenuManager.Instance.quickMenu.View);

            return _internalViewCache;
        }

        internal static string[] GetPrettyEnumNames<T>() where T : Enum
        {
            return Enum.GetNames(typeof(T)).Select(PrettyFormatEnumName).ToArray();
        }

        internal static int GetEnumIndex<T>(T value) where T : Enum
        {
            return Array.IndexOf(Enum.GetValues(typeof(T)), value);
        }

        private static string PrettyFormatEnumName(string name)
        {
            // adds spaces before capital letters (excluding the first letter)
            return System.Text.RegularExpressions.Regex.Replace(name, "(\\B[A-Z])", " $1");
        }
    }
}