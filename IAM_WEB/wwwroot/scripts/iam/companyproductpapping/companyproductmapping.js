$(document).ready(function () {
    $(".slct2").select2({
        placeholder: 'Select',
        //theme: "bootstrap",
        allowClear: false
    });
    $('.dropdown').on('change', function () {
        $(this).valid(); // Manually trigger validation
    });
    // on dropdown change
    $('#DDLCompanyId').on("change", function () {
        var selectedCompany = $(this).val();
        if (selectedCompany && selectedCompany != "") {
            BindProductList(selectedCompany);
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

// Get product list data through array
function GetProductListData() {
    var i;
    var productArray = [];
    var count = $("#parentDiv").children("div").length;
    for (i = 0; i <= count; i++) {
        if ($("#Is_Mapped_" + i).val() == "true") {
            var productId = $("#Product_Id_" + i).val();
            productArray.push(productId);
        }
    }
    return productArray;
}


// --------------------------------------- Get ------------------------------------------- //

function BindProductList(selectedCompany) {
    $.ajax({
        url: "/IAM/CompanyProductMapping/Get",
        type: "POST",
        data: { Company_Id: selectedCompany },
        beforeSend: function () {
            $("#dvProcessing").show();
            $("#productListPartial").html('');
        },
        success: function (data) {
            $("#dvProcessing").hide();

            if (data) {
                $("#productListPartial").html(data);
                $('#btn').removeClass('d-none');
            }
            else {
                showAlert("No Record Found", "Warning", "OkCancel");
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
    var productList = [];
    productList = GetProductListData();
    var model = {
        Company_Id: $('#DDLCompanyId').val(),
        Product_Ids: productList
    }

    $.ajax({
        url: "/IAM/CompanyProductMapping/Save", // Use updated action URL
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
                    if (key === "Product_Ids") {
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
                        window.location.href = "/IAM/CompanyProductMapping/Index";
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