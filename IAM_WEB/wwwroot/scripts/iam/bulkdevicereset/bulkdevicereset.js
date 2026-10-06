var baseUrl = ($('base').attr('href').slice(0, -1));
var tblBulkDeviceReset;
var refInterval;
var refArr = new Array();

$(document).ready(function () {
    LoadBulkDeviceReset();

    $('#ExcelFile').on('change', function () {
        $('#fileName').text(this.files.length > 0 ? this.files[0].name : 'No file chosen');
    });
});

// ---------------- UPLOAD (ajax, controller returns Json) ----------------
$('#BulkDeviceResetForm').on('submit', function (e) {
    e.preventDefault();

    var remarks = $('#Remarks').val().trim();
    var file = $('#ExcelFile')[0].files[0];
    if (!remarks) return Swal.fire('Required', 'Please enter remarks.', 'warning');
    if (!file) return Swal.fire('Required', 'Please select an Excel file.', 'warning');

    var $btn = $('#btnUpload').prop('disabled', true);
    $.ajax({
        url: baseUrl + '/IAM/BulkDeviceReset/SaveFiles',
        type: 'POST',
        data: new FormData(this),
        processData: false,
        contentType: false,
        success: function (res) {
            Swal.fire(res.Status ? 'Done' : 'Error', res.Message, res.Status ? 'success' : 'error');
            if (res.Status) {
                $('#ExcelFile').val('');
                $('#fileName').text('No file chosen');
                $('#Remarks').val('');
                tblBulkDeviceReset.ajax.reload();
            }
        },
        error: function () { Swal.fire('Error', 'Upload failed.', 'error'); },
        complete: function () { $btn.prop('disabled', false); }
    });
});

// ---------------- GRID ----------------
function LoadBulkDeviceReset() {
    tblBulkDeviceReset = $("#tblBulkDeviceReset").DataTable({
        "ajax": {
            "url": baseUrl + '/IAM/BulkDeviceReset/Get',
            "type": "POST",
            "datatype": "json",
            "data": { Search_Val: '' },
            "dataSrc": function (json) { return json.data || []; },
            error: function (xhr) {
                $("#dvProcessing").hide();
                let errorResponse = JSON.parse(xhr.responseText);
                showAlert(errorResponse.responseData, "Error", "OkCancel");
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
        "order": [[8, 'desc']],
        "columns": [
            { "data": "Id", "name": "Id", "width": "60px", responsivePriority: 1, orderable: false },
            { "data": "File_Name", "name": "File_Name", "width": "60px", responsivePriority: 1, orderable: false },
            { "data": "Total_Count", "name": "Total_Count", "width": "60px", responsivePriority: 1, orderable: false },
            { "data": "Success_Count", "name": "Success_Count", "width": "60px", responsivePriority: 1, orderable: false },
            { "data": "Failure_Count", "name": "Failure_Count", "width": "60px", responsivePriority: 1, orderable: false },
            {
                "data": "Remarks", "name": "Remarks", orderable: false,
                "render": function (data) {
                    if (!data) return "";
                    var text = data.length > 30 ? data.substring(0, 10) + "..." : data;
                    return '<span title="' + data + '">' + text + '</span>';
                }
            },
            {
                "data": "Status", "name": "Status", "width": "50px", responsivePriority: 4, orderable: false,
                "render": function (data, type, row) {
                    var s = (row.Status || '').toUpperCase();
                    if (s === 'COMPLETED') return '<span class="badge badge-success">COMPLETED</span>';
                    if (s === 'INPROGRESS') return '<span class="badge badge-warning">INPROGRESS</span>';
                    if (s === 'PARTIAL') return '<span class="badge badge-warning">PARTIAL</span>';
                    if (s === 'FAILED') return '<span class="badge badge-danger">FAILED</span>';
                    return '<span class="badge badge-info">PENDING</span>';
                }
            },
            { "data": "Uploaded_By", "name": "Uploaded_By", "width": "60px", responsivePriority: 1, orderable: false },
            { "data": "Uploaded_Date", "name": "Uploaded_Date", "width": "60px", responsivePriority: 1, orderable: false },
            {
                "data": "Id", "name": "Id", "width": "70px", orderable: false, responsivePriority: 1,
                "render": function (data, type, row) {
                    var status = (row.Status || '').toUpperCase();
                    if (status === "PENDING") {
                        return '<a href="javascript:void(0);" class="tbtn tbUpld btnImport" data-placement="left" data-Id="' + data + '" data-toggle="tooltip" title="Start Reset"></a>';
                    }
                    if (status === "INPROGRESS") {
                        return '<a href="javascript:void(0);" class="tbtn tbWtng" data-placement="left" data-toggle="tooltip" title="Please wait for a while."></a>';
                    }
                    if (status === "COMPLETED" || status === "FAILED" || status === "PARTIAL") {
                        return '<a href="javascript:void(0);" class="tbtn tbDwnld btnDownload" data-placement="left" data-toggle="tooltip" title="Download Log files" data-Id="' + data + '"></a>';
                    }
                    return "";
                }
            }
        ],
        drawCallback: function () { $('[data-toggle="tooltip"]').tooltip(); }
    });
}

function refreshtable() {
    if (refInterval) return;
    if (refArr.length > 0) {
        refInterval = setInterval(function () {
            tblBulkDeviceReset.ajax.reload(null, false);
            $(".tooltip").tooltip("hide");
            stopRefreshIfDone();
        }, 6000);
    }
}

function stopRefreshIfDone() {
    let active = 0;
    tblBulkDeviceReset.rows().every(function () {
        let d = this.data();
        var s = (d.Status || '').toUpperCase();
        if (refArr.includes(d.Id) && s !== "COMPLETED" && s !== "PARTIAL" && s !== "FAILED") active++;
    });
    if (active === 0) {
        clearInterval(refInterval);
        refInterval = null;
        refArr = [];
    }
}

// ---------------- START PROCESSING ----------------
$('#tblBulkDeviceReset').on('click', '.btnImport', function () {
    var id = $(this).attr('data-Id');

    Swal.fire({
        title: 'Are you sure?',
        text: 'Devices in this file will be reset!',
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
                url: baseUrl + '/IAM/BulkDeviceReset/ImportFile/' + id,
                type: 'POST'
            }).then(response => {
                if (!response.Status) throw new Error(response.Message);
                return response;
            }).catch(error => { Swal.showValidationMessage(error); });
        }
    }).then((result) => {
        if (result.isConfirmed) {
            let data = result.value;
            tblBulkDeviceReset.ajax.reload();
            refArr.push(parseInt(id));
            refreshtable();
            Swal.fire(data.Title || 'Started', data.Message, 'success');
        }
    });
});

// ---------------- DOWNLOAD LOG ----------------
$('#tblBulkDeviceReset').on('click', '.btnDownload', function () {
    var _val = $(this).attr('data-Id');
    Swal.fire({
        title: 'Downloading...',
        text: 'Please wait while we are processing your request.',
        imageUrl: baseUrl + '/images/loader.svg',
        showConfirmButton: false,
        allowOutsideClick: false
    });

    var xhr = new XMLHttpRequest();
    xhr.open("POST", baseUrl + "/IAM/BulkDeviceReset/DownloadLogs", true);
    xhr.responseType = "blob";

    xhr.onload = function () {
        if (this.status === 200) {
            var blob = new Blob([xhr.response], { type: xhr.getResponseHeader("Content-Type") });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "Bulk_Device_Reset_Log.xlsx";
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
