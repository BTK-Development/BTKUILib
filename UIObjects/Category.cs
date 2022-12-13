﻿using System.Collections.Generic;
using ABI_RC.Core.InteractionSystem;
using BTKUILib.UIObjects.Components;
using cohtml;

namespace BTKUILib.UIObjects
{
    /// <summary>
    /// This act as category with header and row within Cohtml
    /// </summary>
    public class Category : QMUIElement
    {
        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                UpdateCategoryName();
            }
        }

        internal List<QMUIElement> CategoryElements = new();

        private string _categoryName;
        private Page _linkedPage;
        private bool _showHeader = false;

        internal Category(string categoryName, Page page, bool showHeader = true)
        {
            _categoryName = categoryName;
            _linkedPage = page;
            _showHeader = showHeader;
            
            ElementID = "btkUI-Row-" + UUID;
        }

        public Button AddButton(string buttonText, string buttonIcon, string buttonTooltip)
        {
            var button = new Button(buttonText, buttonIcon, buttonTooltip, this);
            CategoryElements.Add(button);
            
            if(UIUtils.IsQMReady())
                button.GenerateCohtml();

            return button;
        }

        internal override void GenerateCohtml()
        {
            if(!IsGenerated)
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkCreateRow", _linkedPage.ElementID, UUID, _showHeader ? _categoryName : null);
            
            foreach(var element in CategoryElements)
                element.GenerateCohtml();

            IsGenerated = true;
        }
        
        private void UpdateCategoryName()
        {
            if (!UIUtils.IsQMReady()) return;
            
            CVR_MenuManager.Instance.quickMenu.View.TriggerEvent("btkUpdateText", $"btkUI-Row-HeaderText-{UUID}", _categoryName);
        }
    }
}