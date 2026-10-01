let tblAppAuditLog;
let appAuditDetailDrawer;
let detailReqId = 0;

$(document).ready(function () {

    flatpickr("#txtFromDate", { dateFormat: "Y-m-d", allowInput: true });
    flatpickr("#txtToDate", { dateFormat: "Y-m-d", allowInput: true });

    // Default date range = last 7 days
    SetDefaultDateRange();

    appAuditDetailDrawer = new bootstrap.Offcanvas(document.getElementById('appAuditDetailDrawer'));

    LoadAppAuditData();
    LoadStats();

    $("#txtsearch").on("keypress", function (e) {
        if (e.which === 13) {
            LoadAppAuditData();
        }
    });

    $("#btnsearch").on("click", function () {
        LoadAppAuditData();
        LoadStats();
    });

    $("#btnreset").on("click", function () {
        Reset();
    });

    // View icon -> open drawer (user info from the grid row, activity via one call)
    $("#tblAppAuditLog tbody").on("click", ".btn-view-audit", function (e) {
        e.preventDefault();
        if (!tblAppAuditLog) return;
        var data = tblAppAuditLog.row($(this).closest('tr')).data();
        if (!data) return;
        ShowAppAuditDetail(data);
    });
});

function Reset() {
    location.reload();
}

function SetDefaultDateRange() {

    var today = new Date();

    var sevenDaysAgo = new Date();
    sevenDaysAgo.setDate(today.getDate() - 7);

    function formatDate(date) {
        var year = date.getFullYear();
        var month = String(date.getMonth() + 1).padStart(2, '0');
        var day = String(date.getDate()).padStart(2, '0');

        return year + '-' + month + '-' + day;
    }

    $("#txtFromDate").val(formatDate(sevenDaysAgo));
    $("#txtToDate").val(formatDate(today));
}

function LoadAppAuditData() {
    var search = $("#txtsearch").val();
    var fromDate = $("#txtFromDate").val();
    var toDate = $("#txtToDate").val();

    if ($.fn.DataTable.isDataTable("#tblAppAuditLog")) {
        $("#tblAppAuditLog").DataTable().destroy();
    }

    tblAppAuditLog = $("#tblAppAuditLog").DataTable({
        ajax: {
            url: '/IAM/AppAuditLog/Get',
            type: 'POST',
            datatype: 'json',
            data: function (d) {
                d.Search_Val = search;
                d.From_Date = fromDate;
                d.To_Date = toDate;
            },
            dataSrc: function (json) {
                return json.data || [];
            },
            error: function (xhr) {
                $("#dvProcessing").hide();
                let errorResponse = {};
                try { errorResponse = JSON.parse(xhr.responseText); } catch (e) { }
                showAlert(errorResponse.responseData || errorResponse.message || "Something went wrong", "Error", "OkCancel");
            }
        },
        dom: pagingCSS,
        pagingType: pagingType,
        pageLength: pageLength,
        bDestroy: true,
        bAutoWidth: bAutoWidth,
        processing: processing,
        serverSide: serverSide,
        filter: filter,
        responsive: responsive,
        language: language,
        order: [[5, 'desc']],
        columns: [
            {
                data: "User_Name", name: "user_name", responsivePriority: 1, orderable: false,
                render: function (data, type, row) {
                    var userName = data ? data : (row.Login_Id || 'Unknown');
                    var loginId = row.Login_Id ? '<div class="small text-muted">' + escapeHtml(row.Login_Id) + '</div>' : '';
                    return '<div class="fw-semibold">' + escapeHtml(userName) + '</div>' + loginId;
                }
            },//index 0
            {
                data: "Mobile_No", name: "mobile_no", responsivePriority: 3, orderable: false,
                render: function (data) { return escapeHtml(data || '-'); }
            },//index 1
            {
                data: "Device_Name", name: "device", responsivePriority: 2, orderable: false,
                render: function (data, type, row) { return escapeHtml(JoinParts([row.Brand_Name, row.Device_Name], ' ')); }
            },//index 2
            {
                data: "Mobile_Android_Version", name: "mobile_android_version", responsivePriority: 4, orderable: false,
                render: function (data) { return escapeHtml(data || '-'); }
            },//index 3
            {
                data: "App_Version", name: "app_version", responsivePriority: 4, orderable: false,
                render: function (data) { return escapeHtml(data || '-'); }
            },//index 4
            {
                data: "Last_Activity", name: "last_activity", responsivePriority: 2, orderable: true,
                render: function (data) { return escapeHtml(data || '-'); }
            },//index 5
            {
                data: "Total_Actions", name: "total_actions", responsivePriority: 3, orderable: true,
                render: function (data) {
                    return '<span class="badge rounded-pill bg-primary">' + escapeHtml(data != null ? data : 0) + '</span>';
                }
            },//index 6
            {
                data: "Login_Id", name: "login_id", width: "70px", responsivePriority: 1, orderable: false,
                render: function () {
                    return '<a class="tblBtn ms-3 btn-view-audit" href="javascript:;" data-toggle="tooltip" aria-label="View" data-bs-original-title="View"><i class="ugo-icons">info</i></a>';
                }
            }//index 7
        ],
        drawCallback: function () {
            $('[data-toggle="tooltip"]').tooltip();
        }
    });
}

