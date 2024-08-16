using System;

namespace BTKUILib.UIObjects.Components;

/// <summary>
/// TextInput element
/// </summary>
public class TextInput : QMUIElement
{
    /// <summary>
    /// OnTextUpdate action, this is fired when the textinput is changed by the user
    /// </summary>
    public Action<string> OnTextUpdate;

    /// <summary>
    /// TextInput text property, to update this you must use TextInput.SetText
    /// </summary>
    public string Text
    {
        get => _text;
        internal set
        {
            _text = value;
            OnTextUpdate?.Invoke(value);
        }
    }

    /// <summary>
    /// Placeholder text, this will update on the fly, it'll be shown when no text is entered
    /// </summary>
    public string Placeholder
    {
        get => _placeholder;
        set
        {
            _placeholder = value;
            UpdateTextInput();
        }
    }

    private string _text;
    private string _placeholder;
    private InputType _type;
    private string[] _additionalCSSClasses;

    internal TextInput(string text, string placeholder, InputType type, Category category, params string[] additionalCSSClasses)
    {
        _text = text;
        _placeholder = placeholder;
        _type = type;
        _additionalCSSClasses = additionalCSSClasses;

        Parent = category;

        ElementID = $"btkUI-TextInput-{UUID}";

        UserInterface.TextInputs.Add(ElementID, this);
    }

    /// <summary>
    /// Sets the text of the TextInput element without triggering the update event
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        _text = text;
        UpdateTextInput();
    }

    internal override void GenerateCohtml()
    {
        if (!UIUtils.IsQMReady()) return;

        if (RootPage is { IsVisible: false }) return;

        if(!IsGenerated)
            UIUtils.GetInternalView().TriggerEvent("btkCreateTextInput", Parent.ElementID, UUID, Text, Placeholder, _type, _additionalCSSClasses);

        base.GenerateCohtml();

        IsGenerated = true;
    }

    internal override void DeleteInternal(bool tabChange = false)
    {
        base.DeleteInternal(tabChange);

        UserInterface.TextInputs.Remove(ElementID);
    }

    private void UpdateTextInput()
    {
        if(!IsVisible) return;

        if (!BTKUILib.Instance.IsOnMainThread())
        {
            BTKUILib.Instance.MainThreadQueue.Enqueue(UpdateTextInput);
            return;
        }

        UIUtils.GetInternalView().TriggerEvent("btkUpdateText", ElementID, _text);
    }
}

/// <summary>
/// TextInput type
/// </summary>
public enum InputType
{
    /// <summary>
    /// Basic text input
    /// </summary>
    Text,
    /// <summary>
    /// Password type input, this will display stars for the entered text (not implemented)
    /// </summary>
    Password,
    /// <summary>
    /// Number only type, this will validate that it's a number internally (not implemented)
    /// </summary>
    Number
}
