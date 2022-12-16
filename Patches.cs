using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
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
            ApplyPatches(typeof(CVRPlayerManagerJoin));
            ApplyPatches(typeof(CVRPlayerEntityLeave));
            ApplyPatches(typeof(ViewManagerPatches));
            
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
        
        private static void UserJoinPatch(CVRPlayerEntity player)
        {
            try
            {
                QuickMenuAPI.UserJoin?.Invoke(player);                
            }
            catch (Exception e)
            {
                BTKUILib.Log.Error(e);
            }
        }
        
        private static void UserLeavePatch(CVRPlayerEntity player)
        {
            try
            {
                QuickMenuAPI.UserLeave?.Invoke(player);                
            }
            catch (Exception e)
            {
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
    
        [HarmonyPatch]
    class CVRPlayerManagerJoin
    {
        private static readonly MethodInfo _targetMethod = typeof(List<CVRPlayerEntity>).GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);
        private static readonly MethodInfo _userJoinMethod = typeof(Patches).GetMethod("UserJoinPatch", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly FieldInfo _playerEntity = typeof(CVRPlayerManager).GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance).Single(t => t.GetField("p") != null).GetField("p");
        
        static MethodInfo TargetMethod()
        {
            return typeof(CVRPlayerManager).GetMethod(nameof(CVRPlayerManager.TryCreatePlayer), BindingFlags.Instance | BindingFlags.Public);
        }
        
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new CodeMatcher(instructions)
                .MatchForward(true, new CodeMatch(OpCodes.Callvirt, _targetMethod))
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Ldfld, _playerEntity),
                    new CodeInstruction(OpCodes.Call, _userJoinMethod)
                )
                .InstructionEnumeration();

            return code;
        }
    }
    
    [HarmonyPatch]
    class CVRPlayerEntityLeave
    {
        private static readonly MethodInfo _userLeaveMethod = typeof(Patches).GetMethod("UserLeavePatch", BindingFlags.Static | BindingFlags.NonPublic);
        
        static MethodInfo TargetMethod()
        {
            return typeof(CVRPlayerEntity).GetMethod(nameof(CVRPlayerEntity.Recycle), BindingFlags.Instance | BindingFlags.Public);
        }
        
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new CodeMatcher(instructions)
                .Advance(1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, _userLeaveMethod)
                )
                .InstructionEnumeration();
            
            return code;
        }
    }

    [HarmonyPatch(typeof(ViewManager))]
    class ViewManagerPatches
    {
        [HarmonyPatch("SendToWorldUi")]
        [HarmonyPostfix]
        static void SendToWorldUi(string value)
        {
            //Ensure that we check if the keyboard action was used within 3 minutes, this will avoid the next keyboard usage triggering the action
            if (DateTime.Now.Subtract(QuickMenuAPI.TimeSinceKeyboardOpen).TotalMinutes <= 3)
                QuickMenuAPI.OnKeyboardSubmitted?.Invoke(value);

            QuickMenuAPI.OnKeyboardSubmitted = null;
        }
    }
}