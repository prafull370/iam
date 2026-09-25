
$(document).ready(function () {
    $('#contactNo').on('input', function () {
        $(this).val($(this).val().replace(/[^0-9]/g, ''));
    });
    $('#tollFreeNo').on('input', function () {
        $(this).val($(this).val().replace(/[^0-9\-]/g, ''));
    });
    var recordId = $("#hdnCompanyId").val();
    if (recordId !== "" && recordId !== undefined && recordId !== "0") {
        $("#btnSubmit").text("Update");
    }
    if ($("#btnSubmit").text() === '') {
        LoadCompanyData();
    }
});

//Search on press of Enter Key
$("#txtsearch").on("keypress", function (event) {
    if (event.which === 13) {
        LoadCompanyData();
    }
});

//Get Data on search button click
$("#btnsearch").on("click", function () {
    LoadCompanyData();
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

// ------------------------------------- 1. Listing ------------------------------------------------//
// Get Company Data
function LoadCompanyData() {
    var search = $("#txtsearch").val();
    tblassignfile = $("#tblCompanylist").DataTable({
        "ajax": {
            "url": '/IAM/Company/Get',
            "type": "POST",
            "datatype": "json",
            "data": {
                Search_Val: search
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
        "order": [[5, 'desc']],
        "columns": [
            {
                "data": "Company_Name", "name": "Company_Name", "width": "140px", responsivePriority: 1, orderable: false,
                "render": function (data, type, row) {
                    // tooltip to show the full description when hovered over. If the description is empty, we can display "No Description".
                    return '<span data-bs-toggle="tooltip" data-bs-placement="top" title="' + (row.Description || 'No Description') + '" style="cursor: pointer;">' + data + '</span>';
                }
            },//index 0
            {
                "data": "Company_Short_Name", "name": "Company_Short_Name", "width": "80px", responsivePriority: 2, orderable: false,
            },//index 1
            {
                "data": "Contact_Person_Name", "name": "Contact_Person_Name", "width": "100px", responsivePriority: 1, orderable: false,
            },//index 2
            {
                "data": "Contact_Mobile_No", "name": "Contact_Mobile_No", "width": "80px", responsivePriority: 1, orderable: true
            },//index 3
            {
                "data": "Is_Active", "name": "Is_Active", "width": "70px", responsivePriority: 2, orderable: false,
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
            },//index 4
            {
                "data": "Created_On", "name": "Created_On", "width": "120px", responsivePriority: 3, orderable: true
            },//index 5
            { data: "END_Company_Id", name: "END_Company_Id", visible: false },
            {
                "data": "Company_Id", "name": "Company_Id", "width": "70px", responsivePriority: 5, orderable: false,
                // ADD THIS HIDDEN COLUMN
    
                render: function (data, type, row) {
                    return '<a class="tblBtn"  onclick="GetCompanyDetails(' + row.Company_Id + ')" id="btnmodel" data-bs-toggle="modal" data-bs-target="#vlModal"><i class="ugo-icons">grid_view</i></a>'
                        + '<a class="tblBtn ms-3" href="/IAM/Company/Edit?Id=' + encodeURIComponent(row.END_Company_Id) + '" data-toggle="tooltip" aria-label="Edit" data-bs-original-title="Edit"><i class="ugo-icons">edit</i></a>';
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
function GetCompanyDetails(id) {
    if (id !== null && id !== undefined && id !== "") {
        $.ajax({
            url: '/IAM/Company/Details', // Use updated action URL
            type: "GET",
            data: { "Id": id },
            beforeSend: function () {
                $("#tbodyCompany").html('');
                $("#dvProcessing").show();
            },
            success: function (response) {
                $("#dvProcessing").hide();
                if (response) {
                    $("#tbodyCompany").html(response);
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
function DownloadCompanyList() {
    var search = $("#txtsearch").val();
    $.ajax({
        "url": '/IAM/Company/DownloadList',
        "type": "POST",
        "datatype": "json",
        "data": {
            Search_Val: search
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

    var form = $("#" + formId);
    // Check if the form is valid before proceeding
    if (!form.valid()) {
        return; // Stop submission if validation fails
    }

    var formData = new FormData(form[0]); // Serialize form data

    var submitButton = $("#btnSubmit"); // Target the submit button
    var currentAction = form.attr("action"); // Get current action

    // Check button text and update the action URL accordingly
    // Add Route Accordingly
    if (submitButton.text().trim() === "Update") {
        form.attr("action", "/IAM/Company/Update"); // Set action to Update
    } else {
        form.attr("action", "/IAM/Company/Save"); // Default Save action
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
                        window.location.href = "/IAM/Company/List";
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