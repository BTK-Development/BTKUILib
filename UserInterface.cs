using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Networking;
using ABI_RC.Core.Savior;
using BTKUILib.UIObjects;
using BTKUILib.UIObjects.Components;
using BTKUILib.UIObjects.Objects;
using DarkRift;
using MelonLoader;
using MelonLoader.ICSharpCode.SharpZipLib.Zip;

namespace BTKUILib
{
    internal class UserInterface
    {
        internal static UserInterface Instance;
        
        internal readonly static List<Page> RootPages = new();
        internal readonly static List<QMUIElement> QMElements = new();
        internal readonly static Dictionary<string, TextInput> TextInputs = new();
        internal readonly static Dictionary<string, SliderFloat> Sliders = new();
        internal readonly static Dictionary<string, QMInteractable> Interactables = new();
        internal readonly static List<string> CustomCSSStyles = new();
        internal readonly static List<CustomElement> CustomElements = new();
        internal readonly static Dictionary<string, List<Page>> ModPages = new();
        internal readonly static Dictionary<string, Category> Categories = new();
        internal readonly static List<Page> GeneratedPages = new();
        internal static bool BTKUIReady;
        internal static bool IsInPlayerList;
        internal MultiSelection SelectedMultiSelect;
        internal static Page SelectedRootPage;

        private string _lastTab = "CVRMainQM";

        internal void SetupUI()
        {
            Instance = this;
            
            BTKUILib.Log.Msg("Checking if BTKUI is updated...");
            CheckUpdateUI();
        }

