using System;
using JetBrains.Annotations;
using MelonLoader;

namespace BTKUILib.UIObjects.Components
{
    /// <summary>
    /// Basic button element
    /// </summary>
    public class Button : QMInteractable
    {
        private readonly ABI_RC.Systems.UI.UILib.UIObjects.Components.Button _internalButton;
        
        /// <summary>
        /// Get or set the text displayed on this button, will update on the fly
        /// </summary>
        public string ButtonText
        {
            get => _internalButton.ButtonText;
            set => _internalButton.ButtonText = value;
        }

        /// <summary>
        /// Get or set the button icon, will update on the fly
        /// Can take a URL, this is limited to images hosted on https://files.abidata.io/
        /// </summary>
        public string ButtonIcon
        {
            get => _internalButton.ButtonIcon;
            set => _internalButton.ButtonIcon = value;
        }

        /// <summary>
        /// Get or set the tooltip displayed on this button, will update on the fly
        /// </summary>
        public string ButtonTooltip
        {
            get => _internalButton.ButtonTooltip;
            set => _internalButton.ButtonTooltip = value;
        }

        /// <summary>
        /// Action to listen for clicks of the button
        /// </summary>
        public Action OnPress;
        /// <summary>
        /// OnHeld is fired when the button is held down for a set amount of time
        /// </summary>
        public Action OnHeld;

        internal Button(ABI_RC.Systems.UI.UILib.UIObjects.Components.Button button) : base(button)
        {
            _internalButton = button;
            button.OnPress += () =>
            {
                OnPress?.Invoke();
            };
            button.OnHeld += () =>
            {
                OnHeld?.Invoke();
            };
        }

        /// <inheritdoc />
        public override void Delete()
        {
            _internalButton.Delete();
        }
    }

    /// <summary>
    /// Configures the visual style of a button with UILib
    /// </summary>
    public enum ButtonStyle
    {
        /// <summary>
        /// Default button with an icon on top and text at the bottom
        /// </summary>
        TextWithIcon,
        /// <summary>
        /// Button without an icon and with text that can fill the entire thing
        /// </summary>
        TextOnly,
        /// <summary>
        /// Button with an icon behind the text, icon can fill entire button as well as text
        /// </summary>
        FullSizeImage
    }
}