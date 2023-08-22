using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using BTKUILib.UIObjects;
using BTKUILib.UIObjects.Components;
using BTKUILib.UIObjects.Objects;
using cohtml;
using MelonLoader;
using MelonLoader.ICSharpCode.SharpZipLib.Zip;

namespace BTKUILib
{
    internal class UserInterface
    {
        internal static UserInterface Instance;
        
        internal static List<Page> RootPages = new();
        internal static List<QMUIElement> QMElements = new();
        internal static Dictionary<string, SliderFloat> Sliders = new();
        internal static Dictionary<string, QMInteractable> Interactables = new();
        internal static bool BTKUIReady; 
        internal MultiSelection SelectedMultiSelect;

        private string _lastTab = "CVRMainQM";

        internal void SetupUI()
        {
            Instance = this;
            
            QuickMenuAPI.UserJoin += UserJoin;
            QuickMenuAPI.UserLeave += UserLeave;
            QuickMenuAPI.OnWorldLeave += OnWorldLeave;
            
            BTKUILib.Log.Msg("Checking if BTKUI is updated...");
            CheckUpdateUI();
        }

        internal void OnMenuRegenerate()
        {
            MelonDebug.Msg("Registering events");

            BTKUIReady = false;

            foreach (var element in QMElements)
                element.IsGenerated = false;
            
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-ButtonAction", new Action<string>(HandleButtonAction));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-Toggle", new Action<string, bool>(OnToggle));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-PopupConfirmOK", new Action(ConfirmOK));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-PopupConfirmNo", new Action(ConfirmNo));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-PopupNoticeOK", new Action(NoticeClose));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-SliderValueUpdated", new Action<string, string>(OnSliderUpdated));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-OpenedPage", new Action<string, string>(OnOpenedPageEvent));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-BackAction", new Action<string, string>(OnBackActionEvent));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-DropdownSelected", new Action<int>(DropdownSelected));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-NumSubmit", new Action<string>(OnNumberInputSubmitted));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-RootCreated", new Action<string, string>(OnRootCreated));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-TabChange", new Action<string>(OnTabChange));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-SelectedPlayer", new Action<string, string>(OnSelectedPlayer));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-UILoaded", new Action(OnMenuIsLoaded));
        }

        private void OnMenuIsLoaded()
        {
            MelonDebug.Msg("BTKUILib menu is loaded, setting up!");
            
            QuickMenuAPI.OnMenuRegenerate?.Invoke(CVR_MenuManager.Instance);
            
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkModInit");

            //Run the ml prefs tab generation
            BTKUILib.Instance.GenerateMlPrefsTab();

            //Set BTKUIReady before creating elements, but after generated MLPrefs tab to ensure ordering isn't weird
            BTKUIReady = true;
            
            //Begin creating the UI elements!
            foreach (var root in RootPages)
            {
                MelonDebug.Msg($"Creating root page | Name: {root.PageName} | ModName: {root.ModName} | ElementID: {root.ElementID}");
                root.GenerateCohtml();
            }
            
            BTKUILib.Log.Msg($"Setup {RootPages.Count} root pages! BTKUILib is ready!");
        }

        internal void RegisterRootPage(Page rootPage)
        {
            RootPages.Add(rootPage);

            if (!UIUtils.IsQMReady()) return;
            
            MelonDebug.Msg($"Creating root page | Name: {rootPage.PageName} | ModName: {rootPage.ModName} | ElementID: {rootPage.ElementID}");
            rootPage.GenerateCohtml();
        }

        private void UserLeave(CVRPlayerEntity obj)
        {
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkRemovePlayer", obj.Uuid, CVRPlayerManager.Instance.NetworkPlayers.Count);
        }

        private void UserJoin(CVRPlayerEntity obj)
        {
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkAddPlayer", obj.Username, obj.Uuid, obj.ApiProfileImageUrl, CVRPlayerManager.Instance.NetworkPlayers.Count);
        }
        
        private void OnWorldLeave()
        {
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkLeaveWorld");
        }

        #region Cohtml Event Functions
        
        private void OnSelectedPlayer(string playerName, string playerID)
        {
            QuickMenuAPI.SelectedPlayerName = playerName;
            QuickMenuAPI.SelectedPlayerID = playerID;
            QuickMenuAPI.OnPlayerSelected?.Invoke(playerName, playerID);
        }
        
        private void OnTabChange(string tabTarget)
        {
            if (tabTarget == "CVRMainQM")
            {
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkChangeTab", tabTarget, "CVR", "", "");
                QuickMenuAPI.OnTabChange?.Invoke(tabTarget, _lastTab);
                _lastTab = tabTarget;
                return;
            }

            var root = RootPages.FirstOrDefault(x => x.ElementID == tabTarget);

            if (root == null)
            {
                BTKUILib.Log.Error("RootPage was not found! Cannot switch tabs!");
                return;
            }
            
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkChangeTab", tabTarget, root.ModName, root.MenuTitle, root.MenuSubtitle);
            QuickMenuAPI.OnTabChange?.Invoke(tabTarget, _lastTab);
            _lastTab = tabTarget;
        }

        private void OnRootCreated(string elementID, string uuid)
        {
            var page = RootPages.FirstOrDefault(x => x.UUID == uuid);

            if (page == null)
            {
                BTKUILib.Log.Error("OnRootCreated fired but the UUID did not match an existing page?!");
                return;
            }

            page.ElementID = elementID;
        }

        private void OnNumberInputSubmitted(string input)
        {
            if (!float.TryParse(input, out var inputFloat))
            {
                QuickMenuAPI.ShowNotice("Invalid Input!", "You entered a value that is not a valid input!");
                return;
            }

            if (inputFloat > 9999 || inputFloat < -9999)
            {
                QuickMenuAPI.ShowNotice("Invalid Input!", "You entered a value that's outside the limits of 9999 and -9999! You must keep your value within those limits.");
                return;
            }
            
            QuickMenuAPI.NumberInputComplete?.Invoke(inputFloat);
            QuickMenuAPI.NumberInputComplete = null;
        }
        
        private void DropdownSelected(int index)
        {
            if (SelectedMultiSelect != null)
                SelectedMultiSelect.SelectedOption = index;
        }
        
        private void OnBackActionEvent(string targetPage, string lastPage)
        {
            QuickMenuAPI.OnBackAction?.Invoke(targetPage, lastPage);
        }

        private void OnOpenedPageEvent(string targetPage, string lastPage)
        {
            QuickMenuAPI.OnOpenedPage?.Invoke(targetPage, lastPage);
        }
        
        private void OnSliderUpdated(string sliderID, string value)
        {
            if (!float.TryParse(value, out var valueFloat))
                return;

            if (!Sliders.ContainsKey(sliderID)) return;

            Sliders[sliderID].SliderValue = valueFloat;
        }

        private void OnToggle(string toggleID, bool state)
        {
            if (!Interactables.ContainsKey(toggleID))
            {
                BTKUILib.Log.Error($"OnToggle triggered for toggle that is not registered as an interactable! {toggleID}");
                return;
            }
            
            Interactables[toggleID].OnInteraction(state);
        }
        
        private void NoticeClose()
        {
            QuickMenuAPI.NoticeOk?.Invoke();
        }

        private void ConfirmNo()
        {
            QuickMenuAPI.ConfirmNo?.Invoke();
        }

        private void ConfirmOK()
        {
            QuickMenuAPI.ConfirmYes?.Invoke();
        }
        
        private void HandleButtonAction(string buttonUUID)
        {
            if (!Interactables.ContainsKey(buttonUUID))
            {
                BTKUILib.Log.Error($"{buttonUUID} is not registered as an interactable!");
                return;
            }
            
            Interactables[buttonUUID].OnInteraction();
        }

        #endregion
        
        private void CheckUpdateUI()
        {
            bool updateNeeded = true;
            string resourceHash;
            
            using (var uiResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BTKUILib.BTKUIBuild.zip"))
            {
                if (uiResourceStream == null)
                {
                    BTKUILib.Log.Error("Unable to load BTKUI Resource! BTKUI may not function correctly!");
                    return;
                }
                
                using var tempStream = new MemoryStream((int) uiResourceStream.Length);
                uiResourceStream.CopyTo(tempStream);
                
                resourceHash = UIUtils.CreateMD5(tempStream.ToArray());
                
                var uiDir = new DirectoryInfo("ChilloutVR_Data\\StreamingAssets\\Cohtml\\UIResources\\GameUI\\mods\\BTKUI");
                if (uiDir.Exists)
                {
                    var file = uiDir.GetFiles().FirstOrDefault(x => x.Name.Equals("BTKUIBuildHash"));

                    if (file != null)
                    {
                        var fileHash = File.ReadAllText(file.FullName);

                        updateNeeded = !resourceHash.Equals(fileHash, StringComparison.InvariantCultureIgnoreCase);
                    }
                }
                else
                {
                    uiDir.Create();
                }

                if (updateNeeded && resourceHash != null)
                {
                    BTKUILib.Log.Msg("BTKUI needs to be updated, extracting updated resources!");
                    
                    var fastZip = new FastZip();
                    
                    fastZip.ExtractZip(uiResourceStream, uiDir.FullName, FastZip.Overwrite.Always, null, "", "", true, true);
                    
                    File.WriteAllText(Path.Combine(uiDir.FullName, "BTKUIBuildHash"), resourceHash);
                }
                
                if(!updateNeeded)
                    BTKUILib.Log.Msg("BTKUI is up to date!");
            }
        }
    }
}