
$(document).ready(function () {
    var recordId = $("#hdnActionId").val();
    if (recordId !== "" && recordId !== undefined && recordId !== "0") {
        $("#btnSubmit").text("Update");
    }
    if ($("#btnSubmit").text() === '') {
        GetActionList();
    }
});

//Search on press of Enter Key
$("#txtsearch").on("keypress", function (event) {
    if (event.which === 13) {
        GetActionList();
    }
});

//Get Data on search button click
$("#btnsearch").on("click", function () {
    GetActionList();
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
// Get Action Data
function GetActionList() {
    var search = $("#txtsearch").val();
    tblassignfile = $("#tblActionlist").DataTable({
        "ajax": {
            "url": '/IAM/Action/Get',
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
        "order": [[2, 'desc']],
        "columns": [
            {
                "data": "Action_Name", "name": "Action_Name", "width": "120px", responsivePriority: 1, orderable: false,
                "render": function (data, type, row) {
                    var description = row.Action_Desc ? $('<div/>').text(row.Action_Desc).html(): "No Description";
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
                "data": "Is_Active", "name": "Is_Active", "width": "80px", responsivePriority: 4, orderable: false,
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
            },//index 2
            {
                "data": "Created_On", "name": "Created_On", "width": "120px", responsivePriority: 3, orderable: true
            },//index 3
            {
                "data": "ENC_Action_Id", "name": "ENC_Action_Id", "width": "70px", responsivePriority: 5, orderable: false,
                render: function (data, type, row) {
                    return '<div permission-action="Edit"><a class="tblBtn ms-3" href="/IAM/Action/Edit?Id=' + encodeURIComponent(row.ENC_Action_Id) + '" data-toggle="tooltip" aria-label="Edit" data-bs-original-title="Edit"><i class="ugo-icons">edit</i></a></div>';
                }
            }//index 4
        ],
        drawCallback: function () {
            $('[data-toggle="tooltip"]').tooltip();
        }

    });
}

//-------------------------------------- 2. Get Details ---------------------------------------//
// Get Details
function GetActionDetails(id) {
    if (id !== null && id !== undefined && id !== "") {
        $.ajax({
            url: '/IAM/Action/Details', // Use updated action URL
            type: "GET",
            data: { "Id": id },
            beforeSend: function () {
                $("#tbodyAction").html('');
                $("#dvProcessing").show();
            },
            success: function (response) {
                $("#dvProcessing").hide();
                var html = '';
                $("#tbodyAction").html('');
                if (response.Status) {
                    var data = response.ResponseData;
                    html += '<tr>';
                    html += '<td>' + data.Action_Name + '</td>';
                    html += '<td>' + data.Action_Desc + '</td>';
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
                $("#tbodyAction").html(html);
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
function DownloadActionList() {
    var search = $("#txtsearch").val();
    $.ajax({
        "url": '/IAM/Action/DownloadList',
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
        success: function (data) {
        },
        error: function (xhr) {
            $("#dvProcessing").hide();
            let errorResponse = JSON.parse(xhr.responseText);
            showAlert(errorResponse.responseData, "Error", "OkCancel")
        }
    });
}

//---------------------------------------- 4. Save ---------------------------------------------//
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
        form.attr("action", "/IAM/Action/Update"); // Set action to Update
    } else {
        form.attr("action", "/IAM/Action/Save"); // Default Save action
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
                        window.location.href = "/IAM/Action/Index";
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