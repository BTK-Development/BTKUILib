using System;
using System.Collections.Generic;
using System.Linq;

namespace BTKUILib.UIObjects.Components;

public class SliderList<T> : QMUIElement
{
    public string SliderName
    {
        get => _sliderName;
        set
        {
            _sliderName = value;
        }
    }

    public string SliderTooltip
    {
        get => _sliderTooltip;
        set
        {
            _sliderTooltip = value;
        }
    }

    public T DefaultOption
    {
        get => _optionList.Values.ToArray()[_defaultValue];
        set
        {
            if (!_optionList.ContainsValue(value)) return;

            _defaultValue = Array.IndexOf(_optionList.Values.ToArray(), value);
        }
    }

    public bool AllowDefaultReset
    {
        get => _allowDefaultReset;
        set
        {
            _allowDefaultReset = value;
        }
    }

    public T SliderValue
    {
        get => _optionList.Values.ToArray()[_sliderValue];
        set
        {
            if (!_optionList.ContainsValue(value)) return;

            _sliderValue = Array.IndexOf(_optionList.Values.ToArray(), value);
        }
    }

    public Action<T> OnValueUpdated;

    private int _sliderValue;
    private string _sliderName;
    private string _sliderTooltip;
    private int _defaultValue;
    private bool _allowDefaultReset;
    private Dictionary<string, T> _optionList = new();

    internal SliderList(QMUIElement parent, string sliderName, string sliderTooltip, T defaultOption, bool allowDefaultReset, T sliderValue, params (string, T)[] optionsList)
    {
        SliderName = sliderName;
        SliderTooltip = sliderTooltip;
        DefaultOption = defaultOption;
        AllowDefaultReset = allowDefaultReset;
        SliderValue = sliderValue;
        Parent = parent;

        foreach (var pair in optionsList)
        {
            if(_optionList.ContainsKey(pair.Item1)) continue;

            _optionList.Add(pair.Item1, pair.Item2);
        }

        ElementID = $"btkUI-SliderList-{UUID}";
    }

    internal override void GenerateCohtml()
    {
        if (!IsVisible) return;

        if (!Parent.IsVisible) return;

        if (!UIUtils.IsQMReady()) return;

        var settings = new SliderListSettings<T>
        {
            SliderName = _sliderName,
            SliderTooltip = _sliderTooltip,
            DefaultValue = _defaultValue,
            AllowDefaultReset = _allowDefaultReset,
            OptionsList = $"[{string.Join(", ", _optionList.Keys.Select(x => "\"" + x + "\""))}]"
        };


    }
}

struct SliderListSettings<T>
{
    public string SliderName;
    public string SliderTooltip;
    public int DefaultValue;
    public bool AllowDefaultReset;
    public string OptionsList;
}