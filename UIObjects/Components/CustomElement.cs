using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ABI_RC.Core.InteractionSystem;
using BTKUILib.UIObjects.Objects;

namespace BTKUILib.UIObjects.Components;

/// <summary>
/// Custom element component can be used to create custom templates and functionality that gets injected into Cohtml
/// </summary>
public class CustomElement : QMUIElement
{
    internal ABI_RC.Systems.UI.UILib.UIObjects.Components.CustomElement InternalCE;
    
    /// <summary>
    /// Called when the custom element has completed its GenerateCohtml function, you can safely use engineOn functions from here
    /// </summary>
    public Action OnElementGenerated { get; set; }

    /// <summary>
    /// Custom element constructor, most parts of a custom element cannot be changed after generation
    /// </summary>
    /// <param name="template">CVR QM template code</param>
    /// <param name="elementType">Type of custom element, controls where and how the element reacts</param>
    /// <param name="parentPage">Parent page of the element, only used for on page elements</param>
    /// <param name="parentCategory">Parent category of the element, only used for in category elements</param>
    public CustomElement(string template, ElementType elementType, Page parentPage = null, Category parentCategory = null)
    {
        InternalCE = new ABI_RC.Systems.UI.UILib.UIObjects.Components.CustomElement(template, (ABI_RC.Systems.UI.UILib.UIObjects.Components.ElementType)elementType, parentPage?.InternalPage, parentCategory?.InternalCategory);
        InternalElement = InternalCE;
        InternalCE.OnElementGenerated += () =>
        {
            OnElementGenerated?.Invoke();
        };
    }

    /// <summary>
    /// Creates an action that can be used within Cohtml, these must be added before generation occurs!
    /// </summary>
    /// <param name="actionName">Action name, used in the h: value of a template element</param>
    /// <param name="actionCode">Javascript code to be executed on click</param>
    public void AddAction(string actionName, string actionCode)
    {
        InternalCE.AddAction(actionName, actionCode);
    }

    /// <summary>
    /// Remove specific action from list, this only affects the C# side, it cannot be changed on the fly
    /// </summary>
    /// <param name="actionName"></param>
    public void RemoveAction(string actionName)
    {
        InternalCE.RemoveAction(actionName);
    }

    /// <summary>
    /// Clears all actions from list, this only affects the C# side, it cannot be changed on the fly
    /// </summary>
    public void ClearActions()
    {
        InternalCE.ClearActions();
    }

    /// <summary>
    /// Creates a engine.on function within Cohtml, these can be called from C# with parameters
    /// All must be added before GenerateCohtml is called as they cannot be added afterwards!
    ///
    /// You will want to store the reference to this CustomEngineOnFunction so you can call it later!
    /// </summary>
    /// <param name="function">CustomEngineOnFunction object containing code and parameters</param>
    public void AddEngineOnFunction(CustomEngineOnFunction function)
    {
        InternalCE.AddEngineOnFunction(function.InternalCEFunction);
    }

    /// <summary>
    /// Remove specific function from list, this only affects the C# side, it cannot be changed on the fly
    /// </summary>
    /// <param name="functionName"></param>
    public void RemoveEngineOnFunction(string functionName)
    {
        InternalCE.RemoveEngineOnFunction(functionName);
    }

    /// <summary>
    /// Clears all functions from list, this only affects the C# side, it cannot be changed on the fly
    /// </summary>
    public void ClearEngineOnFunctions()
    {
        InternalCE.ClearEngineOnFunctions();
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