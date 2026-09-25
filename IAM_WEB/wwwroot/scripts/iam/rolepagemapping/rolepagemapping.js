$(document).ready(function () {
    // Show Empty State
    $("#imgchooseflt").show();
    $("#btn").addClass("d-none");

    //Add Bootstrap SelectList class
    $(".slct2").select2({
        placeholder: 'Select',
        //theme: "bootstrap",
        allowClear: false
    });

    $('.dropdown').on('change', function () {
        $(this).valid(); // Manually trigger validation
    });

    // on dropdown change
    $('#DDLRoleId').on("change", function () {
        var selectedProduct = $(this).val();
        if (selectedProduct && selectedProduct != "") {
            BindList(selectedProduct);
        }
        else {
            $("#dvProcessing").show();
            $('#btn').addClass('d-none');
            $("#dvProcessing").hide();
        }

    });
});

//To Reload the page
$("#btnReset").on("click", function () {
    Reset();
})

function Reset() {
    location.reload();
}

// Check Uncheck Logic
$(document).on("change", ".parent-checkbox", function () {
    var group = $(this).data("group");
    var isChecked = $(this).is(":checked");

    // Check/uncheck all child checkboxes with the same group
    $(`tr#child-row[data-group='${group}'] .child-checkbox`).prop("checked", isChecked);
});


// Expand-Collapse Logic
// Toggle parent click
$(document).on("click", ".toggle-parent", function () {
    const $row = $(this).closest('tr');
    const level = getRowLevel($row);
    const isExpanded = $row.hasClass('expanded');

    $row.toggleClass('expanded', !isExpanded);
    toggleChildren($row, level, !isExpanded);
});

// Utility functions
function getRowLevel($tr) {
    const className = $tr.attr('class');
    const match = className.match(/level-(\d+)/);
    return match ? parseInt(match[1]) : -1;
}

function toggleChildren($row, parentLevel, show) {
    let next = $row.next();

    while (next.length) {
        const nextLevel = getRowLevel(next);
        if (nextLevel <= parentLevel) break;

        if (show && nextLevel === parentLevel + 1) {
            next.removeClass('d-none');
        } else {
            next.addClass('d-none');
            next.removeClass('expanded');
        }

        if (!show) {
            toggleChildren(next, getRowLevel(next), false);
        }

        next = next.next();
    }
}

// Check Validation
function Validation() {
    var checkboxSelectedCount = $('tr.page input[type="checkbox"]:checked').length;
    var radioSelectedCount = $('tr.page input[type="radio"]:checked').length;
    if (checkboxSelectedCount == 0) {
        showAlert("Please select at least one page", "Warning", "OkCancel");
        return false;
    }
    else if (radioSelectedCount == 0) {
        showAlert("Please select at least one landing page", "Warning", "OkCancel");
        return false;
    }
    else {
        return true;

    }
}

// Make page active first before marking a landing page
$(document).on("change", "input[type='radio']", function () {
    var $radio = $(this);
    var $checkbox = $radio.closest("tr").find("input[type='checkbox']");

    // If the corresponding checkbox is not checked
    if (!$checkbox.is(":checked")) {
        // Prevent radio selection
        $radio.prop("checked", false);
        showAlert("Please select the checkbox before selecting Landing Page","Warning");
    }
});


// Bind Role Page Mapping
function BindList(Role_Id) {
    $.ajax({
        url: "/IAM/RolePageMapping/Get",
        type: "POST",
        data: {
            Role_Id: Role_Id
        },
        beforeSend: function () {
            $('#rolePagePartial').html('');
            $('#btn').addClass('d-none');
            $("#dvProcessing").show();
        },
        success: function (data) {
            $("#dvProcessing").hide();
            var response = data.trim();
            if (data.trim().startsWith('<')) {
                $("#imgchooseflt").hide();
                $('#btn').removeClass('d-none');
                $('#rolePagePartial').html(data);
            }
            else {
                $("#imgchooseflt").show();
                $('#rolePagePartial').empty();
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

function SelectedArrayList() {
    var selectedList = [];

    $("input[type='checkbox']:checked").each(function () {
        var $checkbox = $(this);

        var productId = parseInt($checkbox.data("product-id")) || 0;
        var serviceId = parseInt($checkbox.data("service-id")) || 0;
        var moduleId = parseInt($checkbox.data("module-id")) || 0;
        var pageId = parseInt($checkbox.data("page-id")) || 0;
        var isParent = $checkbox.data("is-parent") === true || $checkbox.data("is-parent") === "true";

        // Find if corresponding radio button is checked
        var isLandingPage = $("input[type='radio'][data-page-id='" + pageId + "']").is(":checked");

        // Push to array only if Page_Id is present (ensures only actual page rows are considered)
        if (pageId > 0) {
            selectedList.push({
                Product_Id: productId,
                Service_Id: serviceId,
                Module_Id: moduleId,
                Page_Id: pageId,
                Is_Landing_Page: isLandingPage,
                Is_Parent: isParent
            });
        }
    });

    return selectedList;
}


// Save Form
function SubmitForm(formId) {
    event.preventDefault(); // Prevent default form submission

    // Check Validation
    if (!Validation()) {
        return;
    }; 

    var form = $("#" + formId);
    // Check if the form is valid before proceeding
    if (!form.valid()) {
        return; // Stop submission if validation fails
    }
    var RoleList = SelectedArrayList();

    var model = {
        Role_Id: $('#DDLRoleId').val(),
        RolePage_Data: RoleList
    };

    $.ajax({
        url: '/IAM/RolePageMapping/Save', // Use updated action URL
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
                        window.location.href = "/IAM/RolePageMapping/Index";
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
            //let errorResponse = JSON.parse(xhr.responseText);
            //showAlert(errorResponse.responseData, "Error", "OkCancel");
        }
    });
}






