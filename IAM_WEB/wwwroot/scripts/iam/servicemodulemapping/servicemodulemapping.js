$(document).ready(function () {
    // Show Empty State
    $("#imgchooseflt").show();
    $("#btn").addClass("d-none");

    $(".slct2").select2({
        placeholder: 'Select',
        //theme: "bootstrap",
        allowClear: false
    });
    $('.dropdown').on('change', function () {
        $(this).valid(); // Manually trigger validation
    });
    // on dropdown change
    $('#DDLServiceId').on("change", function () {
        var selectedService = $(this).val();
        if (selectedService && selectedService != "") {
            BindModuleList(selectedService);
        }
    })


});

//Search on press of Enter Key
$(document).on('keydown', function (e) {
    if (e.key === 'Enter' || e.keyCode === 13) {
        SubmitForm(); // Call your function
    }
});

//To Reload the page
$("#btnReset").on("click", function () {
    Reset();
})

function Reset() {
    location.reload();
}

// on checkbox change
function BindCheckbox(elem, id) {
    $("#Is_Mapped_" + id).val($(elem).is(":checked") ? "true" : "false");
}

// Get Module list data through array
function GetModuleListData() {
    var i;
    var ModuleArray = [];
    var count = $("#parentDiv").children("div").length;
    for (i = 0; i <= count; i++) {
        if ($("#Is_Mapped_" + i).val() == "true") {
            var ModuleId = $("#Module_Id_" + i).val();
            ModuleArray.push(ModuleId);
        }
    }
    return ModuleArray;
}


// --------------------------------------- Get ------------------------------------------- //

function BindModuleList(selectedService) {
    $.ajax({
        url: "/IAM/ServiceModuleMapping/Get",
        type: "POST",
        data: { Service_Id: selectedService },
        beforeSend: function () {
            $("#dvProcessing").show();
            $("#ModuleListPartial").html('');
        },
        success: function (response) {
            $("#dvProcessing").hide();
            var data = response.trim();
            if (data && data.trim().startsWith('<')) {
                $("#imgchooseflt").hide();
                $("#ModuleListPartial").html(data);
                $('#btn').removeClass('d-none');
            }
            else {
                $("#ModuleListPartial").empty();
                $("#imgchooseflt").show();
                $('#btn').addClass('d-none');
            }
        },
        error: function (xhr, status, error) {
            $("#dvProcessing").hide();
            let errorResponse = JSON.parse(xhr.responseText);
            showAlert(errorResponse.responseData, "Error", "OkCancel");
        }
    });
}


//---------------------------------------- Save ---------------------------------------------//

function SubmitForm(formId) {
    event.preventDefault(); // Prevent default form submission
    var form = $("#" + formId);
    // Check if the form is valid before proceeding
    if (!form.valid()) {
        return; // Stop submission if validation fails
    }
    var ModuleList = [];
    ModuleList = GetModuleListData();
    var model = {
        Service_Id: $('#DDLServiceId').val(),
        Module_Ids: ModuleList
    }

    $.ajax({
        url: "/IAM/ServiceModuleMapping/Save", // Use updated action URL
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(model),
        beforeSend: function () {
            $("#dvProcessing").show();
        },
        success: function (response) {
            $("#dvProcessing").hide();

            if (!response.Status && response.ResponseType.toUpperCase() === "VALIDATIONFAILLED") {
                // Clear previous errors
                $(".text-danger").empty();

                // Show new validation errors
                $.each(response.ResponseData, function (key, errorMessages) {
                    if (key === "Module_Ids") {
                        showAlert(errorMessages, "Warning", "OkCancel");
                    }
                    var field = $('[name="' + key + '"]');
                    if (field.length > 0) {
                        var errorSpan = field.closest("div").find(".text-danger");
                        if (errorSpan.length > 0) {
                            errorSpan.text(errorMessages[0]); // Show first error
                        }
                    }
                });
            }
            else {
                if (response.Status) {
                    showAlert(response.Message, "Success", "OkCancel");

                    // Optionally reload after 2 seconds
                    setTimeout(() => {
                        // Add Route Accordingly
                        window.location.href = "/IAM/ServiceModuleMapping/Index";
                    }, 2000);
                }
                else if (!response.Status && response.ResponseType.toUpperCase() === "DUPLICATE") {
                    showAlert(response.Message, "Warning", "OkCancel");
                }
                else {
                    showAlert(response.Message, "Error", "OkCancel");
                }
            }
        },
        error: function (xhr, status, error) {
            $("#dvProcessing").hide();
            let errorResponse = JSON.parse(xhr.responseText);
            showAlert(errorResponse.responseData, "Error", "OkCancel");
        }
    });
}