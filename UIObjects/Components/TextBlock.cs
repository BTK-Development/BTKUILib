namespace BTKUILib.UIObjects.Components;

/// <summary>
/// TextBlock element
/// </summary>
public class TextBlock : QMUIElement
{
    private ABI_RC.Systems.UI.UILib.UIObjects.Components.TextBlock _internalTextBlock;
    
    /// <summary>
    /// Text property of this TextBlock, changing this will update on the fly
    /// </summary>
    public string Text
    {
        get => _internalTextBlock.Text;
        set => _internalTextBlock.Text = value;
    }

    internal TextBlock(ABI_RC.Systems.UI.UILib.UIObjects.Components.TextBlock block) : base(block)
    {
        _internalTextBlock = block;
    }
}
