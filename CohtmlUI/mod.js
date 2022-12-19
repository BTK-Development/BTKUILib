cvr.menu.prototype.BTKUI = {
    uiRefBTK: {},
    breadcrumbsBTK: [],
    currentPageBTK: "",
    currentDraggedSliderBTK: {},
    currentSliderBarBTK: {},
    currentSliderKnobBTK: {},
    setSliderFunctionBTK: {},
    pushPageBTK: {},
    changeTabBTK: {},
    updateTitle: {},
    currentMod: "",
    isDraggingBTK: false,
    selectedPlayerIDBTK: "",
    selectedPlayerNameBTK: "",

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
        updateTitle = this.btkUpdateTitle;
        selectedPlayerIDBTK = "";
        selectedPlayerNameBTK = "";
        changeTabBTK = this.btkChangeTab;

        menu.templates["btkUI-btn"] = {c: "btkUI-btn hide", s: [{c: "icon"}], x: "btkUI-pushPage", a:{"id" : "btkUI-UserMenu", "data-page": "btkUI-PlayerList"}};
        menu.templates["btkUI-shared"] = {c: "btkUI-shared hide", s:[
                {c: "container btk-popup-container hide", a: {"id": "btkUI-PopupConfirm"}, s:[
                        {c: "row", s: [
                                {c: "col align-self-center", s: [{c: "header", h: "Notice", a: {"id": "btkUI-PopupConfirmHeader"}}]}
                            ]},
                        {c: "content notice-text", h:"This is some text!", a: {"id": "btkUI-PopupConfirmText"}},
                        {c: "control-row", s: [
                                {c: "col", s: [{c: "button", s: [{c: "text", h: "Yes", a: {"id": "btkUI-PopupConfirmOk"}}], x:"btkUI-ConfirmOK"}]},
                                {c: "col", s: [{c: "button", s: [{c: "text", h: "No", a: {"id": "btkUI-PopupConfirmNo"}}], x:"btkUI-ConfirmNO"}]}
                            ]},
                    ]},
                {c: "container btk-popup-container hide", a: {"id": "btkUI-PopupNotice"}, s:[
                        {c:"row", s:[
                                {c:"col align-self-center", s:[{c:"header", h:"Notice", a: {"id": "btkUI-PopupNoticeHeader"}}]}
                            ]},
                        {c: "content notice-text", h:"This is some text!", a: {"id": "btkUI-PopupNoticeText"}},
                        {c:"row justify-content-center", s:[
                                {c:"col offset-4 align-self-center", s:[{c:"button", s:[{c:"text", h:"OK", a: {"id": "btkUI-PopupNoticeOK"}}], x:"btkUI-NoticeClose"}]}
                            ]},
                    ]},
                {c: "container container-tabs", s:[
                        {c:"row justify-content-md-center", a: {"id": "btkUI-TabRoot"}, s:[
                                {c: "col-md-2 tab selected", s:[
                                        {c: "tab-content", a:{"id":"btkUI-Tab-CVRQM-Icon"}}
                                    ], a:{"id":"btkUI-Tab-CVRMainQM", "tabTarget": "CVRMainQM"}, x: "btkUI-TabChange"},
                            ]}
                    ]},
                {c: "container-tooltip hide", s:[{c:"content", h:"tooltip info", a:{"id": "btkUI-Tooltip"}}], a:{"id": "btkUI-TooltipContainer"}}
            ], a: {"id": "btkUI-SharedRoot"}};
        menu.templates["btkUI-menu"] = {c: "btkUI menu-category hide", s: [
                {c: "container container-main", s:[
                        {c:"row", s:[
                                {c:"col", s:[
                                        {c: "header", a:{"id": "btkUI-MenuHeader"}, h: "RootPage"},
                                        {c: "content", s:[
                                                {c: "subtitle", a: {"id": "btkUI-MenuSubtitle"}, h: "This is a page!"},
                                            ]}
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
                {c: "container container-controls hide", a:{"id": "btkUI-PlayerList"}, s:[
                        {c: "row header-section", s:[
                                {c:"col-1", s:[{c: "icon-back", x: "btkUI-Home"}]},
                                {c:"col", s:[{c:"header", h:"Players in World"}]}
                            ]},
                        {c: "scroll-view", s:[{c: "content-subpage scroll-content", s:[
                                    {c: "row", a:{"id": "btkUI-PlayerListContent"}}
                                ]}, {c: "scroll-marker-v"}]}]},
                {c: "container container-controls hide", a:{"id": "btkUI-PlayerSelectPage"}, s:[
                        {c: "row header-section", s:[
                                {c:"col-1", s:[{c: "icon-back", x: "btkUI-Back"}]},
                                {c:"col", s:[{c:"header", h:"User", a:{"id": "btkUI-PlayerSelectHeader"}}]}
                            ]},
                        {c: "scroll-view", s:[{c: "content-subpage scroll-content", s: [
                                    {c: "row", a:{"id": "btkUI-PlayerSelectPage-Content"}},
                                ]}, {c: "scroll-marker-v"}]}]},
            ], a:{"id":"btkUI-Root"}};

        menu.templates["btkUIRowContent"] = {c:"row justify-content-start", a:{"id": "btkUI-Row-[UUID]"}};
        menu.templates["btkSlider"] = {c:"", s:[{c:"col-12", s:[{c:"text", h:"[slider-name] - [current-value]", a:{"id": "btkUI-SliderTitle-[slider-id]", "data-title": "[slider-name]"}}]}, {c: "col-12", s:[{c:"slider", s:[{c:"sliderBar", s:[{c:"slider-knob", a:{"id": "btkUI-SliderKnob-[slider-id]"}}], a:{"id": "btkUI-SliderBar-[slider-id]"}}], a:{"id":"btkUI-Slider-[slider-id]", "data-slider-id": "[slider-id]", "data-slider-value": "[current-value]", "data-min": "[min-value]", "data-max": "[max-value]"}}], a:{"id":"btkUI-Slider-[slider-id]-Tooltip", "data-tooltip": "[tooltip-text]"}}]};
        menu.templates["btkToggle"] = {c:"col-3", a:{"id": "btkUI-Toggle-[toggle-id]-Root"}, s:[{c: "toggle", s:[{c:"row", s:[{c:"col align-content-start", s:[{c:"enable circle", a:{"id": "btkUI-toggle-enable"}}]}, {c:"col align-content-end", s:[{c:"disable circle active", a:{"id": "btkUI-toggle-disable"}}]}]},{c:"text-sm", h:"[toggle-name]", a:{"id": "btkUI-Toggle-[toggle-id]-Text"}}], x: "btkUI-Toggle", a:{"id": "btkUI-Toggle-[toggle-id]", "data-toggle": "[toggle-id]", "data-toggleState": "false", "data-tooltip": "[tooltip-data]"}}]};
        menu.templates["btkButton"] = {c:"col-3", a:{"id": "btkUI-Button-[UUID]"}, s:[{c: "button", s:[{c:"icon", a:{"id": "btkUI-Button-[UUID]-Image"}}, {c:"text", h:"[button-text]", a:{"id": "btkUI-Button-[UUID]-Text"}}], x: "btkUI-ButtonAction", a:{"id": "btkUI-Button-[UUID]-Tooltip","data-tooltip": "[button-tooltip]", "data-action": "[button-action]"}}]};
        menu.templates["btkMultiSelectOption"] = {c:"col-12", s: [{c:"dropdown-option", s: [{c:"selection-icon"}, {c:"option-text", h: "[option-text]"}], a: {"id": "btkUI-DropdownOption-[option-index]", "data-index": "[option-index]"}, x: "btkUI-DropdownSelect"}]}
        menu.templates["btkUIRootPage"] = {c: "container container-controls hide", a:{"id": "btkUI-[ModName]-MainPage"}, s:[{c: "scroll-view", s:[{c: "content scroll-content", s:[], a:{"id": "btkUI-[ModName]-MainPage-Content"}}, {c: "scroll-marker-v"}]}]};
        menu.templates["btkUIPage"] = {c: "container container-controls hide", a:{"id": "btkUI-[ModName]-[ModPage]"}, s:[{c: "row header-section", s:[{c:"col-1", s:[{c: "icon-back", x: "btkUI-Back"}]}, {c:"col", s:[{c:"header", h:"[PageHeader]", a:{"id": "btkUI-[ModName]-[ModPage]-Header"}}]}]}, {c: "scroll-view", s:[{c: "content-subpage scroll-content", s:[], a:{"id": "btkUI-[ModName]-[ModPage]-Content"}}, {c: "scroll-marker-v"}]}]};
        menu.templates["btkUIRowHeader"] = {c: "row", a: {"id": "btkUI-Row-[UUID]-HeaderRoot"}, s:[{c:"col", s:[{c:"header", h:"[Header]", a:{"id": "btkUI-Row-[UUID]-HeaderText"}}]}]};
        menu.templates["btkUITab"] = {c: "col-md-2 tab", s:[{c: "tab-content", a:{"id":"btkUI-Tab-[TabName]-Image"}}], a:{"id":"btkUI-Tab-[TabName]", "tabTarget": "btkUI-[TabName]-MainPage"}, x: "btkUI-TabChange"};
        menu.templates["btkPlayerListEntry"] = {c:"col-3", s:[{c:"button-user", x:"btkUI-SelectPlayer", s:[{c:"text", h:"[player-name]"}], a:{"id": "btkUI-PlayerButton-[player-id]-Icon","data-id": "[player-id]", "data-name": "[player-name]", "data-tooltip": "Open up the player options for [player-name]"}}], a:{"id": "btkUI-PlayerButton-[player-id]"}};

        menu.templates["core-quickmenu"].l.push("btkUI-btn");
        menu.templates["core-quickmenu"].l.push("btkUI-shared");
        menu.templates["core-quickmenu"].l.push("btkUI-menu");

        uiRefBTK.actions["btkUI-open"] = this.actions.btkOpen;
        uiRefBTK.actions["btkUI-Test"] = this.actions.test;
        uiRefBTK.actions["btkUI-pushPage"] = this.actions.btkPushPage;
        uiRefBTK.actions["btkUI-Back"] = this.actions.btkBack;
        uiRefBTK.actions["btkUI-Home"] = this.actions.btkHome;
        uiRefBTK.actions["btkUI-Toggle"] = this.actions.btkToggle;
        uiRefBTK.actions["btkUI-ButtonAction"] = this.actions.btkButtonAction;
        uiRefBTK.actions["btkUI-ConfirmOK"] = this.actions.btkConfirmOK;
        uiRefBTK.actions["btkUI-ConfirmNO"] = this.actions.btkConfirmNo;
        uiRefBTK.actions["btkUI-NoticeClose"] = this.actions.btkNoticeClose;
        uiRefBTK.actions["btkUI-DropdownSelect"] = this.actions.btkDropdownSelect;
        uiRefBTK.actions["btkUI-NumInput"] = this.actions.btkNumInput;
        uiRefBTK.actions["btkUI-NumBack"] = this.actions.btkNumBack;
        uiRefBTK.actions["btkUI-NumSubmit"] = this.actions.btkNumSubmit;
        uiRefBTK.actions["btkUI-TabChange"] = this.actions.btkTabChange;
        uiRefBTK.actions["btkUI-SelectPlayer"] = this.actions.selectPlayer;

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
        engine.on("btkCreatePage", this.btkCreatePage);
        engine.on("btkChangeTab", this.btkChangeTab);
        engine.on("btkUpdateTitle", this.btkUpdateTitle);
        engine.on("btkCreateRow", this.btkCreateRow);
        engine.on("btkUpdateText", this.btkUpdateText);
        engine.on("btkPushPage", this.btkPushPage);
        engine.on("btkAddPlayer", this.btkAddPlayer);
        engine.on("btkRemovePlayer", this.btkRemovePlayer);
        engine.on("btkSliderUpdateSettings", this.btkSliderUpdateSettings);
        engine.on("btkDeleteElement", this.btkDeleteElement);
        engine.on("btkUpdateIcon", this.btkUpdateIcon);
        engine.on("btkUpdateTooltip", this.btkUpdateTooltip);
        engine.on("btkLeaveWorld", this.btkLeaveWorld);
    },

    init: function(menu){
        console.log("btkUI Init");

        document.addEventListener('mouseover', this.btkOnHover);
        document.addEventListener('mousemove', this.btkSliderMouseMove);
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
        cvr("#btkUI-UserMenu").show();
        cvr("#btkUI-SharedRoot").show();
    },

    btkAddPlayer: function(username, userid, userImage){
        let playerCheck = document.getElementById("btkUI-PlayerButton-" + userid);

        if(playerCheck != null) return;

        cvr("#btkUI-PlayerListContent").appendChild(cvr.render(uiRefBTK.templates["btkPlayerListEntry"], {
            "[player-name]": username,
            "[player-id]": userid,
        }, uiRefBTK.templates, uiRefBTK.actions));

        let user = document.querySelector("#btkUI-PlayerButton-" + userid + "-Icon");
        user.style.background = "url('" + userImage + "')";
        user.style.backgroundRepeat = "no-repeat";
        user.style.backgroundSize = "cover";
    },

    btkRemovePlayer: function(userid){
        let element = document.querySelector("#btkUI-PlayerButton-" + userid);
        if(element != null){
            element.parentElement.removeChild(element);
        }
    },

    btkLeaveWorld: function(){
        cvr("#btkUI-PlayerListContent").clear();
    },

    btkCreateSlider: function(parent, sliderName, sliderID, currentValue, minValue, maxValue, tooltipText, additionalClasses){
        let parentElement = cvr("#" + parent + "-Content");

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
                sliderID = targetElement.getAttribute("data-slider-id");
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

    btkSliderMouseMove: function(e){
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

        let sliderID = currentDraggedSliderBTK.getAttribute("data-slider-id");

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

    btkSliderUpdateSettings: function(sliderID, sliderName, sliderTooltip, minValue, maxValue){
          let sliderText = document.getElementById("btkUI-SliderTitle-" + sliderID);
          let sliderData = document.getElementById("btkUI-Slider-" + sliderID);
          let sliderTT = document.getElementById("btkUI-Slider-" + sliderID + "-Tooltip");

          sliderText.setAttribute("data-title", sliderName);
          sliderData.setAttribute("data-min", minValue);
          sliderData.setAttribute("data-max", maxValue);
          sliderTT.setAttribute("data-tooltip", sliderTooltip);

          let value = Number(sliderData.getAttribute("data-slider-value"));

          if(!Number.isNaN(value))
            sliderText.innerHTML = sliderName + " - " + value.toFixed(2);
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

    btkCreateToggle: function(parentID, toggleName, toggleID, tooltip, state)
    {
        let target = cvr("#" + parentID);

        if(target == null) {
            console.error("Attempted to create a toggle in a parent that doesn't exist! - " + parentID);
            return;
        }

        target.appendChild(cvr.render(uiRefBTK.templates["btkToggle"], {
            "[toggle-name]": toggleName,
            "[toggle-id]": toggleID,
            "[tooltip-data]": tooltip,
        }, uiRefBTK.templates, uiRefBTK.actions));

        newToggle = document.getElementById("btkUI-Toggle-" + toggleID);

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
    btkCreateButton: function(parent, buttonName, buttonIcon, tooltip, buttonUUID, modName){
        cvr("#" + parent).appendChild(cvr.render(uiRefBTK.templates["btkButton"], {
            "[button-text]": buttonName,
            "[button-tooltip]": tooltip,
            "[button-action]": buttonUUID,
            "[UUID]": buttonUUID,
        }, uiRefBTK.templates, uiRefBTK.actions));

        if(buttonIcon !== null && buttonIcon.length > 0) {
            let button = document.getElementById("btkUI-Button-" + buttonUUID + "-Image");
            button.style.backgroundImage = "url('mods/BTKUI/images/" + modName + "/" + buttonIcon + ".png')";
            button.style.backgroundRepeat = "no-repeat";
            button.style.backgroundSize = "contain";
        }
    },
    btkOpenMultiSelect: function(name, options, selectedIndex){
        let element = cvr("#btkUI-Dropdown-OptionRoot");
        element.clear();

        for(let i=0; i<options.length; i++){
            let option = element.appendChild(cvr.render(uiRefBTK.templates["btkMultiSelectOption"], {
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
        cvr("#" + currentPageBTK).hide();

        breadcrumbsBTK.push(currentPageBTK);

        engine.call("btkUI-OpenedPage", "DropdownPage", currentPageBTK);

        currentPageBTK = "btkUI-DropdownPage";
    },
    btkOpenNumberInput: function(name, number) {
        let display = document.getElementById("btkUI-numDisplay");
        let data = number;
        display.setAttribute("data-display", data);
        display.innerHTML = data;

        cvr("#btkUI-NumberInputHeader").innerHTML("Editing: " + name);

        uiRefBTK.core.playSoundCore("Click");

        cvr("#btkUI-NumberEntry").show();
        cvr("#" + currentPageBTK).hide();

        breadcrumbsBTK.push(currentPageBTK);

        engine.call("btkUI-OpenedPage", "NumberEntry", currentPageBTK);

        currentPageBTK = "btkUI-NumberEntry";
    },

    btkPushPage: function (targetPage, modPage = currentMod){
        if(currentPageBTK === targetPage)
            return;

        uiRefBTK.core.switchCategorySelected("btkUI");

        cvr("#" + targetPage).show();
        if(currentPageBTK.length > 0)
            cvr("#" + currentPageBTK).hide();

        breadcrumbsBTK.push(currentPageBTK);

        engine.call("btkUI-OpenedPage", targetPage, currentPageBTK);

        currentPageBTK = targetPage;

        if(modPage !== currentMod){
            //We're going to a new root page, clear the breadcrumbs
            currentMod = modPage;

            breadcrumbsBTK.length = 0;
            breadcrumbsBTK.push(targetPage);
        }
    },

    btkUpdateText: function (elementID, text){
        let element = cvr("#" + elementID);

        if(element === null){
            console.error("Unable to update text of element " + elementID);
            return;
        }

        element.innerHTML(text);
    },

    btkCreateRow: function (parentID, rowUUID, rowHeader = null){
        if(rowHeader != null){
            cvr("#" + parentID + "-Content").appendChild(cvr.render(uiRefBTK.templates["btkUIRowHeader"], {
                "[UUID]": rowUUID,
                "[Header]": rowHeader
            }, uiRefBTK.templates, uiRefBTK.actions))
        }

        cvr("#" + parentID + "-Content").appendChild(cvr.render(uiRefBTK.templates["btkUIRowContent"], {
            "[UUID]": rowUUID
        }, uiRefBTK.templates, uiRefBTK.actions))
    },

    btkCreatePage: function (pageName, modName, tabIcon, elementID, rootPage, cleanedPageName){
        let elementCheck = null;

        if(rootPage)
            elementCheck = document.getElementById("btkUI-" + modName + "-MainPage");
        else
            elementCheck = document.getElementById(elementID);

        if(elementCheck !== null) return;

        if(!rootPage) {
            cvr("#btkUI-Root").appendChild(cvr.render(uiRefBTK.templates["btkUIPage"], {
                "[ModName]": modName,
                "[ModPage]": cleanedPageName,
                "[PageHeader]": pageName,
            }, uiRefBTK.templates, uiRefBTK.actions));

            return;
        }

        cvr("#btkUI-TabRoot").appendChild(cvr.render(uiRefBTK.templates["btkUITab"], {
            "[TabName]": modName
        }, uiRefBTK.templates, uiRefBTK.actions));

        console.log(tabIcon)

        if(tabIcon !== null && tabIcon.length > 0) {
            let tab = document.getElementById("btkUI-Tab-" + modName + "-Image");
            tab.style.backgroundImage = "url('mods/BTKUI/images/" + modName + "/" + tabIcon + ".png')";
            tab.style.backgroundRepeat = "no-repeat";
            tab.style.backgroundSize = "contain";
        }

        cvr("#btkUI-Root").appendChild(cvr.render(uiRefBTK.templates["btkUIRootPage"], {
            "[ModName]": modName
        }, uiRefBTK.templates, uiRefBTK.actions));
    },

    btkChangeTab: function (rootTarget, rootMod, menuTitle, menuSubtitle){
        console.log("Setting to rootTarget " + rootTarget + " | currentMod = " + currentMod + " | rootMod = " + rootMod);

        if(rootTarget === "CVRMainQM"){
            uiRefBTK.core.switchCategorySelected("quickmenu-home");
            return;
        }

        uiRefBTK.core.switchCategorySelected("btkUI");

        //Clean things up before changing roots
        if(currentMod !== rootMod && currentPageBTK.length > 0){
               cvr("#" + currentPageBTK).hide();
        }

        updateTitle(menuTitle, menuSubtitle);

        pushPageBTK(rootTarget, rootMod);
    },

    btkUpdateTitle: function (menuTitle, menuSubtitle){
        cvr("#btkUI-MenuHeader").innerHTML(menuTitle);
        cvr("#btkUI-MenuSubtitle").innerHTML(menuSubtitle);
    },

    btkDeleteElement: function (elementID){
        let element = document.getElementById(elementID);

        if(element === null) {
            console.log("Unable to find element with ID " + elementID + " unable to delete!");
            return;
        }

        element.parentElement.removeChild(element);
    },

    btkUpdateIcon: function (elementID, modName, icon) {
        let element = document.getElementById(elementID + "-Image");

        if(element === null){
            console.log("Unable to find element with ID " + elementID + " unable to update icon!");
            return;
        }

        if(icon === null || icon.length <= 0) return;

        element.style.backgroundImage = "url('mods/BTKUI/images/" + modName + "/" + icon + ".png')";
        element.style.backgroundRepeat = "no-repeat";
        element.style.backgroundSize = "contain";
    },

    btkUpdateTooltip: function (elementID, tooltipText){
        let element = document.getElementById(elementID);

        if(element === null){
            console.log("Unable to find element with ID " + elementID + " unable to update tooltip!");
            return;
        }

        element.setAttribute("data-tooltip", tooltipText);
    },

    actions: {
        btkOpen: function(){
            uiRefBTK.core.playSoundCore("Click");
            uiRefBTK.core.switchCategorySelected("btkUI");
            engine.call("btkUI-OpenMainMenu");
        },
        btkTabChange: function(e){
            uiRefBTK.core.playSoundCore("Click");

            let target = e.currentTarget.getAttribute("tabTarget");

            if(target === null) {
                console.error("Tab did not have a tabTarget!" + e);
                return;
            }

            var tabs = document.querySelectorAll(".container-tabs .tab");
            for(let i=0; i < tabs.length; i++){
                let tab = tabs[i];
                tab.classList.remove("selected");
            }

            e.currentTarget.classList.add("selected");

            engine.call("btkUI-TabChange", target);
        },
        btkPushPage: function (e){
            uiRefBTK.core.playSoundCore("Click");

            let target = e.currentTarget.getAttribute("data-page");

            pushPageBTK(target);
        },
        btkBack: function (){
            uiRefBTK.core.playSoundCore("Click");

            let target = breadcrumbsBTK.pop();

            cvr("#" + target).show();
            cvr("#" + currentPageBTK).hide();

            engine.call("btkUI-BackAction", target, currentPageBTK);

            currentPageBTK = target;
        },
        btkHome: function (){
            uiRefBTK.core.playSoundCore("Click");

            cvr("#" + currentPageBTK).hide();
            currentPageBTK = "";

            var tabs = document.querySelectorAll(".container-tabs .tab");
            for(let i=0; i < tabs.length; i++){
                let tab = tabs[i];
                tab.classList.remove("selected");

                if(tab.id === "btkUI-Tab-CVRMainQM")
                    tab.classList.add("selected");
            }

            changeTabBTK("CVRMainQM", "", "", "")
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

            let target = breadcrumbsBTK.pop();

            cvr("#" + target).show();
            cvr("#" + currentPageBTK).hide();

            engine.call("btkUI-BackAction", target, currentPageBTK);

            currentPageBTK = target;
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
        selectPlayer: function(e){
            uiRef.core.playSoundCore("Click");

            if(currentPage === "btkUI-PlayerSelectPage")
                return;

            let playerID = e.currentTarget.getAttribute("data-id");
            let playerName = e.currentTarget.getAttribute("data-name");

            selectedPlayerNameBTK = playerName;
            selectedPlayerIDBTK = playerID;

            cvr("#btkUI-PlayerSelectPage").show();
            cvr("#" + currentPageBTK).hide();

            breadcrumbsBTK.push(currentPageBTK);
            currentPageBTK = "btkUI-PlayerSelectPage";

            engine.call("btkUI-SelectedPlayer", selectedPlayerNameBTK, selectedPlayerIDBTK);

            cvr("#btkUI-PlayerSelectHeader").innerHTML(playerName);
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