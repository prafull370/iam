// Brand-led categorical order (validated for colour-vision deficiency); grey is reserved for the folded "Other" slice
var RoleChartColors = ['#0052cc', '#1D9E75', '#EF9F27', '#7F77DD', '#D85A30'];
var RoleOtherColor = '#9CA3AF';
var ChartFont = "'Inter', sans-serif";
var ChartTextColor = '#6c757d';

var loginTrendChart = null;
var roleChart = null;

$(document).ready(function () {
    LoadKpi();
    LoadLoginTrend();
    LoadRoleDistribution();
    LoadSecurity();
    LoadRecentActivity();
});

// Prevents HTML injection from server values
function escapeHtml(value) {
    return $('<div/>').text(value ?? '').html();
}

function FormatNumber(value) {
    return (Number(value) || 0).toLocaleString();
}

function StateHtml(message, cls) {
    return '<div class="dbchart-state ' + (cls || 'text-muted') + '">' + escapeHtml(message) + '</div>';
}

// ------------------------------------- 1. KPI Cards ------------------------------------------------//
function LoadKpi() {
    $.ajax({
        url: '/IAM/Dashboard/GetKpi',
        type: 'GET',
        dataType: 'json',
        success: function (result) {
            var d = result.ResponseData;
            if (!d) { SetKpiUnavailable(); return; }

            // Total users + growth vs last month
            $('#kpiTotalUsers').text(FormatNumber(d.Total_Users));
            $('#kpiTotalUsersSub').html(GrowthText(d.Users_This_Month, d.Users_Last_Month));

            // Active users + % of total
            var pct = d.Total_Users > 0 ? Math.round((d.Active_Users / d.Total_Users) * 100) : 0;
            $('#kpiActiveUsers').text(FormatNumber(d.Active_Users));
            $('#kpiActiveUsersSub').text(pct + '% of total users');

            // DISCOMs / Products / Roles: total + active | inactive
            SetKpi('#kpiDiscoms', d.Total_Discoms, d.Active_Discoms);
            SetKpi('#kpiProducts', d.Total_Products, d.Active_Products);
            SetKpi('#kpiRoles', d.Total_Roles, d.Active_Roles);
        },
        error: function () {
            SetKpiUnavailable();
            showAlert("Unable to load dashboard KPIs.", "Error", "OkCancel");
        }
    });
}

function SetKpi(selector, total, active) {
    $(selector).text(FormatNumber(total));
    $(selector + 'Sub').html(
        '<span class="dbkpi-active">' + FormatNumber(active) + ' Active</span>' +
        '<span class="dbkpi-sep">|</span>' +
        '<span>' + FormatNumber(total - active) + ' Inactive</span>');
}

function SetKpiUnavailable() {
    $('#kpiTotalUsers, #kpiActiveUsers, #kpiDiscoms, #kpiProducts, #kpiRoles').text('–');
}

function GrowthText(thisMonth, lastMonth) {
    if (!lastMonth) {
        return FormatNumber(thisMonth) + ' new this month';
    }
    var change = Math.round(((thisMonth - lastMonth) / lastMonth) * 100);
    var up = change >= 0;
    return '<span class="dbtrend ' + (up ? 'dbtrend-up' : 'dbtrend-down') + '">' +
        '<i class="ugo-icons">' + (up ? 'trending_up' : 'trending_down') + '</i>' + Math.abs(change) + '%</span>' +
        ' vs last month';
}

// ------------------------------------- 2. Login Trend ------------------------------------------------//
function LoadLoginTrend() {
    $('#loginTrendLoader').removeClass('d-none').html(
        '<div class="spinner-border spinner-border-sm text-primary" role="status"><span class="visually-hidden">Loading...</span></div>');

    $.ajax({
        url: '/IAM/Dashboard/GetLoginTrend',
        type: 'GET',
        dataType: 'json',
        success: function (result) {
            var rows = result.ResponseData || [];

            if (rows.length === 0) {
                $('#loginTrendTotal').text('0');
                $('#loginTrendLoader').html('<span class="text-muted">No login data available</span>');
                return;
            }

            $('#loginTrendLoader').addClass('d-none');
            DrawLoginTrend(rows);
        },
        error: function (xhr, status, error) {
            console.error('Login Trend Error:', status, error);
            $('#loginTrendLoader').html('<span class="text-danger">Unable to load login trend</span>');
        }
    });
}

