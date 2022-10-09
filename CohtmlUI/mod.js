cvr.menu.prototype.BTKUI = {
    uiRefBTK: {},
    breadcrumbsBTK: [],
    currentPageBTK: "",
    currentDraggedSliderBTK: {},
    currentSliderBarBTK: {},
    currentSliderKnobBTK: {},
    setSliderFunctionBTK: {},
    pushPageBTK: {},
    currentMod: "",
    isDraggingBTK: false,

    info: function(){
        return {
            name: "BTKUI Library",
            version_major: 0,
            version_minor: 1,
            description: "BTKUI Library",
            author: "BTK Development Team",
            author_id: "",
            external_links: [
            ],
            stylesheets: [
                {filename: "BTKUI.css", modes: ["quickmenu"]},
                {filename: "bootstrap-grid.min.css", modes: ["quickmenu"]}
            ],
            compatibility: [],
            icon: "BTKIcon.png",
            feature_level: 1,
            supported_modes: ["quickmenu"]
        };
    },

    translation: function(menu){
        menu.translations["en"]["btkUI-title"] = "BTKUI";
    },

    register: function(menu){
        uiRefBTK = menu;
        breadcrumbsBTK = [];
        currentPageBTK = "MainPage";
        currentMod = "BTKUI";
        currentDraggedSliderBTK = {};
        currentSliderBarBTK = {};
        currentSliderKnobBTK = {};
        isDraggingBTK = false;
        setSliderFunctionBTK = this.btkSliderSetValue;
        pushPageBTK = this.btkPushPage;

        console.log("Setting up btkUI");

        menu.templates["btkUI-btn"] = {c: "btkUI-btn", s: [{c: "icon"}], x: "btkUI-open", a:{"id":"btkUI-QMButton"}};
        menu.templates["btkUI-menu"] = {c: "btkUI menu-category hide", s: [
                {c: "container container-main", s:[
                        {c:"row", s:[
                                {c:"col", s:[
                                        {c: "header", a:{"id": "btkUI-MenuHeader"}, h: "BTKUI"},
                                    ]},
                            ]}
                    ]},
                {c: "container container-controls hide", a:{"id": "btkUI-Settings"}, s:[{c: "scroll-view", s:[{c: "content scroll-content", s:[

                            ]}, {c: "scroll-marker-v"}]}]},
                {c: "container container-controls hide", a:{"id": "btkUI-DropdownPage"}, s:[
                        {c: "row header-section", s:[
                                {c:"col-1", s:[{c: "icon-back", x: "btkUI-Back"}]},
                                {c:"col", s:[{c:"header", h:"Dropdown", a:{"id": "btkUI-DropdownHeader"}}]}
                            ]},
                        {c: "scroll-view", s:[{c: "content-subpage scroll-content", s:[
                                {c:"row", a:{"id": "btkUI-Dropdown-OptionRoot"}},
                            ]}, {c: "scroll-marker-v"}]}]},
                {c: "container container-controls hide", a:{"id": "btkUI-NumberEntry"}, s:[{c: "scroll-view", s:[{c: "content scroll-content", s:[
                                {c: "row", s:[
                                        {c:"col-1", s:[{c: "icon-close", x: "btkUI-Back"}]},
                                        {c:"col", s:[{c:"header", h:"Number Input", a:{"id": "btkUI-NumberInputHeader"}}]}
                                    ]},
                                {c: "row", s:[
                                        {c:"col-2", s:[{c: "int-button", s:[{c:"button-text", h:"1"}], x: "btkUI-NumInput", a:{"str": "1"}}]},
                                        {c:"col-2", s:[{c: "int-button", s:[{c:"button-text", h:"2"}], x: "btkUI-NumInput", a:{"str": "2"}}]},
                                        {c:"col-2", s:[{c: "int-button", s:[{c:"button-text", h:"3"}], x: "btkUI-NumInput", a:{"str": "3"}}]},
                                        {c:"col", s:[{c: "int-box", s:[{a:{"id" : "btkUI-numDisplay", "data-display" : ""}, c:"text", h:""}]}]},
                                        {c:"col-2", s:[{c: "int-button", s:[{c:"icon-checkmark"}], x: "btkUI-NumSubmit"}]},
                                    ]},
                                {c: "row", s:[
                                        {c:"col-2", s:[{c: "int-button", s:[{c:"button-text", h:"4"}], x: "btkUI-NumInput", a:{"str": "4"}}]},
                                        {c:"col-2", s:[{c: "int-button", s:[{c:"button-text", h:"5"}], x: "btkUI-NumInput", a:{"str": "5"}}]},
                                        {c:"col-2", s:[{c: "int-button", s:[{c:"button-text", h:"6"}], x: "btkUI-NumInput", a:{"str": "6"}}]},
                                    ]},
                                {c: "row", s:[
                                        {c:"col-2", s:[{c: "int-button", s:[{c:"button-text", h:"7"}], x: "btkUI-NumInput", a:{"str": "7"}}]},
                                        {c:"col-2", s:[{c: "int-button", s:[{c:"button-text", h:"8"}], x: "btkUI-NumInput", a:{"str": "8"}}]},
                                        {c:"col-2", s:[{c: "int-button", s:[{c:"button-text", h:"9"}], x: "btkUI-NumInput", a:{"str": "9"}}]},
                                    ]},
                                {c: "row", s:[
                                        {c:"col-2", s:[{c: "int-button", s:[{c:"icon-back"}], x: "btkUI-NumBack"}]},
                                        {c:"col-2", s:[{c: "int-button", s:[{c:"button-text", h:"0"}], x: "btkUI-NumInput", a:{"str": "0"}}]},
                                        {c:"col-2", s:[{c: "int-button", s:[{c:"button-text", h:"."}], x: "btkUI-NumInput", a:{"str": "."}}]},
                                    ]},
                            ]}, {c: "scroll-marker-v"}]}]},
                {c: "container popup-container hide", a: {"id": "btkUI-PopupConfirm"}, s:[
                        {c: "row", s: [
                                {c: "col align-self-center", s: [{c: "header", h: "Notice", a: {"id": "btkUI-PopupConfirmHeader"}}]}
                            ]},
                        {c: "content notice-text", h:"This is some text!", a: {"id": "btkUI-PopupConfirmText"}},
                        {c: "control-row", s: [
                                {c: "col", s: [{c: "button", s: [{c: "text", h: "Yes", a: {"id": "btkUI-PopupConfirmOk"}}], x:"btkUI-ConfirmOK"}]},
                                {c: "col", s: [{c: "button", s: [{c: "text", h: "No", a: {"id": "btkUI-PopupConfirmNo"}}], x:"btkUI-ConfirmNO"}]}
                            ]},
                    ]},
                {c: "container popup-container hide", a: {"id": "btkUI-PopupNotice"}, s:[
                        {c:"row", s:[
                                {c:"col align-self-center", s:[{c:"header", h:"Notice", a: {"id": "btkUI-PopupNoticeHeader"}}]}
                            ]},
                        {c: "content notice-text", h:"This is some text!", a: {"id": "btkUI-PopupNoticeText"}},
                        {c:"row justify-content-center", s:[
                                {c:"col offset-4 align-self-center", s:[{c:"button", s:[{c:"text", h:"OK", a: {"id": "btkUI-PopupNoticeOK"}}], x:"btkUI-NoticeClose"}]}
                            ]},
                    ]},
                {c: "container-back", s:[{c: "icon"}, {c: "content", h: "Back to QuickMenu"}], x: "switchCategory", a: {"data-category": "quickmenu-home"}},
                {c: "container-tooltip hide", s:[{c:"content", h:"tooltip info", a:{"id": "btkUI-Tooltip"}}], a:{"id": "btkUI-TooltipContainer"}}
            ], a:{"id":"btkUI-Root"}};


        menu.templates["btkMenuRow"] = {c:"row justify-content-start", a:{"id": "[row-id]"}};
        menu.templates["btkSlider"] = {c:"", s:[{c:"col-12", s:[{c:"text", h:"[slider-name] - [current-value]", a:{"id": "btkUI-SliderTitle-[slider-id]", "data-title": "[slider-name]"}}]}, {c: "col-12", s:[{c:"slider", s:[{c:"sliderBar", s:[{c:"slider-knob", a:{"id": "btkUI-SliderKnob-[slider-id]"}}], a:{"id": "btkUI-SliderBar-[slider-id]"}}], a:{"id":"btkUI-Slider-[slider-id]", "data-slider": "[slider-id]", "data-slider-value": "[current-value]", "data-min": "[min-value]", "data-max": "[max-value]"}}], a:{"data-tooltip": "[tooltip-text]"}}]};
        menu.templates["btkToggle"] = {c:"col-3", s:[{c: "toggle", s:[{c:"row", s:[{c:"col align-content-start", s:[{c:"enable circle", a:{"id": "btkUI-toggle-enable"}}]}, {c:"col align-content-end", s:[{c:"disable circle active", a:{"id": "btkUI-toggle-disable"}}]}]},{c:"text-sm", h:"[toggle-name]"}], x: "btkUI-Toggle", a:{"id": "btkUI-Toggle-[toggle-location]-[toggle-page]-[toggle-id]", "data-toggle": "[toggle-location]-[toggle-page]-[toggle-id]", "data-toggleState": "false", "data-tooltip": "[tooltip-data]"}}]};
        menu.templates["btkButton"] = {c:"col-3", s:[{c: "button", s:[{c:"icon-[button-icon]"}, {c:"text", h:"[button-text]"}], x: "btkUI-ButtonAction", a:{"data-tooltip": "[button-tooltip]", "data-action": "[button-action]"}}]};
        menu.templates["btkMultiSelectOption"] = {c:"col-12", s: [{c:"dropdown-option", s: [{c:"selection-icon"}, {c:"option-text", h: "[option-text]"}], a: {"id": "btkUI-DropdownOption-[option-index]", "data-index": "[option-index]"}, x: "btkUI-DropdownSelect"}]}
        menu.templates["btkUIRootPage"] = {c: "container container-controls", a:{"id": "btkUI-[ModName]-[ModPage]"}, s:[{c: "scroll-view", s:[{c: "content scroll-content", s:[]}, {c: "scroll-marker-v"}]}]};
        menu.templates["btkUIPage"] = {c: "container container-controls hide", a:{"id": "btkUI-[ModName]-[ModPage]"}, s:[{c: "row header-section", s:[{c:"col-1", s:[{c: "icon-back", x: "btkUI-Back"}]}, {c:"col", s:[{c:"header", h:"[PageHeader]"}]}]}, {c: "scroll-view", s:[{c: "content-subpage scroll-content", s:[]}, {c: "scroll-marker-v"}]}]};
        menu.templates["btkUIRow"] = {c: "row", a: {"id": "btkUI-[ModName-[ModPage]-[ModRow]"}, s:[{c:"col", s:[{c:"header", h:"[Header]"}]}]};

        menu.templates["core-quickmenu"].l.push("btkUI-btn");
        menu.templates["core-quickmenu"].l.push("btkUI-menu");

        uiRefBTK.actions["btkUI-open"] = this.actions.btkOpen;
        uiRefBTK.actions["btkUI-Test"] = this.actions.test;
        uiRefBTK.actions["btkUI-pushPage"] = this.actions.btkPushPage;
        uiRefBTK.actions["btkUI-Back"] = this.actions.btkBack;
        uiRefBTK.actions["btkUI-Toggle"] = this.actions.btkToggle;
        uiRefBTK.actions["btkUI-ButtonAction"] = this.actions.btkButtonAction;
        uiRefBTK.actions["btkUI-ConfirmOK"] = this.actions.btkConfirmOK;
        uiRefBTK.actions["btkUI-ConfirmNO"] = this.actions.btkConfirmNo;
        uiRefBTK.actions["btkUI-NoticeClose"] = this.actions.btkNoticeClose;
        uiRefBTK.actions["btkUI-DropdownSelect"] = this.actions.btkDropdownSelect;
        uiRefBTK.actions["btkUI-NumInput"] = this.actions.btkNumInput;
        uiRefBTK.actions["btkUI-NumBack"] = this.actions.btkNumBack;
        uiRefBTK.actions["btkUI-NumSubmit"] = this.actions.btkNumSubmit;

        engine.on("btkModInit", this.btkUILibInit);
        engine.on("btkCreateToggle", this.btkCreateToggle);
        engine.on("btkSetToggleState", this.btkSetToggleState);
        engine.on("btkShowConfirm", this.btkShowConfirmationBox);
        engine.on("btkShowNotice", this.btkShowNoticeBox);
        engine.on("btkCreateSlider", this.btkCreateSlider);
        engine.on("btkSliderSetValue", this.btkSliderSetValue);
        engine.on("btkCreateButton", this.btkCreateButton);
        engine.on("btkOpenMultiSelect", this.btkOpenMultiSelect);
        engine.on("btkOpenNumberInput", this.btkOpenNumberInput);
    },

    init: function(menu){
        console.log("btkUI Init");

        document.addEventListener('mouseover', this.btkOnHover);
        document.addEventListener('mousemove', this.btkkSliderMouseMove);
        document.addEventListener("mouseup", this.btkSliderMouseUp);
        document.addEventListener('mousedown', this.btkSliderMouseDown);
    },

    btkOnHover: function (e){
        targetElement = e.target;
        tooltipInfo = null;

        if(targetElement != null) {

            while (tooltipInfo == null && targetElement != null && targetElement.classList != null && !targetElement.classList.contains("menu-category")) {
                tooltipInfo = targetElement.getAttribute("data-tooltip");
                targetElement = targetElement.parentElement;
            }

            if (tooltipInfo != null) {
                document.getElementById("btkUI-Tooltip").innerHTML = tooltipInfo;

                cvr("#btkUI-TooltipContainer").show();
                return;
            }
        }

        cvr("#btkUI-TooltipContainer").hide();
    },
    btkUILibInit: function () {
        cvr("#btkUI-QMButton").show();
    },

    btkCreateSlider: function(parent, sliderName, sliderID, currentValue, minValue, maxValue, tooltipText, additionalClasses){
        let parentElement = cvr("#" + parent);

        if(parentElement === null){
            console.error("parentElement wasn't found! Unable to create slider!")
            return;
        }

        let slider = cvr.render(uiRefBTK.templates["btkSlider"], {
            "[slider-name]": sliderName,
            "[slider-id]": sliderID,
            "[current-value]": currentValue.toFixed(2),
            "[min-value]": minValue,
            "[max-value]": maxValue,
            "[tooltip-text]": tooltipText,
        }, uiRefBTK.templates, uiRefBTK.actions);

        if(additionalClasses != null) {
            for (let i = 0; i < additionalClasses.length; i++) {
                slider.classList.add(additionalClasses[i]);
            }
        }

        parentElement.appendChild(slider);

        //Set the slider value using our function
        setSliderFunctionBTK(sliderID, currentValue);
    },

    btkSliderMouseDown: function(e){
        let targetElement = e.target;
        let sliderID = null;

        if(targetElement != null) {
            while (sliderID == null && targetElement != null && targetElement.classList != null && !targetElement.classList.contains("menu-category")) {
                sliderID = targetElement.getAttribute("data-slider");
                if(sliderID == null)
                    targetElement = targetElement.parentElement;
            }
        }

        if (sliderID == null) return;

        isDraggingBTK = true;
        currentDraggedSliderBTK = targetElement;
        currentSliderKnobBTK = document.getElementById("btkUI-SliderKnob-" + sliderID);
        currentSliderBarBTK = document.getElementById("btkUI-SliderBar-" + sliderID);
    },

    btkSliderMouseUp: function(e){
        if(currentDraggedSliderBTK === null && !isDraggingBTK)
            return;

        currentDraggedSliderBTK = null;
        isDraggingBTK = false;
    },

    btkkSliderMouseMove: function(e){
        if(!isDraggingBTK)
            return;
        if(currentDraggedSliderBTK === null)
            return;

        let rect = currentSliderBarBTK.getBoundingClientRect();
        let rectKnob = currentSliderKnobBTK.getBoundingClientRect();
        let start = rect.left;
        let end = rect.right;
        let max = (end - start) - rectKnob.width;
        let current = Math.min(Math.max((e.clientX + 50 - start) - rectKnob.width, 0), max);

        currentSliderKnobBTK.style.left = current + 'px';

        //Update the slider value
        let sliderMin = parseInt(currentDraggedSliderBTK.getAttribute("data-min"));
        let sliderMax = parseInt(currentDraggedSliderBTK.getAttribute("data-max"));

        let newValue = (sliderMax - sliderMin) * (current) / (max) + sliderMin;
        newValue = newValue.toFixed(2);
        currentDraggedSliderBTK.setAttribute("data-slider-value", newValue);

        let sliderID = currentDraggedSliderBTK.getAttribute("data-slider");

        let sliderTitle = document.getElementById("btkUI-SliderTitle-" + sliderID);
        sliderTitle.innerHTML = sliderTitle.getAttribute("data-title") + " - " + newValue;

        engine.call("btkUI-SliderValueUpdated", sliderID, newValue);
    },

    btkSliderSetValue: function (sliderID, value){
        let slider = document.getElementById("btkUI-Slider-" + sliderID);
        let sliderKnob = document.getElementById("btkUI-SliderKnob-" + sliderID);

        value = Number(value);

        if(slider === null || sliderKnob === null){
            console.error("Unable to set slider value for " + sliderID + "!");
            return;
        }

        let sliderMin = parseInt(slider.getAttribute("data-min"));
        let sliderMax = parseInt(slider.getAttribute("data-max"));

        slider.setAttribute("data-slider-value", value);

        let sliderTitle = document.getElementById("btkUI-SliderTitle-" + sliderID);
        if(!Number.isNaN(value))
            sliderTitle.innerHTML = sliderTitle.getAttribute("data-title") + " - " + value.toFixed(2);
        else
            sliderTitle.innerHTML = sliderTitle.getAttribute("data-title") + " - " + value;

        let slider0Max = sliderMax - sliderMin;
        value = value - sliderMin;

        let max = 1021-100;
        sliderKnob.style.left = (value / slider0Max)*max + 'px';
    },

    btkShowConfirmationBox: function(title, content, okText, noText){
        let header = document.getElementById("btkUI-PopupConfirmHeader");
        let text = document.getElementById("btkUI-PopupConfirmText");
        let ok = document.getElementById("btkUI-PopupConfirmOk");
        let no = document.getElementById("btkUI-PopupConfirmNo");

        header.innerHTML = title;
        text.innerHTML = content;

        if(okText === null){
            ok.innerHTML = "Yes";
        }
        else{
            ok.innerHTML = okText;
        }

        if(noText === null){
            no.innerHTML = "No";
        }
        else{
            no.innerHTML = noText;
        }

        cvr("#btkUI-PopupConfirm").show();
    },

    btkShowNoticeBox: function(title, content, okText){
        let header = document.getElementById("btkUI-PopupNoticeHeader");
        let text = document.getElementById("btkUI-PopupNoticeText");
        let ok = document.getElementById("btkUI-PopupNoticeOK");

        header.innerHTML = title;
        text.innerHTML = content;

        if(okText === null){
            ok.innerHTML = "Yes";
        }
        else{
            ok.innerHTML = okText;
        }

        cvr("#btkUI-PopupNotice").show();
    },

    btkCreateToggle: function(settingsCategory, pageID, toggleName, toggleID, tooltip, state)
    {
        let targetRow = cvr("#btkUI-" + pageID + "-" + settingsCategory);

        if(targetRow == null) {
            console.error("Attempted to create a settings toggle for a category that doesn't exist! Category=" + settingsCategory)
            return;
        }

        targetRow.appendChild(cvr.render(uiRefBTK.templates["btkToggle"], {
            "[toggle-name]": toggleName,
            "[toggle-id]": toggleID,
            "[tooltip-data]": tooltip,
            "[toggle-location]": settingsCategory,
            "[toggle-page]": pageID
        }, uiRefBTK.templates, uiRefBTK.actions));

        newToggle = document.getElementById("btkUI-Toggle-" + settingsCategory + "-" + pageID + "-" + toggleID);

        let enabled = newToggle.querySelector("#btkUI-toggle-enable");
        let disabled = newToggle.querySelector("#btkUI-toggle-disable");

        if(state){
            enabled.classList.add("active");
            disabled.classList.remove("active");
        }
        else{
            enabled.classList.remove("active");
            disabled.classList.add("active");
        }

        newToggle.setAttribute("data-toggleState", state.toString());
    },

    btkSetToggleState: function(toggleID, state){
        let element = document.getElementById(toggleID);

        let enabled = element.querySelector("#btkUI-toggle-enable");
        let disabled = element.querySelector("#btkUI-toggle-disable");

        if(state){
            enabled.classList.add("active");
            disabled.classList.remove("active");
        }
        else{
            enabled.classList.remove("active");
            disabled.classList.add("active");
        }

        element.setAttribute("data-toggleState", state.toString());
    },
    btkCreateButton: function(parent, buttonName, buttonIcon, tooltip, buttonAction){
        cvr("#" + parent).appendChild(cvr.render(uiRefBTK.templates["btkButton"], {
            "[button-text]": buttonName,
            "[button-icon]": buttonIcon,
            "[button-tooltip]": tooltip,
            "[button-action]": buttonAction,
        }, uiRefBTK.templates, uiRefBTK.actions));
    },
    btkOpenMultiSelect: function(name, options, selectedIndex){
        let element = cvr("#btkUI-Dropdown-OptionRoot");
        element.clear();

        for(let i=0; i<options.length; i++){
            let option = element.appendChild(cvr.render(uiRefBTK.templates["twMultiSelectOption"], {
                "[option-text]": options[i],
                "[option-index]": i,
            }, uiRefBTK.templates, uiRefBTK.actions));

            let optionIcon = option.querySelector(".selection-icon");
            if(i!==selectedIndex)
                optionIcon.classList.remove("selected");
            if(i===selectedIndex)
                optionIcon.classList.add("selected");
        }

        cvr("#btkUI-DropdownHeader").innerHTML(name);

        cvr("#btkUI-DropdownPage").show();
        cvr("#btkUI-" + currentPageBTK).hide();

        breadcrumbsBTK.push(currentPage);

        engine.call("btkUI-OpenedPage", "DropdownPage", currentPage);

        currentPageBTK = "DropdownPage";
    },
    btkOpenNumberInput: function(name, number) {
        let display = document.getElementById("btkUI-numDisplay");
        let data = number;
        display.setAttribute("data-display", data);
        display.innerHTML = data;

        cvr("#btkUI-NumberInputHeader").innerHTML("Editing: " + name);

        uiRefBTK.core.playSoundCore("Click");

        cvr("#btkUI-NumberEntry").show();
        cvr("#btkUI-" + currentPageBTK).hide();

        breadcrumbsBTK.push(currentPage);

        engine.call("btkUI-OpenedPage", "NumberEntry", currentPage);

        currentPageBTK = "NumberEntry";
    },

    btkPushPage: function (targetPage, modPage = currentMod){
        if(currentPage === target)
            return;

        cvr("#btkUI-" + target).show();
        cvr("#btkUI-" + currentPageBTK).hide();

        breadcrumbsBTK.push(currentPageBTK);

        engine.call("btkUI-OpenedPage", target, currentPageBTK);

        currentPageBTK = target;

        if(modPage !== currentMod){
            //We're going to a new root page, clear the breadcrumbs
            currentMod = modPage;

            breadcrumbsBTK.clear();
        }
    },

    btkCreatePage: function (pageName, modName, rootPage){

    },
    

    actions: {
        btkOpen: function(){
            uiRefBTK.core.playSoundCore("Click");
            uiRefBTK.core.switchCategorySelected("btkUI");
            engine.call("btkUI-OpenMainMenu");
        },
        btkPushPage: function (e){
            uiRefBTK.core.playSoundCore("Click");

            let target = e.currentTarget.getAttribute("data-page");

            pushPageBTK(target);
        },
        btkBack: function (){
            uiRefBTK.core.playSoundCore("Click");

            let target = breadcrumbsBTK.pop();

            cvr("#btkUI-" + target).show();
            cvr("#btkUI-" + currentPageBTK).hide();

            engine.call("btkUI-BackAction", target, currentPageBTK);

            currentPageBTK = target;
        },
        btkButtonAction: function (e){
            uiRefBTK.core.playSoundCore("Click");
            let action = e.currentTarget.getAttribute("data-action");

            if(action != null){
                engine.call("btkUI-ButtonAction", action);
            }
        },
        btkNumInput: function (e){
            let str = e.currentTarget.getAttribute("str");
            let display = document.getElementById("btkUI-numDisplay");
            let data = display.getAttribute("data-display");
            if (str === "." && data.includes(".")){
                return;
            }
            if (data.length >= 5){
                return;
            }
            data = data + str;
            display.setAttribute("data-display", data);
            display.innerHTML = data;
        },
        btkNumSubmit: function (){
            let display = document.getElementById("btkUI-numDisplay");
            let data = display.getAttribute("data-display");

            if(data != null){
                engine.call("btkUI-NumSubmit", data);
            }
            uiRefBTK.core.playSoundCore("Click");

            let target = breadcrumbs.pop();

            cvr("#btkUI-" + target).show();
            cvr("#btkUI-" + currentPage).hide();

            engine.call("btkUI-BackAction", target, currentPage);

            currentPage = target;
        },
        btkNumBack: function (){
            let display = document.getElementById("btkUI-numDisplay");
            let data = display.getAttribute("data-display");
            data = data.slice(0, -1);
            display.setAttribute("data-display", data);
            display.innerHTML = data;
        },
        btkToggle: function(e){
            uiRefBTK.core.playSoundCore("Click");

            let enabled = e.currentTarget.querySelector("#btkUI-toggle-enable");
            let disabled = e.currentTarget.querySelector("#btkUI-toggle-disable");

            let toggleID = e.currentTarget.getAttribute("data-toggle");
            let state = (e.currentTarget.getAttribute("data-toggleState") === 'true');

            state = !state;

            if(state){
                enabled.classList.add("active");
                disabled.classList.remove("active");
            }
            else{
                enabled.classList.remove("active");
                disabled.classList.add("active");
            }

            e.currentTarget.setAttribute("data-toggleState", state.toString());

            engine.call("btkUI-Toggle", toggleID, state);
        },
        btkDropdownSelect: function(e){
            uiRefBTK.core.playSoundCore("Click");
            let dropdown = document.getElementById("btkUI-Dropdown-OptionRoot");
            let options = dropdown.getElementsByClassName("dropdown-option");
            let index = parseInt(e.currentTarget.getAttribute("data-index"));

            for(let i=0; i<options.length; i++){
                let option = options[i];
                let optionIcon = option.querySelector(".selection-icon");
                if(i!==index)
                    optionIcon.classList.remove("selected");
                if(i===index)
                    optionIcon.classList.add("selected");
            }

            engine.call("btkUI-DropdownSelected", index);
        },
        btkConfirmOK: function (){
            uiRefBTK.core.playSoundCore("Click");
            engine.call("btkUI-PopupConfirmOK");
            cvr("#btkUI-PopupConfirm").hide();
        },
        btkConfirmNo: function (){
            uiRefBTK.core.playSoundCore("Click");
            engine.call("btkUI-PopupConfirmNo");
            cvr("#btkUI-PopupConfirm").hide();
        },
        btkNoticeClose: function (){
            uiRefBTK.core.playSoundCore("Click");
            engine.call("btkUI-PopupNoticeOK");
            cvr("#btkUI-PopupNotice").hide();
        },
    }
}