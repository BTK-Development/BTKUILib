using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BTKUILib.UIObjects.Objects;

public class ContextMenu
{
    public string MenuTitle { get; set; }

    internal Dictionary<string, ContextMenuOption> MenuOptions { get; set; } = new();

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

    public void AddContextOption(string optionText, string optionTooltip, ContextMenuOptionType optionType, Action<bool> onClick = null, bool state = false)
    {
        var option = new ContextMenuOption(optionText, optionTooltip, optionType, onClick, state);
        MenuOptions.Add(option.OptionUUID, option);
    }

    internal void ActionTriggered(string actionUUID, bool state)
    {
        if (!MenuOptions.ContainsKey(actionUUID)) return;

        MenuOptions[actionUUID].OnClick?.Invoke(state);
    }
}

internal class ContextMenuOption
{
    public string OptionUUID { get; private set; } = Guid.NewGuid().ToString();
    public string OptionName { get; set; }
    public string OptionTooltip { get; set; }
    public ContextMenuOptionType OptionType { get; set; }
    public bool OptionState { get; set; }

    [JsonIgnore]
    public Action<bool> OnClick { get; set; }

    public ContextMenuOption(string optionName, string optionTooltip, ContextMenuOptionType optionType = ContextMenuOptionType.Button, Action<bool> onClick = null, bool optionState = false)
    {
        OptionName = optionName;
        OptionTooltip = optionTooltip;
        OptionType = optionType;
        OnClick = onClick;
        OptionState = optionState;
    }
}

public enum ContextMenuOptionType
{
    Button,
    Toggle,
    Separator
}