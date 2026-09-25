$(document).ready(function () {

    // Show Empty State
    $("#imgchooseflt").show();
    $("#btn").addClass("d-none");

    // Initial state
    $("#DDLDesgId").prop("disabled", true);

    $(".slct2").select2({
        placeholder: 'Select',
        //theme: "bootstrap",
        allowClear: false
    });
    $('.dropdown').on('change', function () {
        $(this).valid(); // Manually trigger validation
    });
    // on dropdown change
    $('#DDLDesgId').on("change", function () {
        var selectedDesg = $(this).val();
        if (selectedDesg && selectedDesg != "") {
            BindRoleList(selectedDesg);
        }
    })

    //role select
    $('.role-select').each(function () {
        var selectedRoleId = $(this).data('roleid');
        $(this).val(selectedRoleId);
    });

    
    // when department changes
    $("#DDLDepId").on("change", function () {

        var department = $(this).val();
        // Every change has disable
        $("#DDLDesgId").prop("disabled", true).empty().append('<option value="">Please wait...</option>');
        // Role section hide
        $("#roleListPartial").empty();
        // Empty State 
        $("#btn").addClass("d-none");
        // Show Empty State
        $("#imgchooseflt").show();
        if (department === '' || department === null) {
            Reset();
        }
    });

    // when designation changes
    $("#DDLDesgId").on("change", function () {
        var designation = $(this).val();
        if (designation === '' || designation === null) {
            $('#roleListPartial').html('');
            $('#btn').addClass('d-none');
        }
    });

});


function Addrow() {
    var length = $('#rolesTableBody td').length;
    if (length == 0 || length === undefined) {
        $('.addRow').trigger('click');
    }
}

$(document).on('change', '.role-select', function () {
    var selectedValues = [];

    $(".role-select").each(function () {
        var $this = $(this);
        var val = $this.val();
        var $errorSpan = $this.siblings('span');

        // Reset error first
        $errorSpan.removeClass("error").text("");

        if (!val || val.trim() === "") {
            $errorSpan.addClass("error").text("Please select a role");
        } else if (selectedValues.includes(val)) {
            $errorSpan.addClass("error").text("Please select unique roles");
        } else {
            selectedValues.push(val);
        }
    });
});

// Get select list options dynamically
function createOptions(optionsJson, defaultText) {
    let options = `<option value="">${defaultText}</option>`;
    let items = optionsJson;
    options += items.map(opt => `<option value="${opt.Value}">${opt.Text}</option>`).join('');
    return options;
}

//To add new row
$(document).on('click', '.addRow', function () {
    let rowIndex = $('#rolesTableBody tr').length;
    var roleOptions = JSON.parse($("#rolesId").val());

    let newRow = `
            <tr>
                <td>
                    <select name="DesignationRoles[${rowIndex}].RoleId" class="form-control role-select slct2">
                        ${createOptions(roleOptions, "Select")}
                    </select>
                    <span class="text-danger"></span>
                </td>
                <td class="text-center">
					<a class="btn btn-outlin-danger text-danger deleteRow"><i class="ugo-icons">delete</i></a>
				</td>
            </tr>`;
    $('#rolesTableBody').append(newRow);
    Addrow();
});

//To bind selected ones
function BindSelectList() {
    $('.role-select').each(function () {
        var selectedRoleId = $(this).data('roleid');
        $(this).val(selectedRoleId);
    });
}

// Delete row
$(document).on('click', '.deleteRow', function () {
    $(this).closest('tr').remove();
    reindexRows();
});

function reindexRows() {
    $('#rolesTableBody tr').each(function (index) {
        $(this).find('select').each(function () {
            let name = $(this).attr('name');
            let newName = name.replace(/\[\d+\]/, `[${index}]`);
            $(this).attr('name', newName);
        });
    });
}

//To Reload the page
$("#btnReset").on("click", function () {
    Reset();
})

function Reset() {
    location.reload();
}




// Validate Dropdown 
function validateDropdown() {
    var isValid = true;
    var selectedArray = [];

    $(".role-select").each(function () {
        var val = $(this).val();

        $(this).siblings('span').removeClass("error").text("");
        if (val === '' || val === undefined || val == null) {
            isValid = false;
            // Highlight error
            $(this).siblings('span').addClass("error").text("Please select a role");
        }
        else if (val && selectedArray.includes(val)) {
            isValid = false;
            // Highlight error
            $(this).siblings('span').addClass("error").text("Please select unique roles");
        }
        else {
            selectedArray.push(val);
        }
    })
    return isValid;

}

// Get Role list data through array
function BindRoles() {
    var roleArray = [];

    $(".role-select").each(function () {
        var val = $(this).val();
        if (val) {
            roleArray.push(val);
        }
    })
    return roleArray;
}


// --------------------------------------- Get ------------------------------------------- //

function BindRoleList(selectedDesg) {
    $.ajax({
        url: "/IAM/DesignationRoleMapping/Get",
        type: "POST",
        data: {
            Department_Id: $("#DDLDepId").val(),
            Designation_Id: selectedDesg
        },
        beforeSend: function () {
            $("#dvProcessing").show();
            $("#roleListPartial").html('');
        },
        success: function (data) {
            $("#dvProcessing").hide();

            if (data && data.trim().startsWith('<')) {
                $("#imgchooseflt").hide();
                $("#roleListPartial").html(data);
                $('#btn').removeClass('d-none');
            }
            else {
                $("#imgchooseflt").show();
                $("#roleListPartial").empty();
                $('#btn').addClass('d-none');
            }
            BindSelectList();
            Addrow();
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
    if (!validateDropdown()) {
        return;
    }
    var roleList = [];
    roleList = BindRoles();
    var model = {
        Department_Id: $('#DDLDepId').val(),
        Designation_Id: $('#DDLDesgId').val(),
        Role_Ids: roleList
    }

    $.ajax({
        url: "/IAM/DesignationRoleMapping/Save", // Use updated action URL
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
                    if (key === "Role_Ids") {
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
                        window.location.href = "/IAM/DesignationRoleMapping/Index";
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