using System;
using MelonLoader;
using System.Collections;
using UnityEngine;

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
        /// <summary>
        /// OnHeld is fired when the button is held down for a set amount of time
        /// </summary>
        public Action OnHeld;

        private string _buttonText;
        private string _buttonIcon;
        private string _buttonTooltip;
        private object _coroutineTimer;
        private float _holdWaitTime;
        private bool _skipClick;
        private Category _category;
        private readonly ButtonStyle _style;

        internal Button(string buttonText, string buttonIcon, string buttonTooltip, Category category, ButtonStyle style = ButtonStyle.TextWithIcon, float holdWaitTime = 0.5f)
        {
            _buttonIcon = buttonIcon;
            _buttonText = buttonText;
            _buttonTooltip = buttonTooltip;
            _category = category;
            _style = style;
            _holdWaitTime = holdWaitTime;

            Parent = category;

            ElementID = "btkUI-Button-" + UUID;
        }

        /// <inheritdoc />
        public override void Delete()
        {
            base.Delete();
            if (Protected) return;
            _category.SubElements.Remove(this);
        }

        internal override void OnInteraction(bool? toggle = null)
        {
            if (_coroutineTimer != null)
            {
                MelonCoroutines.Stop(_coroutineTimer);
                _coroutineTimer = null;
            }

            if (_skipClick)
            {
                _skipClick = false;
                return;
            }

            OnPress?.Invoke();
        }

        internal override void GenerateCohtml()
        {
            if (!UIUtils.IsQMReady()) return;

            if (RootPage is { IsVisible: false }) return;

            if (!IsGenerated)
                UIUtils.GetInternalView().TriggerEvent("btkCreateButton", _category.ElementID, _buttonText, _buttonIcon, _buttonTooltip, UUID, _category.ModName, (int)_style);
            
            base.GenerateCohtml();

            IsGenerated = true;
        }

        internal void MouseDown()
        {
            //Start coroutine
            _coroutineTimer = MelonCoroutines.Start(MouseDownCoroutine());
        }

        private IEnumerator MouseDownCoroutine()
        {
            yield return new WaitForSeconds(_holdWaitTime);

            _skipClick = true;
            _coroutineTimer = null;

            //Wait time passed, fire onheld
            OnHeld?.Invoke();
        }

        private void UpdateButton()
        {
            if(!IsVisible) return;

            if (!BTKUILib.Instance.IsOnMainThread())
            {
                BTKUILib.Instance.MainThreadQueue.Enqueue(UpdateButton);
                return;
            }

            if(_style != ButtonStyle.TextOnly)
                UIUtils.GetInternalView().TriggerEvent("btkUpdateIcon", ElementID, _category.ModName, _buttonIcon, _style == ButtonStyle.TextWithIcon ? "Image" : "");
            UIUtils.GetInternalView().TriggerEvent("btkUpdateTooltip", $"{ElementID}-Root", _buttonTooltip);
            UIUtils.GetInternalView().TriggerEvent("btkUpdateText", $"{ElementID}-Text", _buttonText);
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