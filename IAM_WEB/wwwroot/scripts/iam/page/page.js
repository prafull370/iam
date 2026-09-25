
$(document).ready(function () {

    // Initializing select class
    //$('#DDLPrarentId').select2({
    //    placeholder: "Select Parent",
    //    allowClear: false
    //});

    $('#DDLPrarentId').select2({
        placeholder: "Select Parent",
        allowClear: false,
        width: '100%',
        matcher: function (params, data) {
            if ($.trim(params.term) === '') return data;
            var clean = (data.text || '').replace(/[\u00A0]|└─/g, '').trim().toLowerCase();
            return clean.indexOf(params.term.toLowerCase()) > -1 ? data : null;
        },
        templateSelection: function (item) {
            return (item.text || '').replace(/[\u00A0]|└─/g, '').trim();
        }
    });

    $(".slct2").select2({
        placeholder: 'Select',
        //theme: "bootstrap",
        allowClear: false,
        closeOnSelect: true,      // dropdown auto collapse after select
        language: {
            searchPlaceholder: "Search here"
        }
    });

    // Manually trigger validation
    $('.dropdown').on('change', function () {
        $(this).valid();
    });

    var recordId = $("#hdnPageId").val();
    if (recordId !== "" && recordId !== undefined && recordId !== "0") {
        $("#btnSubmit").text("Update");
    }

    if ($("#btnSubmit").text() === '') {
        LoadPageData();
    }

    $('#DDLServiceId').on("change", function () {
        setTimeout(function () {
            if ($('#DDLModuleId option:eq(1)').length > 0) {
                $('#moduleLabelId').addClass('required');
            }
            else {
                $('#moduleLabelId').removeClass('required');
            }
        }, 500);
    });

    // On checkbox change
    $("#Is_Parent").on("change", function () {
        toggleParentSection('2');
    });
    toggleParentSection('1');


});

//Search on press of Enter Key
$("#txtsearch").on("keypress", function (event) {
    if (event.which === 13) {
        LoadPageData();
    }
});

// Handle parent page
function HandleParentPage() {
    //if ($('#DDLPrarentId').val() === '') {
    //    $('#DDLPrarentId').val('2147483647').trigger('change');
    //    return;
    //}
}
function HandleAccessChange() {
    if ($('#DDLAppId').val() === '') {
        $('#DDLAppId').val('2147483647').trigger('change');
        return;
    }
}

// Remove required class 
if ($('#DDLModuleId option:eq(1)').length === 0) {
    $('#moduleLabelId').removeClass('required');
}

// Function to toggle parent section
function toggleParentSection(val) {
    //if ($("#Is_Parent").is(':checked')) {
    //    $("#divparentId").addClass("d-none");
    //    $('#DDLPrarentId').val('2147483647');
    //}
    //else {
    //    $("#divparentId").removeClass("d-none");
    //    $('#DDLPrarentId').val('2147483647'); // Must exist in <option>
    //}
}

//Get Data on search button click
$("#btnsearch").on("click", function () {
    LoadPageData();
})

//To Reload the page
$("#btnreset").on("click", function () {
    Reset();
})
$("#btnReset").on("click", function () {
    Reset();
})

function Reset() {
    location.reload();
}

// Validation
function ValidateModule() {
    var isValid = true;
    if ($('#DDLModuleId option:eq(1)').length > 0) {
        if ($('#DDLModuleId').val() == '') {
            $('#moduleErrMsg').text('Please Select Module');
            isValid = false;
        }
        else {
            $('#moduleErrMsg').text('');
            isValid = true;
        }
    }
    else {
        $('#moduleErrMsg').text('');
    }
    return isValid;

}

$('#DDLModuleId').on("change", function () {
    var val = $(this).val();
    if (val) {
        $('#moduleErrMsg').text('');
    }
    else {
        $('#moduleErrMsg').text('Please Select Module');
    }
});


