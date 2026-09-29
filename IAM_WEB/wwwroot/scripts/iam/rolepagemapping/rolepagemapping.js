$(document).ready(function () {
    // Show Empty State
    $("#imgchooseflt").show();
    $("#btn").addClass("d-none");

    //Add Bootstrap SelectList class
    $(".slct2").select2({
        placeholder: 'Select',
        //theme: "bootstrap",
        allowClear: false
    });

    $('.dropdown').on('change', function () {
        $(this).valid(); // Manually trigger validation
    });

    // on dropdown change
    $('#DDLRoleId').on("change", function () {
        var selectedProduct = $(this).val();
        if (selectedProduct && selectedProduct != "") {
            BindList(selectedProduct);
        }
        else {
            $("#dvProcessing").show();
            $('#btn').addClass('d-none');
            $("#dvProcessing").hide();
        }

    });
});

//To Reload the page
$("#btnReset").on("click", function () {
    Reset();
})

function Reset() {
    location.reload();
}

// Check Uncheck Logic (any page level, L1 - L5)
// Activating a page activates all its sub pages and its parent pages (a sub page is only
// reachable from the menu when its parents are mapped). Deactivating a page deactivates
// all its sub pages and clears their landing page selection.
$(document).on("change", "#rolePagePartial .page-checkbox", function () {
    var $row = $(this).closest("tr");
    var isChecked = $(this).is(":checked");
    var $descendants = getDescendantRows($row);

    $descendants.find(".page-checkbox").prop("checked", isChecked);

    if (isChecked) {
        getAncestorPageRows($row).find(".page-checkbox").prop("checked", true);
    }
    else {
        $row.add($descendants).find("input[type='radio']").prop("checked", false);
    }

    refreshCounts();
});


// Expand-Collapse Logic
// Toggle parent click
$(document).on("click", ".toggle-parent", function () {
    const $row = $(this).closest('tr');
    const level = getRowLevel($row);
    const isExpanded = $row.hasClass('expanded');

    setExpanded($row, !isExpanded);
    toggleChildren($row, level, !isExpanded);
});

// Keyboard support for expand / collapse
$(document).on("keydown", ".toggle-parent", function (e) {
    if (e.key === "Enter" || e.key === " ") {
        e.preventDefault();
        $(this).trigger("click");
    }
});

$(document).on("click", "#btnExpandAll", function () {
    clearSearch();
    var $rows = getTreeRows();
    $rows.removeClass('d-none');
    setExpanded($rows.filter(function () { return $(this).children('.toggle-parent').length > 0; }), true);
});

$(document).on("click", "#btnCollapseAll", function () {
    clearSearch();
    collapseAll();
});

// Search: show matching rows with their full parent path, hide everything else
var treeSearchTimer;
$(document).on("input", "#txtTreeSearch", function () {
    var term = $(this).val();
    clearTimeout(treeSearchTimer);
    treeSearchTimer = setTimeout(function () { filterTree(term); }, 200);
});

// Utility functions
function getRowLevel($tr) {
    const className = $tr.attr('class') || '';
    const match = className.match(/level-(\d+)/);
    return match ? parseInt(match[1]) : -1;
}

function getTreeRows() {
    return $("#rolePagePartial tbody tr").not(".rpm-no-match");
}

function setExpanded($rows, expanded) {
    $rows.toggleClass('expanded', expanded);
    $rows.children('.toggle-parent').attr('aria-expanded', expanded ? 'true' : 'false');
}

function toggleChildren($row, parentLevel, show) {
    let next = $row.next();

    while (next.length) {
        const nextLevel = getRowLevel(next);
        if (nextLevel <= parentLevel) break;

        if (show && nextLevel === parentLevel + 1) {
            next.removeClass('d-none');
        } else {
            next.addClass('d-none');
            setExpanded(next, false);
        }

        if (!show) {
            toggleChildren(next, getRowLevel(next), false);
        }

        next = next.next();
    }
}

function collapseAll() {
    var $rows = getTreeRows();
    setExpanded($rows, false);
    $rows.not(".level-0").addClass('d-none');
    $rows.filter(".level-0").removeClass('d-none');
}

// All rows nested under $row (rows are rendered depth-first)
function getDescendantRows($row) {
    var level = getRowLevel($row);
    var rows = [];
    var next = $row.next();
    while (next.length && getRowLevel(next) > level) {
        rows.push(next[0]);
        next = next.next();
    }
    return $(rows);
}

// Parent pages of a page row, nearest first
function getAncestorPageRows($row) {
    var rows = [];
    var parentKey = $row.data("parent-node");
    while (parentKey) {
        var $parent = $("#rolePagePartial tr[data-node='" + parentKey + "']");
        if (!$parent.length) break;
        rows.push($parent[0]);
        parentKey = $parent.data("parent-node");
    }
    return $(rows);
}

// Active / total page counts on every collapsible row and in the toolbar
function refreshCounts() {
    var stack = [];
    var totalPages = 0;
    var activePages = 0;

    function close(entry) {
        entry.$count.text(entry.active + "/" + entry.total)
            .toggleClass("all-active", entry.total > 0 && entry.active === entry.total)
            .toggleClass("none-active", entry.active === 0);
    }

    getTreeRows().each(function () {
        var $tr = $(this);
        var level = getRowLevel($tr);

        while (stack.length && stack[stack.length - 1].level >= level) {
            close(stack.pop());
        }

        if ($tr.hasClass("page")) {
            var isActive = $tr.find(".page-checkbox").is(":checked");
            totalPages++;
            if (isActive) activePages++;
            stack.forEach(function (entry) {
                entry.total++;
                if (isActive) entry.active++;
            });
        }

        var $count = $tr.find(".node-count");
        if ($count.length) {
            stack.push({ level: level, $count: $count, total: 0, active: 0 });
        }
    });

    while (stack.length) {
        close(stack.pop());
    }

    $("#rpmSummary").text(totalPages > 0 ? activePages + " of " + totalPages + " pages active" : "");
}

