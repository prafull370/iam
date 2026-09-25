$(document).ready(function () {
    $(".slct2").select2({
        placeholder: 'Select',
        //theme: "bootstrap",
        allowClear: true
    });

    $('.dropdown').on('change', function () {
        $(this).valid(); // Manually trigger validation
    });

    // Show Empty State and hide table 
    $("#imgchooseflt").show();
    $("#dvTableWrapper").hide();

    // Check whether page is Edit 
    var recordId = $("#levelId").val();
    if (recordId !== "" && recordId !== undefined && recordId !== "0") {
        $("#btnSubmit").text("Update");
    }

    // No record at initial
    if ($('tbody tr').length == 0) {
        $('table').append(
            `<tbody>
                <tr class="odd">
                    <td valign="top" colspan="6" class="dataTables_empty">
                        <div style="text-align: center;">
                            <p>No records available at the moment.</p>
                        </div>
                    </td>
                </tr>
            </tbody>
            `
        )
    }

    // Level Validation
    if ($('#levelId').val() != "0") {
        var id;
        if ($('#level1Id').val() !== "0" && $('#level1Id').val() !== "") {
            id = $('#level1Id').val();
            $('#zone_errmsg').text('');
        }
        if ($('#level2Id').val() !== "0" && $('#level2Id').val() !== "") {
            id = $('#level2Id').val();
            $('#circle_errmsg').text('');
        }
        if ($('#level3Id').val() !== "0" && $('#level3Id').val() !== "") {
            id = $('#level3Id').val();
            $('#div_errmsg').text('');
        }
        if ($('#level4Id').val() !== "0" && $('#level4Id').val() !== "") {
            id = $('#level4Id').val();
            $('#sdiv_errmsg').text('');
        }
        if ($('#level5Id').val() !== "0" && $('#level5Id').val() !== "") {
            id = $('#level5Id').val();
            $('#block_errmsg').text('');
        }
        $('#lastSelectedId').val(id);
    }

});

$('#DDLZoneId').on("change", function () {
    var id = $(this).val();
    if (id !== "0" && id !== "") {
        $('#zone_errmsg').text('');
    }
});

$('#DDLCircleId').on("change", function () {
    var id = $(this).val();
    if (id !== "0" && id !== "") {
        $('#circle_errmsg').text('');
    }
});

$('#DDLDivisionId').on("change", function () {
    var id = $(this).val();
    if (id !== "0" && id !== "") {
        $('#div_errmsg').text('');
    }
});

$('#DDLSubDivisionId').on("change", function () {
    var id = $(this).val();
    if (id !== "0" && id !== "") {
        $('#sdiv_errmsg').text('');
    }
});

$('#DDLBlockId').on("change", function () {
    var id = $(this).val();
    if (id !== "0" && id !== "") {
        $('#block_errmsg').text('');
    }
});

//Trigger lvlType
//$('#lvlType').trigger("change");

// Add/Remove Class required Logic
$('#lvlType').on("change", function () {
    $('#ZoneLabel').removeClass('required');
    $('#CircleLabel').removeClass('required');
    $('#DivLabel').removeClass('required');
    $('#SubDivLabel').removeClass('required');
    $('#BlockLabel').removeClass('required');

    let selected = $(this).val();

    if (selected == 'Level2') {
        $('#ZoneLabel').addClass('required');
    }
    else if (selected == 'Level3') {
        $('#ZoneLabel').addClass('required');
        $('#CircleLabel').addClass('required');
    }
    else if (selected == 'Level4') {
        $('#ZoneLabel').addClass('required');
        $('#CircleLabel').addClass('required');
        $('#DivLabel').addClass('required');
    }
    else if (selected == 'Level5') {
        $('#ZoneLabel').addClass('required');
        $('#CircleLabel').addClass('required');
        $('#DivLabel').addClass('required');
        $('#SubDivLabel').addClass('required');
    }
    else if (selected == 'Level6') {
        $('#ZoneLabel').addClass('required');
        $('#CircleLabel').addClass('required');
        $('#DivLabel').addClass('required');
        $('#SubDivLabel').addClass('required');
        $('#BlockLabel').addClass('required');
    }
});

