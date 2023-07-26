using System;
using System.Collections.Generic;
using System.Threading;
using BTKUILib.UIObjects;
using MelonLoader;

namespace BTKUILib
{
    internal static class BuildInfo
    {
        public const string Name = "BTKUILib";
        public const string Author = "BTK Development Team";
        public const string Company = "BTK Development";
        public const string Version = "1.2.0";
    }
    
    internal class BTKUILib : MelonMod
    {
        internal static MelonLogger.Instance Log;
        internal static BTKUILib Instance;

        internal UserInterface UI;
        internal Queue<Action> MainThreadQueue = new();
        internal Dictionary<string, Page> MLPrefsPages = new();

        private MelonPreferences_Entry<bool> _displayPrefsTab;

        private Thread _mainThread;
        private Page _mlPrefsPage;

        public override void OnInitializeMelon()
        {
            Log = LoggerInstance;
            Instance = this;
            _mainThread = Thread.CurrentThread;
            
            Log.Msg("BTKUILib is starting up!");

            MelonPreferences.CreateCategory("BTKUILib", "BTKUILib");
            _displayPrefsTab = MelonPreferences.CreateEntry("BTKUILib", "DisplayPrefsTab", false, "Display MelonPrefs Tab", "Sets if the MelonLoader Prefs tab should be displayed");
            _displayPrefsTab.OnEntryValueChanged.Subscribe((_, b1) =>
            {
                if (!b1 && _mlPrefsPage != null)
                {
                    _mlPrefsPage.DeleteInternal();
                    _mlPrefsPage = null;
                }
                else
                {
                    GenerateMlPrefsTab();
                }
            });
            
            Patches.Initialize(HarmonyInstance);

            UI = new UserInterface();
            UI.SetupUI();

            QuickMenuAPI.PlayerSelectPage = new Page("btkUI-PlayerSelectPage");
        }

        internal void GenerateMlPrefsTab()
        {
            if(_mlPrefsPage != null) return;
            if (!_displayPrefsTab.Value) return;

            _mlPrefsPage = new Page("MelonLoader", "Prefs", true, "Settings");
            _mlPrefsPage.MenuTitle = "MelonLoader Preferences";
            _mlPrefsPage.MenuSubtitle = "Control your MelonLoader Preferences from other mods!";
            _mlPrefsPage.Protected = true;
            
            _mlPrefsPage.GenerateCohtml();

            var prefCat = _mlPrefsPage.AddCategory("Categories");

            foreach (var category in MelonPreferences.Categories)
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