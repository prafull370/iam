function _prop(o, name) {
    if (!o) return undefined;
    if (o[name] !== undefined) return o[name];
    var lc = name.charAt(0).toLowerCase() + name.slice(1);
    if (o[lc] !== undefined) return o[lc];
    for (var k in o) { if (k.toLowerCase() === name.toLowerCase()) return o[k]; }
    return undefined;
}

// DROPDOWN LOADER HELPERS
function ShowDropdownLoader(wrapId, selectId) {
    $('#' + wrapId).addClass('loading');
    if (selectId) {
        $('#' + selectId).prop('disabled', true).trigger('change.select2');
    }
}

function HideDropdownLoader(wrapId, selectId) {
    $('#' + wrapId).removeClass('loading');
    if (selectId) {
        $('#' + selectId).prop('disabled', false).trigger('change.select2');
    }
}

/* Polls a select element until it has actual data options */
function WaitForDropdownOptions(selectId, callback, maxWaitMs) {
    maxWaitMs = maxWaitMs || 6000;
    var intervalMs = 150;
    var elapsed = 0;

    var timer = setInterval(function () {
        var count = $('#' + selectId + ' option').not('[value=""]').not('[value="2147483647"]').length;
        elapsed += intervalMs;
        var ajaxFinished = $("#dvProcessing").hasClass("d-none");
        if (count > 0 || (ajaxFinished && elapsed > 300) || elapsed >= maxWaitMs) {
            clearInterval(timer);
            callback(count > 0);
        }
    }, intervalMs);
}

$(document).ready(function () {

    $(".slct2").select2({
        placeholder: 'Select',
        allowClear: false
    });

    $('.dropdown').on('change', function () {
        var $form = $(this).closest('form');
        if ($form.length && $form.data('validator')) {
            $(this).valid();
        }
    });

    // Product => Service
    $('#DDLProductId').on('change', function () {
        var selectedProductId = $(this).val();
        $('#DDLServiceId, #DDLModuleId, #DDLMasterPageId, #DDLPageId').html('<option value="">Select</option>');
        $('#dvModule').addClass('d-none');
        HideActions();

        if (selectedProductId) {
            ShowDropdownLoader('wrapService', 'DDLServiceId');
            BindDynamicDropdown_WithSelect($('#DDLServiceId'), 'SERVICE', '', selectedProductId, 'Select');
            WaitForDropdownOptions('DDLServiceId', function () {
                HideDropdownLoader('wrapService', 'DDLServiceId');
            });
        }
    });

    // Service => Module (if any); else MasterPages directly
    $('#DDLServiceId').on('change', function () {
        var selectedServiceId = $(this).val();
        $('#DDLModuleId, #DDLMasterPageId, #DDLPageId').html('<option value="">Select</option>');
        $('#dvModule').addClass('d-none');
        HideActions();

        if (selectedServiceId) {
            ShowDropdownLoader('wrapModule', 'DDLModuleId');
            // Also lock Master Page until we know whether Module exists
            ShowDropdownLoader('wrapMasterPage', 'DDLMasterPageId');

            BindDynamicDropdown_WithSelect($('#DDLModuleId'), 'MODULE', '', selectedServiceId, 'Select');

            WaitForDropdownOptions('DDLModuleId', function (hasOptions) {
                HideDropdownLoader('wrapModule', 'DDLModuleId');

                if (!hasOptions) {
                    $('#dvModule').addClass('d-none');
                    BindMasterPageByScope(); // handles its own loader
                } else {
                    $('#dvModule').removeClass('d-none');
                    HideDropdownLoader('wrapMasterPage', 'DDLMasterPageId'); // unlock, user picks Module next
                }
            });
        }
    });

    // Module => MasterPages
    $('#DDLModuleId').on('change', function () {
        $('#DDLMasterPageId, #DDLPageId').html('<option value="">Select</option>');
        HideActions();
        BindMasterPageByScope();
    });

    // MasterPage => Page (if any); else Action directly
    $('#DDLMasterPageId').on('change', function () {
        var selectedMasterPageId = $(this).val();
        $('#DDLPageId').html('<option value="">Select</option>');
        $('#dvChildPage').addClass('d-none');
        HideActions();

        if (selectedMasterPageId) {
            ShowDropdownLoader('wrapChildPage', 'DDLPageId');
            BindDynamicDropdown_WithSelect($('#DDLPageId'), 'CHILDPAGE', '', selectedMasterPageId, 'Select');

            WaitForDropdownOptions('DDLPageId', function (hasOptions) {
                HideDropdownLoader('wrapChildPage', 'DDLPageId');

                if (!hasOptions) {
                    $('#dvChildPage').addClass('d-none');
                    BindActionList(selectedMasterPageId);
                } else {
                    $('#dvChildPage').removeClass('d-none');
                }
            });
        }
    });

    // Page => Actions
    $('#DDLPageId').on('change', function () {
        var selectedPage = $(this).val();
        if (selectedPage && selectedPage != "") {
            BindActionList(selectedPage);
        } else {
            HideActions();
        }
    });

    // Actions checkbox-dropdown open/close
    $('#btnActionsToggle').on('click', function (e) {
        e.stopPropagation();
        $('#actionPanel').toggleClass('d-none');
    });

    $('#actionPanel').on('click', function (e) { e.stopPropagation(); });
    $(document).on('click', function () { $('#actionPanel').addClass('d-none'); });

    // Select All checkbox
    $(document).on('change', '#chkSelectAllActions', function () {
        $('#actionList .pam-check').prop('checked', this.checked);
        UpdateActionText();
    });

    // individual action checkbox
    $(document).on('change', '#actionList .pam-check', function () {
        var total = $('#actionList .pam-check').length;
        var checked = $('#actionList .pam-check:checked').length;
        $('#chkSelectAllActions').prop('checked', total > 0 && checked === total);
        UpdateActionText();
    });

    $('#btnSave').on('click', function () { SubmitForm(); });

});

