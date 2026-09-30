var pendingRequests = 0;

$(document).ready(function () {
    LoadKpi();
    LoadLoginTrend();
    LoadRoleDistribution();
    LoadSecurity();
    LoadRecentActivity();
});

// ------------------------------------- 1. KPI Cards ------------------------------------------------//
function LoadKpi() {
    $.ajax({
        url: '/IAM/Dashboard/GetKpi',
        type: 'GET',
        dataType: 'json',
        success: function (result) {
            var d = result.ResponseData;
            if (!d) { return; }

            // Total users + growth vs last month
            $('#kpiTotalUsers').text(d.Total_Users.toLocaleString());
            $('#kpiTotalUsersSub').html(GrowthText(d.Users_This_Month, d.Users_Last_Month));

            // Active users + % of total
            var pct = d.Total_Users > 0 ? Math.round((d.Active_Users / d.Total_Users) * 100) : 0;
            $('#kpiActiveUsers').text(d.Active_Users.toLocaleString());
            $('#kpiActiveUsersSub').text(pct + '% of total');

            // DISCOMs / Products / Roles: total + active | inactive
            SetKpi('#kpiDiscoms', d.Total_Discoms, d.Active_Discoms);
            SetKpi('#kpiProducts', d.Total_Products, d.Active_Products);
            SetKpi('#kpiRoles', d.Total_Roles, d.Active_Roles);
        },
        error: function () {
            showAlert("Unable to load dashboard KPIs.", "Error", "OkCancel");
        }
    });
}

function SetKpi(selector, total, active) {
    $(selector).text(total.toLocaleString());
    $(selector + 'Sub').text(active + ' Active | ' + (total - active) + ' Inactive');
}

function GrowthText(thisMonth, lastMonth) {
    if (lastMonth === 0) {
        return thisMonth + ' new this month';
    }
    var change = Math.round(((thisMonth - lastMonth) / lastMonth) * 100);
    var cls = change >= 0 ? 'text-success' : 'text-danger';
    var arrow = change >= 0 ? '&uarr;' : '&darr;';
    return '<span class="' + cls + '">' + arrow + ' ' + Math.abs(change) + '%</span> new users vs last month';
}

// ------------------------------------- 2. Role Distribution ------------------------------------------------//
var RoleChartColors = ['#7F77DD', '#1D9E75', '#EF9F27', '#378ADD', '#D85A30', '#9CA3AF'];

function LoadRoleDistribution() {
    $.ajax({
        url: '/IAM/Dashboard/GetRoleDistribution',
        type: 'GET',
        dataType: 'json',
        success: function (result) {
            var rows = result.ResponseData || [];
            var $box = $('#dvRoleDistribution');

            if (rows.length === 0) {
                $box.html('<div class="text-muted small">No role assignments found</div>');
                return;
            }

            var totalUsers = rows.reduce(function (sum, r) { return sum + r.User_Count; }, 0);
            var totalRoles = rows[0].Total_Roles;

            var radius = 38;
            var circumference = 2 * Math.PI * radius;
            var offset = 0;
            var circles = '';
            var legend = '';

            rows.forEach(function (r, i) {
                var color = RoleChartColors[i % RoleChartColors.length];
                var share = totalUsers > 0 ? r.User_Count / totalUsers : 0;
                var dash = share * circumference;

                circles += '<circle cx="52" cy="52" r="' + radius + '" stroke="' + color + '" ' +
                    'stroke-dasharray="' + dash + ' ' + (circumference - dash) + '" ' +
                    'stroke-dashoffset="' + (-offset) + '"></circle>';
                offset += dash;

                legend += '<div class="d-flex justify-content-between small mb-1">' +
                    '<span><span class="d-inline-block rounded-circle me-2" style="width:8px;height:8px;background:' + color + '"></span>' +
                    escapeHtml(r.Role_Name) + '</span>' +
                    '<span class="text-muted">' + Math.round(share * 100) + '% (' + r.User_Count + ')</span></div>';
            });

            var svg =
                '<svg width="120" height="120" viewBox="0 0 104 104" role="img" aria-label="Users by role">' +
                '<g transform="rotate(-90 52 52)" fill="none" stroke-width="16">' + circles + '</g>' +
                '<text x="52" y="50" text-anchor="middle" font-size="16" font-weight="600" fill="currentColor">' + totalRoles + '</text>' +
                '<text x="52" y="64" text-anchor="middle" font-size="9" fill="currentColor" opacity="0.6">Total Roles</text>' +
                '</svg>';

            $box.html('<div class="flex-shrink-0">' + svg + '</div><div class="flex-grow-1">' + legend + '</div>');
        },
        error: function () {
            $('#dvRoleDistribution').html('<div class="text-danger small">Unable to load role distribution</div>');
        }
    });
}