function DrawLoginTrend(rows) {
    var labels = rows.map(function (r) { return r.Day_Label || ''; });
    var values = rows.map(function (r) { return Number(r.Login_Count) || 0; });
    var total = values.reduce(function (sum, v) { return sum + v; }, 0);

    $('#loginTrendTotal').text(FormatNumber(total));

    var options = {
        chart: {
            type: 'bar',
            height: 260,
            fontFamily: ChartFont,
            toolbar: { show: false },
            zoom: { enabled: false },
            animations: { speed: 400 }
        },
        series: [{ name: 'Logins', data: values }],
        colors: ['#0052cc'],
        plotOptions: {
            bar: {
                columnWidth: '45%',
                borderRadius: 4,
                borderRadiusApplication: 'end'
            }
        },
        dataLabels: { enabled: false },
        states: {
            hover: { filter: { type: 'darken', value: 0.85 } }
        },
        grid: {
            borderColor: '#eef0f3',
            strokeDashArray: 3,
            padding: { left: 4, right: 4, top: -10 },
            xaxis: { lines: { show: false } }
        },
        xaxis: {
            categories: labels,
            axisBorder: { show: false },
            axisTicks: { show: false },
            labels: { style: { colors: ChartTextColor, fontSize: '11px' } },
            tooltip: { enabled: false }
        },
        yaxis: {
            min: 0,
            forceNiceScale: true,
            decimalsInFloat: 0,
            labels: {
                style: { colors: ChartTextColor, fontSize: '11px' },
                formatter: function (v) { return FormatNumber(Math.round(v)); }
            }
        },
        tooltip: {
            y: { formatter: function (v) { return FormatNumber(v); } }
        },
        noData: { text: 'No login data available' }
    };

    if (loginTrendChart) { loginTrendChart.destroy(); }
    loginTrendChart = new ApexCharts(document.querySelector('#loginTrendChart'), options);
    loginTrendChart.render();
}

// ------------------------------------- 3. Role Distribution ------------------------------------------------//
function LoadRoleDistribution() {
    $.ajax({
        url: '/IAM/Dashboard/GetRoleDistribution',
        type: 'GET',
        dataType: 'json',
        success: function (result) {
            var rows = result.ResponseData || [];

            if (rows.length === 0) {
                $('#dvRoleDistribution').html(StateHtml('No role assignments found'));
                return;
            }

            DrawRoleDistribution(rows);
        },
        error: function () {
            $('#dvRoleDistribution').html(StateHtml('Unable to load role distribution', 'text-danger'));
        }
    });
}

function DrawRoleDistribution(rows) {
    var totalRoles = rows[0].Total_Roles;

    // Largest roles first; anything beyond the palette folds into a single "Other" slice
    var sorted = rows.slice().sort(function (a, b) { return b.User_Count - a.User_Count; });
    var slices = sorted.slice(0, RoleChartColors.length).map(function (r, i) {
        return { name: r.Role_Name || '—', count: r.User_Count, color: RoleChartColors[i] };
    });
    var rest = sorted.slice(RoleChartColors.length);
    if (rest.length > 0) {
        slices.push({
            name: 'Other (' + rest.length + ' roles)',
            count: rest.reduce(function (sum, r) { return sum + r.User_Count; }, 0),
            color: RoleOtherColor
        });
    }

    var totalUsers = slices.reduce(function (sum, s) { return sum + s.count; }, 0);

    var legend = slices.map(function (s) {
        var pct = totalUsers > 0 ? Math.round((s.count / totalUsers) * 100) : 0;
        return '<li>' +
            '<span class="dbrole-dot" style="background:' + s.color + '"></span>' +
            '<span class="dbrole-name" title="' + escapeHtml(s.name) + '">' + escapeHtml(s.name) + '</span>' +
            '<span class="dbrole-val">' + FormatNumber(s.count) + '<small>' + pct + '%</small></span>' +
            '</li>';
    }).join('');

    $('#dvRoleDistribution').html(
        '<div class="dbrole-chart" id="roleDistributionChart" role="img" aria-label="Users by role"></div>' +
        '<ul class="dbrole-legend">' + legend + '</ul>');

    var options = {
        chart: {
            type: 'donut',
            height: 160,
            fontFamily: ChartFont,
            animations: { speed: 400 }
        },
        series: slices.map(function (s) { return s.count; }),
        labels: slices.map(function (s) { return s.name; }),
        colors: slices.map(function (s) { return s.color; }),
        stroke: { width: 2, colors: ['#fff'] },
        legend: { show: false },
        dataLabels: { enabled: false },
        states: {
            hover: { filter: { type: 'darken', value: 0.9 } }
        },
        plotOptions: {
            pie: {
                expandOnClick: false,
                donut: {
                    size: '75%',
                    labels: {
                        show: true,
                        name: { offsetY: 18, fontSize: '11px', color: ChartTextColor },
                        value: {
                            offsetY: -14, fontSize: '22px', fontWeight: 700, color: '#112E51',
                            formatter: function (v) { return FormatNumber(v); }
                        },
                        total: {
                            show: true,
                            showAlways: true,
                            label: 'Assigned Users',
                            fontSize: '11px',
                            fontWeight: 400,
                            color: ChartTextColor,
                            formatter: function () { return FormatNumber(totalUsers); }
                        }
                    }
                }
            }
        },
        tooltip: {
            y: { formatter: function (v) { return FormatNumber(v) + ' users'; } }
        }
    };

    if (roleChart) { roleChart.destroy(); }
    roleChart = new ApexCharts(document.querySelector('#roleDistributionChart'), options);
    roleChart.render();
}

