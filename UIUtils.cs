using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ABI_RC.Core.InteractionSystem;

namespace BTKUILib
{
    /// <summary>
    /// Basic utilities used within the UI
    /// </summary>
    public static class UIUtils
    {
        private static MD5 _hasher = MD5.Create();
        private static FieldInfo _qmReady = typeof(CVR_MenuManager).GetField("_quickMenuReady", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Check if the CVR_MenuManager view is ready
        /// </summary>
        /// <returns>True if view is ready, false if it's not</returns>
        public static bool IsQMReady()
        {
            if (CVR_MenuManager.Instance == null)
                return false;

            return (bool)_qmReady.GetValue(CVR_MenuManager.Instance);
        }
        
        /// <summary>
        /// Clean non alphanumeric characters from a given string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Cleaned string</returns>
        public static string GetCleanString(string input)
        {
            return Regex.Replace(Regex.Replace(input, "<.*?>", string.Empty), @"[^0-9a-zA-Z_]+", string.Empty);
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
    }
}