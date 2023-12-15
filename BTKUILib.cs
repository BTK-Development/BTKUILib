using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BTKUILib.UIObjects;
using BTKUILib.UIObjects.Objects;
using MelonLoader;

namespace BTKUILib
{
    internal static class BuildInfo
    {
        public const string Name = "BTKUILib";
        public const string Author = "BTK Development Team";
        public const string Company = "BTK Development";
        public const string Version = "2.0.0-preview2";
    }
    
    internal class BTKUILib : MelonMod
    {
        internal static MelonLogger.Instance Log;
        internal static BTKUILib Instance;
        internal static Page UISettingsPage;

        internal UserInterface UI;
        internal Queue<Action> MainThreadQueue = new();
        internal Dictionary<string, Page> MLPrefsPages = new();

        private MelonPreferences_Entry<bool> _displayPrefsTab;
        private MelonPreferences_Entry<string> _playerListStyle;

        private Thread _mainThread;
        private Page _mlPrefsPage;
        private MultiSelection _playerListButtonStyle;
        private string[] _playerListOptions = { "Tab Bar", "Replace TTS", "Replace Groups" };

        public override void OnInitializeMelon()
        {
            Log = LoggerInstance;
            Instance = this;
            _mainThread = Thread.CurrentThread;
            
            Log.Msg("BTKUILib is starting up!");

            MelonPreferences.CreateCategory("BTKUILib", "BTKUILib");
            _displayPrefsTab = MelonPreferences.CreateEntry("BTKUILib", "DisplayPrefsTab", false, "Display MelonPrefs Tab", "Sets if the MelonLoader Prefs tab should be displayed");
            _displayPrefsTab.OnEntryValueChanged.Subscribe((b1, _) =>
            {
                if (_mlPrefsPage != null)
                    _mlPrefsPage.HideTab = b1;
            });

            _playerListStyle = MelonPreferences.CreateEntry("BTKUILib", "PlayerListStyle", "Tab Bar", "PlayerList Button Style", "Sets where the playerlist button will appear");
            _playerListStyle.OnEntryValueChanged.Subscribe((s1, _) =>
            {

            });
            
            Patches.Initialize(HarmonyInstance);

            UI = new UserInterface();
            UI.SetupUI();

            QuickMenuAPI.PlayerSelectPage = new Page("btkUI-PlayerSelectPage");
        }

        internal void GenerateSettingsPage()
        {
            if (UISettingsPage != null) return;

            UISettingsPage = new Page("btkUI-SettingsPage");
            var mainCat = UISettingsPage.AddCategory("Main", false);

            var prefsTabDisplay = mainCat.AddToggle("Show ML Prefs Tab", "Displays the MelonLoader prefs tab", _displayPrefsTab.Value);
            prefsTabDisplay.OnValueUpdated += b =>
            {
                _displayPrefsTab.Value = b;
            };

            var openListStyle = mainCat.AddButton("Playerlist Button Style", "Change the style and position of the playerlist button", "BTKList");
            openListStyle.OnPress += () =>
            {
                QuickMenuAPI.OpenMultiSelect(_playerListButtonStyle);
            };

            _playerListButtonStyle = new MultiSelection("PlayerList Button Style", _playerListOptions, Array.FindIndex(_playerListOptions, x => x.Contains(_playerListStyle.Value)));
            _playerListButtonStyle.OnOptionUpdated += i =>
            {
                _playerListStyle.Value = _playerListOptions[i];
            };
        }

        internal void GenerateMlPrefsTab()
        {
            if(_mlPrefsPage != null) return;

            _mlPrefsPage = Page.GetOrCreatePage("MelonLoader", "Prefs", true, "Settings");
            _mlPrefsPage.MenuTitle = "MelonLoader Preferences";
            _mlPrefsPage.MenuSubtitle = "Control your MelonLoader Preferences from other mods!";
            _mlPrefsPage.Protected = true;
            _mlPrefsPage.HideTab = !_displayPrefsTab.Value;

            var prefCat = _mlPrefsPage.AddCategory("Categories");

            MLPrefsPages.Clear();

            foreach (var category in MelonPreferences.Categories.OrderBy(x => x.DisplayName))
            {
                var page = prefCat.AddPage(category.DisplayName, "Star", $"Opens the preferences category for {category.DisplayName}", "MelonLoader");
                MLPrefsPages.Add(category.Identifier, page);
                var pageCat = page.AddCategory("Preferences");

                foreach (var pref in category.Entries)
                {
                    if (pref.GetReflectedType() == typeof(bool))
                    {
                        var toggle = pageCat.AddToggle(pref.DisplayName, pref.Description, (bool)pref.BoxedValue);
                        toggle.OnValueUpdated += b =>
                        {
                            pref.BoxedValue = b;
                        };

                        if (pref.GetReflectedType() == typeof(string))
                        {
                            var button = pageCat.AddButton($"Edit {pref.DisplayName}", "Pencil", pref.Description);
                            button.OnPress += () =>
                            {
                                QuickMenuAPI.OpenKeyboard((string)pref.BoxedValue, s =>
                                {
                                    pref.BoxedValue = s;
                                });
                            };
                        }
                    }
                }
            }
        }

        internal bool IsOnMainThread(Thread thread = null)
        {
            thread ??= Thread.CurrentThread;

            return thread.Equals(_mainThread);
        }
        
        public override void OnUpdate()
        {
            if (MainThreadQueue.Count == 0) return;

            //If the queue has any amount of objects dequeue and invoke all of them
            while (MainThreadQueue.Count > 0)
            {
                MainThreadQueue.Dequeue()?.Invoke();
            }
        }
    }
}