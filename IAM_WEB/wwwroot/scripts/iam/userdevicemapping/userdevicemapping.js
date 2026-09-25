let selectedDevices = [];

$(document).ready(function () {

    // Show Empty State
    $("#imgchooseflt").show();
    $("#btn").addClass("d-none");

    // Select2
    $(".slct2").select2({
        placeholder: 'Select',
        allowClear: false
    });


    $('.dropdown').on('change', function () {
        $(this).valid();
    });

    // User change
    $('#DDLUserId').on("change", function () {

        let selectedUser = $(this).val();
        selectedDevices = []; // RESET on user change

        if (selectedUser) {
            BindList(selectedUser);
        } else {
            $("#imgchooseflt").show();
            $("#btn").addClass("d-none");
            $("#userdevicePartial").empty();
        }
    });
});

// Reset
$("#btnReset").on("click", function () {
    location.reload();
});


// EXPAND / COLLAPSE
$(document).on("click", ".toggle-parent", function () {

    const $icon = $(this).find('.toggle-icon');
    const isExpanded = $icon.text() === 'remove';

    $icon.text(isExpanded ? 'add' : 'remove');
    $(this).closest('tr').nextUntil('tr.level-0').toggleClass('d-none');
});


// VALIDATION
function Validation() {

    let applications = {};

    $('input[type="radio"]').each(function () {
        let appId = $(this).attr('name').replace('device_', '');
        applications[appId] = false;
    });

    $('input[type="radio"]:checked').each(function () {
        let appId = $(this).attr('name').replace('device_', '');
        applications[appId] = true;
    });

    let unselectedApps = Object.keys(applications)
        .filter(app => applications[app] === false);

    if (unselectedApps.length > 0) {
        showAlert(
            "Please select at least one device for each application",
            "Warning"
        );
        return false;
    }

    return true;
}



function GetSelectedDevices() {

    let selectedDevices = [];
    let userId = $('#DDLUserId').val();

    $('input[type="radio"]:checked').each(function () {

        let applicationId = $(this).attr('name').replace('device_', '');
        let deviceId = $(this).val();
        let mappingId = $(this).data('mapping-id');

        selectedDevices.push({
            userId: userId,
            applicationId: applicationId,
            deviceId: deviceId,
            mappingId: mappingId
        });
    });

    return selectedDevices;
}



// BIND LIST (AJAX)
function BindList(User_Id) {

    $.ajax({
        url: "/IAM/UserDeviceMapping/Get",
        type: "POST",
        data: { User_Id: User_Id },
        beforeSend: function () {
            $('#userdevicePartial').html('');
            $('#btn').addClass('d-none');
            $("#dvProcessing").show();
        },
        success: function (data) {

            $("#dvProcessing").hide();

            if (data && data.trim().startsWith('<')) {
                $("#imgchooseflt").hide();
                $("#btn").removeClass("d-none");
                $("#userdevicePartial").html(data);
            }
            else {
                $("#userdevicePartial").empty();
                $("#btn").addClass("d-none");
                $("#imgchooseflt").show();
            }
        },
        error: function () {
            $("#dvProcessing").hide();
            showAlert("Something went wrong", "Error");
        }
    });
}


// SUBMIT FORM
function SubmitForm(formId) {

    event.preventDefault();

    if (!Validation()) return;

    let model = {
        User_Id: $('#DDLUserId').val(),
        DeviceMappings: GetSelectedDevices()
    };

    $.ajax({
        url: '/IAM/UserDeviceMapping/Save',
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
                    BindList(model.User_Id);
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