function LoadStats() {
    $.get('/IAM/AppAuditLog/Stats', function (result) {
        if (!result || !result.Status) return;
        var data = result.ResponseData || {};
        $("#statTotalUsers").text(data.Total_Users ?? 0);
        $("#statEventsToday").text(data.Events_Today ?? 0);
        $("#statActiveUsersToday").text(data.Active_Users_Today ?? 0);
        $("#statTotalDevices").text(data.Total_Devices ?? 0);
    });
}

function ShowAppAuditDetail(row) {
    var userName = row.User_Name || row.Login_Id || 'Unknown';

    // section 1 comes straight from the grid row
    $("#dtlAvatar").text(InitialsOf(userName));
    $("#dtlUser").text(userName);
    $("#dtlLoginId").text(row.Login_Id || '-');
    $("#dtlMobile").text(row.Mobile_No || '-');
    $("#dtlTotalActions").text(row.Total_Actions != null ? row.Total_Actions : 0);

    // section 2: all actions of this user
    $("#dtlActivityList").html('<div class="text-center text-muted py-3">Loading...</div>');

    var reqId = ++detailReqId;
    $.get('/IAM/AppAuditLog/Details', { Login_Id: row.Login_Id }, function (result) {
        if (reqId !== detailReqId) return; // another row was opened meanwhile
        RenderActivity((result && result.Status) ? (result.ResponseData || []) : []);
    }).fail(function () {
        if (reqId !== detailReqId) return;
        $("#dtlActivityList").html('<div class="text-center text-danger py-3">Failed to load activity.</div>');
    });

    appAuditDetailDrawer.show();
}

// One small card per action (rows already come latest-first from the DB), all fields shown
function RenderActivity(rows) {
    if (!rows.length) {
        $("#dtlActivityList").html('<div class="text-center text-muted py-3">No activity found.</div>');
        return;
    }

    function field(label, value) {
        var val = (value === null || value === undefined || String(value).trim() === '') ? '-' : value;
        return '<div class="col-6">' +
            '<div class="text-muted text-uppercase fw-bold">' + escapeHtml(label) + '</div>' +
            '<div class="text-break">' + escapeHtml(val) + '</div>' +
            '</div>';
    }

    var html = '';
    rows.forEach(function (r) {
        var location = (r.Latitude && r.Longitude) ? (r.Latitude + ', ' + r.Longitude) : '-';

        html += '<div class="card card-body bg-white p-2 mb-2">' +
            '<div class="d-flex justify-content-between align-items-start gap-2 mb-1">' +
            '<span class="badge ' + ResponseBadgeClass(r.Audit_Response) + ' text-wrap text-start">' + escapeHtml(r.Audit_Response || '-') + '</span>' +
            '<span class="text-muted small text-nowrap">' + escapeHtml(r.Created_On || '-') + '</span>' +
            '</div>' +
            '<div class="row g-2 small">' +
            field('Audit Id', r.Audit_Id) +
            field('Request URL', r.Request_Url) +
            field('Device', JoinParts([r.Brand_Name, r.Device_Name], ' ')) +
            field('Device Id', r.Device_Id) +
            field('Android Version', r.Mobile_Android_Version) +
            field('SDK', r.Sdk_Int) +
            field('App Version', r.App_Version) +
            field('Audit App Release', r.Audit_App_Release) +
            field('Application Id', r.Application_Id) +
            field('Audit Application Id', r.Audit_Application_Id) +
            field('MAC Address', r.Mac_Address) +
            field('Location', location) +
            '</div>' +
            '</div>';
    });

    $("#dtlActivityList").html(html);
}

// ---------------- helpers ----------------
function escapeHtml(str) {
    if (str === null || str === undefined) return '';
    return String(str)
        .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;').replace(/'/g, '&#39;');
}

// Joins the non-empty parts, '-' when everything is empty
function JoinParts(parts, sep) {
    var out = (parts || []).filter(function (p) { return p !== null && p !== undefined && String(p).trim() !== ''; });
    return out.length ? out.join(sep) : '-';
}

function InitialsOf(name) {
    if (!name) return '?';
    var parts = name.trim().split(/[\s._@-]+/).filter(Boolean);
    if (parts.length === 0) return '?';
    if (parts.length === 1) return parts[0].substring(0, 2).toUpperCase();
    return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
}

// audit_response text (e.g. "Logged In Successfully") -> existing badge-* classes
function ResponseBadgeClass(text) {
    var v = (text || '').toUpperCase();
    if (v.indexOf('FAIL') > -1 || v.indexOf('INVALID') > -1 || v.indexOf('ERROR') > -1 ||
        v.indexOf('LOCK') > -1 || v.indexOf('BLOCK') > -1 || v.indexOf('DENIED') > -1) return 'badge-danger';
    if (v.indexOf('LOGOUT') > -1 || v.indexOf('LOGGED OUT') > -1) return 'badge-secondary';
    if (v.indexOf('SUCCESS') > -1 || v.indexOf('LOGGED IN') > -1) return 'badge-success';
    return 'badge-info';
}
