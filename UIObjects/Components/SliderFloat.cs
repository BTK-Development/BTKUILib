using System;
using ABI_RC.Core.InteractionSystem;
using cohtml;

namespace BTKUILib.UIObjects.Components
{
    /// <summary>
    /// Slider element
    /// </summary>
    public class SliderFloat : QMUIElement
    {
        /// <summary>
        /// Get or set the name of this slider, will update on the fly
        /// </summary>
        public string SliderName
        {
            get => _sliderName;
            set
            {
                _sliderName = value;
                UpdateSlider();
            }
        }

        /// <summary>
        /// Get or set the tooltip displayed when hovering on this slider, will update on the fly
        /// </summary>
        public string SliderTooltip
        {
            get => _sliderTooltip;
            set
            {
                _sliderTooltip = value; 
                UpdateSlider();
            }
        }

        /// <summary>
        /// Get or set the current min value of the slider, will update on the fly
        /// </summary>
        public float MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value; 
                UpdateSlider();
            }
        }

        /// <summary>
        /// Get or set the current max value of the slider, will update on the fly
        /// </summary>
        public float MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                UpdateSlider();
            }
        }
        
        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set
            {
                _decimalPlaces = value;
                UpdateSlider();
            }
        }

        /// <summary>
        /// Get the current value of the slider
        /// </summary>
        public float SliderValue
        {
            get => _sliderValue;
            internal set
            {
                _sliderValue = value;
                OnValueUpdated?.Invoke(value);
            }
        }

        /// <summary>
        /// Action to listen for changes of the value for the slider
        /// </summary>
        public Action<float> OnValueUpdated;

        private float _sliderValue;
        private string _sliderName;
        private string _sliderTooltip;
        private float _minValue;
        private float _maxValue;
        private int _decimalPlaces;
        private Page _page;

        internal SliderFloat(Page page, string sliderName, string sliderTooltip, float initalValue, float minValue = 0f, float maxValue = 10f, int decimalPlaces = 2)
        {
            _sliderValue = initalValue;
            _sliderName = sliderName;
            _sliderTooltip = sliderTooltip;
            _minValue = minValue;
            _maxValue = maxValue;
            _page = page;
            _decimalPlaces = decimalPlaces;
            
            UserInterface.Sliders.Add(UUID, this);
            
            ElementID = $"btkUI-Slider-{UUID}";
        }

        /// <summary>
        /// Sets the current value of the slider without triggering the action
        /// </summary>
        /// <param name="value"></param>
        public void SetSliderValue(float value)
        {
            _sliderValue = value;
            UpdateSlider();
        }
        
        /// <inheritdoc />
        public override void Delete()
        {
            base.Delete();
            
            if (Protected) return;
            
            _page.PageElements.Remove(this);
        }

        internal override void GenerateCohtml()
        {
            if(!IsGenerated)
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkCreateSlider", _page.ElementID, _sliderName, UUID, _sliderValue, _minValue, _maxValue, _sliderTooltip, _decimalPlaces);

            IsGenerated = true;
        }

        private void UpdateSlider()
        {
            if (!UIUtils.IsQMReady()) return;
            
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkSliderUpdateSettings", UUID, _sliderName, _sliderTooltip, _minValue, _maxValue, _decimalPlaces);
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkSliderSetValue", UUID, SliderValue);
        }
    }
}