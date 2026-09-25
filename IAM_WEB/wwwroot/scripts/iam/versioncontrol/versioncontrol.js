
$(document).ready(function () {
    var recordId = $("#hdnAppId").val();
    if (recordId !== "" && recordId !== undefined && recordId !== "0") {
        $("#btnSubmit").text("Update");
    }
    //if ($("#btnSubmit").text() === '') {
    //    LoadAppData();
    //}
    if ($("#tblVersionlist").length) {
        LoadVersionData();
    }

    flatpickr("#txtFromDate", {
        dateFormat: "d-M-Y",
        minDate: "today",
        defaultDate: "today",
        clickOpens: true,
        allowInput: true
    });

});

$(document).on('change', '#DDLVersionId', function () {

    var val = $(this).val();
    var productId = $('#DDLProductId').val();

    if (val == "1") {

        $("#DivAPP").removeClass('d-none');

        if (productId) {
            LoadAppDropdown(productId); // ✅ call API
        }

    } else {
        $("#DivAPP").addClass('d-none');
        $('#DDLAppId').val('');
    }

});

function LoadAppDropdown(productId) {

    $.ajax({
        url: '/IAM/VersionControl/GetAppByProduct',
        type: 'GET',
        data: { productId: productId },
        success: function (data) {

            var ddl = $('#DDLAppId');
            ddl.empty();
            ddl.append('<option value="">Select</option>');

            $.each(data, function (i, item) {
                ddl.append('<option value="' + item.Id + '">' + item.Name + '</option>');
            });

        },
        error: function () {
            console.log("App dropdown load failed");
        }
    });
}

//Search on press of Enter Key
$("#txtsearch").on("keypress", function (event) {
    if (event.which === 13) {
        LoadAppData();
    }
});

//Get Data on search button click
$("#btnsearch").on("click", function () {
    LoadAppData();
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
function SubmitForm(formId, event) {
    event.preventDefault(); // Prevent default form submission

    var form = $("#" + formId);
    // Check if the form is valid before proceeding
    if (!form.valid()) {
        return; // Stop submission if validation fails
    }
    var formData = new FormData(form[0]);// Serialize form data
    var submitButton = $("#btnSubmit"); // Target the submit button
    var currentAction = form.attr("action"); // Get current action

    // Check button text and update the action URL accordingly
    // Add Route Accordingly
    if (submitButton.text().trim() === "Update") {
        form.attr("action", "/IAM/VersionControl/Update"); // Set action to Update
    } else {
        form.attr("action", "/IAM/VersionControl/Save"); // Default Save action
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
        //success: function (response) {
        //    $("#dvProcessing").hide();

        //    if (!response.Status && response.ResponseType.toUpperCase() === "VALIDATIONFAILLED") {
        //        // Clear previous errors
        //        $(".text-danger").empty();

        //        // Show new validation errors
        //        $.each(response.ResponseData, function (key, errorMessages) {
        //            var field = $('[name="' + key + '"]');
        //            if (field.length > 0) {
        //                var errorSpan = field.closest("div").find(".text-danger");
        //                if (errorSpan.length > 0) {
        //                    errorSpan.text(errorMessages[0]); // Show first error
        //                }
        //            }
        //        });
        //    } else {
        //        if (response.Status) {
        //            showAlert(response.Message, "Success", "OkCancel");

        //            // Optionally reload after 2 seconds
        //            setTimeout(() => {
        //                // Add Route Accordingly
        //                window.location.href = "/IAM/VersionControl/Index";
        //            }, 2000);
        //        }
        //        else if (!response.Status && response.ResponseType.toUpperCase() === "DUPLICATE") {
        //            showAlert(response.Message, "Warning", "OkCancel");
        //        }
        //        else {
        //            showAlert(response.Message, "Error", "OkCancel");
        //        }
        //    }
        //},
        success: function (response) {
            $("#dvProcessing").hide();

            console.log("Response:", response);

            if (response.Status === true) {
                showAlert(response.Message, "Success", "OkCancel");

                setTimeout(() => {
                    window.location.href = "/IAM/VersionControl/Index";
                }, 1500);
            }
            else if (response.ResponseType && response.ResponseType.toUpperCase() === "DUPLICATE") {
                showAlert(response.Message, "Warning", "OkCancel");
            }
            else if (response.ResponseType && response.ResponseType.toUpperCase() === "VALIDATIONFAILED") {
                showAlert(response.Message, "Warning", "OkCancel");
            }
            else {
                showAlert(response.Message || "Something went wrong", "Error", "OkCancel");
            }
        },
        error: function (xhr, status, error) {
            $("#dvProcessing").hide();
            let errorResponse = JSON.parse(xhr.responseText);
            showAlert(errorResponse.responseData, "Error", "OkCancel");
        }
    });
}

function DeleteVersionDetails(id) {
    if (id !== null && id !== undefined && id !== "") {
        $.ajax({
            url: '/IAM/VersionControl/Delete', // Use updated action URL
            type: "POST",
            data: { "AppId": id },
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
function LoadVersionData() {
    var search = $("#txtsearch").val();
    tblassignfile = $("#tblVersionlist").DataTable({
        "ajax": {
            "url":'/IAM/VersionControl/Get',
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
        "order": [[6, 'desc']],
        "columns": [
            {
                "data": "Version_For", "name": "Version_For", "width": "100px", responsivePriority: 0, orderable: false,

            },//index 0
            
            {
                "data": "Product_Name", "name": "Product_Name", "width": "120px", responsivePriority: 2, orderable: false,

            },//index 1
            {
                "data": "Version", "name": "Version", "width": "120px", responsivePriority: 3, orderable: false,

            },//index 1
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
                "data": "Description", "name": "Description", "width": "120px", responsivePriority: 1, orderable: false,

            },//index 1
            {
                "data": "Created_On", "name": "Created_On", "width": "120px", responsivePriority: 5, orderable: true
            },//index 4
            {
                "data": "Version_Id", "name": "Version_Id", "width": "80px", responsivePriority: 6, orderable: false,
                render: function (data, type, row) {
                    return '<a class="tblBtn"  onclick="GetVersionDetails(' + row.Version_Id + ')" id="btnmodel" data-bs-toggle="modal" data-bs-target="#vlModal"><i class="ugo-icons">grid_view</i></a>'
                        //+ '<a class="tblBtn ms-3" href="/IAM/VersionControl/Edit?Id=' + encodeURIComponent(row.ENC_Page_Id) + '" data-toggle="tooltip" aria-label="Edit" data-bs-original-title="Edit"><i class="ugo-icons">edit</i></a>';
                }
            }//index 5
        ],
        drawCallback: function () {
            $('[data-toggle="tooltip"]').tooltip();
        }

    });
}


function GetVersionDetails(id) {
    if (id !== null && id !== undefined && id !== "") {
        $.ajax({
            url: '/IAM/VersionControl/Details', // Use updated action URL
            type: "GET",
            data: { "Id": id },
            beforeSend: function () {
                $("#tbodyApp").html('');
                $("#dvProcessing").show();
            },
            success: function (response) {
                $("#dvProcessing").hide();
                var html = '';
                $("#tbodyApp").html('');
                if (response.Status) {
                    var data = response.ResponseData;
                    html += '<tr>';
                    html += '<td>' + (data.Version_For ?? '')+'</td>';
                    //html += '<td>' + (data.Application_Code ?? '')+'</td>';
                    html += '<td>' + (data.Product_Name ?? '')+'</td>';
                    html += '<td>' + (data.Version ?? '') + '</td>';
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
                $("#tbodyApp").html(html);
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