$('#btnSearch').on('click', function () {
    var lvl = $('#DDLLevelType').val(); // Get selected value

    if (!lvl) {
        $('#lvlErrMsg').text('Please select Level Type');
        return;
    } else {
        GetHierarchyList(); // Call function
        $('#lvlErrMsg').text('');
        if ($('tbody tr').length > 0) {
            if ($('tr').has('p').length == 0) {
                $('#sync').removeClass('d-none'); // Show Sync Button
            }
        }
    }
});

$('#DDLLevelType').on('change', function () {
    var lvl = $('#DDLLevelType').val(); // Get selected value

    if (!lvl) {
        $('#lvlErrMsg').text('Please select Level Type');
        return;
    } else {
        $('#lvlErrMsg').text('');
    }
});

// Validation Logic
function LevelValidation() {
    var val = $('#lvlType').val();
    var isValid = true;
    if (val == 'Level2') {
        if ($('#DDLZoneId').val() == '') {
            $('#zone_errmsg').text('Level1 is required').css("font-size", "0.7rem");
            isValid = false;
        }
    }
    else if (val == 'Level3') {
        if ($('#DDLZoneId').val() == '') {
            $('#zone_errmsg').text('Level1 is required').css("font-size", "0.7rem");
            isValid = false;
        }
        if ($('#DDLCircleId').val() == '') {
            $('#circle_errmsg').text('Level2 is required').css("font-size", "0.7rem");
            isValid = false;
        }
    }
    else if (val == 'Level4') {
        if ($('#DDLZoneId').val() == '') {
            $('#zone_errmsg').text('Level1 is required').css("font-size", "0.7rem");
            isValid = false;
        }
        if ($('#DDLCircleId').val() == '') {
            $('#circle_errmsg').text('Level2 is required').css("font-size", "0.7rem");
            isValid = false;
        }
        if ($('#DDLDivisionId').val() == '') {
            $('#div_errmsg').text('Level3 is required').css("font-size", "0.7rem");
            isValid = false;
        }
    }
    else if (val == 'Level5') {
        if ($('#DDLZoneId').val() == '') {
            $('#zone_errmsg').text('Level1 is required').css("font-size", "0.7rem");
            isValid = false;
        }
        if ($('#DDLCircleId').val() == '') {
            $('#circle_errmsg').text('Level2 is required').css("font-size", "0.7rem");
            isValid = false;
        }
        if ($('#DDLDivisionId').val() == '') {
            $('#div_errmsg').text('Level3 is required').css("font-size", "0.7rem");
            isValid = false;
        }
        if ($('#DDLSubDivisionId').val() == '') {
            $('#sdiv_errmsg').text('Level4 is required').css("font-size", "0.7rem");
            isValid = false;
        }
    }
    else if (val == 'Level6') {
        if ($('#DDLZoneId').val() == '') {
            $('#zone_errmsg').text('Level1 is required').css("font-size", "0.7rem");
            isValid = false;
        }
        if ($('#DDLCircleId').val() == '') {
            $('#circle_errmsg').text('Level2 is required').css("font-size", "0.7rem");
            isValid = false;
        }
        if ($('#DDLDivisionId').val() == '') {
            $('#div_errmsg').text('Level3 is required').css("font-size", "0.7rem");
            isValid = false;
        }
        if ($('#DDLSubDivisionId').val() == '') {
            $('#sdiv_errmsg').text('Level4 is required').css("font-size", "0.7rem");
            isValid = false;
        }
        if ($('#DDLBlockId').val() == '') {
            $('#block_errmsg').text('Level5 is required').css("font-size", "0.7rem");
            isValid = false;
        }
    }
    return isValid;
}


//Reset button
$("#btnReset").on("click", function () {
    Reset();
})

function Reset() {
    location.reload();
}

