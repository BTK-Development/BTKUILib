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
        private readonly ABI_RC.Systems.UI.UILib.UIObjects.Components.SliderFloat _internalSlider;
        
        /// <summary>
        /// Get or set the name of this slider, will update on the fly
        /// </summary>
        public string SliderName
        {
            get => _internalSlider.SliderName;
            set => _internalSlider.SliderName = value;
        }

        /// <summary>
        /// Get or set the tooltip displayed when hovering on this slider, will update on the fly
        /// </summary>
        public string SliderTooltip
        {
            get => _internalSlider.SliderTooltip;
            set => _internalSlider.SliderTooltip = value;
        }

        /// <summary>
        /// Get or set the current min value of the slider, will update on the fly
        /// </summary>
        public float MinValue
        {
            get => _internalSlider.MinValue;
            set => _internalSlider.MinValue = value;
        }

        /// <summary>
        /// Get or set the current max value of the slider, will update on the fly
        /// </summary>
        public float MaxValue
        {
            get => _internalSlider.MaxValue;
            set => _internalSlider.MaxValue = value;
        }
        
        /// <summary>
        /// Get or set the current decimal places displayed on a slider
        /// </summary>
        public int DecimalPlaces
        {
            get => _internalSlider.DecimalPlaces;
            set => _internalSlider.DecimalPlaces = value;
        }

        /// <summary>
        /// Sets the default value a slider can be reset to
        /// </summary>
        public float DefaultValue
        {
            get => _internalSlider.DefaultValue;
            set => _internalSlider.DefaultValue = value;
        }

        /// <summary>
        /// Sets if a slider is allowed to be reset
        /// </summary>
        public bool AllowDefaultReset
        {
            get => _internalSlider.AllowDefaultReset;
            set => _internalSlider.AllowDefaultReset = value;
        }

        /// <summary>
        /// Get the current value of the slider
        /// </summary>
        public float SliderValue => _internalSlider.SliderValue;

        /// <inheritdoc />
        public override bool Hidden
        {
            get => _internalSlider.Hidden;
            set => _internalSlider.Hidden = value;
        }

        /// <summary>
        /// Action to listen for changes of the value for the slider
        /// </summary>
        public Action<float> OnValueUpdated;
        /// <summary>
        /// Fired when the reset button is used on a slider
        /// </summary>
        public Action OnSliderReset;

        internal SliderFloat(ABI_RC.Systems.UI.UILib.UIObjects.Components.SliderFloat slider) : base(slider)
        {
            _internalSlider = slider;
            slider.OnValueUpdated += f =>
            {
                OnValueUpdated?.Invoke(f);
            };
            slider.OnSliderReset += () =>
            {
                OnSliderReset?.Invoke();
            };
        }

        /// <summary>
        /// Sets the current value of the slider without triggering the action
        /// </summary>
        /// <param name="value"></param>
        public void SetSliderValue(float value)
        {
            _internalSlider.SetSliderValue(value);
        }
        
        /// <inheritdoc />
        public override void Delete()
        {
            _internalSlider.Delete();
        }
    }
}