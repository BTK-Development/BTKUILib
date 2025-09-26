using System;
using ABI_RC.Core.InteractionSystem;
using cohtml;

namespace BTKUILib.UIObjects.Components
{
    /// <summary>
    /// Basic toggle button element
    /// </summary>
    public class ToggleButton : QMInteractable
    {
        private ABI_RC.Systems.UI.UILib.UIObjects.Components.ToggleButton _internalToggleButton;
        
        /// <summary>
        /// Gets or sets the current state of the toggle, will update on the fly
        /// </summary>
        public bool ToggleValue
        {
            get => _internalToggleButton.ToggleValue;
            set => _internalToggleButton.ToggleValue = value;
        }

        /// <summary>
        /// Get or set the current name of the toggle, will update on the fly
        /// </summary>
        public string ToggleName
        {
            get => _internalToggleButton.ToggleName;
            set => _internalToggleButton.ToggleName = value;
        }

        /// <summary>
        /// Get or set the current tooltip displayed on this toggle, will update on the fly
        /// </summary>
        public string ToggleTooltip
        {
            get => _internalToggleButton.ToggleTooltip;
            set => _internalToggleButton.ToggleTooltip = value;
        }

        /// <summary>
        /// Action to listen for changes of the toggle state
        /// </summary>
        public Action<bool> OnValueUpdated;

        internal ToggleButton(ABI_RC.Systems.UI.UILib.UIObjects.Components.ToggleButton toggle) : base(toggle)
        {
            _internalToggleButton = toggle;
            toggle.OnValueUpdated += b =>
            {
                OnValueUpdated?.Invoke(b);
            };
        }

        /// <inheritdoc />
        public override void Delete()
        {
            _internalToggleButton.Delete();
        }
    }
}