
$(document).ready(function () {
    $(".slct2").select2({
        placeholder: 'Select',
        //theme: "bootstrap",
        allowClear: false
    });

    $(".slct").select2({
        placeholder: 'All',
        //theme: "bootstrap",
        allowClear: false
    });

    $('#aadhaar').on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    $('#number').on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    flatpickr("#txtFromDate", {
        dateFormat: "d-M-Y", // Format as DD-MMM-YYYY
        minDate: "today", // Disable past dates
        defaultDate: "today" // Set default date as today
    });



    flatpickr("#txtToDate", {
        dateFormat: "d-M-Y",
        minDate: "today",
    });

    flatpickr("#joinDate", {
        dateFormat: "d-M-Y",
        minDate: "today",
    });

    $('.dropdown').on('change', function () {
        $(this).valid(); // Manually trigger validation
    });

    if ($('.pgName').text() == 'User List') {
        GetUserList();
    }
    $("#block__remark_type_id").on("change", function () {
        let enddate = $("#txtToDate").val();
        let blockremark = $("#block__remark_type_id").val()
        if (enddate !== null && enddate !== undefined && enddate !== "") {
            if (blockremark !== null && blockremark !== undefined && blockremark !== "") {
                $("#spnBlockRemark").text('');
            }
            else {
                $("#spnBlockRemark").text('Please select user remark.');
            }
        }
        else {
            $("#spnBlockRemark").text('');
        }
    });
    $("#txtToDate").on("change", function () {
        let enddate = $("#txtToDate").val();
        let blockremark = $("#block__remark_type_id").val()
        if (enddate !== null && enddate !== undefined && enddate !== "") {
            if (blockremark !== null && blockremark !== undefined && blockremark !== "") {
                $("#spnBlockRemark").text('');
            }
            else {
                $("#spnBlockRemark").text('Please select user remark.');
            }
        }
        else {
            $("#spnBlockRemark").text('');
        }
    });
    var recordId = $("#hdnUserId").val();
    
    // Check whether page is Edit 
    if (recordId !== "" && recordId !== undefined && recordId !== "0") {
        $("#submit").text("Update");
    }

});

// Auto uppercase while typing
$("#identity_no").on("input", function () {
    this.value = this.value.toUpperCase();
});


function validateFile(fileInput) {
    // Reset previous errors
    $('.file-error').remove();

    if (fileInput.files.length === 0) {
        // No file selected - this might be okay if file is optional
        return true;
    }

    const file = fileInput.files[0];
    const allowedExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp','.svg', '.tiff', '.ico'];
    const maxSize = 5 * 1024 * 1024; // 5MB

    // Get file extension
    const fileName = file.name;
    const extension = fileName.substring(fileName.lastIndexOf('.')).toLowerCase();

    // Validate extension
    if (!allowedExtensions.includes(extension)) {
        showAlert("Invalid file type. Only Image files are allowed.", "Warning", "Ok");
        return false;
    }

    // Validate file size
    if (file.size > maxSize) {
        showAlert("File is too large. Maximum size is 5MB.", "Warning", "Ok");
        return false;
    }

    return true;
}



var selectedValues = [];

// NonSelectedDropdownOptions
function getNonSelectedDropdownOptions(selector) {
    var cleanOptions = '';
    $(selector).find('option').each(function () {
        cleanOptions += '<option value="' + $(this).val() + '">' + $(this).text() + '</option>';
    });
    return cleanOptions;
}

$(document).on('change', '.discom', function () {
    selectedValues = [];
    if ($("#hdnUserId").val() == 0) {
        var discomVal = $('#discom_0').val();
        if (discomVal !== '' && discomVal !== null) {
            selectedValues.push(discomVal);
        }
    }

    $(".discom").each(function () {
        var $this = $(this);
        var val = $this.val();
        var id = $this.attr('id');
        let rowIndex = id.split('_')[1];
        var $errorSpan = $this.siblings('span#disErrMsg_' + rowIndex);
        let $selectId = $('#discom_' + rowIndex);

        // Reset error first
        $errorSpan.removeClass("error").text("");
        $selectId.next('.select2-container').find('[role="combobox"]').css('border-color', '');

        if (!val || val.trim() === "") {
            $errorSpan.addClass("error").text("Please select Discom");
            $selectId.next('.select2-container').find('[role="combobox"]').css('border-color', '#edbbbb');
        } else if (selectedValues.includes(val)) {
            $errorSpan.addClass("error").text("Please select unique Discom");
            $selectId.next('.select2-container').find('[role="combobox"]').css('border-color', '#edbbbb');
        } else {
            selectedValues.push(val);
        }
    });
});