// ------------------------------------- 1. Listing ------------------------------------------------//
// Get Page Data
function LoadPageData() {
    var search = $("#txtsearch").val();
    var product_id = $("#DDLProductId").val();
    var service_id = $("#DDLServiceId").val();
    var access_id = $("#DDLAccessId").val();
    tblassignfile = $("#tblPagelist").DataTable({
        "ajax": {
            "url": '/IAM/Page/Get',
            "type": "POST",
            "datatype": "json",
            "data": {
                Search_Val: search,
                Product_Id: product_id,
                Service_Id: service_id,
                Access_Id: access_id
            },
            "dataSrc": function (json) {
                return json.data || []; // Ensure `data` exists
            },
            beforeSend: function () {
            },
            error: function (xhr) {
                $("#dvProcessing").hide();
                let errorResponse = JSON.parse(xhr.responseText);
                showAlert(errorResponse.responseData, "Error", "OkCancel")
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
                "data": "Page_Name", "name": "Page_Name", "width": "120px", responsivePriority: 1, orderable: false,
                "render": function (data, type, row) {
                    var description = row.Page_Desc ? $('<div/>').text(row.Page_Desc).html() : "No Description";
                    return `
                        <div class="d-flex align-items-center">
                            <span>${data}</span>
                            <span class="infoIcon ms-1" data-toggle="tooltip" aria-label="${description}" data-bs-original-title="${description}"> 
                            <i class="ugo-icons">info</i>
                            </span>
                        </div>
                    `;
                }
            },//index 0
            {
                "data": "Product_Name", "name": "Product_Name", "width": "120px", responsivePriority: 1, orderable: false
            },//index 1
            {
                "data": "Service_Name", "name": "Service_Name", "width": "120px", responsivePriority: 1, orderable: false
            },//index 2
            {
                "data": "Is_Active", "name": "Is_Active", "width": "50px", responsivePriority: 1, orderable: false,
                "render": function (data, type, row) {
                    if (row.Is_Active === true) {
                        return '<span class="badge badge-success">Active</span>';
                    }
                    else if (row.Is_Active === false) {
                        return '<span class="badge badge-danger">InActive</span>';
                    }
                    else {
                        return '<span class="badge badge-info"> Not Found </span>';
                    }
                }
            },//index 3
            {
                "data": "Is_Parent", "name": "Is_Parent", "width": "50px", responsivePriority: 1, orderable: false,
                "render": function (data, type, row) {
                    if (row.Is_Parent === true) {
                        return '<span class="badge badge-success">Yes</span>';
                    }
                    else if (row.Is_Parent === false) {
                        return '<span class="badge badge-danger">No</span>';
                    }
                    else {
                        return '<span class="badge badge-info"> Not Found </span>';
                    }
                }
            },//index 4
            {
                "data": "Created_On", "name": "Created_On", "width": "100px", responsivePriority: 2, orderable: true
            },//index 5
            {
                "data": "Page_Id", "name": "Page_Id", "width": "80px", responsivePriority: 1, orderable: false,
                render: function (data, type, row) {
                    return '<a class="tblBtn"  onclick="GetPageDetails(' + row.Page_Id + ')" id="btnmodel" data-bs-toggle="modal" data-bs-target="#vlModal"><i class="ugo-icons">grid_view</i></a>'
                        + '<a class="tblBtn ms-3" href="/IAM/Page/Edit?Id=' + encodeURIComponent(row.ENC_Page_Id) + '" data-toggle="tooltip" aria-label="Edit" data-bs-original-title="Edit"><i class="ugo-icons">edit</i></a>';
                }
            }//index 6
        ],
        drawCallback: function () {
            $('[data-toggle="tooltip"]').tooltip();
        }

    });
}