// Prevents HTML injection from role names
function escapeHtml(value) {
    return $('<div/>').text(value ?? '').html();
}

// ------------------------------------- 3. Security Overview ------------------------------------------------//
function LoadSecurity() {
    $.ajax({
        url: '/IAM/Dashboard/GetSecurity',
        type: 'GET',
        dataType: 'json',
        success: function (result) {
            var d = result.ResponseData;
            if (!d) { return; }

            $('#secFailedAttempts').text(d.Failed_Attempt_Users.toLocaleString());
            $('#secLocked').text(d.Locked_Accounts.toLocaleString());
            $('#secNewDevices').text(d.New_Devices_Today.toLocaleString());
            // $('#secErrors').text(d.Errors_Today.toLocaleString());
            $('#secSessions').text(d.Active_Sessions.toLocaleString());
        },
        error: function () {
            showAlert("Unable to load security overview.", "Error", "OkCancel");
        }
    });
}

// ------------------------------------- 4. Recent Activity ------------------------------------------------//
function LoadRecentActivity() {
    $.ajax({
        url: '/IAM/Dashboard/GetRecentActivity',
        type: 'GET',
        dataType: 'json',
        success: function (result) {
            var rows = result.ResponseData || [];
            var $body = $('#tbodyRecentActivity');

            if (rows.length === 0) {
                $body.html('<tr><td colspan="8" class="text-center text-muted">No recent activity</td></tr>');
                return;
            }

            var html = '';
            rows.forEach(function (r) {
                var isOk = (r.Status || '').toUpperCase() === 'OK';
                var statusHtml = isOk
                    ? '<span class="text-success">&#9679; Success</span>'
                    : '<span class="text-danger">&#9679; ' + escapeHtml(r.Status) + '</span>';

                html += '<tr>' +
                    '<td>' + r.User_Id + '</td>' +
                    '<td>' + escapeHtml(r.User_Name) + '</td>' +
                    '<td>' + escapeHtml(r.Activity_Type) + '</td>' +
                    '<td>' + escapeHtml(r.Page_Name) + '</td>' +
                    '<td>' + escapeHtml(r.Description) + '</td>' +
                    '<td>' + escapeHtml(r.Ip_Address) + '</td>' +
                    '<td>' + escapeHtml(r.Created_On) + '</td>' +
                    '<td>' + statusHtml + '</td>' +
                    '</tr>';
            });
            $body.html(html);
            console.log("recent activity Response:", result);
        },
        error: function () {
            $('#tbodyRecentActivity').html('<tr><td colspan="8" class="text-center text-danger">Unable to load recent activity</td></tr>');
        }
    });
}

// ------------------------------------- Login Trend ------------------------------------------------//