if ($('.pgName').text() != 'User List') {
    var discomOptions = getNonSelectedDropdownOptions('#discom_0');

}

//Reset button
$("#btnReset").on("click", function () {
    Reset();
})

function Reset() {
    location.reload();
}

$('#btnSearch').on('click', function () {
    GetUserList();
});

// Encryption Logic
function encryptParam(value) {
    return btoa(encodeURIComponent(value));
}

//----------------------------- Get App permission -----------------------------------//
function GetAppPermission() {
    let designationId = $('#designation_id').val();
    if (designationId === null || designationId === undefined || designationId === "") {
        $('#appPartial').html('');
        return;
    }
    $.ajax({
        url: '/IAM/User/GetAppPermission',
        "type": "POST",
        "datatype": "json",
        "data": {
            Designation_id: designationId
        },
        beforeSend: function () {
            $("#dvProcessing").show();
        },
        success: function (data) {
            $("#dvProcessing").hide();
            $('#appPartial').html(data);
            // Re-initialize Flatpickr after loading
            flatpickr(".appExpTm", {
                enableTime: false,
                dateFormat: "d-M-Y",
                altInput: true,
                altFormat: "d-M-Y",
                allowInput: true,
                minDate: "today"
            });
        },
        error: function (xhr, status, error) {
            $("#dvProcessing").hide();
            let errorResponse = JSON.parse(xhr.responseText);
            showAlert(errorResponse.responseData, "Error", "OkCancel");
        }
    });
}


function GetPermissionById(id) {
    $.ajax({
        url: '/IAM/User/GetPermissionById',
        "type": "POST",
        "datatype": "json",
        "data": {
            User_Id: id
        },
        beforeSend: function () {
            $("#dvProcessing").show();
        },
        success: function (data) {
            $("#dvProcessing").hide();
            $('#appPartial').html(data);
            // Re-initialize Flatpickr after loading
            flatpickr(".appExpTm", {
                enableTime: false,
                dateFormat: "d-M-Y",
                altInput: true,
                altFormat: "d-M-Y",
                allowInput: true,
                minDate: "today"
            });
        },
        error: function (xhr, status, error) {
            $("#dvProcessing").hide();
            let errorResponse = JSON.parse(xhr.responseText);
            showAlert(errorResponse.responseData, "Error", "OkCancel");
        }
    });
}


