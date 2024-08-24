using BTKUILib.UIObjects;
using BTKUILib.UIObjects.Components;
using BTKUILib.UIObjects.Objects;
using System;
using UnityEngine;

namespace BTKUILib;

internal class ColourPicker
{
    internal static ColourPicker Instance;

    private Page _colourPickerPage;

    //Sliders
    private SliderFloat _rSlider;
    private SliderFloat _gSlider;
    private SliderFloat _bSlider;
    private TextInput _rInput;
    private TextInput _gInput;
    private TextInput _bInput;

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
        _colourPreviewUpdate = new CustomEngineOnFunction("internalColourPickerPreview",
                                                          """let element = document.getElementById("btkUI-ColorPreview");element.style.backgroundColor = "#" + colour;""",
                                                          new Parameter("colour", typeof(string), true, false)
        );
        colourPreview.AddEngineOnFunction(_colourPreviewUpdate);
        mainCategory.AddCustomElement(colourPreview);

        var save = mainCategory.AddButton("Save", "Checkmark", "Save this colour");
        save.OnPress += SaveColour;
        save.ColumnCount = 6;

        var rTitle = mainCategory.AddTextBlock("Red: ");
        rTitle.ColumnCount = 2;
        _rInput = mainCategory.AddTextInput(_currentColour.r.ToString("0.00"));
        _rInput.ColumnCount = 2;
        _rSlider = mainCategory.AddSlider("Red", "Adjusts the red level for this colour", _currentColour.r, 0, 1, 2, 0, false, true);
        var gTitle= mainCategory.AddTextBlock("Green: ");
        gTitle.ColumnCount = 2;
        _gInput = mainCategory.AddTextInput(_currentColour.g.ToString("0.00"));
        _gInput.ColumnCount = 2;
        _gSlider = mainCategory.AddSlider("Green", "Adjusts the green level for this colour", _currentColour.g, 0, 1, 2, 0, false, true);
        var bTitle = mainCategory.AddTextBlock("Blue: ");
        bTitle.ColumnCount = 2;
        _bInput = mainCategory.AddTextInput(_currentColour.b.ToString("0.00"));
        _bInput.ColumnCount = 2;
        _bSlider = mainCategory.AddSlider("Blue", "Adjusts the blue level for this colour", _currentColour.b, 0, 1, 2, 0, false, true);

        _rSlider.OnValueUpdated += f => OnColorChanged();
        _gSlider.OnValueUpdated += f => OnColorChanged();
        _bSlider.OnValueUpdated += f => OnColorChanged();
        _rInput.OnTextUpdate += s => OnTextUpdate(s, _rInput, _rSlider);
        _gInput.OnTextUpdate += s => OnTextUpdate(s, _gInput, _gSlider);
        _bInput.OnTextUpdate += s => OnTextUpdate(s, _bInput, _bSlider);
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

    private void OnTextUpdate(string text, TextInput input, SliderFloat slider)
    {
        if (!float.TryParse(text, out var newValue) || newValue > 1 || newValue < 0)
        {
            input.SetText(slider.SliderValue.ToString("0.00"));
            return;
        }

        slider.SetSliderValue(newValue);

        OnColorChanged();
    }

    private void OnColorChanged()
    {
        _currentColour = new Color(_rSlider.SliderValue, _gSlider.SliderValue, _bSlider.SliderValue);
        var colourHtml = ColorUtility.ToHtmlStringRGB(_currentColour);
        _rInput.SetText(_currentColour.r.ToString("0.00"));
        _gInput.SetText(_currentColour.g.ToString("0.00"));
        _bInput.SetText(_currentColour.b.ToString("0.00"));
        _colourPreviewUpdate.TriggerEvent(colourHtml);

        if(_callbackAction != null && _livePreview)
            _callbackAction.Invoke(_currentColour, "#" + ColorUtility.ToHtmlStringRGB(_currentColour));
    }
}
