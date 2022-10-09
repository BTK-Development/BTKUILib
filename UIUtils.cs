using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ABI_RC.Core.InteractionSystem;

namespace BTKUILib
{
    public class UIUtils
    {
        private static MD5 _hasher = MD5.Create();
        private static FieldInfo _qmReady = typeof(CVR_MenuManager).GetField("_quickMenuReady", BindingFlags.Instance | BindingFlags.NonPublic);
        
        public static bool IsQMReady()
        {
            if (CVR_MenuManager.Instance == null)
                return false;

            return (bool)_qmReady.GetValue(CVR_MenuManager.Instance);
        }
        
        public static string GetCleanName(string name)
        {
            return Regex.Replace(Regex.Replace(name, "<.*?>", string.Empty), @"[^0-9a-zA-Z_]+", string.Empty);
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