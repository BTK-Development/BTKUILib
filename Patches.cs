using System;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Networking.API;
using ABI_RC.Core.Networking.API.Responses;
using ABI_RC.Core.Networking.IO.Social;
using ABI_RC.Systems.GameEventSystem;
using HarmonyLib;
using MelonLoader;
using System.Threading.Tasks;

namespace BTKUILib
{
    internal class Patches
    {
        internal static UserDetailsResponse LocalUserDetails;

        private static HarmonyLib.Harmony _modInstance;
        private static bool _firstOnLoadComplete;
        
        internal static void Initialize(HarmonyLib.Harmony modInstance)
        {
            _modInstance = modInstance;
            
            ApplyPatches(typeof(CVRMenuManagerPatch));
            ApplyPatches(typeof(ViewManagerPatches));
            ApplyPatches(typeof(UsersPatch));

            CVRGameEventSystem.Instance.OnConnectionLost.AddListener((message) =>
            {
                try
                {
                    QuickMenuAPI.OnWorldLeave?.Invoke();
                }
                catch (Exception e)
                {
                    BTKUILib.Log.Error(e);
                }
            });

            CVRGameEventSystem.Instance.OnConnected.AddListener(msg =>
            {
                PlayerList.Instance.OnWorldJoin();
            });

            CVRGameEventSystem.Instance.OnConnectionRecovered.AddListener(s =>
            {
                if (PlayerList.Instance == null) return;

                try
                {
                    PlayerList.Instance.ResetListAfterConnRecovery();
                }
                catch (Exception e)
                {
                    BTKUILib.Log.Error(e);
                }
            });
            
            CVRGameEventSystem.Instance.OnDisconnected.AddListener(s =>
            {
                try
                {
                    QuickMenuAPI.OnWorldLeave?.Invoke();
                }
                catch (Exception e)
                {
                    BTKUILib.Log.Error(e);
                }
            });

            CVRGameEventSystem.Authentication.OnLogin.AddListener(resp =>
            {
                Task.Run(async () =>
                {
                    var profileRequest = (await ApiConnection.MakeRequest<UserDetailsResponse>(ApiConnection.ApiOperation.UserDetails, new
                    {
                        userID = resp.UserId
                    }, null, false)).Data;

                    LocalUserDetails = profileRequest;

                }).ContinueWith(t =>
                {
                    if (!t.IsFaulted) return;
                    BTKUILib.Log.Error("Unable to retrieve local user details! User entry in PlayerList will have no image!");
                });
            });
            
            CVRGameEventSystem.World.OnLoad.AddListener((message) =>
            {
                if (_firstOnLoadComplete) return;

                _firstOnLoadComplete = true;

                CVRGameEventSystem.Player.OnJoinEntity.AddListener(entity =>
                {
                    try
                    {
                        QuickMenuAPI.UserJoin?.Invoke(entity);
                    }
                    catch (Exception e)
                    {
                        BTKUILib.Log.Error(e);
                    }
                });

                CVRGameEventSystem.Player.OnLeaveEntity.AddListener(entity =>
                {
                    try
                    {
                        QuickMenuAPI.UserLeave?.Invoke(entity);
                    }
                    catch (Exception e)
                    {
                        BTKUILib.Log.Error(e);
                    }
                });
            });

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
        [HarmonyPatch("RegisterEvents")]
        [HarmonyPostfix]
        static void MarkMenuAsReady(CVR_MenuManager __instance)
        {
            try
            {
                UserInterface.Instance?.OnMenuRegenerate();
            }
            catch (Exception e)
            {
                BTKUILib.Log.Error(e);
            }
        }

        //We'll use this point to detect a menu reload/setup and ensure BTKUIReady is false
        [HarmonyPatch("UpdateModList")]
        [HarmonyPrefix]
        static bool UpdateModListPatch()
        {
            UserInterface.BTKUIReady = false;

            return true;
        }
    }

    [HarmonyPatch(typeof(Users))]
    class UsersPatch
    {
        [HarmonyPatch(nameof(Users.ShowDetails))]
        [HarmonyPrefix]
        static bool ShowDetailsPrefix(string userId)
        {
            if (!CVR_MenuManager.Instance.IsQuickMenuOpen || !BTKUILib.Instance.QMPlayerSelectorRedirect.Value) return true;

            //QM is open, redirect selection to playerlist
            QuickMenuAPI.OpenPlayerListByUserID(userId);
            return false;
        }
    }

    [HarmonyPatch(typeof(ViewManager))]
    class ViewManagerPatches
    {
        [HarmonyPatch("SendToWorldUi")]
        [HarmonyPostfix]
        static void SendToWorldUi(string value)
        {
            var elapsedTime = DateTime.Now.Subtract(QuickMenuAPI.TimeSinceKeyboardOpen);

            //Ensure that we check if the keyboard action was used within 3 minutes, this will avoid the next keyboard usage triggering the action
            if (elapsedTime.TotalMinutes <= 3 && (!QuickMenuAPI.KeyboardCloseFired || elapsedTime.TotalSeconds <= 10))
                QuickMenuAPI.OnKeyboardSubmitted?.Invoke(value);

            QuickMenuAPI.OnKeyboardSubmitted = null;
        }

        [HarmonyPatch(nameof(ViewManager.KeyboardClosed))]
        [HarmonyPostfix]
        static void KeyboardClosedPatch()
        {
            //Update cause the keyboard has been closed
            QuickMenuAPI.TimeSinceKeyboardOpen = DateTime.Now;
            QuickMenuAPI.KeyboardCloseFired = true;
        }
    }
}