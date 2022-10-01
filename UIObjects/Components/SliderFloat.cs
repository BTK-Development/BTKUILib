using System;
using ABI_RC.Core.InteractionSystem;
using cohtml;

namespace BTKUILib.UIObjects.Components
{
    public class SliderFloat : QMUIElement
    {
        public float SliderValue
        {
            get => _sliderValue;
            set
            {
                _sliderValue = value;
                OnValueUpdated?.Invoke(value);
            }
        }
        
        public Action<float> OnValueUpdated;

        private float _sliderValue;

        internal SliderFloat(string elementID, float initalValue)
        {
            SliderValue = initalValue;
            ElementID = elementID;
            
            UserInterface.SliderFloats.Add(this);
        }

        public void UpdateSliderValue(float value)
        {
            SliderValue = value;
            
            if (!UIUtils.IsQMReady()) return;
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkUISliderSetValue", ElementID, value);     
        }

        internal void UpdateSlider()
        {
            if (!UIUtils.IsQMReady()) return;
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkUISliderSetValue", ElementID, SliderValue);
        }
    }
}