// ------------------------------------- List ----------------------------------------------//
function GetUserList() {
    var search = $("#txtsearch").val();
    var status = $('#statusId').val();
    tblassignfile = $("#tblUserlist").DataTable({
        "ajax": {
            "url": '/IAM/User/Get',
            "type": "POST",
            "datatype": "json",
            "data": {
                Search: search,
                Status: status
            },
            "dataSrc": function (json) {
                return json.data || []; // Ensure `data` exists
            },
            beforeSend: function () {
            },
            error: function (xhr) {
                $("#dvProcessing").hide();
                //let errorResponse = JSON.parse(xhr.responseText);
                showAlert("Something went wrong", "Error", "OkCancel")
            }
        },
        dom: pagingCSS,
        "pagingType": pagingType,
        "pageLength": pageLength,
        "bDestroy": bDestroy,
        "bAutoWidth": bAutoWidth,
        "processing": processing,
        "serverSide": serverSide,
        "filter": filter,
        responsive: responsive,
        language: language,
        "order": [[6, 'desc']],
        "columns": [
            {
                "data": "User_Id", "name": "User_Id", "width": "120px", responsivePriority: 1, orderable: false,
                "render": function (data, type, row) {
                    // Escape single quotes in the username to avoid JS errors
                    var userName = row.User_Name.replace(/'/g, "\\'");
                    return `<span style="display: inline-flex; align-items: center; gap: 8px;">
                                <img src="${row.User_Logo || '/images/default-user.svg'}"
                                     width="35" height="35" class="user-avatar rounded-circle" 
                                     onerror="handleImageError(this, '${userName}')" />
                                <span class="">${row.User_Name}</span>
                            </span>
                            `;
                }
            },//index 0
            {
                "data": "User_Code", "name": "User_Code", "width": "80px", responsivePriority: 1, orderable: false
            },//index 1
            {
                "data": "Login_Id", "name": "Login_Id", "width": "70px", responsivePriority: 1, orderable: false
            },//index 2
            {
                "data": "Mobile_No", "name": "Mobile_No", "width": "50px", responsivePriority: 1, orderable: false
            },//index 3
            {
                "data": "Department_Name", "name": "Department_Name", "width": "100px", responsivePriority: 1, orderable: false
            },//index 4
            {
                "data": "Designation_Name", "name": "Designation_Name", "width": "100px", responsivePriority: 1, orderable: false
            },//index 5
            {
                "data": "Created_On", "name": "Created_On", "width": "100px", responsivePriority: 2, orderable: true
            },//index 6
            {
                "data": "User_Id", "name": "User_Id", "width": "120px", responsivePriority: 2, orderable: true,
                "render": function (data, type, row) {
                    return '<span><button class="btn btn-sm custom-color" style="font-size:0.6rem; " onclick="ResetPassword(' + row.User_Id + ')">Reset Password</button></span>'
                }
            },//index 7
            {
                "data": "Status", "name": "Status", "width": "50px", responsivePriority: 1, orderable: false,
                "render": function (data, type, row) {
                    if (row.Status === true) {
                        return '<span class="badge badge-success">Active</span>';
                    }
                    else if (row.Status === false) {
                        return '<span class="badge badge-danger">InActive</span>';
                    }
                    else {
                        return '<span class="badge badge-info"> Not Found </span>';
                    }
                }
            },//index 8
            {
                "data": "ENC_User_Id", "name": "User_Id", "width": "50px", responsivePriority: 1, orderable: false,
                render: function (data, type, row) {
                    var id = encryptParam(row.User_Id);
                    return '<a class="tblBtn ms-3" href="/IAM/User/Edit?Id=' + encodeURIComponent(row.ENC_User_Id) + '" data-toggle="tooltip" aria-label="Edit" data-bs-original-title="Edit"><i class="ugo-icons">edit</i></a>';
                }
            }//index 9
        ],
        drawCallback: function () {
            $('[data-toggle="tooltip"]').tooltip();
        }

    });
}

// Reset Password
function ResetPassword(id) {
    Swal.fire({
        title: "Confirm?",
        text: "Are you sure you want to reset the password?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, Reset!",
        cancelButtonText: "Cancel",
        allowOutsideClick: false,
        allowEscapeKey: false,
        preConfirm: () => {
            return $.ajax({
                url: baseUrl + '/IAM/User/ResetPassword',
                type: 'POST',
                data: { Id: id },
                dataType: 'json'
            }).then(data => {
                if (!data.Status) {
                    Swal.showValidationMessage(data.Message);
                }
                return data;
            }).catch(xhr => {
                let errorMsg = xhr.responseJSON?.Message || "An unexpected error occurred.";
                Swal.showValidationMessage(errorMsg);
            });
        }
    }).then(result => {
        if (result.isConfirmed) {
            const data = result.value;
            if (data && data.Status) {
                Swal.fire("Success", "Password reset successfully.", "success");
            } else {
                Swal.fire("Warning", data.Message || "Unable to reset password.", "warning");
            }
        } else {
            Swal.fire("Cancelled", "Password not reset.", "error");
        }
    });
}


//User Avatar on Image load error
function handleImageError(img, username) {
    const parent = img.parentNode;
    const firstChar = username.charAt(0).toUpperCase();
    const colors = ['#FF5722', '#9C27B0', '#3F51B5', '#009688', '#795548'];
    const bgColor = colors[firstChar.charCodeAt(0) % colors.length];

    // Remove image
    img.remove();

    // Create fallback letter circle
    const span = document.createElement('span');
    span.className = 'user-avatar d-inline-block text-white text-center rounded-circle';
    span.style.cssText = `width:30px;height:30px;line-height:30px;background-color:${bgColor};`;
    span.innerText = firstChar;

    parent.prepend(span);
}

// -------------------------------------- Download Excel ---------------------------------------//

$("#ExcelDownload").on("click", function () {
    // Read values
    var status = $("#statusId").val();
    var search = $("#txtsearch").val();

    // Construct query string
    var query = `?Status=${encodeURIComponent(status)}&Search=${encodeURIComponent(search)}`;

    // Trigger download
    window.open('/IAM/User/DownloadExcelReport' + query, '_blank');
});



//---------------------------------------- Save ---------------------------------------------//

// Validate Discom Dropdowns
function validateDiscomDropdowns() {
    let isValid = true;

    $('.discom').each(function () {
        let $select = $(this);
        let value = $select.val();
        let id = $select.attr('id'); // e.g., discom_0
        let rowIndex = id.split('_')[1]; // Get "0" from "discom_0"
        let $errorSpan = $('#disErrMsg_' + rowIndex);
        let $selectId = $('#discom_' + rowIndex);

        if (!value) {
            isValid = false;
            $errorSpan.text('Please select Discom');
            $selectId.next('.select2-container').find('[role="combobox"]').css('border-color', '#edbbbb');
        } else {
            $errorSpan.text('');
            $selectId.next('.select2-container').find('[role="combobox"]').css('border-color', '');
        }
    })
    return isValid;
}

// Row Add Logic
$('#addRowBtn').on('click', function () {
    var rowCount = $('#tbodyrow tr').length;

    // Clone from existing select element only the OPTIONs
    //var discomOptions = $('#discom_0 option').clone().removeAttr('selected');
    //var zoneOptions = $('#ddlLevel1_0 option').clone().removeAttr('selected');

    var newRow = $('<tr>' +
        '<td>' +
        '<select class="form-control form-control-sm company slct2 dropdown discom" id="discom_' + rowCount + '">' +
        '</select>' +
        '<span class="text-danger" style="font-size:0.7rem;" id="disErrMsg_' + rowCount + '"></span>' +
        '</td>' +
        '<td>' +
        '<select class="form-control form-control-sm lvl1 slct2 dropdown" id="ddlLevel1_' + rowCount + '">' +
        '</select>' +
        '</td>' +
        '<td><select class="form-control form-control-sm lvl2 slct2" id="ddlLevel2_' + rowCount + '"><option value="">Select</option></select></td>' +
        '<td><select class="form-control form-control-sm lvl3 slct2" id="ddlLevel3_' + rowCount + '"><option value="">Select</option></select></td>' +
        '<td><select class="form-control form-control-sm lvl4 slct2" id="ddlLevel4_' + rowCount + '"><option value="">Select</option></select></td>' +
        '<td><select class="form-control form-control-sm lvl5 slct2" id="ddlLevel5_' + rowCount + '"><option value="">Select</option></select></td>' +
        '<td><select class="form-control form-control-sm lvl6 slct2" id="ddlLevel6_' + rowCount + '"><option value="">Select</option></select></td>' +
        '<td class="dllrw"><a href="javascript:void(0);" class="btn text-danger deleteRow" style="opacity:0.95" ><i class="ugo-icons">delete</i></a></td>' +
        '</tr>');

    // Append options safely before Select2
    newRow.find('#discom_' + rowCount).append(discomOptions);
    var zoneOptions = getNonSelectedDropdownOptions('#ddlLevel1_0');
    newRow.find('#ddlLevel1_' + rowCount).append(zoneOptions);

    // Append row to table
    $('#tbodyrow').append(newRow);

    // Initialize Select2 only after elements exist in DOM
    newRow.find('.slct2').select2({
        placeholder: 'Select',
        allowClear: false
    });
});


// Remove Row Logic
$(document).on('click', '.deleteRow', function () {
    $(this).closest('tr').remove();
});

// Add multiple user hierarchy logic
function AddUserHierarchy() {
    var arr1 = [];
    for (i = 0; i < $('#tbodyrow tr').length; i++) {
        var discomId = $('#discom_' + i).val();
        var level1 = $('#ddlLevel1_' + i).val() || '0';
        var level2 = $('#ddlLevel2_' + i).val() || '0';
        var level3 = $('#ddlLevel3_' + i).val() || '0';
        var level4 = $('#ddlLevel4_' + i).val() || '0';
        var level5 = $('#ddlLevel5_' + i).val() || '0';
        var level6 = $('#ddlLevel6_' + i).val() || '0';
        arr1.push({
            Discom_Id: discomId,
            Level1_Id: level1,
            Level2_Id: level2,
            Level3_Id: level3,
            Level4_Id: level4,
            Level5_Id: level5,
            Level6_Id: level6
        });
    }
    return arr1;
}

// Add multiple user app permission logic
function UserAppPermission() {
    var arr2 = [];
    for (i = 0; i < $('.apprm .list').length; i++) {
        var appid = $('#appid_' + i).val();
        var chkbox = $('#chkbox_' + i).is(':checked');
        var otpStatus = $('#otpStatus_' + i).is(':checked');
        var appExpTime = $('#appExpTm_' + i).val();
        //Add 07/04/2026
        var applicationid = $('#applicationid_' + i).val();
        var appversion = $('#appversion_' + i).val();
        if (chkbox) {
            arr2.push({
                App_Id: appid,
                Otp_Status: otpStatus,
                Is_Checked: chkbox,
                Expiry_Date: appExpTime,
                Application_Id: applicationid,
                App_Version: appversion
            });
        }
    }
    return arr2;
}


// File upload and view logic
$("#fileUpload").on("change", function () {
    var fileInput = this; // 'this' refers to the input element
    if (!validateFile(this)) {
        return;
    }
    else if (fileInput.files && fileInput.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $("#viewImage").attr("src", e.target.result); // Set image preview
        };

        reader.readAsDataURL(fileInput.files[0]); // Convert file to base64
    }
});


