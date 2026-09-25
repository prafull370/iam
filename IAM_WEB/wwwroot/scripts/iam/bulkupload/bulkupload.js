
var baseUrl = ($('base').attr('href').slice(0, -1));
var tblassignfile;
var refInterval; //Interval
var refArr = new Array();
$(document).ready(function () {
    SetSampleFile();
    LoadBulkUploadFiles();

});
function LoadBulkUploadFiles() {
    tblassignfile = $("#tblBulkUploadlist").DataTable({
        "ajax": {
            "url": '/IAM/BulkUpload/Get',
            "type": "POST",
            "datatype": "json",
            "data": {
                Search_Val: ''
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
                "data": "File_Name", "name": "File_Name", "width": "200px", responsivePriority: 2, orderable: false,
                "render": function (data, type, row) {
                    if (!IsNull(row.File_Path)) {
                        var item = row.File_Path.replace('wwwroot', '');
                        return '<a class="text-bold" href="' + baseUrl + item + '" download>' + data + '</a>';
                    } else {
                        return data;
                    }
                }
            },//index 1
            {
                "data": "Bulk_Type", "name": "Bulk_Type", "width": "60px", responsivePriority: 1, orderable: false,

            },//index 0
            {
                "data": "Total_Count", "name": "Total_Count", "width": "30px", responsivePriority: 2, orderable: false,

            },//index 1
            {
                "data": "Success_Count", "name": "Success_Count", "width": "30px", responsivePriority: 2, orderable: false,

            },//index 1
            {
                "data": "Failure_Count", "name": "Failure_Count", "width": "30px", responsivePriority: 2, orderable: false,

            },//index 1
            {
                "data": "Status", "name": "Status", "width": "50px", responsivePriority: 4, orderable: false,
                "render": function (data, type, row) {
                    if (row.Status.toUpperCase() === 'COMPLETED') {
                        return '<span class="badge badge-success">Completed</span>';
                    }
                    else if (row.Status.toUpperCase() === 'INPROGRESS') {
                        return '<span class="badge badge-warning">Inprogress</span>';
                    }
                    else {
                        return '<span class="badge badge-primary">Pending</span>';
                    }
                }
            },//index 3
            {
                "data": "Uploaded_Date", "name": "Uploaded_Date", "width": "150px", responsivePriority: 2, orderable: false
            },//index 4
            {
                "data": "Uploaded_By", "name": "Uploaded_By", "width": "120px", responsivePriority: 2, orderable: false
            },//index 4

            {
                "data": "Id", "name": "Id", "width": "70px", orderable: false, responsivePriority: 1,
                "render": function (data, type, row) {
                    if (row.Status.toUpperCase() === "INPROGRESS") {

                        return '<a href="javascript:void(0);" class="tbtn tbWtng" data-placement="left" data-toggle="tooltip" title="please wait for a while.">';
                    }
                    else if (row.Status.toUpperCase() === "COMPLETED") {
                        return ` <div permission-action="Download"> <a href="javascript:void(0);" class="tbtn tbDwnld btnDownload" data-placement="left" data-toggle="tooltip"title="Download Log Files" data-name="${row.File_Name}"data-type="${row.Bulk_Type}"data-id="${data}"></a></div>`;
                    }
                    else {
                        return '<div permission-action="Import"><a href="javascript:void(0);" class="tbtn tbUpld btnImport" data-placement="left" data-Id=' + data + ' data-toggle="tooltip" title="Import"></div>';
                    }
                }
            }//index 9
        ],
        drawCallback: function () {
            $('[data-toggle="tooltip"]').tooltip();
        },
        initComplete: function (settings, json) {
            refreshtable();
        }

    });
}
function SetSampleFile() {
    let UploadType = $("#ddlUploadType").val();
    if (UploadType === "1") {
        $("#fileUser").removeClass('d-none');
        $("#fileVendor").addClass('d-none');
        $("#fileLevel").addClass('d-none');
    }
    else if (UploadType === "2") {
        $("#fileUser").addClass('d-none');
        $("#fileVendor").removeClass('d-none');
        $("#fileLevel").addClass('d-none');
    }
    else if (UploadType === "3") {
        $("#fileUser").addClass('d-none');
        $("#fileVendor").addClass('d-none');
        $("#fileLevel").removeClass('d-none');
    }
    else {
        $("#fileUser").addClass('d-none');
        $("#fileVendor").addClass('d-none');
        $("#fileLevel").addClass('d-none');
    }

}
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
        form.attr("action", "/IAM/BulkUpload/Update"); // Set action to Update
    } else {
        form.attr("action", "/IAM/BulkUpload/SaveFiles"); // Default Save action
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
            else if (!response.Status && response.ResponseType.toUpperCase() === "FORMATERROR") {
                Swal.fire(
                    "Invalid Excel Format",
                    response.Message +
                    "<br><b>Missing Columns:</b> " + response.MissingColumns.join(", ") +
                    "<br><b>Extra Columns:</b> " + response.ExtraColumns.join(", "),
                    "error"
                );
            }
            else {
                if (response.Status) {
                    showAlert(response.Message, "Success", "OkCancel");

                    // Optionally reload after 2 seconds
                    setTimeout(() => {
                        // Add Route Accordingly
                        window.location.href = "/IAM/BulkUpload/Index";
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
function refreshtable() {

    // If interval already running, do nothing
    if (refInterval) return;

    if (refArr.length > 0) {

        refInterval = setInterval(function () {

            tblassignfile.ajax.reload(null, false);

            $(".tooltip").tooltip("hide");

            stopRefreshIfCompleted(); // 👈 important

        }, 6000);

    }
}
function stopRefreshIfCompleted() {

    let activeCount = 0;

    tblassignfile.rows().every(function () {
        let data = this.data();

        if (refArr.includes(data.Id) &&
            data.Status.toUpperCase() !== "COMPLETED") {
            activeCount++;
        }
    });

    if (activeCount === 0) {
        clearInterval(refInterval);
        refInterval = null;
        refArr = [];
    }
}


$('#tblBulkUploadlist').on('click', '.btnImport', function () {

    var id = $(this).attr('data-Id');


    Swal.fire({
        title: 'Are you sure?',
        text: 'You will start import data!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, do it!',
        cancelButtonText: 'No, cancel!',
        showLoaderOnConfirm: true,
        allowOutsideClick: () => !Swal.isLoading(),
        preConfirm: () => {

            return $.ajax({
                url: baseUrl + '/IAM/BulkUpload/ImportFile/' + id,
                type: 'POST'
            }).then(response => {
                if (!response.Status) {
                    throw new Error(response.Message);
                }
                return response;
            }).catch(error => {
                Swal.showValidationMessage(error);
            });
        }
    }).then((result) => {

        if (result.isConfirmed) {

            let data = result.value;

            tblassignfile.ajax.reload();
            refArr.push(parseInt(id));
            refreshtable();

            Swal.fire(data.Title, data.Message, 'success');
        }
    });
});
$('#tblBulkUploadlist').on('click', '.btnDownload', function () {

    var _val = $(this).attr('data-Id');
    var type = $(this).attr('data-type');
    Swal.fire({
        title: 'Downloading...',
        text: 'Please wait while we are processing your request.',
        imageUrl: baseUrl + '/images/loader.svg',
        showConfirmButton: false,
        allowOutsideClick: false
    });

    var xhr = new XMLHttpRequest();
    xhr.open("POST", baseUrl + "/IAM/BulkUpload/DownloadLogs", true);
    xhr.responseType = "blob";

    xhr.onload = function () {
        if (this.status === 200) {

            var filename = type + "_Bulk_Upload_Log.xlsx";
            var blob = new Blob([xhr.response], {
                type: xhr.getResponseHeader("Content-Type")
            });

            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = filename;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }

        Swal.close();
    };

    xhr.onerror = function () {
        Swal.close();
        Swal.fire("Error", "Download failed.", "error");
    };

    var formData = new FormData();
    formData.append("id", _val);
    xhr.send(formData);
});

Array.prototype.remove = function () {
    var what, a = arguments, L = a.length, ax;
    while (L && this.length) {
        what = a[--L];
        while ((ax = this.indexOf(what)) !== -1) {
            this.splice(ax, 1);
        }
    }
    return this;
};