// To reload the page
$("#btnReset").on("click", function () {
    Reset();
});

function Reset() {
    location.reload();
}

// --------------------------------------- Pages ------------------------------------------- //

function BindMasterPageByScope() {
    var serviceId = $('#DDLServiceId').val();
    var moduleId = $('#DDLModuleId').val();

    ShowDropdownLoader('wrapMasterPage', 'DDLMasterPageId');

    if (moduleId && parseInt(moduleId) > 0) {
        BindDynamicDropdown_WithSelect($('#DDLMasterPageId'), 'PAGES', '', moduleId, 'Select', 'MODULE');
    } else if (serviceId) {
        BindDynamicDropdown_WithSelect($('#DDLMasterPageId'), 'PAGES', '', serviceId, 'Select');
    }

    WaitForDropdownOptions('DDLMasterPageId', function () {
        HideDropdownLoader('wrapMasterPage', 'DDLMasterPageId');
    });
}

// --------------------------------------- Actions ----------------------------------------- //

function BindActionList(selectedPage) {
    $.ajax({
        url: "/IAM/PageActionMapping/Get",
        type: "GET",
        data: { pageId: selectedPage },
        beforeSend: function () {
            $("#dvProcessing").removeClass("d-none");
            $("#actionList").html('');
        },
        success: function (response) {
            $("#dvProcessing").addClass("d-none");
            var ok = _prop(response, 'Status');
            var data = _prop(response, 'ResponseData');

            if (ok && data) {
                RenderActions(_prop(data, 'Actions'), _prop(data, 'MappedActionIds'));
                $("#dvActions").removeClass("d-none");
            } else {
                HideActions();
                showAlert(_prop(response, 'Message') || "No Record Found", "Warning", "OkCancel");
            }
        },
        error: function (xhr, status, error) {
            $("#dvProcessing").addClass("d-none");
            HideActions();
            var msg = "Error while loading actions.";
            try { msg = _prop(JSON.parse(xhr.responseText), 'responseData') || msg; } catch (e) { }
            showAlert(msg, "Error", "OkCancel");
        }
    });
}

