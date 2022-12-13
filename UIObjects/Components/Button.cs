using System;
using ABI_RC.Core.InteractionSystem;
using cohtml;

namespace BTKUILib.UIObjects.Components;

public class Button : QMInteractable
{
    public string ButtonText
    {
        get => _buttonText;
        set
        {
            _buttonText = value; 
            UpdateButton();
        }
    }

    public string ButtonIcon
    {
        get => _buttonIcon;
        set
        {
            _buttonIcon = value;
            UpdateButton();
        }
    }
    
    public string ButtonTooltip
    {
        get => _buttonTooltip;
        set
        {
            _buttonTooltip = value; 
            UpdateButton();
        }
    }

    public Action OnPress;

    private string _buttonText;
    private string _buttonIcon;
    private string _buttonTooltip;
    private Category _category;

    internal Button(string buttonText, string buttonIcon, string buttonTooltip, Category category)
    {
        _buttonIcon = buttonIcon;
        _buttonText = buttonText;
        _category = category;
        
        ElementID = "btkUI-Button-" + UUID;
    }

    internal override void OnInteraction(bool? toggle = null)
    {
        OnPress?.Invoke();
    }

    internal override void GenerateCohtml()
    {
        if(!IsGenerated)
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkCreateButton", _category.ElementID, _buttonText, _buttonIcon, "This is a test", UUID);

        IsGenerated = true;
    }

    private void UpdateButton()
    {
        
    }
}