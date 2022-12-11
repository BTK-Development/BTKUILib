using System;
using System.Collections;
using ABI_RC.Core.InteractionSystem;
using cohtml;
using HarmonyLib;
using MelonLoader;

namespace BTKUILib
{
    internal class Patches
    {
        private static HarmonyLib.Harmony _modInstance;
        
        internal static void Initialize(HarmonyLib.Harmony modInstance)
        {
            _modInstance = modInstance;
            
            ApplyPatches(typeof(CVRMenuManagerPatch));
            
            BTKUILib.Log.Msg("Applied patches!");
        }
        
        private static void ApplyPatches(Type type)
        {
            MelonDebug.Msg($"Applying {type.Name} patches!");
            try
            {
                _modInstance.PatchAll(type);
            }
            catch (Exception e)
            {
                BTKUILib.Log.Error($"Failed while patching {type.Name}!");
                BTKUILib.Log.Error(e);
            }
        }
    }
    
    [HarmonyPatch(typeof(CVR_MenuManager))]
    class CVRMenuManagerPatch
    {
        [HarmonyPatch("markMenuAsReady")]
        [HarmonyPostfix]
        static void MarkMenuAsReady(CVR_MenuManager __instance)
        {
            try
            {
                QuickMenuAPI.OnMenuRegenerate?.Invoke(__instance);
            }
            catch (Exception e)
            {
                BTKUILib.Log.Error(e);
            }
        }
    }
}