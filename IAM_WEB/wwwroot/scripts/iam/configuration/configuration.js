
$(document).ready(function () {
    var recordId = $("#hdnConfigurationId").val();
    if (recordId !== "" && recordId !== undefined && recordId !== "0") {
        $("#btnSubmit").text("Update");
    }
    if ($("#btnSubmit").text() === '') {
        LoadConfigurationData();
    }
});

//Search on press of Enter Key
$("#txtsearch").on("keypress", function (event) {
    if (event.which === 13) {
        LoadConfigurationData();
    }
});

//Get Data on search button click
$("#btnsearch").on("click", function () {
    LoadConfigurationData();
})

//To Reload the page
$("#btnreset").on("click", function () {
    Reset();
})
$("#btnReset").on("click", function () {
    Reset();
})

//To Download Excel
//$("#ExcelDownload").on("click", function () {
//    DownloadCompanyList();
//})

function SubmitForm(formId) {
    event.preventDefault(); // Prevent default form submission

    var form = $("#" + formId);
    // Check if the form is valid before proceeding
    if (!form.valid()) {
        return; // Stop submission if validation fails
    }
    var formData = form.serialize(); // Serialize form data
    var submitButton = $("#btnSubmit"); // Target the submit button
    var currentAction = form.attr("action"); // Get current action

    // Check button text and update the action URL accordingly
    // Add Route Accordingly
    if (submitButton.text().trim() === "Update") {
        form.attr("action", "/IAM/Configuration/Update"); // Set action to Update
    } else {
        form.attr("action", "/IAM/Configuration/Save"); // Default Save action
    }

    $.ajax({
        url: form.attr("action"), // Use updated action URL
        type: "POST",
        data: formData,
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
            } else {
                if (response.Status) {
                    showAlert(response.Message, "Success", "OkCancel");

                    // Optionally reload after 2 seconds
                    setTimeout(() => {
                        // Add Route Accordingly
                        window.location.href = "/IAM/Configuration/Index";
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

function DeleteConfigurationDetails(id) {
    if (id !== null && id !== undefined && id !== "") {
        $.ajax({
            url: '/IAM/Configuration/Delete', // Use updated action URL
            type: "POST",
            data: { "ConfigurationId": id },
            beforeSend: function () {
                $("#dvProcessing").show();
            },
            success: function (response) {
                $("#dvProcessing").hide();
                if (response.Status) {
                    showAlert(response.Message, "Success", "OkCancel");

                    // Optionally reload after 2 seconds
                    setTimeout(() => {
                        // Add Route Accordingly
                        location.reload();
                    }, 2000);
                }
                else if (!response.Status && response.ResponseType.toUpperCase() === "DUPLICATE") {
                    showAlert(response.Message, "Warning", "OkCancel");
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
        showAlert("Invalid recordid.", "Error", "OkCancel");
    }
}
function Reset() {
    location.reload();
}
function LoadConfigurationData() {
    var search = $("#txtsearch").val();
    tblassignfile = $("#tblConfigurationlist").DataTable({
        "ajax": {
            "url":'/IAM/Configuration/Get',
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
                "data": "Config_Name", "name": "Config_Name", "width": "120px", responsivePriority: 1, orderable: false,
                "render": function (data, type, row) {
                    var description = row.Description ? $('<div/>').text(row.Description).html() : "No Description";
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
                "data": "Config_Value", "name": "Config_Value", "width": "120px", responsivePriority: 2, orderable: false,

            },//index 1
            //{
            //    "data": "Description", "name": "Description", "width": "120px", responsivePriority: 2, orderable: false,

            //},//index 1
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
            },//index 3
            {
                "data": "Created_On", "name": "Created_On", "width": "120px", responsivePriority: 2, orderable: true
            },//index 4
            {
                "data": "ENC_Config_Id", "name": "ENC_Config_Id", "width": "70px", responsivePriority: 5, orderable: false,
                render: function (data, type, row) {
                    return '<a class="tblBtn ms-3" href="/IAM/Configuration/Edit?Id=' + encodeURIComponent(row.ENC_Config_Id) + '" data-toggle="tooltip" aria-label="Edit" data-bs-original-title="Edit"><i class="ugo-icons">edit</i></a>';
                }
            }//index 5
        ],
        drawCallback: function () {
            $('[data-toggle="tooltip"]').tooltip();
        }

    });
}


function GetConfigurationDetails(id) {
    if (id !== null && id !== undefined && id !== "") {
        $.ajax({
            url: '/IAM/Configuration/Details', // Use updated action URL
            type: "GET",
            data: { "Id": id },
            beforeSend: function () {
                $("#tbodyConfiguration").html('');
                $("#dvProcessing").show();
            },
            success: function (response) {
                $("#dvProcessing").hide();
                var html = '';
                $("#tbodyConfiguration").html('');
                if (response.Status) {
                    var data = response.ResponseData;
                    html += '<tr>';
                    html += '<td>' + (data.Config_Name ?? '')+'</td>';
                    html += '<td>' + (data.Config_Value ?? '')+'</td>';
                    html += '<td>' + (data.Description ?? '')+'</td>';
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
                $("#tbodyConfiguration").html(html);
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


//function DownloadCompanyList() {
//    var search = $("#txtsearch").val();

//    $.ajax({
//        url: '/IAM/Configuration/DownloadList',
//        type: 'POST',
//        data: {
//            Search_Val: search
//        },
//        xhrFields: {
//            responseType: 'blob'  // Important for binary file
//        },
//        success: function (blob, status, xhr) {
//            // Try to get filename from the response headers
//            const disposition = xhr.getResponseHeader('Content-Disposition');
//            let filename = "Download.xlsx";

//            if (disposition && disposition.indexOf('attachment') !== -1) {
//                const matches = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(disposition);
//                if (matches != null && matches[1]) {
//                    filename = matches[1].replace(/['"]/g, '');
//                }
//            }
//            // Create a download link
//            const url = window.URL.createObjectURL(blob);
//            const a = document.createElement('a');
//            a.href = url;
//            a.download = filename;
//            document.body.appendChild(a);
//            a.click();
//            window.URL.revokeObjectURL(url);
//            a.remove();
//        },
//        error: function (xhr) {
//            $("#dvProcessing").hide();
//            let reader = new FileReader();
//            reader.onload = function () {
//                let errorResponse = JSON.parse(reader.result);
//                showAlert(errorResponse.responseData || "Error occurred", "Error", "OkCancel");
//            };
//            reader.readAsText(xhr.responseText);
//        }
//    });
//}


