using System;
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
        internal readonly ABI_RC.Systems.UI.UILib.UIObjects.Category InternalCategory;
        
        /// <summary>
        /// Category name, will update on the fly
        /// </summary>
        public string CategoryName
        {
            get => InternalCategory.CategoryName;
            set => InternalCategory.CategoryName = value;
        }

        /// <inheritdoc />
        public override bool Hidden
        {
            get => InternalCategory.Hidden;
            set => InternalCategory.Hidden = value;
        }

        /// <summary>
        /// Fired when a category is collapsed or expanded
        /// </summary>
        public Action<bool> OnCollapse;

        internal Category(ABI_RC.Systems.UI.UILib.UIObjects.Category internalCategory)
        {
            InternalCategory = internalCategory;
            InternalElement = internalCategory;
            InternalCategory.OnCollapse += b =>
            {
                OnCollapse?.Invoke(b);
            };
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
            var internalButton = InternalCategory.AddButton(buttonText, buttonIcon, buttonTooltip, (ABI_RC.Systems.UI.UILib.UIObjects.Components.ButtonStyle)style, holdWaitTime);

            return new Button(internalButton);
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
            var internalToggle = InternalCategory.AddToggle(toggleText, toggleTooltip, state);

            return new ToggleButton(internalToggle);
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
            var internalSlider = InternalCategory.AddSlider(sliderName, sliderTooltip, initialValue, minValue, maxValue,
                decimalPlaces, defaultValue, allowReset, noTitle);

            return new SliderFloat(internalSlider);
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
            var internalPage = InternalCategory.AddPage(pageName, pageIcon, pageTooltip, modName);
            
            return new Page(internalPage);
        }

        /// <summary>
        /// Add a custom element to this category
        /// </summary>
        /// <param name="element"></param>
        public void AddCustomElement(CustomElement element)
        {
            InternalCategory.AddCustomElement(element.InternalCE);
        }

        /// <summary>
        /// Creates a textblock in this category
        /// </summary>
        /// <param name="text">Text to be set in the text block</param>
        /// <returns>TextBlock object, you can use this to configure the textblock further or update it down the road</returns>
        public TextBlock AddTextBlock(string text)
        {
            var internalBlock = InternalCategory.AddTextBlock(text);
            var block = new TextBlock(internalBlock);

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
            var internalInput = InternalCategory.AddTextInput(text, placeholder,(ABI_RC.Systems.UI.UILib.UIObjects.Components.InputType)type);
            var input = new TextInput(internalInput);
            
            return input;
        }

        /// <inheritdoc />
        public override void Delete()
        {
            InternalCategory.Delete();
        }

        /// <summary>
        /// Deletes all children of this category
        /// </summary>
        public void ClearChildren()
        {
            InternalCategory.ClearChildren();
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
    }
}