using System;
using ABI_RC.Core.InteractionSystem;
using cohtml;

namespace BTKUILib.UIObjects.Components
{

    /// <summary>
    /// Basic button element
    /// </summary>
    public class Button : QMInteractable
    {
        /// <summary>
        /// Get or set the text displayed on this button, will update on the fly
        /// </summary>
        public string ButtonText
        {
            get => _buttonText;
            set
            {
                _buttonText = value;
                UpdateButton();
            }
        }

        /// <summary>
        /// Get or set the button icon, will update on the fly
        /// Can take a URL, this is limited to images hosted on https://files.abidata.io/
        /// </summary>
        public string ButtonIcon
        {
            get => _buttonIcon;
            set
            {
                _buttonIcon = value;
                UpdateButton();
            }
        }

        /// <summary>
        /// Get or set the tooltip displayed on this button, will update on the fly
        /// </summary>
        public string ButtonTooltip
        {
            get => _buttonTooltip;
            set
            {
                _buttonTooltip = value;
                UpdateButton();
            }
        }

        /// <summary>
        /// Action to listen for clicks of the button
        /// </summary>
        public Action OnPress;

        private string _buttonText;
        private string _buttonIcon;
        private string _buttonTooltip;
        private Category _category;
        private readonly ButtonStyle _style;

        internal Button(string buttonText, string buttonIcon, string buttonTooltip, Category category, ButtonStyle style = ButtonStyle.TextWithIcon)
        {
            _buttonIcon = buttonIcon;
            _buttonText = buttonText;
            _buttonTooltip = buttonTooltip;
            _category = category;
            _style = style;

            ElementID = "btkUI-Button-" + UUID;
        }

        /// <inheritdoc />
        public override void Delete()
        {
            base.Delete();
            if (Protected) return;
            _category.CategoryElements.Remove(this);
        }

        internal override void OnInteraction(bool? toggle = null)
        {
            OnPress?.Invoke();
        }

        internal override void GenerateCohtml()
        {
            if (!IsGenerated)
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkCreateButton", _category.ElementID, _buttonText, _buttonIcon, _buttonTooltip, UUID, _category.ModName, (int)_style);
            
            base.GenerateCohtml();

            IsGenerated = true;
        }

        private void UpdateButton()
        {
            if (!BTKUILib.Instance.IsOnMainThread())
            {
                BTKUILib.Instance.MainThreadQueue.Enqueue(UpdateButton);
                return;
            }

            if(_style != ButtonStyle.TextOnly)
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkUpdateIcon", ElementID, _category.ModName, _buttonIcon, _style == ButtonStyle.TextWithIcon ? "Image" : "Tooltip");
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkUpdateTooltip", $"{ElementID}-Tooltip", _buttonTooltip);
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkUpdateText", $"{ElementID}-Text", _buttonText);
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