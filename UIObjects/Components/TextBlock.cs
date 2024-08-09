namespace BTKUILib.UIObjects.Components;

public class TextBlock : QMUIElement
{
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

    internal TextBlock(string text, Category category)
    {
        _text = text;

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