function RenderActions(actions, mappedIds) {
    actions = actions || [];
    mappedIds = (mappedIds || []).map(Number);

    if (actions.length === 0) {
        $("#actionList").html('<div class="text-muted py-1">No actions available.</div>');
        $('#chkSelectAllActions').prop('checked', false);
        UpdateActionText();
        return;
    }

    var html = '';
    $.each(actions, function (i, action) {
        var id = _prop(action, 'Action_Id');
        var name = _prop(action, 'Action_Name');
        var risky = _prop(action, 'Is_Risky');
        var isChecked = mappedIds.indexOf(Number(id)) >= 0 ? "checked" : "";
        var riskTag = risky ? ' <span class="pam-risk">RISK</span>' : '';

        html += '<label class="d-flex align-items-center py-1" style="cursor:pointer;">';
        html += '<input type="checkbox" class="pam-check form-check-input mt-0 me-2" value="' + id + '" ' + isChecked + ' />';
        html += '<span>' + name + '</span>' + riskTag;
        html += '</label>';
    });

    $("#actionList").html(html);

    var total = actions.length;
    var checked = mappedIds.length;
    $('#chkSelectAllActions').prop('checked', total > 0 && checked >= total);
    UpdateActionText();
}

function UpdateActionText() {
    var names = $("#actionList .pam-check:checked").map(function () {
        return $(this).closest('label').find('span').first().text();
    }).get();

    var count = names.length;
    var text = count === 0 ? "Select" : (count <= 2 ? names.join(", ") : names.slice(0, 2).join(", ") + " +" + (count - 2));
    $("#lblActionText").text(text).toggleClass('text-muted', count === 0);
}

function HideActions() {
    $("#dvActions").addClass("d-none");
    $("#actionPanel").addClass("d-none");
    $("#actionList").html('');
    $('#chkSelectAllActions').prop('checked', false);
    $('#lblActionText').text('Select').addClass('text-muted');
}

function GetSelectedActionData() {
    var actionArray = [];
    $("#actionList .pam-check:checked").each(function () {
        actionArray.push({ Action_Id: Number($(this).val()) });
    });
    return actionArray;
}

// --------------------------------------- Save -------------------------------------------- //

function SubmitForm() {
    if (typeof event !== "undefined" && event) { event.preventDefault(); }
    if (!Validation()) { return; }

    var model = {
        Product_Id: Number($("#DDLProductId").val()),
        Service_Id: Number($("#DDLServiceId").val()),
        Module_Id: $("#DDLModuleId").val() ? Number($("#DDLModuleId").val()) : null,
        Page_Id: $("#DDLPageId").val() ? Number($("#DDLPageId").val()) : Number($("#DDLMasterPageId").val()),
        Actions: GetSelectedActionData()
    };

    $.ajax({
        url: "/IAM/PageActionMapping/Save",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(model),
        beforeSend: function () {
            $("#dvProcessing").removeClass("d-none");
        },
        success: function (response) {
            $("#dvProcessing").addClass("d-none");
            var rtype = (_prop(response, 'ResponseType') || '').toUpperCase();

            if (!_prop(response, 'Status') && rtype === "VALIDATIONFAILLED") {
                $.each(_prop(response, 'ResponseData'), function (key, errorMessages) {
                    if (key === "Actions") { showAlert(errorMessages, "Warning", "OkCancel"); }
                });
            }
            else if (_prop(response, 'Status')) {
                showAlert(_prop(response, 'Message'), "Success", "OkCancel");
                setTimeout(() => { window.location.href = "/IAM/PageActionMapping/Index"; }, 2000);
            }
            else if (rtype === "DUPLICATE") {
                showAlert(_prop(response, 'Message'), "Warning", "OkCancel");
            }
            else {
                showAlert(_prop(response, 'Message'), "Error", "OkCancel");
            }
        },
        error: function (xhr, status, error) {
            $("#dvProcessing").addClass("d-none");
            var msg = "Error while saving.";
            try { msg = _prop(JSON.parse(xhr.responseText), 'responseData') || msg; } catch (e) { }
            showAlert(msg, "Error", "OkCancel");
        }
    });
}

// --------------------------------------- Validation -------------------------------------- //

function Validation() {
    if (!$("#DDLProductId").val()) { showAlert("Please select a Product.", "Warning", "OkCancel"); return false; }
    if (!$("#DDLServiceId").val()) { showAlert("Please select a Service.", "Warning", "OkCancel"); return false; }
    if (!$("#DDLMasterPageId").val()) { showAlert("Please select a Page.", "Warning", "OkCancel"); return false; }
    if (GetSelectedActionData().length === 0) { showAlert("Please select at least one Action.", "Warning", "OkCancel"); return false; }
    return true;
}