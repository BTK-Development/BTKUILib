using System;
using System.Collections.Generic;
using System.Linq;
using BTKUILib.UIObjects.Components;
using Newtonsoft.Json;

namespace BTKUILib.UIObjects.Objects;

/// <summary>
/// Contains information required to open a context menu
/// Make sure to keep this reference if you'd like to change parts of the menu
/// </summary>
public class ContextMenu
{
    /// <summary>
    /// Title of context menu, appears at the top of the popup window
    /// </summary>
    public string MenuTitle { get; set; }

    internal Dictionary<string, ContextMenuOption> MenuOptions { get; } = new();

    private readonly List<SliderFloat> _contextSliders = new();

    /// <summary>
    /// Opens the context menu overtop any page
    /// </summary>
    public void OpenContextMenu()
    {
        if (!UIUtils.IsQMReady()) return;

        if (!BTKUILib.Instance.IsOnMainThread())
        {
            BTKUILib.Instance.MainThreadQueue.Enqueue(OpenContextMenu);
            return;
        }

        UserInterface.Instance.SelectedContextMenu = this;

        UIUtils.GetInternalView().TriggerEvent("btkOpenContextMenu", MenuTitle, JsonConvert.SerializeObject(MenuOptions.Values));
    }

    /// <summary>
    /// Clears all created menu options as well as the references to sliders
    /// </summary>
    public void Clear()
    {
        MenuOptions.Clear();
        _contextSliders.Clear();
    }

    /// <summary>
    /// Add a context menu option to the menu
    /// </summary>
    /// <param name="optionText">Option text/name</param>
    /// <param name="optionTooltip">Option tooltip</param>
    /// <param name="optionType">Option type, this defines what element is generated, you cannot create a Slider here!</param>
    /// <param name="onClick">OnClick action, this returns a float representing a bool for the toggle element</param>
    /// <param name="state">Current state of the toggle (if toggle element)</param>
    /// <returns>Reference to generated ContextOption, you can edit these and it'll be updated when the menu is opened again</returns>
    public ContextMenuOption AddContextOption(string optionText, string optionTooltip, ContextMenuOptionType optionType, Action<float> onClick = null, float state = 0f)
    {
        if (optionType == ContextMenuOptionType.Slider)
        {
            BTKUILib.Log.Warning("You cannot add a slider to the context menu using AddContextOption, you must use AddContextSlider!");
            return null;
        }

        var option = new ContextMenuOption(optionText, optionTooltip, optionType, onClick, state);
        MenuOptions.Add(option.OptionUuid, option);

        return option;
    }

    /// <summary>
    /// Add context slider to context menu
    /// </summary>
    /// <param name="sliderName">Name of slider</param>
    /// <param name="sliderTooltip">Slider tooltip</param>
    /// <param name="initialValue">Initial value of slider</param>
    /// <param name="onChange">OnChange action returning value of slider</param>
    /// <param name="minValue">Min slider value</param>
    /// <param name="maxValue">Max slider value</param>
    /// <param name="decimalPlaces">Amount of decimal places in slider readout</param>
    /// <param name="defaultValue">Default value of slider</param>
    /// <param name="allowDefaultReset">Allow slider to be reset to default value</param>
    /// <returns>Generated SliderFloat reference</returns>
    public SliderFloat AddContextSlider(string sliderName, string sliderTooltip, float initialValue, Action<float> onChange, float minValue = 0f, float maxValue = 10f, int decimalPlaces = 2, float defaultValue = 0f, bool allowDefaultReset = true)
    {
        var slider = new SliderFloat(null, sliderName, sliderTooltip, initialValue, minValue, maxValue, decimalPlaces, defaultValue, allowDefaultReset, false, true);
        var option = new ContextMenuOption(sliderName, sliderTooltip, ContextMenuOptionType.Slider, onChange, initialValue, slider.SliderSettings);

        slider.OnValueUpdated += onChange;
        option.OptionUuid = slider.UUID;

        MenuOptions.Add(slider.UUID, option);
        _contextSliders.Add(slider);

        return slider;
    }

    internal void ActionTriggered(string actionUUID, float state)
    {
        if (!MenuOptions.ContainsKey(actionUUID)) return;

        MenuOptions[actionUUID].OnInteract?.Invoke(state);
    }
}

/// <summary>
/// ContextMenuOption
/// </summary>
public class ContextMenuOption
{
    /// <summary>
    /// ID of context menu option, this is generated for most elements
    /// </summary>
    public string OptionUuid { get; internal set; } = Guid.NewGuid().ToString();
    /// <summary>
    /// Name of menu option, used for button/toggle text as well as slider name
    /// </summary>
    public string OptionName { get; set; }
    /// <summary>
    /// Tooltip text
    /// </summary>
    public string OptionTooltip { get; set; }
    /// <summary>
    /// Sets which type element this is, cannot be changed after creation
    /// </summary>
    public ContextMenuOptionType OptionType { get; }
    /// <summary>
    /// Value of toggle/slider
    /// </summary>
    public float OptionValue { get; set; }
    [JsonProperty]
    internal SliderSettings? OptionSliderSettings { get; set; }

    /// <summary>
    /// Action for interaction, returns float representation for toggles and floats
    /// </summary>
    [JsonIgnore]
    public Action<float> OnInteract { get; set; }

    internal ContextMenuOption(string optionName, string optionTooltip, ContextMenuOptionType optionType = ContextMenuOptionType.Button, Action<float> onInteract = null, float optionValue = 0f, SliderSettings? sliderSettings = null)
    {
        OptionName = optionName;
        OptionTooltip = optionTooltip;
        OptionType = optionType;
        OnInteract = onInteract;
        OptionValue = optionValue;
        OptionSliderSettings = sliderSettings;
    }
}

/// <summary>
/// Option type enum
/// </summary>
public enum ContextMenuOptionType
{
    /// <summary>
    /// Button element, creates a basic text only button
    /// </summary>
    Button,
    /// <summary>
    /// Toggle element, creates a simple toggle button
    /// </summary>
    Toggle,
    /// <summary>
    /// Separator element, creates a simple separator in the list with a name
    /// </summary>
    Separator,
    /// <summary>
    /// Slider element, used by AddContextSlider to generate the correct element, you shouldn't be selecting this yourself!
    /// </summary>
    Slider
}