//-------------------------------------- 2. Get Details ---------------------------------------//
// Get Details
function GetPageDetails(id) {
    if (id !== null && id !== undefined && id !== "") {
        $.ajax({
            url: '/IAM/Page/Details', // Use updated action URL
            type: "GET",
            data: { "Id": id },
            beforeSend: function () {
                $("#tbodyPage").html('');
                $("#dvProcessing").show();
            },
            success: function (response) {
                $("#dvProcessing").hide();
                if (response) {
                    $("#tbodyPage").html(response);
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
    else {
        showAlert("Invalid Record Id", "Error", "OkCancel");
    }
}

//---------------------------------------- 3. Download Report ----------------------------------------//
function DownloadPageList() {
    var search = $("#txtsearch").val();
    var product_id = $("#DDLProductId").val();
    var service_id = $("#DDLServiceId").val();
    var access_id = $("#DDLAccessId").val();
    $.ajax({
        "url": '/IAM/Page/DownloadList',
        "type": "POST",
        "datatype": "json",
        "data": {
            Search_Val: search,
            Product_Id: product_id,
            Service_Id: service_id,
            Access_Id: access_id
        },
        "dataSrc": function (json) {
            console.log("API Response:", json); // Debugging
            return json.data || []; // Ensure `data` exists
        },
        beforeSend: function () {
        },
        success: function (data) {
            //var link = document.createElement("a");
            //var fName = data.filename;
            //if (link.download !== undefined) {
            //    link.setAttribute('href', baseUrl + data.filepath);
            //    link.setAttribute("download", fName);
            //    document.body.appendChild(link);
            //    link.click();
            //    document.body.removeChild(link);
            //}
        },
        error: function (xhr) {
            $("#dvProcessing").hide();
            let errorResponse = JSON.parse(xhr.responseText);
            showAlert(errorResponse.responseData, "Error", "OkCancel")
        }
    });
}

//---------------------------------------- 4. Save ---------------------------------------------//
$("#fileUpload").on("change", function () {
    var fileInput = this; // 'this' refers to the input element

    if (fileInput.files && fileInput.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $("#viewImage").attr("src", e.target.result); // Set image preview
        };

        reader.readAsDataURL(fileInput.files[0]); // Convert file to base64
    }
});

function SubmitForm(formId) {
    event.preventDefault(); // Prevent default form submission
    HandleParentPage();
    // Check if the form is valid before proceeding
    var form = $("#" + formId);

    if (!ValidateModule()) {
        return;
    }

    // Stop submission if validation fails
    if (!form.valid()) {
        return;
    }
    var module = '';
    module = ($("#DDLModuleId").val() == null || $("#DDLModuleId").val() === '') ? 0 : $("#DDLModuleId").val();


    //setting value of parent page if not selected
    //var parentId = $('#DDLPrarentId').val() == null ? $('#hdnParentId').val() : $('#DDLPrarentId').val();
    var parentVal = $('#DDLPrarentId').val();
    var parentId = (parentVal === '' || parentVal === '0' || parentVal === '2147483647') ? null : parentVal;
    var appId = $('#DDLAppId').val() === null || $('#DDLAppId').val() === undefined || $('#DDLAppId').val() === '' ? $('#hdnAppId').val() : $('#DDLAppId').val();

    var formData = new FormData(form[0]); // Serialize form data
    formData.append("Module_Id", module);
    //formData.append("Parent_Id", parentId);
    if (parentId !== null) {
        formData.set("Parent_Id", parentId);
    } else {
        formData.delete("Parent_Id"); // Server side nullable int? will be null
    }
    formData.append("App_Id", appId);

    var submitButton = $("#btnSubmit"); // Target the submit button
    var currentAction = form.attr("action"); // Get current action

    // Check button text and update the action URL accordingly
    // Add Route Accordingly
    if (submitButton.text().trim() === "Update") {
        form.attr("action", "/IAM/Page/Update"); // Set action to Update
    } else {
        form.attr("action", "/IAM/Page/Save"); // Default Save action
    }

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
            else {
                if (response.Status) {
                    showAlert(response.Message, "Success", "OkCancel");

                    // Optionally reload after 2 seconds
                    setTimeout(() => {
                        // Add Route Accordingly
                        window.location.href = "/IAM/Page/Index";
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