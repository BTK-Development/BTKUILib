using System;

namespace BTKUILib.UIObjects.Components
{
    public class ToggleButton : QMUIElement
    {
        public bool ToggleValue
        {
            get => _toggleValue;
            set
            {
                _toggleValue = value;
                OnValueUpdated?.Invoke(value);
            }
        }

        public Action<bool> OnValueUpdated;

        private bool _toggleValue;

        internal ToggleButton(string elementID, bool initialValue)
        {
            _toggleValue = initialValue;
            ElementID = elementID;
            
            UserInterface.ToggleButtons.Add(this);
        }

        public void UpdateToggleValue(bool value)
        {
            ToggleValue = value;

            UpdateToggle();
        }

        internal void UpdateToggle()
        {
            if (!UIUtils.IsQMReady()) return;
            QuickMenuAPI.SetToggleState(this, ToggleValue);
        }
    }
}