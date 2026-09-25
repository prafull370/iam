

// Bind Common Dropdowns
function BindDynamicDropdown(ddlElement, ddlFlag, selectedvalue = '', Id = null, placeholderText = "Select", flag = null, excludeId) {
    $.ajax({
        url: baseUrl + "/IAM/CommonDDL/Get",
        type: 'POST',
        data: {
            Flag: ddlFlag,
            Id: Id,
            DDL_Flag: flag,
            Exclude_Id: excludeId || 0
        },
        beforeSend: function () {
            ddlElement.val('').html('');
            ddlElement.append($("<option></option>").val('').text('Please wait...'));
        },
        success: function (data) {
            ddlElement.val('').html('');
            ddlElement.append($("<option></option>").val('').html(placeholderText));
            $.each(data, function (i, item) {
                ddlElement.append($("<option></option>").val(item.Id).html(item.Name));
            });

            // ✅ Re-parse the form to ensure client-side validation works
            $.validator.unobtrusive.parse(ddlElement.closest("form"));
            ddlElement.val(selectedvalue);
            ddlElement.prop("disabled", false); 
        },
        error: function () {
            console.log('Error while binding dropdown: ' + ddlFlag);
        }
    });
}
function BindDynamicDropdown_WithSelect(ddlElement, ddlFlag, selectedvalue = '', Id = null, placeholderText = "Select", flag = null) {
    $.ajax({
        url: baseUrl + "/IAM/CommonDDL/Get",
        type: 'POST',
        data: {
            Flag: ddlFlag,
            Id: Id,
            DDL_Flag: flag
        },
        beforeSend: function () {
            ddlElement.val('').html('');
            ddlElement.append($("<option></option>").val('').text('Please wait...'));
        },
        success: function (data) {
            ddlElement.val('').html('');
            ddlElement.append($("<option></option>").val('').html(placeholderText));
            ddlElement.append($("<option></option>").val('2147483647').html('select'));
            $.each(data, function (i, item) {
                ddlElement.append($("<option></option>").val(item.Id).html(item.Name));
            });

            // ✅ Re-parse the form to ensure client-side validation works
            $.validator.unobtrusive.parse(ddlElement.closest("form"));
            ddlElement.val(selectedvalue);
        },
        error: function () {
            console.log('Error while binding dropdown: ' + ddlFlag);
        }
    });
}
function BindDynamicDropdownWithCallback(ddlElement, ddlFlag, selectedvalue = '', Id = null, placeholderText = "Select", callback = null) {
    $.ajax({
        url: '/IAM/CommonDDL/Get',
        type: 'POST',
        data: {
            Flag: ddlFlag,
            Id: Id
        },
        success: function (data) {
            ddlElement.val('').html('');
            ddlElement.append($("<option></option>").val('').html(placeholderText));
            $.each(data, function (i, item) {
                ddlElement.append($("<option></option>").val(item.Id).html(item.Name));
            });

            if (selectedvalue) {
                ddlElement.val(selectedvalue);
            }

            if (callback) callback();
        }
    });
}

// Form State Tracking
var __formOriginalState = {};

// Page load / dropdown-bind 
function CaptureOriginalState(formId) {
    var form = $("#" + formId);
    var state = {};
    form.find("input, select, textarea").each(function () {
        var $el = $(this);
        var name = $el.attr("name");
        if (!name) return;
        state[name] = ($el.attr("type") === "checkbox") ? $el.is(":checked") : $el.val();
    });
    __formOriginalState[formId] = state;
}

// State comparison
function GetChangedFields(formId) {
    var form = $("#" + formId);
    var original = __formOriginalState[formId] || {};
    var oldData = {}, newData = {};

    form.find("input, select, textarea").each(function () {
        var $el = $(this);
        var name = $el.attr("name");
        if (!name) return;
        var currentVal = ($el.attr("type") === "checkbox") ? $el.is(":checked") : $el.val();
        var originalVal = original[name];
        if (String(originalVal) !== String(currentVal)) {
            oldData[name] = originalVal;
            newData[name] = currentVal;
        }
    });
    return { oldData: oldData, newData: newData };
}



