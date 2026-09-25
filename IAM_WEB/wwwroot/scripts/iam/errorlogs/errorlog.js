let tblErrorLog;

$(document).ready(function () {
    $('.dropdown').on('change', function () {
        $(this).valid();
    });

    flatpickr("#txtFromDate", {
        dateFormat: "Y-m-d",
        allowInput: true
    });

    flatpickr("#txtEndDate", {
        dateFormat: "Y-m-d",
        allowInput: true
    });

    LoadErrorLogData();

    $("#txtsearch").on("keyup", function (e) {
        if (e.key === "Enter") {
            LoadErrorLogData();
        }
    });

    $("#btnsearch").on("click", function () {
        LoadErrorLogData();
    });

    $("#btnreset").on("click", function () {
        $("#txtsearch").val("");
        $("#txtFromDate").val("");
        $("#txtEndDate").val("");
        LoadErrorLogData();
    });
});

function LoadErrorLogData() {
    let search = $("#txtsearch").val();
    let fromDate = $("#txtFromDate").val();
    let endDate = $("#txtEndDate").val();

    if ($.fn.DataTable.isDataTable("#tblErrorDatalist")) {
        $("#tblErrorDatalist").DataTable().destroy();
    }

    tblErrorLog = $("#tblErrorDatalist").DataTable({
        ajax: {
            url: '/IAM/ErrorLog/Get',
            type: 'POST',
            datatype: 'json',
            data: function (d) {
                d.Search_Val = search;
                d.FromDate = fromDate;
                d.EndDate = endDate;
            },
            dataSrc: function (json) {
                return json.data || [];
            },
            error: function (xhr) {
                $("#dvProcessing").hide();
                let errorResponse = xhr.responseJSON;
                showAlert(errorResponse?.responseData || "Something went wrong", "Error", "OkCancel");
            }
        },

        dom: pagingCSS,
        pagingType: pagingType,
        pageLength: pageLength,
        destroy: true,
        autoWidth: false,
        processing: true,
        serverSide: true,
        filter: false,
        responsive: true,
        language: language,
        order: [[4, 'desc']],

        columns: [
            {
                data: "ErrorMessage",
                name: "ErrorMessage",
                width: "250px",
                orderable: false
            },
            {
                data: "ErrorSource",
                name: "ErrorSource",
                width: "120px",
                orderable: false
            },
            {
                data: "ErrorAction",
                name: "ErrorAction",
                width: "150px",
                orderable: false
            },
            {
                data: "ErrorType",
                name: "ErrorType",
                width: "120px",
                orderable: false
            },
            {
                data: "ErrorDate",
                name: "ErrorDate",
                width: "150px",
                orderable: false
            }
        ],

        drawCallback: function () {
            $('[data-toggle="tooltip"]').tooltip();
        }
    });
}