// Get last selected ID
$('#DDLZoneId, #DDLCircleId, #DDLDivisionId, #DDLSubDivisionId').on("change", function () {
    var selectedListId = $(this).val();
    $('#lastSelectedId').val(selectedListId);
})

// Encryption Logic
function encryptParam(value) {
    return btoa(encodeURIComponent(value));
}

// ------------------------------------- List ----------------------------------------------//
function GetHierarchyList() {
    //var search = $("#txtsearch").val();
    var ddlLvl = $('#DDLLevelType').val();
    tblassignfile = $("#tblLevellist").DataTable({
        "ajax": {
            "url": '/IAM/LevelHierarchy/Get',
            "type": "POST",
            "datatype": "json",
            "data": {
                Level_Type: ddlLvl
            },
            "dataSrc": function (json) {
                if (json.data && json.data.length > 0) {

                    $("#imgchooseflt").hide();
                    $("#dvTableWrapper").show();

                } else {

                    $("#imgchooseflt").show();
                    $("#dvTableWrapper").hide();

                }

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
                "data": "Level_Name", "name": "Level_Name", "width": "150px", responsivePriority: 1, orderable: false,
                "render": function (data, type, row) {
                    var description = row.Remarks ? $('<div/>').text(row.Remarks).html() : "No Description";
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
                "data": "Created_On", "name": "Created_On", "width": "80px", responsivePriority: 2, orderable: true
            },//index 2
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
            },
            {
                "data": "Level_Id", "name": "Level_Id", "width": "50px", responsivePriority: 1, orderable: false,
                render: function (data, type, row) {
                    var id = encryptParam(row.Level_Id);
                    var name = encryptParam(ddlLvl);
                    return '<a class="tblBtn ms-3" href="/IAM/LevelHierarchy/Edit?Id=' + id + '&Name=' + name + '" data-toggle="tooltip" aria-label="Edit" data-bs-original-title="Edit"><i class="ugo-icons">edit</i></a>';
                }
            }//index 5
        ],
        drawCallback: function () {
            $('[data-toggle="tooltip"]').tooltip();
            $('#lvlErrMsg').text('');
            if ($('tbody tr').length > 0) {
                if ($('tbody tr').has('p').length === 0) {
                    $('#sync').removeClass('d-none'); // Show Sync Button
                }
                else {
                    $('#sync').addClass('d-none');
                }
            }
        }

    });
}

//---------------------------------------- Save ---------------------------------------------//

function SubmitForm(formId) {
    event.preventDefault(); // Prevent default form submission
    var form = $("#" + formId);
    // Check if the form is valid before proceeding
    var isValid = LevelValidation();
    if (!isValid) {
        e.preventDefault(); // Prevent form submission
        return;
    }
    if (!form.valid()) {
        return; // Stop submission if validation fails
    }
    var submitButton = $("#btnSubmit"); // Target the submit button
    var currentAction = form.attr("action"); // Get current action
    if ($('#lvlType').val() == 'Level1') {
        $("#lastSelectedId").val('0');
    }
    // Check button text and update the action URL accordingly
    // Add Route Accordingly
    if (submitButton.text().trim() === "Update") {
        form.attr("action", "/IAM/LevelHierarchy/Update"); // Set action to Update
    } else {
        form.attr("action", "/IAM/LevelHierarchy/Save"); // Default Save action
    }

    var formData = new FormData(form[0]);
    if (submitButton.text().trim() === "Update") {
        formData.append("Level_Id", $("#levelId").val());
    }
    else {
        formData.append("Level_Id", $("#lastSelectedId").val());
    }
    formData.append("Level1_Id", $("#level1Id").val());
    formData.append("Level2_Id", $("#level2Id").val());
    formData.append("Level3_Id", $("#level3Id").val());
    formData.append("Level4_Id", $("#level4Id").val());
    formData.append("Level5_Id", $("#level5Id").val());

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

            if (response.Status) {
                showAlert(response.Message, "Success", "OkCancel");

                // Optionally reload after 2 seconds
                setTimeout(() => {
                    // Add Route Accordingly
                    window.location.href = "/IAM/LevelHierarchy/Index";
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
