using System;
using System.Collections.Generic;
using ABI_RC.Core.InteractionSystem;

namespace BTKUILib.UIObjects.Components;

public class CustomElement : QMUIElement
{
    //btkUI-Custom-[UUID] required in "id" of root
    private string _template;
    private ElementType _type;
    private Page _parentPage;
    private Category _parentCategory;
    private Dictionary<string, string> _actionFunctions = new();

    public CustomElement(string template, ElementType type, Page parentPage = null, Category parentCategory = null)
    {
        _template = template;
        _type = type;
        _parentPage = parentPage;
        _parentCategory = parentCategory;

        ElementID = "btkUI-Custom-" + UUID;

        UserInterface.CustomElements.Add(this);
    }

    /// <summary>
    /// Creates an action that can be used within Cohtml, these must be added before generation occurs!
    /// </summary>
    /// <param name="actionName">Action name, used in the h: value of a template element</param>
    /// <param name="actionCode">Javascript code to be executed on click</param>
    public void AddAction(string actionName, string actionCode)
    {
        if (_actionFunctions.ContainsKey(actionName))
        {
            BTKUILib.Log.Error("Duplicate action name given for custom element!");
            return;
        }

        _actionFunctions.Add(actionName, actionCode);
    }

    /// <summary>
    /// Remove specific action from list, this only affects the C# side, it cannot be changed on the fly
    /// </summary>
    /// <param name="actionName"></param>
    public void RemoveAction(string actionName)
    {
        if (_actionFunctions.ContainsKey(actionName))
            _actionFunctions.Remove(actionName);
    }

    /// <summary>
    /// Clears all actions from list, this only affects the C# side, it cannot be changed on the fly
    /// </summary>
    public void ClearActions()
    {
        _actionFunctions.Clear();
    }

    internal override void GenerateCohtml()
    {
        if (!UIUtils.IsQMReady()) return;

        if (!IsGenerated)
        {
            foreach(var action in _actionFunctions)
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkAddCustomAction", action.Key, action.Value);

            switch (_type)
            {
                case ElementType.GlobalElement:
                    CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkCreateCustomGlobal", UUID, _template);
                    break;
                case ElementType.CustomPage:
                    break;
                case ElementType.OnPageElement:
                    break;
                case ElementType.InCategoryElement:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public override void Delete()
    {
        base.Delete();
    }
}

/// <summary>
/// The element type determines what should be expected for this element, as well as controls if it appears in special places like btkUI-shared
/// </summary>
public enum ElementType
{
    /// <summary>
    /// GlobalElement makes this element generate with btkUI-Shared, which is always visible
    /// </summary>
    GlobalElement,
    /// <summary>
    /// CustomPage will make this element generate as a page
    /// </summary>
    CustomPage,
    /// <summary>
    /// OnPageElement makes this element generate within a page, expects a target page to be set
    /// </summary>
    OnPageElement,
    /// <summary>
    /// InCategoryElement makes this generate within a category, expects a target category to be set
    /// </summary>
    InCategoryElement,
}