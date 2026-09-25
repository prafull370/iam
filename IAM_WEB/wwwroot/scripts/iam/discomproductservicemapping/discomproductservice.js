$(document).ready(function () {

    // Show Empty State initially
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
    $('#DDLProductId').on("change", function () {
        var selectedProduct = $(this).val();
        if (selectedProduct && selectedProduct != "") {
            BindServiceList(selectedProduct);
        }
        else {
            $("#dvProcessing").show();
            $('#btn').addClass('d-none');
            $("#Product_Url").val('');
            $("#Product_Api_Url").val('');
            $("#dvProcessing").hide();
        }
    })

    // when discom changes
    $("#DDLDiscomId").on("change", function () {
        var discom = $(this).val();
        if (discom === '' || discom === null) {
            Reset();
        }
    });

});

//Search on press of Enter Key
$(document).on('keydown', function (e) {
    if (e.key === 'Enter' || e.keyCode === 13) {
        SubmitForm(); // Call your function
    }
});


function validate(ctrl) {
    var $ctrl = $(ctrl);
    var id = $ctrl.attr('id');
    var val = $ctrl.val();

    // Assume the error span is next to the input
    var $errorSpan = $ctrl.siblings('span.text-danger');

    if (val !== null && val !== undefined && val.trim() !== "") {
        $ctrl.removeClass('border-validation');
        $errorSpan.text('');
    } else if (id.split("_")[1] === "Url") {
        $ctrl.addClass('border-validation');
        $errorSpan.text('Service Url is required.');
    } else if (id.split("_")[1] === "Api") {
        $errorSpan.text('Service API Url is required.');
        $ctrl.addClass('border-validation');
    }
}

//To Reload the page
$("#btnReset").on("click", function () {
    Reset();
})

function Reset() {
    location.reload();
}

// Get Service list data through array
function GetServiceListData() {
    var i;
    var serviceArray = [];
    var count = $("#Domaintbody").children("tr").length;
    for (i = 0; i < count; i++) {
        if ($("#Service_Id_" + i).val() != '') {
            var serviceId = $("#Service_Id_" + i).val();
            var serviceUrl = $("#Service_Url_" + i).val();
            var serviceApiUrl = $("#Service_Api_Url_" + i).val();
            serviceArray.push({
                Service_Id: serviceId,
                Service_Url: serviceUrl,
                Service_Api_Url: serviceApiUrl
            });
        }
    }
    return serviceArray;
}


// --------------------------------------- Get ------------------------------------------- //

function BindServiceList(selectedProduct) {
    $.ajax({
        url: "/IAM/DiscomProductServiceMapping/Get",
        type: "POST",
        data: {
            Discom_Id: $("#DDLDiscomId").val(),
            Product_Id: selectedProduct
        },
        beforeSend: function () {
            $('#btn').addClass('d-none');
            $("#Product_Url").val('');
            $("#Product_Api_Url").val('');
            $("#dvProcessing").show();
            $("#servicePartial").html('');
        },
        success: function (response) {
            $("#dvProcessing").hide();
            var data = response.trim();
            if (data.startsWith('<')) {
                $("#imgchooseflt").hide();
                $("#servicePartial").html(data);
                $("#Product_Url").val($('#hdnPrUrl').val() ?? '');
                $("#Product_Api_Url").val($('#hdnPrApiUrl').val() ?? '');
                $('#btn').removeClass('d-none');
            }
            else {
                $("#imgchooseflt").show();
                $("#servicePartial").html('');
                $('#btn').addClass('d-none');
                showAlert("Product not mapped with service", "Warning", "OkCancel");
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
    var serviceList = [];
    serviceList = GetServiceListData();
    var model = {
        Discom_Id: $('#DDLDiscomId').val(),
        Product_Id: $('#DDLProductId').val(),
        Product_Url: $('#Product_Url').val(),
        Product_Api_Url: $('#Product_Api_Url').val(),
        Service_Data: serviceList
    }

    $.ajax({
        url: "/IAM/DiscomProductServiceMapping/Save", // Use updated action URL
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
                    const match = key.match(/Service_Data\[(\d+)\]\.(\w+)/);
                    if (match) {
                        const index = match[1];
                        const field = match[2];
                        const errorText = errorMessages[0]; // Show first message only

                        let spanId = "";

                        if (field === "Service_Url") {
                            spanId = "#spnService_Url_" + index;
                        } else if (field === "Service_Api_Url") {
                            spanId = "#spnService_API_Url_" + index;
                        }

                        if (spanId) {
                            $(spanId).text(errorText);
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
                        window.location.href = "/IAM/DiscomProductServiceMapping/Index";
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