// ------------------------------------- 4. Security Overview ------------------------------------------------//
function LoadSecurity() {
    $.ajax({
        url: '/IAM/Dashboard/GetSecurity',
        type: 'GET',
        dataType: 'json',
        success: function (result) {
            var d = result.ResponseData;
            if (!d) { SetSecurityUnavailable(); return; }

            $('#secFailedAttempts').text(FormatNumber(d.Failed_Attempt_Users));
            $('#secLocked').text(FormatNumber(d.Locked_Accounts));
            $('#secNewDevices').text(FormatNumber(d.New_Devices_Today));
            // $('#secErrors').text(d.Errors_Today.toLocaleString());
            $('#secSessions').text(FormatNumber(d.Active_Sessions));
        },
        error: function () {
            SetSecurityUnavailable();
            showAlert("Unable to load security overview.", "Error", "OkCancel");
        }
    });
}

function SetSecurityUnavailable() {
    $('#secFailedAttempts, #secLocked, #secNewDevices, #secSessions').text('–');
}

// ------------------------------------- 5. Recent Activity ------------------------------------------------//
function LoadRecentActivity() {
    $.ajax({
        url: '/IAM/Dashboard/GetRecentActivity',
        type: 'GET',
        dataType: 'json',
        success: function (result) {
            var rows = result.ResponseData || [];
            var $body = $('#tbodyRecentActivity');

            if (rows.length === 0) {
                $body.html('<tr><td colspan="8" class="text-center text-muted py-4">No recent activity</td></tr>');
                return;
            }

            $('#recentActivityCount').text('Latest ' + rows.length + ' user actions across modules');

            var html = '';
            rows.forEach(function (r) {
                var isOk = (r.Status || '').toUpperCase() === 'OK';
                var statusHtml = isOk
                    ? '<span class="badge badge-success">Success</span>'
                    : '<span class="badge badge-danger">' + escapeHtml(r.Status || 'Failed') + '</span>';

                html += '<tr>' +
                    '<td>' + escapeHtml(r.User_Id) + '</td>' +
                    '<td class="fw-semibold text-nowrap">' + escapeHtml(r.User_Name) + '</td>' +
                    '<td class="text-nowrap">' + escapeHtml(r.Activity_Type) + '</td>' +
                    '<td class="text-nowrap">' + escapeHtml(r.Page_Name) + '</td>' +
                    '<td><div class="dbtbl-desc" title="' + escapeHtml(r.Description) + '">' + escapeHtml(r.Description) + '</div></td>' +
                    '<td class="text-nowrap">' + escapeHtml(r.Ip_Address) + '</td>' +
                    '<td class="text-nowrap">' + escapeHtml(r.Created_On) + '</td>' +
                    '<td class="text-nowrap">' + statusHtml + '</td>' +
                    '</tr>';
            });
            $body.html(html);
        },
        error: function () {
            $('#tbodyRecentActivity').html('<tr><td colspan="8" class="text-center text-danger py-4">Unable to load recent activity</td></tr>');
        }
    });
}
