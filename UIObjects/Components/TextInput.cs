using System;

namespace BTKUILib.UIObjects.Components;

/// <summary>
/// TextInput element
/// </summary>
public class TextInput : QMUIElement
{
    private readonly ABI_RC.Systems.UI.UILib.UIObjects.Components.TextInput _internalTextInput;
    
    /// <summary>
    /// OnTextUpdate action, this is fired when the textinput is changed by the user
    /// </summary>
    public Action<string> OnTextUpdate;

    /// <summary>
    /// TextInput text property, to update this you must use TextInput.SetText
    /// </summary>
    public string Text => _internalTextInput.Text;

    /// <summary>
    /// Placeholder text, this will update on the fly, it'll be shown when no text is entered
    /// </summary>
    public string Placeholder
    {
        get => _internalTextInput.Placeholder;
        set => _internalTextInput.Placeholder = value;
    }

    internal TextInput(ABI_RC.Systems.UI.UILib.UIObjects.Components.TextInput text) : base(text)
    {
        _internalTextInput = text;
    }

    /// <summary>
    /// Sets the text of the TextInput element without triggering the update event
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        _internalTextInput.SetText(text);
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
