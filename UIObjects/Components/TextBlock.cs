namespace BTKUILib.UIObjects.Components;

/// <summary>
/// TextBlock element
/// </summary>
public class TextBlock : QMUIElement
{
    /// <summary>
    /// Text property of this TextBlock, changing this will update on the fly
    /// </summary>
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            UpdateText();
        }
    }

    private string _text;
    private string[] _additionalCSSClasses;

    internal TextBlock(string text, Category category, params string[] additionalCSSClasses)
    {
        _text = text;
        _additionalCSSClasses = additionalCSSClasses;

        Parent = category;

        ElementID = $"btkUI-TextBlock-{UUID}-Root";
    }

    internal override void GenerateCohtml()
    {
        if (!UIUtils.IsQMReady()) return;

        if (RootPage is { IsVisible: false }) return;

        if(!IsGenerated)
            UIUtils.GetInternalView().TriggerEvent("btkCreateTextBlock", Parent.ElementID, UUID, Text);

        base.GenerateCohtml();

        IsGenerated = true;
    }

    private void UpdateText()
    {
        if(!IsVisible) return;

        if (!BTKUILib.Instance.IsOnMainThread())
        {
            BTKUILib.Instance.MainThreadQueue.Enqueue(UpdateText);
            return;
        }

        if (!UIUtils.IsQMReady()) return;

        UIUtils.GetInternalView().TriggerEvent("btkUpdateText", ElementID, Text);
    }
}
