$(document).ready(function () {
    var pickLists = $(".PickList");
    pickLists.change(linkedOptionChange);
    pickLists.trigger("change");
    var triggeredItemList = new Array();
    var conditionalDisplayItem = $("[conditionaldisplay='true']")
    for (var i = 0; i < conditionalDisplayItem.length; i++) {
        var item = conditionalDisplayItem[i];
        var triggers = $(item).attr("triggers").split("|");
        for (var j = 0; j < triggers.length - 1; j++) {
            var trigger = triggers[j];
            if (arrayContains(triggeredItemList, trigger)) {
                continue;
            }
            else {
                triggeredItemList.push(trigger);
            }
            $("#" + trigger).change(trigger, DisplayChecker);
            $("#" + trigger).blur(trigger, DisplayChecker);
        }
    }
    triggeredItemList = new Array();
    for (var i = 0; i < conditionalDisplayItem.length; i++) {
        var item = conditionalDisplayItem[i];
        var triggers = $(item).attr("triggers").split("|");
        for (var j = 0; j < triggers.length - 1; j++) {
            var trigger = triggers[j];
            if (arrayContains(triggeredItemList, trigger)) {
                continue;
            }
            else {
                triggeredItemList.push(trigger);
            }
            $("#" + trigger).trigger("change", trigger);
            $("#" + trigger).trigger("blur", trigger);
        }
    }
    RegisterCalculateField();
    
})

function RegisterCalculateField()
{
    var calculateFields = $("[calculatefield='True']");
    for (var i = 0; i < calculateFields.length; i++) {
        var item = calculateFields[i];
        var calculateArgs = $(item).attr("calculateargs").split("|");
        var expression = $(item).attr("calculateexpression");
        for (var j = 0; j < calculateArgs.length; j++) {
            var calculateArgName = calculateArgs[j];
            if (calculateArgName == "") {
                continue;
            }
            $("#" + calculateArgName).change(function () {
                var currentArgs = $(item).attr("calculateargs").split("|");
                var currentExpression = $(item).attr("calculateexpression");
                for (var i = 0; i < currentArgs.length; i++) {
                    if (currentArg == "") {
                        continue;
                    }
                    var currentArg = currentArgs[i];
                    var currentArgValue = $("#" + currentArg).val();
                    if (currentArgValue == "") {
                        currentArgValue = 0;
                    }
                    if (currentArgValue == undefined) {
                        continue;
                    }
                    currentExpression = currentExpression.replace(currentArg, currentArgValue);
                }
                $(item).val(eval(currentExpression + ";"));
            })
        }
    }
}

function DisplayChecker(trigger) {
    var triggerName = trigger.data != undefined ? trigger.data : trigger;
    var triggerControledElements = $("[triggers*='" + triggerName + "']")
    for (var j = 0; j < triggerControledElements.length; j++) {
        var parent = $("[field='" + triggerName + "'");
        if (parent.css("display") == "none") {
            continue;
        }
        var currentControledElement = triggerControledElements[j];
        var triggerValue = $(currentControledElement).attr("conditionValue");
        var condition = $(currentControledElement).attr("condition");
        var expression = $(currentControledElement).attr("expression");
        var display = false;
        var triggers = $(currentControledElement).attr("triggers").split("|");
        for (var k = 0; k < triggers.length - 1; k++)
        {
            var currentTrigger = triggers[k];
            var triggerDom = $("#" + currentTrigger);
            var value = triggerDom.val();
            expression = expression.replace(new RegExp(currentTrigger,'g'), "'" + value + "'");
        }
        display = eval(expression);
        //var value = $(this).val().toLowerCase();
        //if ($(this).attr("type") == "checkbox")
        //{
        //    value = this.checked.toString();
        //}
        //switch (condition) {
        //    case "Equal":
        //        display = value == triggerValue.toLowerCase();
        //        break;
        //    case "NotEqual":
        //        display = value != triggerValue.toLowerCase();
        //        break;
        //    default:
        //        display = false;
        //}
        if (display) {
            if ($(currentControledElement).attr("displayMode") != null ||
                $(currentControledElement).attr("displayMode") != undefined) {
                $(currentControledElement).css("display", $(currentControledElement).attr("displayMode"));
            }
            else {
                $(currentControledElement).css("display", "table-row");
            }
            var field = $(currentControledElement).attr("field");
            $("#" + field).trigger("change", field)
            $("#" + field).trigger("blur", field);
        }
        else {
            $(currentControledElement).css("display", "none");
            //$(currentControledElement).find("input[type='text']").val("");
            var field = $(currentControledElement).attr("field");
            var child = $("[triggers*='" + field + "']");
            child.css("display", "none");

        }
    }

}

function linkedOptionChange() {
    var currentLOVID = getSelectedLOVID(this);
    var mylov = $(this).attr("mylov");
    var childPickLists = $(".PickList[parentlov='" + mylov + "']");
    for (var i = 0; i < childPickLists.length; i++) {
        filterOptions(currentLOVID, $(childPickLists[i]));
    }
}

function getSelectedLOVID(picklist) {
    var parentOptions = $(picklist).find("option");
    for (var i = 0; i < parentOptions.length; i++) {
        var current = parentOptions[i];
        if (current.selected &&
            $(current).val() != "" &&
            $(current).parent("span.toggleOption").length == 0)//in case the wrapped item is selected, skip the wrapped item
        {
            return $(current).attr("lovid");
        }
    }
}

function filterOptions(currentLOVID, picklistToFilter) {
    var options = picklistToFilter.find("option");
    //for (var i = 0; i < options.length; i++) {
    //    var current = $(options[i]);
    //    var options = current.find("option");
    if (picklistToFilter.attr("multiple") == null) {
        for (var j = 0; j < options.length; j++) {
            var currentOption = $(options[j]);
            var parentLOVID = currentOption.attr("parentLovID");
            if (currentLOVID != parentLOVID) {
                if (currentOption.parent("span.toggleOption").length) {
                    currentOption.parent("span.toggleOption").hide();
                }
                else {
                    currentOption.wrap('<span class="toggleOption" style="display: none;" />');
                }
            }
            else {
                if (currentOption.parent("span.toggleOption").length) {
                    currentOption.unwrap();
                }
            }
        }
    }
    else {
        for (var j = 0; j < options.length; j++) {
            var currentOption = $(options[j]);
            var parentLOVID = currentOption.attr("parentLovID");
            if (currentLOVID != parentLOVID) {
                if (!currentOption.parent("span.toggleOption").length) {
                    currentOption.wrap('<span class="toggleOption" />');
                }
            }
            else if (currentOption.parent("span.toggleOption").length) {
                currentOption.unwrap();
            }
        }
    }
    picklistToFilter.trigger("change");
}

function arrayContains(array,key)
{
    for(var i=0;i<array.length;i++)
    {
        if(array[i]==key)
        {
            return true;
        }
    }
    return false;
}



