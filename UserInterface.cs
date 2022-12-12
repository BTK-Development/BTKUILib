using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ABI_RC.Core.InteractionSystem;
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
        public static UserInterface Instance;
        public static List<SliderFloat> SliderFloats = new();
        public static List<ToggleButton> ToggleButtons = new();
        public static List<Page> RootPages = new();
        internal static List<QMUIElement> QMElements = new(); 

        public Page SelectedPage;

        private MultiSelection SelectedMultiSelect;

        internal void SetupUI()
        {
            Instance = this;

            QuickMenuAPI.OnMenuRegenerate += OnMenuRegenerate;
            
            BTKUILib.Log.Msg("Checking if BTKUI is updated...");
            CheckUpdateUI();
        }

        #region Element Creation

        public static void CreatePage(Page page)
        {
            //Check if CreatePage has been ran on main thread, otherwise queue up for main thread
            if (BTKUILib.Instance.IsOnMainThread())
            {
                BTKUILib.Instance.MainThreadQueue.Enqueue(() =>
                {
                    CreatePage(page);
                });

                return;
            }

            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkCreatePage", page.PageName, page.RootPage);
        }

        #endregion

        private void OnMenuRegenerate(CVR_MenuManager cvrMenuManager)
        {
            MelonDebug.Msg("Registering events");

            foreach (var element in QMElements)
                element.IsGenerated = false;
            
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-ButtonAction", new Action<string>(HandleButtonAction));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-Toggle", new Action<string, bool>(OnToggle));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-PopupConfirmOK", new Action(ConfirmOK));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-PopupConfirmNo", new Action(ConfirmNo));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-PopupNoticeOK", new Action(NoticeClose));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-OpenMainMenu", new Action(OpenMainMenu));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-SliderValueUpdated", new Action<string, string>(OnSliderUpdated));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-OpenedPage", new Action<string, string>(OnOpenedPageEvent));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-BackAction", new Action<string, string>(OnBackActionEvent));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-DropdownSelected", new Action<int>(DropdownSelected));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-NumSubmit", new Action<string>(OnNumberInputSubmitted));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-RootCreated", new Action<string, string>(OnRootCreated));
            CVR_MenuManager.Instance.quickMenu.View.BindCall("btkUI-TabChange", new Action<string>(OnTabChange));
            
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkModInit");
            
            //Begin creating the UI elements!
            foreach (var root in RootPages)
            {
                BTKUILib.Log.Msg($"Creating root page | Name: {root.PageName} | ModName: {root.ModName} | ElementID: {root.ElementID}");
                root.GenerateCohtml();
            }
        }

        #region Cohtml Event Functions
        
        private void OnTabChange(string tabTarget)
        {
            if (tabTarget == "CVRMainQM")
            {
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkChangeTab", tabTarget, "CVR", "", "");
                return;
            }
                
            
            var root = RootPages.FirstOrDefault(x => x.ElementID == tabTarget);

            if (root == null)
            {
                BTKUILib.Log.Error("RootPage was not found! Cannot switch tabs!");
                return;
            }
            
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkChangeTab", tabTarget, root.ModName, root.MenuTitle, root.MenuSubtitle);
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

            var sliders = SliderFloats.Where(x => x.ElementID.Equals(sliderID));

            foreach (var slider in sliders)
                slider.SliderValue = valueFloat;
        }
        
        private void OpenMainMenu()
        {
            QuickMenuAPI.ShowConfirm("Testing", "Hello world!", "Memes", () =>
            {
                BTKUILib.Log.Msg("Hey a button press");
            }, "No Memes", () =>
            {
                BTKUILib.Log.Msg("Hey a different button press!");
            });
        }
        
        private void OnToggle(string toggleID, bool state)
        {
            MelonDebug.Msg($"Toggle state changed for {toggleID} to {state}");
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
        
        private void HandleButtonAction(string action)
        {
            /*if (_multiSelectionOptions.ContainsKey(action))
            {
                var ms = _multiSelectionOptions[action];
                UIUtils.OpenMultiSelect(ms);
                return;
            }

            if (_generatedRemoteControlSingles.ContainsKey(action) && ParamControlLeadPair != null)
            {
                var param = _generatedRemoteControlSingles[action];
                UIUtils.OpenNumberInput(param.ParameterTarget, param.ParameterValue, f =>
                {
                    param.ParameterValue = f;
                    param.IsUpdated = true;
                    AvatarParameterManager.Instance.SendUpdatedParameters(ParamControlLeadPair);
                });
                
                return;
            }
            
            if (!_handlers.TryGetValue(action, out var func))
                return;
            
            Con.Debug($"Found action for {action}");

            func();*/
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