function filterTree(term) {
    term = $.trim(term || "").toLowerCase();
    var $rows = getTreeRows();
    var $noMatch = $("#rolePagePartial .rpm-no-match");

    $rows.removeClass("search-match");

    if (!term) {
        $noMatch.addClass("d-none");
        collapseAll();
        return;
    }

    var stack = [];
    var matches = 0;

    $rows.each(function () {
        var $tr = $(this);
        var level = getRowLevel($tr);

        while (stack.length && getRowLevel(stack[stack.length - 1]) >= level) {
            stack.pop();
        }

        $tr.addClass("d-none");
        setExpanded($tr, false);

        if ($tr.find(".tree-label").text().toLowerCase().indexOf(term) > -1) {
            matches++;
            $tr.addClass("search-match").removeClass("d-none");
            stack.forEach(function ($ancestor) {
                $ancestor.removeClass("d-none");
                setExpanded($ancestor, true);
            });
        }

        stack.push($tr);
    });

    $noMatch.toggleClass("d-none", matches > 0);
}

function clearSearch() {
    $("#txtTreeSearch").val("");
    getTreeRows().removeClass("search-match");
    $("#rolePagePartial .rpm-no-match").addClass("d-none");
}

// Check Validation
function Validation() {
    var checkboxSelectedCount = $('tr.page input[type="checkbox"]:checked').length;
    var radioSelectedCount = $('tr.page input[type="radio"]:checked').length;
    if (checkboxSelectedCount == 0) {
        showAlert("Please select at least one page", "Warning", "OkCancel");
        return false;
    }
    else if (radioSelectedCount == 0) {
        showAlert("Please select at least one landing page", "Warning", "OkCancel");
        return false;
    }
    else {
        return true;

    }
}

// Make page active first before marking a landing page
$(document).on("change", "input[type='radio']", function () {
    var $radio = $(this);
    var $checkbox = $radio.closest("tr").find("input[type='checkbox']");

    // If the corresponding checkbox is not checked
    if (!$checkbox.is(":checked")) {
        // Prevent radio selection
        $radio.prop("checked", false);
        showAlert("Please select the checkbox before selecting Landing Page","Warning");
    }
});


// Bind Role Page Mapping
function BindList(Role_Id) {
    $.ajax({
        url: "/IAM/RolePageMapping/Get",
        type: "POST",
        data: {
            Role_Id: Role_Id
        },
        beforeSend: function () {
            $('#rolePagePartial').html('');
            $('#btn').addClass('d-none');
            $("#dvProcessing").show();
        },
        success: function (data) {
            $("#dvProcessing").hide();
            var response = data.trim();
            if (data.trim().startsWith('<')) {
                $("#imgchooseflt").hide();
                $('#btn').removeClass('d-none');
                $('#rolePagePartial').html(data);
                $("#txtTreeSearch").val("");
                refreshCounts();
            }
            else {
                $("#imgchooseflt").show();
                $('#rolePagePartial').empty();
                $('#btn').addClass('d-none');
            }
        },
        error: function (xhr, status, error) {
            $("#dvProcessing").hide();
            let errorResponse = JSON.parse(xhr.responseText);
            showAlert(errorResponse.responseData, "Error", "OkCancel");
        }
    });
}

function SelectedArrayList() {
    var selectedList = [];

    $("input[type='checkbox']:checked").each(function () {
        var $checkbox = $(this);

        var productId = parseInt($checkbox.data("product-id")) || 0;
        var serviceId = parseInt($checkbox.data("service-id")) || 0;
        var moduleId = parseInt($checkbox.data("module-id")) || 0;
        var pageId = parseInt($checkbox.data("page-id")) || 0;
        var isParent = $checkbox.data("is-parent") === true || $checkbox.data("is-parent") === "true";

        // Find if corresponding radio button is checked
        var isLandingPage = $("input[type='radio'][data-page-id='" + pageId + "']").is(":checked");

        // Push to array only if Page_Id is present (ensures only actual page rows are considered)
        if (pageId > 0) {
            selectedList.push({
                Product_Id: productId,
                Service_Id: serviceId,
                Module_Id: moduleId,
                Page_Id: pageId,
                Is_Landing_Page: isLandingPage,
                Is_Parent: isParent
            });
        }
    });

    return selectedList;
}


// Save Form
function SubmitForm(formId) {
    event.preventDefault(); // Prevent default form submission

    // Check Validation
    if (!Validation()) {
        return;
    }; 

    var form = $("#" + formId);
    // Check if the form is valid before proceeding
    if (!form.valid()) {
        return; // Stop submission if validation fails
    }
    var RoleList = SelectedArrayList();

    var model = {
        Role_Id: $('#DDLRoleId').val(),
        RolePage_Data: RoleList
    };

    $.ajax({
        url: '/IAM/RolePageMapping/Save', // Use updated action URL
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(model),
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
                        window.location.href = "/IAM/RolePageMapping/Index";
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
            //let errorResponse = JSON.parse(xhr.responseText);
            //showAlert(errorResponse.responseData, "Error", "OkCancel");
        }
    });
}






