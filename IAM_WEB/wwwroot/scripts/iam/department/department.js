$(document).ready(function () {
    var recordId = $("#hdnDepartmentId").val();
    if (recordId !== "" && recordId !== undefined && recordId !== "0") {
        $("#btnSubmit").text("Update");
    }
    if ($("#btnSubmit").text() === '') {
        LoadDepartmentData();
    }
});

//Search on press of Enter Key
$("#txtsearch").on("keypress", function (event) {
    if (event.which === 13) {
        LoadDepartmentData();
    }
});

//Get Data on search button click
$("#btnsearch").on("click", function () {
    LoadDepartmentData();
})

//To Reload the page
$("#btnreset").on("click", function () {
    Reset();
})
$("#btnReset").on("click", function () {
    Reset();
})

//To Download Excel
$("#ExcelDownload").on("click", function () {
    DownloadCompanyList();
})

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
        form.attr("action", "/IAM/Department/Update"); // Set action to Update
    } else {
        form.attr("action", "/IAM/Department/Save"); // Default Save action
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
                        window.location.href = "/IAM/Department/Index";
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

//function DeleteDepartmentDetails(id) {
//    if (id !== null && id !== undefined && id !== "") {
//        $.ajax({
//            url: '/IAM/Department/Delete', // Use updated action URL
//            type: "POST",
//            data: { "DepartmentId": id },
//            beforeSend: function () {
//                $("#dvProcessing").show();
//            },
//            success: function (response) {
//                $("#dvProcessing").hide();
//                if (response.Status) {
//                    showAlert(response.Message, "Success", "OkCancel");

//                    // Optionally reload after 2 seconds
//                    setTimeout(() => {
//                        // Add Route Accordingly
//                        location.reload();
//                    }, 2000);
//                }
//                else if (!response.Status && response.ResponseType.toUpperCase() === "DUPLICATE") {
//                    showAlert(response.Message, "Warning", "OkCancel");
//                }
//                else {
//                    showAlert(response.Message, "Error", "OkCancel");
//                }
//            },
//            error: function (xhr, status, error) {
//                $("#dvProcessing").hide();
//                let errorResponse = JSON.parse(xhr.responseText);
//                showAlert(errorResponse.responseData, "Error", "OkCancel");
//            }
//        });
//    }
//    else {
//        showAlert("Invalid recordid.", "Error", "OkCancel");
//    }
//}
function Reset() {
    location.reload();
}
function LoadDepartmentData() {
    var search = $("#txtsearch").val();
    tblassignfile = $("#tblDepartmentlist").DataTable({
        "ajax": {
            "url":'/IAM/Department/Get',
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
        "order": [[3, 'desc']],
        "columns": [
            {
                "data": "Department_Name", "name": "Department_Name", "width": "120px", responsivePriority: 1, orderable: false,
                "render": function (data, type, row) {
                    var description = row.Department_Desc ? $('<div/>').text(row.Department_Desc).html() : "No Description";
                    return `
                        <div class="d-flex align-items-center">
                            <span>${data}</span>
                            <span class="infoIcon ms-1" data-toggle="tooltip" aria-label="${description}" data-bs-original-title="${description}"> 
                            <i class="ugo-icons">info</i>
                            </span>
                        </div>
                    `;
                }
            },
            {
                "data": "Department_Code", "name": "Department_Code", "width": "120px", responsivePriority: 2, orderable: false,

            },
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
                "data": "ENC_Department_Id", "name": "ENC_Department_Id", "width": "70px", responsivePriority: 5, orderable: false,
                render: function (data, type, row) {
                    //  Wrap edit link and entire cell in permission-action="Edit" so PermissionHelper can hide it
                    return '<div permission-action="Edit"><a class="tblBtn ms-3" href="/IAM/Department/Edit?Id=' + encodeURIComponent(row.ENC_Department_Id) + '" data-toggle="tooltip" aria-label="Edit" data-bs-original-title="Edit"><i class="ugo-icons">edit</i></a></div>';
                }
            }//index 5
        ],
        drawCallback: function () {
            $('[data-toggle="tooltip"]').tooltip();
            // Reapply permissions after table rows are rendered
            if (typeof Permission !== 'undefined' && Permission.reapply) {
                Permission.reapply();
            }
        }

    });
}


function GetDepartmentDetails(id) {
    if (id !== null && id !== undefined && id !== "") {
        $.ajax({
            url: '/IAM/Department/Details', // Use updated action URL
            type: "GET",
            data: { "Id": id },
            beforeSend: function () {
                $("#tbodyDepartment").html('');
                $("#dvProcessing").show();
            },
            success: function (response) {
                $("#dvProcessing").hide();
                var html = '';
                $("#tbodyDepartment").html('');
                if (response.Status) {
                    var data = response.ResponseData;
                    html += '<tr>';
                    html += '<td>' + (data.Department_Name ?? '')+'</td>';
                    html += '<td>' + (data.Department_Code ?? '')+'</td>';
                    html += '<td>' + (data.Department_Desc ?? '')+'</td>';
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
                $("#tbodyDepartment").html(html);
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
//        url: '/IAM/Department/DownloadList',
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


