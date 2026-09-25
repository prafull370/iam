/* =========================================================
   DOCUMENT READY
========================================================= */
$(document).ready(function () {

    // Show Empty State initially
    $("#imgchooseflt").show();
    $("#btn").addClass("d-none");

    $(".slct2").select2({
        placeholder: 'Select',
        allowClear: false
    });

    $('.dropdown').on('change', function () {
        $(this).valid();
    });

    $('#DDLRoleId').on("change", function () {

        let roleId = $(this).val();

        if (roleId && roleId !== "") {
            BindList(roleId);
        } else {
            $("#imgchooseflt").show();
            $('#rolePagePartial').empty();
            $('#btn').addClass('d-none');
        }
    });
});


/* =========================================================
   RESET
========================================================= */
$("#btnReset").on("click", function () {
    location.reload();
});


/* =========================================================
   EXPAND / COLLAPSE TREE
========================================================= */
$(document).on("click", ".toggle-parent", function () {

    const $icon = $(this).find('.toggle-icon');
    const isExpanded = $icon.text() === 'remove';

    $icon.text(isExpanded ? 'add' : 'remove');

    let $row = $(this).closest('tr');
    let level = getRowLevel($row);

    toggleChildren($row, level, !isExpanded);
});

function getRowLevel($tr) {
    const match = $tr.attr('class').match(/level-(\d+)/);
    return match ? parseInt(match[1]) : -1;
}

function toggleChildren($row, parentLevel, show) {

    let $next = $row.next();

    while ($next.length) {

        let nextLevel = getRowLevel($next);
        if (nextLevel <= parentLevel) break;

        if (show && nextLevel === parentLevel + 1) {
            $next.removeClass('d-none');
        } else {
            $next.addClass('d-none');
            $next.find('.toggle-icon').text('add');
        }

        if (!show) {
            toggleChildren($next, nextLevel, false);
        }

        $next = $next.next();
    }
}


/* =========================================================
   PARENT → CHILD CHECKBOX
========================================================= */
$(document).on("change", ".parent-checkbox", function () {

    let group = $(this).data("group");
    let isChecked = $(this).is(":checked");

    $(`tr#child-row[data-group='${group}'] .child-checkbox`)
        .prop("checked", isChecked)
        .trigger("change");
});


/* =========================================================
   PAGE CHECKBOX ↔ LANDING RADIO LOGIC
========================================================= */
$(document).on("change", ".parent-checkbox, .child-checkbox", function () {

    const pageId = $(this).data("page-id");
    const isChecked = $(this).is(":checked");

    const $radio = $(`input.landing-radio[data-page-id='${pageId}']`);

    if (isChecked) {
        $radio.prop("disabled", false);
    } else {
        $radio.prop("checked", false).prop("disabled", true);
    }
});


/* =========================================================
   LANDING PAGE SELECTION (ONE PER APP + SERVICE)
========================================================= */
$(document).on("change", ".landing-radio", function () {

    const appId = $(this).data("app-id");
    const serviceId = $(this).data("service-id");
    const pageId = $(this).data("page-id");

    const $checkbox = $(`input[type='checkbox'][data-page-id='${pageId}']`);

    // Page must be selected
    if (!$checkbox.is(":checked")) {
        $(this).prop("checked", false);
        showAlert("Please select the page before marking it as Landing Page", "Warning");
        return;
    }

    // Uncheck other landing pages for same App + Service
    $(`input.landing-radio[data-app-id='${appId}'][data-service-id='${serviceId}']`)
        .not(this)
        .prop("checked", false);
});


/* =========================================================
   INITIAL STATE AFTER PARTIAL LOAD
========================================================= */
$(document).on("DOMNodeInserted", "#rolePagePartial", function () {

    $(".landing-radio").each(function () {

        const pageId = $(this).data("page-id");
        const isChecked = $(`input[type='checkbox'][data-page-id='${pageId}']`).is(":checked");

        $(this).prop("disabled", !isChecked);
    });
});


/* =========================================================
   VALIDATION
========================================================= */
function Validation() {

    const checkedPages = $('tr.page input[type="checkbox"]:checked');

    if (checkedPages.length === 0) {
        showAlert("Please select at least one page", "Warning");
        return false;
    }

    let landingRequired = {};

    $(".landing-radio").each(function () {
        const key = `${$(this).data("app-id")}_${$(this).data("service-id")}`;
        landingRequired[key] = false;
    });

    $(".landing-radio:checked").each(function () {
        const key = `${$(this).data("app-id")}_${$(this).data("service-id")}`;
        landingRequired[key] = true;
    });

    for (let key in landingRequired) {
        if (!landingRequired[key]) {
            showAlert("Each Service must have exactly one Landing Page", "Warning");
            return false;
        }
    }

    return true;
}


/* =========================================================
   COLLECT DATA
========================================================= */
function SelectedArrayList() {

    let selectedList = [];

    $("input[type='checkbox']:checked").each(function () {

        let pageId = parseInt($(this).data("page-id")) || 0;
        if (!pageId) return;

        selectedList.push({
            App_Id: parseInt($(this).data("app-id")),
            Service_Id: parseInt($(this).data("service-id")),
            Page_Id: pageId,
            Is_Landing_Page: $(`input.landing-radio[data-page-id='${pageId}']`).is(":checked")
        });
    });

    return selectedList;
}


/* =========================================================
   AJAX : BIND LIST
========================================================= */
function BindList(Role_Id) {

    $.ajax({
        url: "/IAM/RolePageMapping/AppGet",
        type: "POST",
        data: { Role_Id },
        beforeSend: function () {
            $('#rolePagePartial').html('');
            $('#btn').addClass('d-none');
            $("#dvProcessing").show();
        },
        success: function (data) {

            $("#dvProcessing").hide();

            if (typeof data === "string" && data.trim().startsWith('<')) {
                $('#btn').removeClass('d-none');
                $('#rolePagePartial').html(data);
            } else {
                $("#imgchooseflt").show();
                $('#rolePagePartial').empty();
                $('#btn').addClass('d-none');
                showAlert("Data Not Found", "Warning");
            }
        },
        error: function () {
            $("#dvProcessing").hide();
            showAlert("Server Error", "Error");
        }
    });
}


/* =========================================================
   SUBMIT
========================================================= */
function SubmitForm(formId) {

    event.preventDefault();

    if (!Validation()) return;

    const model = {
        Role_Id: $('#DDLRoleId').val(),
        RolePage_Data: SelectedArrayList()
    };

    $.ajax({
        url: '/IAM/RolePageMapping/AppSave',
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(model),
        beforeSend: function () {
            $("#dvProcessing").show();
        },
        success: function (response) {

            $("#dvProcessing").hide();

            if (response.Status) {
                showAlert(response.Message, "Success");
                setTimeout(() => {
                    window.location.href = "/IAM/RolePageMapping/App";
                }, 2000);
            } else {
                showAlert(response.Message, "Warning");
            }
        },
        error: function () {
            $("#dvProcessing").hide();
            showAlert("Server Error", "Error");
        }
    });
}
