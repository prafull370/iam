

$(document).ready(function () {
    $('.dropdown').on('change', function () {
        $(this).valid();
    });

    $("#imgchooseflt").show();
    $("#tblSyncDatalist").addClass("d-none");
});
function LoadSyncData() {
    var product = $("#DDLProductId").val();
    var type = $("#DdlSyncType").val();
    var status = $("#DdlSatus").val();

    // ✅ VALIDATION
    if (!type || type === "" || type === "0") {
        showAlert("Please select type", "Warning", "OkCancel");
        return;
    }

    if (!product || product === "" || product === "0") {
        showAlert("Please select product", "Warning", "OkCancel");
        return;
    }

    $("#imgchooseflt").hide();
    $("#tblSyncDatalist").removeClass("d-none");


    tblassignfile = $("#tblSyncDatalist").DataTable({
        "ajax": {
            "url": '/IAM/DataSync/Get',
            "type": "POST",
            "datatype": "json",
            "data": {
                Product_Id: product,
                Sync_Type_Id: type,
                Status: status
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
                "data": "Record_Name", "name": "Record_Name", "width": "100px", responsivePriority: 1, orderable: false
            },//index 0
            {
                "data": "Sync_Status", "name": "Sync_Status", "width": "50px", responsivePriority: 1, orderable: false,
                "render": function (data, type, row) {
                    if (row.Sync_Status === true) {
                        return '<span class="badge badge-success">Synced</span>';
                    }
                    else if (row.Sync_Status === false) {
                        return '<span class="badge badge-danger">Not Synced</span>';
                    }
                    else {
                        return '<span class="badge badge-info"> Not Found </span>';
                    }
                }
            },//index 3
            {
                "data": "Synced_On", "name": "Synced_On", "width": "80px", responsivePriority: 2, orderable: false
            },//index 2
            {
                "data": "Message", "name": "Message", "width": "100px", responsivePriority: 1, orderable: false
            }
        ],
        drawCallback: function () {
            $('[data-toggle="tooltip"]').tooltip();
        }

    });
}
//})

function SyncData() {
        var product = $("#DDLProductId").val();
        var type = $("#DdlSyncType").val();
        var status = $("#DdlSatus").val();
        $.ajax({
            "url": '/IAM/DataSync/SyncData',
            "type": "POST",
            "datatype": "json",
            "data": {
                Product_Id: product,
                Sync_Type_Id: type,
                Status: false
            },
            beforeSend: function () {
                $("#dvProcessing").hide();
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
                        LoadSyncData();
                    }
                    else {
                        showAlert(response.Message, "Error", "OkCancel");
                    }
                }
            },
            error: function (xhr) {
                $("#dvProcessing").hide();
                let errorResponse = JSON.parse(xhr.responseText);
                showAlert(errorResponse.responseData, "Error", "OkCancel")
            }
        });
}
