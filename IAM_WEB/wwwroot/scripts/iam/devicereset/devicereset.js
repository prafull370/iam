let table;

$(document).ready(function () {

    $("#imgchooseflt").hide();
    $("#dvTableWrapper").show();

    table = $("#tblDepartmentlist").DataTable({
        "ajax": {
            "url": '/IAM/DeviceReset/Get',
            "type": "POST",
            "datatype": "json",
            beforeSend: function () {
                //$("#dvProcessing").show();
                $("#imgchooseflt").hide();
                $("#dvTableWrapper").show();
            },
            "data": function (d) {
                d.Search_Val = $("#txtsearch").val();
            },
            "dataSrc": function (json) {
                return json.data || [];
            },
            "error": function (xhr) {
                let errorResponse = JSON.parse(xhr.responseText || "{}");
                showAlert(errorResponse.responseData || "Something went wrong", "Error", "OkCancel");
            }
        },
        dom: pagingCSS,
        "pagingType": pagingType,
        "pageLength": pageLength,
        "bAutoWidth": bAutoWidth,
        "processing": processing,
        "serverSide": serverSide,
        responsive: responsive,
        language: language,
        "order": [[1, 'desc']],
        "destroy": true,

        "columns": [
            { "data": "User_Name" },
            { "data": "Login_Id" },
            { "data": "Device_Id" },
            { "data": "Mobile_No" },
            { "data": "Department_Name" },
            { "data": "Designation_Name" },

            // Action button
            {
                "data": "Login_Id",
                "render": function (data, type, row) {

                    if (!row.Login_Id) {
                        console.error("Login_Id missing:", row);
                        return "";
                    }

                    return `<button class="btn btn-primary  btn-sm btn-reset" data-id="${row.Login_Id}">
                                Reset
                            </button>`;
                }
            }
        ],
        drawCallback: function () {
            $("#dvProcessing").hide();
            var api = this.api();
            if (api.rows().count() == 0) {
                $("#dvTableWrapper").hide();
                $("#imgchooseflt").removeClass("d-none").show();

            }
            else {
                $("#imgchooseflt").hide();
                $("#dvTableWrapper").show();
            }
        }
    });

});


// 🔍 ENTER KEY SEARCH
$("#txtsearch").on("keypress", function (e) {
    if (e.which === 13) {
        e.preventDefault();
        table.ajax.reload();
    }
});

// 🔍 BUTTON SEARCH
$("#btnsearch").on("click", function () {
    table.ajax.reload();
});

// 🔄 RESET BUTTON
$("#btnreset").on("click", function () {
    $("#txtsearch").val('');
    table.ajax.reload();
});


// CLICK EVENT (BEST PRACTICE)
$(document).on("click", ".btn-reset", function () {

    let Login_Id = $(this).data("id");

    if (!Login_Id) {
        showAlert("Invalid Login Id", "Error", "OkCancel");
        return;
    }

    // Confirmation popup
    Swal.fire({
        title: "Are you sure?",
        text: "Do you want to reset this device?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, Reset",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (result.isConfirmed) {
            ResetDevice(Login_Id);
        }
    });
});


// 🔁 RESET DEVICE FUNCTION
function ResetDevice(Login_Id) {

    console.log("Login Id sent:", Login_Id);

    $.ajax({
        url: '/IAM/DeviceReset/UpdateDeviceId',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            Login_Id: Login_Id,
            Updated_By: 1,
            Ip_Address: '127.0.0.1'
        }),
        beforeSend: function () {
            $("#dvProcessing").show();
        },
        success: function (res) {
            $("#dvProcessing").hide();

            if (res.Status) {
                showAlert(res.Message || "Device Reset Successfully", "Success", "OkCancel");

                setTimeout(() => {
                    table.ajax.reload();
                }, 1000);
            }
            else {
                showAlert(res.Message, "Error", "OkCancel");
            }
        },
        error: function (xhr) {
            $("#dvProcessing").hide();

            let errorResponse = JSON.parse(xhr.responseText || "{}");
            showAlert(errorResponse.responseData || "Something went wrong", "Error", "OkCancel");
        }
    });
}