function LoadLoginTrend() {

    // Show loader
    $("#loginTrendLoader").removeClass("d-none");
    $("#loginTrendChart").addClass("d-none");

    $.ajax({
        url: '/IAM/Dashboard/GetLoginTrend',
        type: 'GET',
        dataType: 'json',

        success: function (result) {

            var rows = result.ResponseData || [];

            DrawLoginTrend(rows);

            // Hide loader
            $("#loginTrendLoader").addClass("d-none");

            // Show chart
            $("#loginTrendChart").removeClass("d-none");
        },

        error: function (xhr, status, error) {

            console.error("Login Trend Error:", status, error);
            console.error("Response:", xhr.responseText);

            // Hide loader
            $("#loginTrendLoader").addClass("d-none");

            // Show message
            $("#loginTrendChart").removeClass("d-none");

            DrawLoginTrendMessage("Unable to load login trend");
        }
    });
}

function DrawLoginTrend(rows) {

    var canvas = document.getElementById('loginTrendChart');

    if (!canvas) {
        console.error("loginTrendChart canvas not found");
        return;
    }

    var ctx = canvas.getContext('2d');

    if (!ctx) {
        console.error("Unable to get canvas context");
        return;
    }

    // Get actual canvas/container width
    var width = canvas.parentElement
        ? canvas.parentElement.clientWidth
        : canvas.clientWidth;

    if (!width || width < 100) {
        width = 500;
    }

    var height = 160;

    canvas.width = width;
    canvas.height = height;

    ctx.clearRect(0, 0, width, height);

    // No data
    if (!Array.isArray(rows) || rows.length === 0) {

        DrawLoginTrendMessage("No login data available");
        return;
    }

    console.log("Rows received for chart:", rows);

    // Convert Login_Count to number
    var values = rows.map(function (item) {
        return Number(item.Login_Count) || 0;
    });

    var maxCount = Math.max.apply(null, values);

    if (maxCount <= 0) {
        maxCount = 1;
    }

    var chartTop = 15;
    var chartBottom = 115;
    var chartHeight = chartBottom - chartTop;

    var gap = 12;

    var barWidth =
        (width - (gap * (rows.length + 1))) / rows.length;

    // Prevent invalid bar width
    if (barWidth < 5) {
        barWidth = 5;
    }

    rows.forEach(function (item, index) {

        var count = Number(item.Login_Count) || 0;

        var barHeight =
            (count / maxCount) * chartHeight;

        var x =
            gap + index * (barWidth + gap);

        var y =
            chartBottom - barHeight;

        // ---------------- Bar ---------------- //

        ctx.fillStyle =
            index === rows.length - 1
                ? '#1769aa'
                : '#378ADD';

        // Use fillRect instead of roundRect
        // for better browser compatibility
        ctx.fillRect(
            x,
            y,
            barWidth,
            barHeight
        );

        // ---------------- Count ---------------- //

        ctx.font = '11px Arial';
        ctx.fillStyle = '#495057';

        var countText = String(count);

        var countWidth =
            ctx.measureText(countText).width;

        ctx.fillText(
            countText,
            x + (barWidth - countWidth) / 2,
            y - 5
        );

        // ---------------- Day Label ---------------- //

        ctx.font = '11px Arial';
        ctx.fillStyle = '#6c757d';

        var label =
            item.Day_Label ||
            item.day_label ||
            item.Date_Label ||
            '';

        var labelWidth =
            ctx.measureText(label).width;

        ctx.fillText(
            label,
            x + (barWidth - labelWidth) / 2,
            140
        );
    });
}


function DrawLoginTrendMessage(message) {

    var canvas = document.getElementById('loginTrendChart');

    if (!canvas) {
        return;
    }

    var ctx = canvas.getContext('2d');

    var width = canvas.parentElement
        ? canvas.parentElement.clientWidth
        : 500;

    if (!width || width < 100) {
        width = 500;
    }

    canvas.width = width;
    canvas.height = 160;

    ctx.clearRect(0, 0, width, 160);

    ctx.font = '12px Arial';
    ctx.fillStyle = '#6c757d';

    ctx.fillText(
        message,
        10,
        30
    );
}