        internal void OnMenuRegenerate()
        {
            MelonDebug.Msg("Registering events");

            BTKUIReady = false;

            foreach (var element in QMElements)
            {
                element.IsGenerated = false;
                element.IsVisible = false;
            }

            foreach (var root in RootPages)
            {
                root.TabGenerated = false;
            }

            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-ButtonAction", new Action<string>(HandleButtonAction));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-Toggle", new Action<string, bool>(OnToggle));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-PopupConfirmOK", new Action(ConfirmOK));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-PopupConfirmNo", new Action(ConfirmNo));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-PopupNoticeOK", new Action(NoticeClose));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-SliderValueUpdated", new Action<string, string, bool>(OnSliderUpdated));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-OpenedPage", new Action<string, string>(OnOpenedPageEvent));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-BackAction", new Action<string, string>(OnBackActionEvent));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-DropdownSelected", new Action<int>(DropdownSelected));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-NumSubmit", new Action<string>(OnNumberInputSubmitted));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-RootCreated", new Action<string, string>(OnRootCreated));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-TabChange", new Action<string>(OnTabChange));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-UILoaded", new Action(OnMenuIsLoaded));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-CollapseCategory", new Action<string, bool>(OnCollapseCategory));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-TextInputClick", new Action<string>(OnTextInputClock));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-ButtonMouseDown", new Action<string>(ButtonMouseDown));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-ExpandPlayerProfile", new Action(ExpandPlayerProfile));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-ExpandPlayerList", new Action(ExpandPlayerList));
        }

        private void ExpandPlayerList()
        {
            if (string.IsNullOrEmpty(MetaPort.Instance.CurrentInstanceId) || NetworkManager.Instance.GameNetwork.ConnectionState != ConnectionState.Connected)
            {
                QuickMenuAPI.ShowAlertToast("You are not connected to an instance!");
                return;
            }

            CVR_MenuManager.Instance.ToggleQuickMenu(false);
            ViewManager.Instance.GetInstanceDetails(MetaPort.Instance.CurrentInstanceId, true);
        }

        private void ExpandPlayerProfile()
        {
            CVR_MenuManager.Instance.ToggleQuickMenu(false);
            ViewManager.Instance.RequestUserDetailsPage(PlayerList.Instance.SelectedPlayer.Uuid);
        }

        private void ButtonMouseDown(string buttonID)
        {
            if (!Interactables.TryGetValue(buttonID, out var buttonInt)) return;
            if (buttonInt is not Button button) return;

            //We have our button now
            button.MouseDown();
        }

        private void OnTextInputClock(string elementID)
        {
            if (!TextInputs.TryGetValue(elementID, out var textInput)) return;

            QuickMenuAPI.OpenKeyboard(textInput.Text, s =>
            {
                textInput.Text = s;
            });
        }

        private void OnCollapseCategory(string rowID, bool state)
        {
            if (!Categories.TryGetValue(rowID, out var category)) return;

            category.Collapsed = state;

            category.OnCollapse?.Invoke(state);
        }

        private void OnMenuIsLoaded()
        {
            MelonDebug.Msg("BTKUILib menu is loaded, setting up!");
            
            QuickMenuAPI.OnMenuRegenerate?.Invoke(CVR_MenuManager.Instance);

            foreach(var css in CustomCSSStyles)
                UIUtils.GetInternalView().TriggerEvent("btkSetCustomCSS", css);
            
            UIUtils.GetInternalView().TriggerEvent("btkModInit", Enum.GetName(typeof(PlayerListStyleEnum), BTKUILib.Instance.PlayerListStyle.Value));

            //Run the ml prefs tab generation
            BTKUILib.Instance.GenerateMlPrefsTab();

            //Generate our settings page
            BTKUILib.Instance.GenerateSettingsPage();

            //Set BTKUIReady before creating elements, but after generated MLPrefs tab to ensure ordering isn't weird
            BTKUIReady = true;
            IsInPlayerList = false;
            
            //Begin creating the UI elements!
            foreach (var root in RootPages)
            {
                MelonDebug.Msg($"Creating root page | Name: {root.PageName} | ModName: {root.ModName} | ElementID: {root.ElementID}");
                root.GenerateTab();
            }

            //Generate custom elements
            foreach (var custom in CustomElements.Where(x => x.ElementType == ElementType.GlobalElement))
            {
                custom.GenerateCohtml();
            }

            QuickMenuAPI.PlayerSelectPage.IsVisible = true;
            QuickMenuAPI.PlayerSelectPage.GenerateCohtml();
            BTKUILib.UISettingsPage.IsVisible = true;
            BTKUILib.UISettingsPage.GenerateCohtml();

            QuickMenuAPI.OnMenuGenerated?.Invoke(CVR_MenuManager.Instance);
            
            BTKUILib.Log.Msg($"Setup {RootPages.Count} root pages and {CustomElements.Count} custom elements! BTKUILib is ready!");
        }

        internal void RegisterRootPage(Page rootPage)
        {
            RootPages.Add(rootPage);

            if (!UIUtils.IsQMReady()) return;
            
            MelonDebug.Msg($"Creating root page | Name: {rootPage.PageName} | ModName: {rootPage.ModName} | ElementID: {rootPage.ElementID}");
            rootPage.GenerateTab();
        }

        //Store all pages connected to a specific mod name to catch unintended cases like orphaned pages
        internal bool AddModPage(string modName, Page page)
        {
            modName = UIUtils.GetCleanString(modName);
            bool duplicate = false;

            var modPageList = ModPages.TryGetValue(modName, out var pageList) ? pageList : new List<Page>();

            if (modPageList.Any(x => x.PageName == page.PageName))
                duplicate = true;

            modPageList.Add(page);

            if(!ModPages.ContainsKey(modName))
                ModPages.Add(modName, modPageList);

            return duplicate;
        }

        #region Cohtml Event Functions

        internal void OnTabChange(string tabTarget)
        {
            IsInPlayerList = false;

            if (SelectedRootPage != null && SelectedRootPage.ElementID != tabTarget)
            {
                SelectedRootPage.TabChange();
                SelectedRootPage.IsVisible = false;

                //Catch orphaned of injected root pages
                foreach (var page in GeneratedPages)
                {
                    page.DeleteInternal(true);
                    if (page.RootPage == page)
                        page.IsVisible = false;
                }

                GeneratedPages.Clear();

                SelectedRootPage = null;
            }

            if (tabTarget == "CVRMainQM")
            {
                UIUtils.GetInternalView().TriggerEvent("btkChangeTab", tabTarget, "CVR", "", "");
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

            SelectedRootPage = root;

            root.IsVisible = true;
            root.GenerateCohtml();

            if (ModPages.TryGetValue(root.ModName, out var pages))
            {
                foreach (var page in pages)
                {
                    //Hacky, make sure disconnected root pages get IsVisible set correctly
                    if (page.RootPage == page)
                        page.IsVisible = true;
                    if(!page.IsGenerated)
                        page.GenerateCohtml();
                }
            }
            
            UIUtils.GetInternalView().TriggerEvent("btkChangeTab", tabTarget, root.ModName, root.MenuTitle, root.MenuSubtitle);
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
            if (targetPage == "btkUI-PlayerList")
                IsInPlayerList = true;
            if (lastPage == "btkUI-PlayerList")
                IsInPlayerList = false;

            QuickMenuAPI.CurrentPageID = targetPage;

            QuickMenuAPI.OnBackAction?.Invoke(targetPage, lastPage);

            GeneratedPages.FirstOrDefault(x => x.ElementID == lastPage)?.OnPageClosed?.Invoke();
            GeneratedPages.FirstOrDefault(x => x.ElementID == targetPage)?.OnPageOpen?.Invoke();
        }

        private void OnOpenedPageEvent(string targetPage, string lastPage)
        {
            if (targetPage == "btkUI-PlayerList")
                IsInPlayerList = true;

            QuickMenuAPI.CurrentPageID = targetPage;

            QuickMenuAPI.OnOpenedPage?.Invoke(targetPage, lastPage);

            GeneratedPages.FirstOrDefault(x => x.ElementID == lastPage)?.OnPageClosed?.Invoke();
            GeneratedPages.FirstOrDefault(x => x.ElementID == targetPage)?.OnPageOpen?.Invoke();
        }
        
        private void OnSliderUpdated(string sliderID, string value, bool resetFired)
        {
            if (!float.TryParse(value, out var valueFloat))
                return;

            if (!Sliders.ContainsKey(sliderID)) return;

            Sliders[sliderID].SliderValue = valueFloat;

            if (!resetFired) return;

            Sliders[sliderID].OnSliderReset?.Invoke();
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