// Submit form logic
function SubmitForm(formId) {
    event.preventDefault(); // Prevent default form submission
    var form = $("#" + formId);
    let isDiscomValid = validateDiscomDropdowns();

    if (!isDiscomValid) {
        event.preventDefault(); // Stop form from submitting
        return;
    }

    //user Document Upload
    $(document).on("change", ".docfile", function () {
        var f = this.files[0];
        if (!f) return;
        var ok = /\.(pdf|jpe?g|png)$/i.test(f.name) && f.size <= 5 * 1024 * 1024;
        if (!ok) {
            showAlert("Only PDF/JPG/PNG up to 5MB allowed.", "Warning", "OkCancel");
            this.value = "";
        }
    });
    

    // Check if the form is valid before proceeding
    if (!form.valid()) {
        return; // Stop submission if validation fails
    }
    var submitButton = $("#submit"); // Target the submit button
    var currentAction = form.attr("action"); // Get current action

    // Check button text and update the action URL accordingly
    // Add Route Accordingly
    if (submitButton.text().trim() === "Update") {
        form.attr("action", "/IAM/User/Update"); // Set action to Update
    } else {
        form.attr("action", "/IAM/User/Save"); // Default Save action
    }

    var uh = AddUserHierarchy();
    var up = UserAppPermission();
    var selectedSkills = $('#skill_id').val() || [];

    var skillObjects = selectedSkills.map(function (id) {
        return {
            Skill_Id: parseInt(id)
        };
    });

    var formData = new FormData(form[0]);
    formData.append("User_Id", $("#hdnUserId").val() || '0');
    formData.append("End_Date", $("#txtToDate").val());
    formData.append("User_Hierarchy", JSON.stringify(uh));
    formData.append("App_Permission", JSON.stringify(up));
    formData.append("User_Skill", JSON.stringify(skillObjects));

    $.ajax({
        url: form.attr("action"), // Use updated action URL
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
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
            else
            {
                if (response.Status) {
                    showAlert(response.Message, "Success", "OkCancel");

                    // Optionally reload after 2 seconds
                    setTimeout(() => {
                        // Add Route Accordingly
                        window.location.href = "/IAM/User/Index";
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

function BindRolePermission() {

    $.ajax({
        url: "/IAM/User/GetRolesPermission",
        type: "POST",
        data: {
            Department_Id: $("#department_id").val(),
            Designation_Id:$('#designation_id').val()
        },
        beforeSend: function () {
            $("#dvProcessing").show();
            $("#tbodyRole").html('');
        },
        success: function (response) {
            $("#dvProcessing").hide();
            if (response) {
                $("#tbodyRole").html(response);
            }
            else {
                showAlert(response.Message, "Error", "OkCancel");
            }
        },
        error: function (xhr, status, error) {
            $("#dvProcessing").hide();
            let errorResponse = JSON.parse(xhr.responseText);
            showAlert(errorResponse.responseData, "Error", "OkCancel");
        }
    });
}