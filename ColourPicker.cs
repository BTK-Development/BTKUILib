using BTKUILib.UIObjects;
using BTKUILib.UIObjects.Components;
using BTKUILib.UIObjects.Objects;
using System;
using UnityEngine;

namespace BTKUILib;

public class ColourPicker
{
    internal static ColourPicker Instance;

    private Page _colourPickerPage;

    //Sliders
    private SliderFloat _rSlider;
    private SliderFloat _gSlider;
    private SliderFloat _bSlider;
    private SliderFloat _aSlider;

    private CustomEngineOnFunction _colourPreviewUpdate;
    private Color _currentColour = Color.white;
    private Action<Color, string> _callbackAction;
    private bool _livePreview;

    internal ColourPicker()
    {
        Instance = this;

        SetupColourPicker();
    }

    internal void OpenColourPicker(Color currentColor, Action<Color, string> callback, bool livePreview = false)
    {
        _currentColour = currentColor;
        _callbackAction = callback;
        _livePreview = livePreview;

        _rSlider.SetSliderValue(_currentColour.r);
        _gSlider.SetSliderValue(_currentColour.g);
        _bSlider.SetSliderValue(_currentColour.b);

        _colourPickerPage.InPlayerlist = UserInterface.IsInPlayerList;

        _colourPickerPage.OpenPage();

        OnColorChanged();
    }

    private void SetupColourPicker()
    {
        if(_colourPickerPage != null) return;

        QuickMenuAPI.OnBackAction += OnBackAction;

        _colourPickerPage = Page.GetOrCreatePage("BTKUILib", "Colour Picker", false, null, null, true);
        QuickMenuAPI.AddRootPage(_colourPickerPage);

        var mainCategory = _colourPickerPage.AddCategory("", false, false);

        var colourPreview = new CustomElement("""{"c":"col-6", "s":[{"c":"colour-preview", "a":{"id" : "btkUI-ColorPreview"}}], "a":{"id":"[UUID]"}}""", ElementType.InCategoryElement, null, mainCategory);
        _colourPreviewUpdate = new CustomEngineOnFunction("twUpdateLeashPreview",
                                                          """let element = document.getElementById("btkUI-ColorPreview");element.style.backgroundColor = "#" + colour;""",
                                                          new Parameter("colour", typeof(string), true, false)
        );
        colourPreview.AddEngineOnFunction(_colourPreviewUpdate);
        mainCategory.AddCustomElement(colourPreview);

        var save = mainCategory.AddButton("Save", "Checkmark", "Save this colour");
        save.OnPress += SaveColour;

        _rSlider = mainCategory.AddSlider("Red", "Adjusts the red level for this colour", _currentColour.r, 0, 1);
        _gSlider = mainCategory.AddSlider("Green", "Adjusts the green level for this colour", _currentColour.g, 0, 1);
        _bSlider = mainCategory.AddSlider("Blue", "Adjusts the blue level for this colour", _currentColour.b, 0, 1);

        _rSlider.OnValueUpdated += f => OnColorChanged();
        _gSlider.OnValueUpdated += f => OnColorChanged();
        _bSlider.OnValueUpdated += f => OnColorChanged();
    }

    private void OnBackAction(string _, string previousPage)
    {
        _callbackAction = null;
    }

    private void SaveColour()
    {
        if(_callbackAction != null)
            _callbackAction.Invoke(_currentColour, "#" + ColorUtility.ToHtmlStringRGB(_currentColour));

        QuickMenuAPI.GoBack();
    }

    private void OnColorChanged()
    {
        var color = new Color(_rSlider.SliderValue, _gSlider.SliderValue, _bSlider.SliderValue);
        var colorhtml = ColorUtility.ToHtmlStringRGB(color);
        _colourPreviewUpdate.TriggerEvent(colorhtml);

        if(_callbackAction != null && _livePreview)
            _callbackAction.Invoke(_currentColour, "#" + ColorUtility.ToHtmlStringRGB(_currentColour));
    }
}
