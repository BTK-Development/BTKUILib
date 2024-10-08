﻿using System;
using BTKUILib.UIObjects.Components;
using MelonLoader;
using UnityEngine;
using Button = BTKUILib.UIObjects.Components.Button;

namespace BTKUILib.UIObjects
{
    /// <summary>
    /// This act as category with header and row within Cohtml
    /// </summary>
    public class Category : QMUIElement
    {
        /// <summary>
        /// Category name, will update on the fly
        /// </summary>
        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                UpdateCategoryName();
            }
        }

        /// <inheritdoc />
        public override bool Hidden
        {
            get => base.Hidden;
            set
            {
                base.Hidden = value;

                if (!UIUtils.IsQMReady()) return;
                UIUtils.GetInternalView().TriggerEvent("btkSetHidden", $"{ElementID}-HeaderRoot", value);
            }
        }

        /// <summary>
        /// Fired when a category is collapsed or expanded
        /// </summary>
        public Action<bool> OnCollapse;

        internal string ModName => _modName ?? LinkedPage.ModName;

        internal readonly Page LinkedPage;
        internal bool Collapsed;
        private string _categoryName;
        private readonly string _modName;
        private bool _showHeader = false;
        private bool _canCollapse;

        internal Category(string categoryName, Page page, bool showHeader = true, string modName = null, bool canCollapse = true, bool collapsed = false)
        {
            _categoryName = categoryName;
            LinkedPage = page;
            _showHeader = showHeader;
            _modName = UIUtils.GetCleanString(modName);
            Collapsed = collapsed;
            _canCollapse = canCollapse;

            Parent = page;
            
            ElementID = "btkUI-Row-" + UUID;

            UserInterface.Categories.Add(ElementID, this);
        }
        
        /// <summary>
        /// Creates a simple button
        /// </summary>
        /// <param name="buttonText">Text to be displayed on the button</param>
        /// <param name="buttonIcon">Icon for the button</param>
        /// <param name="buttonTooltip">Tooltip to be displayed when hovering on the button</param>
        /// <returns></returns>
        public Button AddButton(string buttonText, string buttonIcon, string buttonTooltip)
        {
            return AddButton(buttonText, buttonIcon, buttonTooltip, ButtonStyle.TextWithIcon);
        }

        /// <summary>
        /// Creates a simple button
        /// </summary>
        /// <param name="buttonText">Text to be displayed on the button</param>
        /// <param name="buttonIcon">Icon for the button</param>
        /// <param name="buttonTooltip">Tooltip to be displayed when hovering on the button</param>
        /// <param name="style">Sets the button style, this cannot be changed after creation!</param>
        /// <returns></returns>
        public Button AddButton(string buttonText, string buttonIcon, string buttonTooltip, ButtonStyle style)
        {
            return AddButton(buttonText, buttonIcon, buttonTooltip, style, 0.5f);
        }

        /// <summary>
        /// Creates a simple button
        /// </summary>
        /// <param name="buttonText">Text to be displayed on the button</param>
        /// <param name="buttonIcon">Icon for the button</param>
        /// <param name="buttonTooltip">Tooltip to be displayed when hovering on the button</param>
        /// <param name="style">Sets the button style, this cannot be changed after creation!</param>
        /// <param name="holdWaitTime">Sets the amount of time before the OnHeld action is fired</param>
        /// <returns></returns>
        public Button AddButton(string buttonText, string buttonIcon, string buttonTooltip, ButtonStyle style, float holdWaitTime)
        {
            var button = new Button(buttonText, buttonIcon, buttonTooltip, this, style, holdWaitTime);
            SubElements.Add(button);

            if(UIUtils.IsQMReady())
                button.GenerateCohtml();

            return button;
        }

        /// <summary>
        /// Simple toggle element
        /// </summary>
        /// <param name="toggleText">Text to be displayed on toggle</param>
        /// <param name="toggleTooltip">Tooltip to be displayed when hovering on the toggle</param>
        /// <param name="state">Initial state of the toggle</param>
        /// <returns>Newly created toggle object</returns>
        public ToggleButton AddToggle(string toggleText, string toggleTooltip, bool state)
        {
            var toggle = new ToggleButton(toggleText, toggleTooltip, state, this);
            SubElements.Add(toggle);
            
            if(UIUtils.IsQMReady())
                toggle.GenerateCohtml();

            return toggle;
        }

                /// <summary>
        /// Create a slider on the page
        /// </summary>
        /// <param name="sliderName">Name of the slider, displayed above the slider</param>
        /// <param name="sliderTooltip">Tooltip displayed when hovering on the slider</param>
        /// <param name="initialValue">Initial value of the slider</param>
        /// <param name="minValue">Minimum value that the slider can slide to</param>
        /// <param name="maxValue">Maximum value the slider can slide to</param>
        /// <returns></returns>
        public SliderFloat AddSlider(string sliderName, string sliderTooltip, float initialValue, float minValue, float maxValue)
        {
            return AddSlider(sliderName, sliderTooltip, initialValue, minValue, maxValue, 2, 0f, false, false);
        }

        /// <summary>
        /// Create a slider on the page
        /// </summary>
        /// <param name="sliderName">Name of the slider, displayed above the slider</param>
        /// <param name="sliderTooltip">Tooltip displayed when hovering on the slider</param>
        /// <param name="initialValue">Initial value of the slider</param>
        /// <param name="minValue">Minimum value that the slider can slide to</param>
        /// <param name="maxValue">Maximum value the slider can slide to</param>
        /// <param name="decimalPlaces">Set the number of decimal places displayed on the slider</param>
        /// <returns></returns>
        public SliderFloat AddSlider(string sliderName, string sliderTooltip, float initialValue, float minValue, float maxValue, int decimalPlaces)
        {
            return AddSlider(sliderName, sliderTooltip, initialValue, minValue, maxValue, decimalPlaces, 0f, false, false);
        }

        /// <summary>
        /// Create a slider on the page
        /// </summary>
        /// <param name="sliderName">Name of the slider, displayed above the slider</param>
        /// <param name="sliderTooltip">Tooltip displayed when hovering on the slider</param>
        /// <param name="initialValue">Initial value of the slider</param>
        /// <param name="minValue">Minimum value that the slider can slide to</param>
        /// <param name="maxValue">Maximum value the slider can slide to</param>
        /// <param name="decimalPlaces">Set the number of decimal places displayed on the slider</param>
        /// <param name="defaultValue">Default value for this slider</param>
        /// <param name="allowReset">Allow this slider to be reset using the reset button</param>
        /// <returns></returns>
        public SliderFloat AddSlider(string sliderName, string sliderTooltip, float initialValue, float minValue, float maxValue, int decimalPlaces, float defaultValue, bool allowReset)
        {
            return AddSlider(sliderName, sliderTooltip, initialValue, minValue, maxValue, decimalPlaces, defaultValue, allowReset, false);
        }

        /// <summary>
        /// Create a slider on the page
        /// </summary>
        /// <param name="sliderName">Name of the slider, displayed above the slider</param>
        /// <param name="sliderTooltip">Tooltip displayed when hovering on the slider</param>
        /// <param name="initialValue">Initial value of the slider</param>
        /// <param name="minValue">Minimum value that the slider can slide to</param>
        /// <param name="maxValue">Maximum value the slider can slide to</param>
        /// <param name="decimalPlaces">Set the number of decimal places displayed on the slider</param>
        /// <param name="defaultValue">Default value for this slider</param>
        /// <param name="allowReset">Allow this slider to be reset using the reset button</param>
        /// <param name="noTitle">Disables the title component of this slider, this also disables the reset button!</param>
        /// <returns></returns>
        public SliderFloat AddSlider(string sliderName, string sliderTooltip, float initialValue, float minValue, float maxValue, int decimalPlaces, float defaultValue, bool allowReset, bool noTitle)
        {
            var slider = new SliderFloat(this, sliderName, sliderTooltip, initialValue, minValue, maxValue, decimalPlaces, defaultValue, allowReset, true, noTitle);
            SubElements.Add(slider);

            if(UIUtils.IsQMReady())
                slider.GenerateCohtml();

            return slider;
        }

        /// <summary>
        /// Create a new subpage as well as the button required to open it
        /// </summary>
        /// <param name="pageName">Name of the new page, this will appear at the top of the page</param>
        /// <param name="pageIcon">Icon to be used on the button</param>
        /// <param name="pageTooltip">Tooltip to be displayed when hovering on the button</param>
        /// <param name="modName">Mod name, this should be the same as your root page</param>
        /// <returns>Newly created page object with SubpageButton set to the created button</returns>
        public Page AddPage(string pageName, string pageIcon, string pageTooltip, string modName)
        {
            modName = UIUtils.GetCleanString(modName);
            var page = Page.GetOrCreatePage(modName, pageName, category: this);
            SubElements.Add(page);

            if (modName == "BTKUILib" && LinkedPage.ElementID == "btkUI-PlayerSelectPage")
            {
                page.InPlayerlist = true;
            }

            var pageButton = new Button(pageName, pageIcon, pageTooltip, this);
            SubElements.Add(pageButton);
            pageButton.OnPress += () =>
            {
                page.OpenPage();
            };

            if (UIUtils.IsQMReady())
            {
                page.GenerateCohtml();
                pageButton.GenerateCohtml();
            }

            page.SubpageButton = pageButton;

            return page;
        }

        /// <summary>
        /// Add a custom element to this category
        /// </summary>
        /// <param name="element"></param>
        public void AddCustomElement(CustomElement element)
        {
            if (element.ElementType != ElementType.InCategoryElement)
            {
                BTKUILib.Log.Error($"You cannot add a {element.ElementType} custom element to a Category!");
                return;
            }

            SubElements.Add(element);

            if(UIUtils.IsQMReady())
                element.GenerateCohtml();
        }

        /// <summary>
        /// Creates a textblock in this category
        /// </summary>
        /// <param name="text">Text to be set in the text block</param>
        /// <returns>TextBlock object, you can use this to configure the textblock further or update it down the road</returns>
        public TextBlock AddTextBlock(string text)
        {
            var block = new TextBlock(text, this);

            SubElements.Add(block);

            if(UIUtils.IsQMReady())
                block.GenerateCohtml();

            return block;
        }

        /// <summary>
        /// Creates a TextInput in this category
        /// </summary>
        /// <param name="text">Initial text to be set in the TextInput</param>
        /// <param name="placeholder">Placeholder text to be displayed when no text is entered</param>
        /// <param name="type">Type of TextInput (most not implemented)</param>
        /// <returns></returns>
        public TextInput AddTextInput(string text, string placeholder = "", InputType type = InputType.Text)
        {
            var input = new TextInput(text, placeholder, type, this);

            SubElements.Add(input);

            if(UIUtils.IsQMReady())
                input.GenerateCohtml();

            return input;
        }

        /// <inheritdoc />
        public override void Delete()
        {
            //Delete the row header with the row
            UIUtils.GetInternalView().TriggerEvent("btkDeleteElement", ElementID + "-HeaderRoot");
            
            base.Delete();

            if (Protected) return;
            LinkedPage.SubElements.Remove(this);
        }

        /// <summary>
        /// Deletes all children of this category
        /// </summary>
        public void ClearChildren()
        {
            //Iterate through each subelement and ensure ClearChildren and Delete is fired
            foreach (var subElement in SubElements.ToArray())
            {
                if(subElement.Deleted) continue;

                switch (subElement)
                {
                    case Page page:
                        page.ClearChildren();
                        break;
                    case Category cat:
                        cat.ClearChildren();
                        break;
                }

                subElement.Delete();
            }

            SubElements.Clear();

            if(UIUtils.IsQMReady() && IsVisible)
                UIUtils.GetInternalView().TriggerEvent("btkClearChildren", ElementID);
        }

        /// <summary>
        /// Adds a ToggleButton to this Category based on a MelonPref
        /// </summary>
        /// <param name="entry">MelonPreferences_Entry to use for creating ToggleButton</param>
        /// <returns>Preconfigured ToggleButton with action to drive MelonPref</returns>
        public ToggleButton AddMelonToggle(MelonPreferences_Entry<bool> entry)
        {
            ToggleButton toggle = AddToggle(entry.DisplayName, entry.Description, entry.Value);
            toggle.OnValueUpdated += b => entry.Value = b;
            return toggle;
        }

        /// <summary>
        /// Adds a SliderFloat to this category based on a MelonPref
        /// </summary>
        /// <param name="entry">MelonPreferences_Entry to use for creating SliderFloat</param>
        /// <param name="min">Minimum value that the slider can slide to</param>
        /// <param name="max">Maximum value the slider can slide to</param>
        /// <param name="decimalPlaces">Set the number of decimal places displayed on the slider</param>
        /// <param name="allowReset">Allow this slider to be reset using the reset button</param>
        /// <returns>Preconfigured SliderFloat with action to drive MelonPref</returns>
        public SliderFloat AddMelonSlider(MelonPreferences_Entry<float> entry, float min,
            float max, int decimalPlaces = 2, bool allowReset = true)
        {
            SliderFloat slider = AddSlider(entry.DisplayName, entry.Description,
                                           Mathf.Clamp(entry.Value, min, max), min, max, decimalPlaces, entry.DefaultValue, allowReset);
            slider.OnValueUpdated += f => entry.Value = f;
            return slider;
        }

        /// <summary>
        /// Adds a Button to this Category to open the keyboard based on a MelonPref
        /// </summary>
        /// <param name="entry">MelonPreferences_Entry to use for creating this button</param>
        /// <param name="buttonIcon">Icon for the button</param>
        /// <param name="buttonStyle">Sets the button style, this cannot be changed after creation!</param>
        /// <returns>Preconfigured ToggleButton with action to open the Keyboard for this MelonPref</returns>
        public Button AddMelonStringInput(MelonPreferences_Entry<string> entry, string buttonIcon = "", ButtonStyle buttonStyle = ButtonStyle.TextOnly)
        {
            Button button = AddButton(entry.DisplayName, buttonIcon, entry.Description, buttonStyle);
            button.OnPress += () => QuickMenuAPI.OpenKeyboard(entry.Value, s => entry.Value = s);
            return button;
        }

        /// <summary>
        /// Adds a Button to open the NumberInput based on a MelonPref
        /// </summary>
        /// <param name="entry">MelonPreferences_Entry to use for creating this button</param>
        /// <param name="buttonIcon">Icon for the button</param>
        /// <param name="buttonStyle">Sets the button style, this cannot be changed after creation!</param>
        /// <returns>Preconfigured Button with action to open NumberInput for this MelonPref</returns>
        public Button AddMelonNumberInput(MelonPreferences_Entry<float> entry, string buttonIcon = "", ButtonStyle buttonStyle = ButtonStyle.TextOnly)
        {
            Button button = AddButton(entry.DisplayName, buttonIcon, entry.Description, buttonStyle);
            button.OnPress += () => QuickMenuAPI.OpenNumberInput(entry.DisplayName, entry.Value, f => entry.Value = f);
            return button;
        }

        internal override void GenerateCohtml()
        {
            if (!UIUtils.IsQMReady()) return;

            if (RootPage is { IsVisible: false }) return;

            if(!IsGenerated)
                UIUtils.GetInternalView().TriggerEvent("btkCreateRow", LinkedPage.ElementID, UUID, _canCollapse, Collapsed, _showHeader ? _categoryName : null);
            
            foreach(var element in SubElements)
                element.GenerateCohtml();

            base.GenerateCohtml();

            IsGenerated = true;
        }

        private void UpdateCategoryName()
        {
            if (!BTKUILib.Instance.IsOnMainThread())
            {
                BTKUILib.Instance.MainThreadQueue.Enqueue(UpdateCategoryName);
                return;
            }

            if (!UIUtils.IsQMReady()) return;

            UIUtils.GetInternalView().TriggerEvent("btkUpdateText", $"btkUI-Row-{UUID}-HeaderText", _categoryName);
        }
    }
}