
$(document).ready(function () {
    var recordId = $("#hdnDesignationId").val();
    if (recordId !== "" && recordId !== undefined && recordId !== "0") {
        $("#btnSubmit").text("Update");
    }
    if ($("#btnSubmit").text() === '') {
        LoadDesignationData();
    }
    $('.dropdown').on('change', function () {
        $(this).valid(); // Manually trigger validation
    });
});

//Search on press of Enter Key
$("#txtsearch").on("keypress", function (event) {
    if (event.which === 13) {
        LoadDesignationData();
    }
});

//Get Data on search button click
$("#btnsearch").on("click", function () {
    LoadDesignationData();
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
// Get Designation Data
function LoadDesignationData() {
    var search = $("#txtsearch").val();
    tblassignfile = $("#tblDesignationlist").DataTable({
        "ajax": {
            "url": '/IAM/Designation/Get',
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
        "order": [[4, 'desc']],
        "columns": [
            {
                "data": "Designation_Name", "name": "Designation_Name", "width": "100px", responsivePriority: 1, orderable: false,
                "render": function (data, type, row) {
                    var description = row.Designation_Desc ? $('<div/>').text(row.Designation_Desc).html() : "No Description";
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
                "data": "Designation_Code", "name": "Designation_Code", "width": "80px", responsivePriority: 2, orderable: false
            },//index 2
            {
                "data": "Department_Name", "name": "Department_Name", "width": "100px", responsivePriority: 1, orderable: false
            },//index 1
            //{
            //    "data": "Designation_Desc", "name": "Designation_Desc", "width": "100px", responsivePriority: 1, orderable: false
            //},//index 2
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
                "data": "Created_On", "name": "Created_On", "width": "100px", responsivePriority: 2, orderable: true
            },//index 4
            {
                "data": "ENC_Designation_Id", "name": "ENC_Designation_Id", "width": "80px", responsivePriority: 1, orderable: false,
                render: function (data, type, row) {
                    return '<a class="tblBtn ms-3" href="/IAM/Designation/Edit?Id=' + encodeURIComponent(row.ENC_Designation_Id) + '" data-toggle="tooltip" aria-label="Edit" data-bs-original-title="Edit"><i class="ugo-icons">edit</i></a>';
                }
            }//index 5
        ],
        drawCallback: function () {
            $('[data-toggle="tooltip"]').tooltip();
        }

    });
}

//-------------------------------------- 2. Get Details ---------------------------------------//
// Get Details
function GetDesignationDetails(id) {
    if (id !== null && id !== undefined && id !== "") {
        $.ajax({
            url: '/IAM/Designation/Details', // Use updated action URL
            type: "GET",
            data: { "Id": id },
            beforeSend: function () {
                $("#tbodyDesignation").html('');
                $("#dvProcessing").show();
            },
            success: function (response) {
                $("#dvProcessing").hide();
                var html = '';
                $("#tbodyDesignation").html('');
                if (response.Status) {
                    var data = response.ResponseData;
                    html += '<tr>';
                    html += '<td>' + (data.Department_Name ?? '') + '</td>';
                    html += '<td>' + (data.Designation_Name ?? '') + '</td>';
                    html += '<td>' + (data.Designation_Code ?? '') + '</td>';
                    html += '<td>' + (data.Designation_Desc ?? '') + '</td>';
                    // ----------------- IS ACTIVE ----------------- //
                    if (data.Is_Active == true) {
                        html += '<td><span class="badge badge-success">Active</span></td>';
                    }
                    else if (data.Is_Active == false) {
                        html += '<td><span class="badge badge-danger">InActive</span></td>';
                    }
                    else {
                        html += '<td><span class="badge badge-primary">Not Found</span></td>';
                    }
                    html += '</tr>';
                }
                else {
                    showAlert(response.Message, "Error", "OkCancel");
                }
                $("#tbodyDesignation").html(html);
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
function DownloadDesignationList() {
    var search = $("#txtsearch").val();
    $.ajax({
        "url": '/IAM/Designation/DownloadList',
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
        form.attr("action", "/IAM/Designation/Update"); // Set action to Update
    } else {
        form.attr("action", "/IAM/Designation/Save"); // Default Save action
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
                        window.location.href = "/IAM/Designation/Index";
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