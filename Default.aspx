<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%= PageTitleText %></title>
    <style type="text/css">
        body {
            margin: 0;
            font-family: Arial, Helvetica, sans-serif;
            color: #222222;
            background-color: #eef4f6;
        }
        form {
            min-height: 100vh;
        }
        .page {
            display: flex;
            flex-direction: column;
            width: 100%;
            min-height: 100vh;
            max-width: none;
            box-sizing: border-box;
            margin: 0 auto;
            padding: 2vw;
        }
        .page * {
            box-sizing: border-box;
        }
        .topBar {
            position: relative;
            background-color: #ffffff;
            border: 1px solid #cfdde4;
            border-radius: 6px;
            padding: 18px 20px;
        }
        .topicFilter {
            position: absolute;
            top: 10px;
            right: 16px;
            display: block;
            width: 190px;
        }
        .topicFilterLabel {
            display: block;
            color: #333333;
            font-size: 11px;
            font-weight: bold;
            line-height: 14px;
            margin-bottom: 3px;
        }
        .topicSelect {
            width: 100%;
            height: 82px;
            border: 1px solid #aebfca;
            border-radius: 4px;
            color: #173f63;
            font-size: 10px;
            line-height: 12px;
            background-color: #ffffff;
        }
        .topicButton {
            flex: 1 1 0;
            min-height: 20px;
            border: 1px solid #8fa8b9;
            border-radius: 4px;
            background-color: #e9f1f6;
            color: #183f65;
            cursor: pointer;
            font-size: 10px;
            font-weight: bold;
        }
        .topicButton:hover {
            background-color: #dcebf4;
        }
        .topicButtons {
            display: flex;
            gap: 4px;
            margin-top: 3px;
        }
        .topicAddRow {
            display: flex;
            gap: 4px;
            margin-top: 4px;
        }
        .topicAddBox {
            flex: 1 1 auto;
            min-width: 0;
            border: 1px solid #aebfca;
            border-radius: 4px;
            color: #173f63;
            font-size: 10px;
            padding: 2px 4px;
        }
        .topicOptionHidden {
            background-color: #d9d9d9;
            color: #555555;
        }
        .title {
            display: block;
            color: #1f4f82;
            font-size: 28px;
            font-weight: bold;
            padding-right: 210px;
            margin-bottom: 6px;
        }
        .subtitle {
            display: block;
            color: #555555;
            font-size: 14px;
            line-height: 20px;
            padding-right: 210px;
        }
        .searchRow {
            margin-top: 14px;
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            align-items: center;
        }
        .locationLabel {
            color: #333333;
            font-size: 13px;
            font-weight: bold;
        }
        .locationBox {
            width: 310px;
            max-width: 100%;
            border: 1px solid #aebfca;
            border-radius: 4px;
            padding: 7px 8px;
            font-size: 13px;
        }
        .button {
            border: 1px solid #8fa8b9;
            border-radius: 4px;
            background-color: #e9f1f6;
            color: #183f65;
            cursor: pointer;
            font-size: 13px;
            font-weight: bold;
            padding: 7px 12px;
        }
        .button:hover {
            background-color: #dcebf4;
        }
        .note {
            display: block;
            color: #666666;
            font-size: 12px;
            line-height: 18px;
            margin-top: 8px;
        }
        .tilePanel {
            flex: 0 0 auto;
            width: 100%;
            margin-top: clamp(12px, 1.3vw, 20px);
        }
        .suggestionLink {
            display: flex;
            align-items: center;
            cursor: pointer;
            flex: 1 1 0;
            min-height: 36px;
            margin-top: 3px;
            overflow: hidden;
            color: #173f63;
            text-decoration: none;
            border-radius: 4px;
            background-color: #f7fbfd;
        }
        .suggestionLink:hover {
            background-color: #eaf3f8;
        }
        .suggestionImage {
            display: block;
            flex: 0 0 34px;
            width: 34px;
            height: 34px;
            object-fit: cover;
            background-color: #d8e5ec;
        }
        .suggestionText {
            display: block;
            padding: 3px 7px;
            font-size: 12px;
            font-weight: normal;
            line-height: 14px;
        }
        .tileSuggestions {
            display: flex;
            flex: 1 1 auto;
            flex-direction: column;
            justify-content: space-between;
            margin-top: 8px;
            min-height: 0;
        }
        .randomArea {
            margin-top: auto;
            flex: 1 1 auto;
            display: flex;
            flex-direction: column;
            padding-top: clamp(12px, 1.4vw, 22px);
            width: 100%;
        }
        .randomGrid {
            position: relative;
            flex: 1 1 auto;
            height: 100%;
            min-height: clamp(260px, 34vh, 460px);
            overflow: hidden;
            width: 100%;
        }
        .randomLink {
            position: absolute;
            display: inline-flex;
            align-items: center;
            overflow: hidden;
            color: #173f63;
            text-decoration: none;
            border-radius: 12px;
            opacity: 0.88;
            max-width: 130px;
        }
        .randomLink:hover {
            opacity: 1;
            outline: 1px solid #2b74a8;
            z-index: 5;
        }
        .randomImage {
            display: block;
            flex: 0 0 26px;
            width: 26px;
            height: 26px;
            object-fit: cover;
            border-radius: 50%;
            background-color: #d8e5ec;
        }
        .randomText {
            display: block;
            overflow: hidden;
            padding-left: 3px;
            font-size: 10px;
            line-height: 12px;
            text-overflow: ellipsis;
            white-space: nowrap;
        }
        .osmAttribution {
            display: inline-block;
            color: #666666;
            font-size: 10px;
            line-height: 13px;
        }
        .randomStatus {
            display: inline-block;
            color: #666666;
            font-size: 10px;
            line-height: 13px;
            margin-right: 12px;
        }
        .osmDiagnostic {
            display: block;
            width: 100%;
            max-height: 110px;
            overflow: auto;
            margin-top: 4px;
            color: #777777;
            font-family: Consolas, Courier New, monospace;
            font-size: 10px;
            line-height: 12px;
            white-space: pre-wrap;
        }
        .randomFooter {
            flex: 0 0 auto;
            display: grid;
            grid-template-columns: minmax(180px, 1fr) minmax(260px, auto);
            align-items: center;
            column-gap: 16px;
            min-height: 16px;
            margin-top: 4px;
            width: 100%;
        }
        .osmEmptyFooter {
            color: #777777;
            font-size: 10px;
            line-height: 13px;
            min-width: 0;
            text-align: left;
            white-space: nowrap;
        }
        .randomFooterRight {
            display: flex;
            justify-content: flex-end;
            align-items: center;
            gap: 8px;
            min-width: 0;
            text-align: right;
            white-space: nowrap;
        }
        .loadPlacesButton {
            border: 1px solid #8fa8b9;
            border-radius: 4px;
            background-color: #e9f1f6;
            color: #183f65;
            cursor: pointer;
            font-size: 10px;
            font-weight: bold;
            min-height: 20px;
            padding: 2px 8px;
        }
        .loadPlacesButton:hover {
            background-color: #dcebf4;
        }
        .osmAttribution a {
            color: #0066cc;
            text-decoration: none;
        }
        .tileGrid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: clamp(10px, 1vw, 18px);
            width: 100%;
        }
        .aroundTile {
            display: flex;
            flex-direction: column;
            min-height: clamp(330px, 33vw, 430px);
            color: #222222;
            text-decoration: none;
            border: 1px solid #c8d5dd;
            border-radius: 6px;
            background-color: #ffffff;
            overflow: hidden;
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.08);
        }
        .aroundTile:hover {
            border-color: #2b74a8;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.16);
        }
        .tileImage {
            display: block;
            width: 100%;
            height: clamp(58px, 7vw, 86px);
            object-fit: cover;
            background-color: #d8e5ec;
        }
        .tileContent {
            display: flex;
            flex: 1 1 auto;
            flex-direction: column;
            padding: 8px 10px 10px 10px;
        }
        .tileTitle {
            display: block;
            color: #173f63;
            font-size: 14px;
            font-weight: bold;
            line-height: 17px;
            margin-bottom: 3px;
        }
        .tileText {
            display: block;
            color: #444444;
            font-size: 12px;
            line-height: 15px;
            min-height: 34px;
        }
        .tileSource {
            display: block;
            color: #0066cc;
            font-size: 12px;
            font-weight: bold;
            margin-top: 5px;
        }
        .tileBody {
            display: block;
        }
        .tileBottom {
            display: flex;
            flex: 1 1 auto;
            flex-direction: column;
            margin-top: 6px;
        }
        .tileCulture { background-color: #fff9f1; }
        .tileTourism { background-color: #f4fbff; }
        .tileFood { background-color: #f8fff5; }
        .tileEvents { background-color: #fff6fb; }
        .tileCivic { background-color: #f7f7ff; }
        .tileCustom { background-color: #f5fbf7; }
        @media (min-width: 1400px) {
            .tileGrid {
                grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            }
        }
        @media (max-width: 620px) {
            .page {
                padding: 12px;
                min-height: 0;
            }
            .topicFilter {
                position: static;
                width: 100%;
                margin-bottom: 10px;
            }
            .title,
            .subtitle {
                padding-right: 0;
            }
            .tileGrid {
                grid-template-columns: 1fr;
                margin-top: 12px;
            }
            .aroundTile {
                min-height: 0;
            }
            .tileText {
                min-height: 0;
            }
        }
    </style>
    <script type="text/javascript">
        var aroundMeHiddenTopics = {};
        var aroundMeCustomTopics = [];
        var aroundMeRestoringTopics = false;
        var aroundMeTopicCookieName = "AroundMeTopicState";

        function aroundMeEncode(value) {
            var div = document.createElement("div");
            div.appendChild(document.createTextNode(value || ""));
            return div.innerHTML;
        }

        function aroundMeHashText(value) {
            var hash = 0;
            for (var i = 0; i < value.length; i++) {
                hash = ((hash << 5) - hash) + value.charCodeAt(i);
                hash = hash & hash;
            }
            return Math.abs(hash);
        }

        function aroundMeGetCustomTopicImage(topic) {
            var text = topic.toLowerCase();
            var imageMap = [
                { keys: ["hotel", "lodging", "resort", "motel"], url: "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=800&q=80" },
                { keys: ["shop", "shopping", "store", "mall"], url: "https://images.unsplash.com/photo-1441986300917-64674bd600d8?auto=format&fit=crop&w=800&q=80" },
                { keys: ["sport", "fitness", "gym", "stadium"], url: "https://images.unsplash.com/photo-1461896836934-ffe607ba8211?auto=format&fit=crop&w=800&q=80" },
                { keys: ["school", "college", "education", "library"], url: "https://images.unsplash.com/photo-1524995997946-a1c2e315a42f?auto=format&fit=crop&w=800&q=80" },
                { keys: ["health", "doctor", "clinic", "hospital"], url: "https://images.unsplash.com/photo-1505751172876-fa1923c5c528?auto=format&fit=crop&w=800&q=80" },
                { keys: ["transport", "bus", "train", "airport"], url: "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=800&q=80" },
                { keys: ["nature", "hiking", "trail", "garden"], url: "https://images.unsplash.com/photo-1441974231531-c6227db76b6e?auto=format&fit=crop&w=800&q=80" },
                { keys: ["night", "bar", "club", "music"], url: "https://images.unsplash.com/photo-1501612780327-45045538702b?auto=format&fit=crop&w=800&q=80" },
                { keys: ["home", "real estate", "apartment", "housing"], url: "https://images.unsplash.com/photo-1560518883-ce09059eeffa?auto=format&fit=crop&w=800&q=80" },
                { keys: ["business", "office", "work", "startup"], url: "https://images.unsplash.com/photo-1486406146926-c627a92ad1ab?auto=format&fit=crop&w=800&q=80" }
            ];

            for (var i = 0; i < imageMap.length; i++) {
                for (var k = 0; k < imageMap[i].keys.length; k++) {
                    if (text.indexOf(imageMap[i].keys[k]) >= 0) {
                        return imageMap[i].url;
                    }
                }
            }

            var fallbacks = [
                "https://images.unsplash.com/photo-1494526585095-c41746248156?auto=format&fit=crop&w=800&q=80",
                "https://images.unsplash.com/photo-1500534314209-a25ddb2bd429?auto=format&fit=crop&w=800&q=80",
                "https://images.unsplash.com/photo-1521737604893-d14cc237f11d?auto=format&fit=crop&w=800&q=80",
                "https://images.unsplash.com/photo-1531058020387-3be344556be6?auto=format&fit=crop&w=800&q=80",
                "https://images.unsplash.com/photo-1519671482749-fd09be7ccebf?auto=format&fit=crop&w=800&q=80"
            ];
            return fallbacks[aroundMeHashText(text) % fallbacks.length];
        }

        function aroundMeGetCustomDetailImages(topic) {
            var base = aroundMeHashText(topic.toLowerCase());
            var images = [
                "https://images.unsplash.com/photo-1566127992631-137a642a90f4?auto=format&fit=crop&w=120&q=80",
                "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=120&q=80",
                "https://images.unsplash.com/photo-1501281668745-f7f57925c3b4?auto=format&fit=crop&w=120&q=80",
                "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?auto=format&fit=crop&w=120&q=80",
                "https://images.unsplash.com/photo-1488459716781-31db52582fe9?auto=format&fit=crop&w=120&q=80",
                "https://images.unsplash.com/photo-1529107386315-e1a2ed48a620?auto=format&fit=crop&w=120&q=80",
                "https://images.unsplash.com/photo-1441974231531-c6227db76b6e?auto=format&fit=crop&w=120&q=80"
            ];
            var picked = [];
            for (var i = 0; i < 5; i++) {
                picked.push(images[(base + i) % images.length]);
            }
            return picked;
        }

        function aroundMeSetCookie(name, value, days) {
            var expires = "";
            if (days) {
                var date = new Date();
                date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                expires = "; expires=" + date.toUTCString();
            }
            document.cookie = name + "=" + encodeURIComponent(value) + expires + "; path=/";
        }

        function aroundMeGetCookie(name) {
            var prefix = name + "=";
            var parts = document.cookie.split(";");
            for (var i = 0; i < parts.length; i++) {
                var part = parts[i].trim();
                if (part.indexOf(prefix) === 0) {
                    return decodeURIComponent(part.substring(prefix.length));
                }
            }
            return "";
        }

        function aroundMeSaveTopicState() {
            if (aroundMeRestoringTopics) {
                return;
            }

            var hidden = [];
            for (var topic in aroundMeHiddenTopics) {
                if (aroundMeHiddenTopics.hasOwnProperty(topic) && aroundMeHiddenTopics[topic]) {
                    hidden.push(topic);
                }
            }

            var state = {
                hidden: hidden,
                custom: aroundMeCustomTopics
            };
            aroundMeSetCookie(aroundMeTopicCookieName, JSON.stringify(state), 365);
        }

        function aroundMeRestoreTopicState() {
            var value = aroundMeGetCookie(aroundMeTopicCookieName);
            if (!value) {
                return;
            }

            try {
                aroundMeRestoringTopics = true;
                var state = JSON.parse(value);
                if (state && state.custom && state.custom.length) {
                    for (var c = 0; c < state.custom.length; c++) {
                        aroundMeCreateTopicTile(state.custom[c]);
                    }
                }

                aroundMeHiddenTopics = {};
                if (state && state.hidden && state.hidden.length) {
                    for (var h = 0; h < state.hidden.length; h++) {
                        aroundMeHiddenTopics[state.hidden[h]] = true;
                    }
                }
                aroundMeApplyHiddenTopics();
            } catch (ignore) {
            } finally {
                aroundMeRestoringTopics = false;
            }
        }

        function aroundMeSetArea(area) {
            if (!area) {
                return;
            }

            var areaText = area.trim();
            if (areaText.length === 0) {
                return;
            }

            var title = document.getElementById("<%= LabelTitle.ClientID %>");
            var subtitle = document.getElementById("<%= LabelSubtitle.ClientID %>");
            var locationBox = document.getElementById("<%= TextBoxLocation.ClientID %>");
            if (title) {
                title.innerHTML = "AroundMe - " + areaText;
            }
            if (subtitle) {
                subtitle.innerHTML = "Culture, tourism, food, events, and civic information around " + areaText + ".";
            }
            if (locationBox && locationBox.value.trim().length === 0) {
                locationBox.value = areaText;
            }
            document.title = "AroundMe - " + areaText;

            var tiles = document.querySelectorAll(".aroundTile[data-search]");
            for (var i = 0; i < tiles.length; i++) {
                var query = tiles[i].getAttribute("data-search");
                tiles[i].href = "https://www.google.com/search?q=" + encodeURIComponent(areaText + " " + query);
            }

            var suggestions = document.querySelectorAll(".suggestionLink[data-search]");
            for (var s = 0; s < suggestions.length; s++) {
                var suggestionQuery = suggestions[s].getAttribute("data-search");
                suggestions[s].setAttribute("data-href", "https://www.google.com/search?q=" + encodeURIComponent(areaText + " " + suggestionQuery));
            }

        }

        function aroundMeHideSelectedTopics() {
            var select = document.getElementById("TopicSelect");
            if (!select) {
                return false;
            }

            for (var i = 0; i < select.options.length; i++) {
                if (select.options[i].selected) {
                    aroundMeHiddenTopics[select.options[i].value] = true;
                }
            }

            aroundMeApplyHiddenTopics();
            aroundMeSaveTopicState();
            return false;
        }

        function aroundMeApplyHiddenTopics() {
            var topicItems = document.querySelectorAll("[data-topic]");
            for (var t = 0; t < topicItems.length; t++) {
                var topic = topicItems[t].getAttribute("data-topic");
                topicItems[t].style.display = aroundMeHiddenTopics[topic] ? "none" : "";
            }

            var select = document.getElementById("TopicSelect");
            if (select) {
                for (var i = 0; i < select.options.length; i++) {
                    if (aroundMeHiddenTopics[select.options[i].value]) {
                        select.options[i].className = "topicOptionHidden";
                    } else {
                        select.options[i].className = "";
                    }
                }
            }
            aroundMeSaveTopicState();
            aroundMePlaceRandomLinksUnderTiles();
            aroundMeUpdateRandomStatus();
        }

        function aroundMeShowAllTopics() {
            aroundMeHiddenTopics = {};
            var topicItems = document.querySelectorAll("[data-topic]");
            for (var t = 0; t < topicItems.length; t++) {
                topicItems[t].style.display = "";
            }

            var select = document.getElementById("TopicSelect");
            if (select) {
                for (var i = 0; i < select.options.length; i++) {
                    select.options[i].selected = false;
                    select.options[i].className = "";
                }
            }

            aroundMeSaveTopicState();
            aroundMePlaceRandomLinksUnderTiles();
            aroundMeUpdateRandomStatus();
            return false;
        }

        function aroundMeAddTopic() {
            var input = document.getElementById("TopicAddText");
            if (!input) {
                return false;
            }

            var topic = input.value.trim();
            if (topic.length === 0) {
                return false;
            }

            aroundMeCreateTopicTile(topic);
            aroundMeSaveTopicState();
            input.value = "";
            return false;
        }

        function aroundMeCreateTopicTile(topic) {
            var select = document.getElementById("TopicSelect");
            var tileGrid = document.querySelector(".tileGrid");
            if (!select || !tileGrid) {
                return false;
            }

            if (topic.length === 0) {
                return false;
            }

            for (var i = 0; i < select.options.length; i++) {
                if (select.options[i].value.toLowerCase() === topic.toLowerCase()) {
                    select.options[i].selected = false;
                    delete aroundMeHiddenTopics[select.options[i].value];
                    aroundMeApplyHiddenTopics();
                    return false;
                }
            }

            var option = document.createElement("option");
            option.value = topic;
            option.text = topic;
            select.appendChild(option);
            var customExists = false;
            for (var ct = 0; ct < aroundMeCustomTopics.length; ct++) {
                if (aroundMeCustomTopics[ct].toLowerCase() === topic.toLowerCase()) {
                    customExists = true;
                    break;
                }
            }
            if (!customExists) {
                aroundMeCustomTopics.push(topic);
            }

            var areaBox = document.getElementById("<%= TextBoxLocation.ClientID %>");
            var area = areaBox && areaBox.value.trim().length > 0 ? areaBox.value.trim() : "<%= Server.HtmlEncode(CurrentArea) %>";
            var encodedTopic = aroundMeEncode(topic);
            var encodedArea = aroundMeEncode(area);
            var searchTopic = encodeURIComponent(area + " " + topic);
            var imageUrl = aroundMeGetCustomTopicImage(topic);
            var detailImages = aroundMeGetCustomDetailImages(topic);
            var suggestions = ["Best " + topic, topic + " near me", topic + " events", topic + " places", topic + " guide"];

            var html = "";
            html += "<a class=\"aroundTile tileCustom\" href=\"https://www.google.com/search?q=" + searchTopic + "\" target=\"_blank\" data-topic=\"" + encodedTopic + "\" data-search=\"" + encodedTopic + "\" title=\"Open " + encodedTopic + "\">";
            html += "<img class=\"tileImage\" src=\"" + imageUrl + "\" alt=\"" + encodedTopic + "\" />";
            html += "<span class=\"tileContent\">";
            html += "<span class=\"tileBody\">";
            html += "<span class=\"tileTitle\">" + encodedTopic + "</span>";
            html += "<span class=\"tileText\">Custom AroundMe topic for " + encodedArea + ".</span>";
            html += "<span class=\"tileSource\">Custom search</span>";
            html += "</span>";
            html += "<span class=\"tileBottom\"><span class=\"tileSuggestions\">";
            for (var s = 0; s < suggestions.length; s++) {
                var suggestion = suggestions[s];
                var encodedSuggestion = aroundMeEncode(suggestion);
                var searchText = encodeURIComponent(area + " " + suggestion);
                html += "<span class=\"suggestionLink\" data-href=\"https://www.google.com/search?q=" + searchText + "\" data-search=\"" + encodedSuggestion + "\" onclick=\"return aroundMeOpenSuggestion(event, this);\" title=\"Open " + encodedSuggestion + "\">";
                html += "<img class=\"suggestionImage\" src=\"" + detailImages[s] + "\" alt=\"" + encodedSuggestion + "\" />";
                html += "<span class=\"suggestionText\">" + encodedSuggestion + "</span>";
                html += "</span>";
            }
            html += "</span></span></span></a>";

            tileGrid.insertAdjacentHTML("beforeend", html);
            aroundMeCreateCustomRandomItems(topic, area, suggestions, detailImages);
            delete aroundMeHiddenTopics[topic];
            aroundMeApplyHiddenTopics();
            return false;
        }

        function aroundMeCreateCustomRandomItems(topic, area, suggestions, detailImages) {
            var randomGrid = document.querySelector(".randomGrid");
            if (!randomGrid) {
                return;
            }

            var encodedTopic = aroundMeEncode(topic);
            var existingLinks = randomGrid.querySelectorAll(".randomLink[data-custom-topic]");
            for (var r = existingLinks.length - 1; r >= 0; r--) {
                if (existingLinks[r].getAttribute("data-custom-topic") === encodedTopic) {
                    existingLinks[r].parentNode.removeChild(existingLinks[r]);
                }
            }

            var detailIndex = aroundMeGetDetailIndex();
            var selectedSuggestion = suggestions[Math.min(detailIndex, suggestions.length - 1)];
            var selectedImage = detailImages[Math.min(detailIndex, detailImages.length - 1)];
            var variants = [
                selectedSuggestion,
                "Best " + selectedSuggestion,
                selectedSuggestion + " nearby",
                selectedSuggestion + " open now",
                "Top rated " + selectedSuggestion
            ];
            var cells = [
                { left: 5, top: 12, width: 86 },
                { left: 25, top: 34, width: 104 },
                { left: 48, top: 18, width: 96 },
                { left: 67, top: 56, width: 112 },
                { left: 82, top: 36, width: 98 }
            ];
            var html = "";
            for (var i = 0; i < variants.length && i < cells.length; i++) {
                var suggestion = variants[i];
                var title = suggestion;
                var encodedTitle = aroundMeEncode(title);
                var url = "https://www.google.com/search?q=" + encodeURIComponent(area + " " + suggestion);
                var style = "left:" + cells[i].left + "%;top:" + cells[i].top + "%;width:" + cells[i].width + "px;height:26px;";
                html += "<a class=\"randomLink\" style=\"" + style + "\" href=\"" + url + "\" target=\"_blank\" data-topic=\"" + encodedTopic + "\" data-custom-topic=\"" + encodedTopic + "\" title=\"Open " + encodedTitle + "\">";
                html += "<img class=\"randomImage\" src=\"" + selectedImage + "\" alt=\"" + encodedTitle + "\" />";
                html += "<span class=\"randomText\">" + encodedTitle + "</span>";
                html += "</a>";
            }
            randomGrid.insertAdjacentHTML("beforeend", html);
            aroundMePlaceRandomLinksUnderTiles();
            aroundMeUpdateRandomStatus();
        }

        function aroundMeGetDetailIndex() {
            var field = document.getElementById("<%= HiddenDetailIndex.ClientID %>");
            var value = field ? parseInt(field.value, 10) : 0;
            if (isNaN(value) || value < 0) {
                return 0;
            }
            return value % 5;
        }

        function aroundMePlaceRandomLinksUnderTiles() {
            var randomGrid = document.querySelector(".randomGrid");
            if (!randomGrid) {
                return;
            }

            var gridRect = randomGrid.getBoundingClientRect();
            if (gridRect.width <= 0 || gridRect.height <= 0) {
                return;
            }

            var tileMap = {};
            var tiles = document.querySelectorAll(".aroundTile[data-topic]");
            for (var t = 0; t < tiles.length; t++) {
                var tileTopic = tiles[t].getAttribute("data-topic");
                if (tileTopic && tiles[t].style.display !== "none") {
                    tileMap[tileTopic] = tiles[t];
                }
            }

            var grouped = {};
            var links = document.querySelectorAll(".randomLink[data-topic]");
            for (var i = 0; i < links.length; i++) {
                var topic = links[i].getAttribute("data-topic");
                if (!topic || !tileMap[topic] || links[i].style.display === "none") {
                    continue;
                }

                if (!grouped[topic]) {
                    grouped[topic] = [];
                }
                grouped[topic].push(links[i]);
            }

            for (var groupTopic in grouped) {
                if (!grouped.hasOwnProperty(groupTopic)) {
                    continue;
                }

                var tileRect = tileMap[groupTopic].getBoundingClientRect();
                var leftMin = ((tileRect.left - gridRect.left) / gridRect.width) * 100;
                var bandWidth = (tileRect.width / gridRect.width) * 100;
                var topicLinks = grouped[groupTopic];
                var rowStep = 92 / Math.max(topicLinks.length, 1);
                var topicSeed = aroundMeHashText(groupTopic + ":" + topicLinks.length);

                for (var linkIndex = 0; linkIndex < topicLinks.length; linkIndex++) {
                    var link = topicLinks[linkIndex];
                    var hash = aroundMeHashText(groupTopic + ":" + linkIndex + ":" + link.href + ":" + topicSeed);
                    var widthPixels = 92 + (hash % 54);
                    var widthPercent = (widthPixels / gridRect.width) * 100;
                    var usableWidth = Math.max(0, bandWidth - widthPercent - 2);
                    var leftJitter = ((hash % 100) / 100) * usableWidth;
                    var topJitterLimit = Math.max(1, Math.min(rowStep * 0.28, 4));
                    var topJitter = (((Math.floor(hash / 100) % 100) / 100) * topJitterLimit);
                    var leftPercent = Math.max(0, Math.min(98 - widthPercent, leftMin + 1 + leftJitter));
                    var topPercent = Math.max(1, Math.min(94, 3 + (linkIndex * rowStep) + topJitter));

                    link.style.left = leftPercent.toFixed(2) + "%";
                    link.style.top = topPercent.toFixed(2) + "%";
                    link.style.width = widthPixels + "px";
                    link.style.height = "26px";
                }
            }
        }

        function aroundMeUpdateRandomStatus() {
            var status = document.getElementById("RandomStatus");
            if (!status) {
                return;
            }

            var links = document.querySelectorAll(".randomLink");
            var visible = 0;
            for (var i = 0; i < links.length; i++) {
                if (links[i].style.display !== "none") {
                    visible++;
                }
            }

            if (links.length === 0) {
                status.innerHTML = "Random links: 0 total";
            } else {
                status.innerHTML = "Random links: " + visible + " visible / " + links.length + " total";
            }

            var emptyFlag = document.getElementById("OsmNotLoadedFlag");
            var loadedEmptyFlag = document.getElementById("OsmLoadedEmptyFlag");
            var emptyFooter = document.getElementById("OsmEmptyFooter");
            if (emptyFooter) {
                if (loadedEmptyFlag) {
                    emptyFooter.innerHTML = "No OpenStreetMap places found";
                    emptyFooter.style.display = "";
                } else if (emptyFlag) {
                    emptyFooter.innerHTML = "Nearby OpenStreetMap places not loaded";
                    emptyFooter.style.display = "";
                } else {
                    emptyFooter.style.display = "none";
                }
            }
        }

        function aroundMeEnsurePlacesLoaded() {
            if (window.location.search.toLowerCase().indexOf("loadplaces=1") >= 0) {
                return;
            }

            var separator = window.location.search.length > 0 ? "&" : "?";
            window.location.href = window.location.pathname + window.location.search + separator + "loadplaces=1";
        }

        function aroundMeStartAutoReload() {
            window.setInterval(function () {
                var button = document.getElementById("<%= ButtonLoadPlaces.ClientID %>");
                if (!button || button.disabled) {
                    return;
                }

                if (window.aroundMeReloading) {
                    return;
                }

                window.aroundMeReloading = true;
                button.click();
            }, 60000);
        }

        function aroundMeOpenSuggestion(event, item) {
            if (event) {
                event.preventDefault();
                event.cancelBubble = true;
                if (event.stopPropagation) {
                    event.stopPropagation();
                }
            }

            var url = item.getAttribute("data-href");
            if (url) {
                window.open(url, "_blank");
            }
            return false;
        }

        function aroundMeDetectIpArea() {
            var locationBox = document.getElementById("<%= TextBoxLocation.ClientID %>");
            if (!locationBox || locationBox.value.trim().length > 0) {
                return;
            }

            fetch("https://ipapi.co/json/")
                .then(function (response) { return response.json(); })
                .then(function (data) {
                    var parts = [];
                    if (data.city) { parts.push(data.city); }
                    if (data.region) { parts.push(data.region); }
                    if (data.country_name) { parts.push(data.country_name); }
                    aroundMeSetArea(parts.join(", "));
                })
                .catch(function () {
                    aroundMeSetArea("<%= Server.HtmlEncode(CurrentArea) %>");
                });
        }

        if (window.addEventListener) {
            window.addEventListener("load", aroundMeRestoreTopicState);
            window.addEventListener("load", aroundMeDetectIpArea);
            window.addEventListener("load", aroundMePlaceRandomLinksUnderTiles);
            window.addEventListener("load", aroundMeUpdateRandomStatus);
            window.addEventListener("load", aroundMeEnsurePlacesLoaded);
            window.addEventListener("load", aroundMeStartAutoReload);
            window.addEventListener("resize", aroundMePlaceRandomLinksUnderTiles);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="HiddenDetailIndex" runat="server" Value="0" />
        <div class="page">
            <div class="topBar">
                <div class="topicFilter">
                    <label class="topicFilterLabel" for="TopicSelect">Topics</label>
                    <select id="TopicSelect" class="topicSelect" multiple="multiple" title="Select one or more topics to hide, then click Hide. Hold Ctrl to select multiple topics.">
                        <option value="Culture">Culture</option>
                        <option value="Tourism">Tourism</option>
                        <option value="Eating">Eating</option>
                        <option value="Events">Events</option>
                        <option value="Festivals">Festivals</option>
                        <option value="Farmers Markets">Farmers Markets</option>
                        <option value="Politics and Civic">Politics and Civic</option>
                    </select>
                    <div class="topicButtons">
                        <button type="button" class="topicButton" onclick="return aroundMeHideSelectedTopics();" title="Hide the selected topics and their matching random items.">Hide</button>
                        <button type="button" class="topicButton" onclick="return aroundMeShowAllTopics();" title="Show all topics and random items again.">Show All</button>
                    </div>
                    <div class="topicAddRow">
                        <input id="TopicAddText" class="topicAddBox" type="text" placeholder="Add topic" title="Type a new topic name to add a custom AroundMe tile." />
                        <button type="button" class="topicButton" onclick="return aroundMeAddTopic();" title="Create and show a custom tile for the typed topic.">Show</button>
                    </div>
                </div>
                <asp:Label ID="LabelTitle" runat="server" CssClass="title"></asp:Label>
                <asp:Label ID="LabelSubtitle" runat="server" CssClass="subtitle"></asp:Label>
                <div class="searchRow">
                    <asp:Label ID="LabelLocation" runat="server" AssociatedControlID="TextBoxLocation" CssClass="locationLabel" Text="Town, region, or country:"></asp:Label>
                    <asp:TextBox ID="TextBoxLocation" runat="server" CssClass="locationBox" ToolTip="Type another town, region, or country to rebuild the AroundMe links."></asp:TextBox>
                    <asp:Button ID="ButtonShow" runat="server" CssClass="button" Text="Show AroundMe" ToolTip="Refresh the dashboard for the entered place." />
                </div>
                <asp:Label ID="LabelNote" runat="server" CssClass="note"></asp:Label>
            </div>

            <div class="tilePanel">
                <div class="tileGrid">
                    <asp:Literal ID="LiteralTiles" runat="server"></asp:Literal>
                </div>
            </div>

            <div class="randomArea">
                <div class="randomGrid">
                    <asp:Literal ID="LiteralRandomLinks" runat="server"></asp:Literal>
                </div>
                <div class="randomFooter">
                    <span id="OsmEmptyFooter" class="osmEmptyFooter">Nearby OpenStreetMap places not loaded</span>
                    <span class="randomFooterRight">
                        <asp:Button ID="ButtonLoadPlaces" runat="server" CssClass="loadPlacesButton" Text="Reload Nearby Places" ToolTip="Load nearby place links from OpenStreetMap." />
                        <span id="RandomStatus" class="randomStatus"></span>
                        <span class="osmAttribution">Place data &copy; <a href="https://www.openstreetmap.org/copyright" target="_blank">OpenStreetMap contributors</a></span>
                    </span>
                </div>
                <asp:Label ID="LabelOsmDiagnostic" runat="server" CssClass="osmDiagnostic"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
