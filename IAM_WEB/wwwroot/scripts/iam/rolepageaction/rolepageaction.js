
(function () {
    'use strict';
    var state = {
        productId: null,
        roles: [],
        pages: [],
        actions: [],           // action catalogue (for names)
        groups: [],
        actionById: {},        // actionId -> catalogue row (name)
        avail: new Map(),      // 'roleId_pageId' -> [ {actionId, enabled, granted} ]
        grants: new Map(),     // 'roleId_pageId_actionId' -> 'A' | 'D'
        dirty: new Map()
    };

    var ROLE_COLORS = ['#3b6fe0', '#d08700', '#5b6675', '#7c3aed', '#0e9f6e', '#dc2626', '#0e6f78', '#d6336c'];
    var key = function (r, p, a) {
        return r + '_' + p + '_' + a;
    };
    var rpKey = function (r, p) {
        return r + '_' + p;
    };
    var antiforgery = function () {
        return $('input[name="__RequestVerificationToken"]').val() || '';
    };

    function prop(obj, name) {
        if (!obj) return undefined;
        if (obj[name] !== undefined) return obj[name];
        var lower = name.charAt(0).toLowerCase() + name.slice(1);
        if (obj[lower] !== undefined) return obj[lower];

        for (var k in obj) {
            if (Object.prototype.hasOwnProperty.call(obj, k) && k.toLowerCase() === name.toLowerCase()) return obj[k];
        }
        return undefined;
    }

    function truthy(v) {
        return v === true || v === 'true' || v === 1 || v === '1';
    }
    function normalizeEffect(v) {
        if (v === true || v === "true" || v === 1 || v === "1")
            return true;

        return false;
    }

    $(document).ready(function () {

        // Show Empty State
        $("#imgchooseflt").show();
        $("#dvMatrix").addClass("d-none");

        if ($.fn && typeof $.fn.select2 === 'function') {
            $('.slct2').select2({ placeholder: 'Select', allowClear: false });
        }

        $('#DDLProductId').on('change', function () {
            var pid = $(this).val();

            clearState();

            $("#permMatrixHead").empty();
            $("#permMatrixBody").empty();

            if (pid && pid !== '') {
                $("#imgchooseflt").hide();
                state.productId = pid;
                loadMatrix(pid);
            }
            else {
                $("#imgchooseflt").show();
                clearState();
                $('#dvMatrix').addClass('d-none');
            }
        });

        $('#btnReset').on('click', function () {
            location.reload();
        });

        $('#btnSavePermissions').on('click', saveMatrix);
        $('#btnExportGrants').on('click', exportGrants);
        bindCellEvents();
    });

    // ---- Load ------------------------------------------------------------

    function loadMatrix(productId) {
        $.ajax({
            url: '/IAM/RolePageAction/Get',
            type: 'GET',
            data: { Product_Id: productId },

            beforeSend: function () {
                $("#imgchooseflt").hide();
                $('#dvProcessing').removeClass('d-none');
                $('#dvMatrix').addClass('d-none');
            },

            success: function (res) {
                $('#dvProcessing').addClass('d-none');
                var ok = prop(res, 'Status');
                var data = prop(res, 'ResponseData');
                if (!ok || !data) {
                    $('#dvMatrix').addClass('d-none');
                    showAlert(prop(res, 'Message') || 'Failed to load data.', 'Error', 'Ok');
                    return;
                }

                state.roles = prop(data, 'Roles') || [];
                state.pages = prop(data, 'Pages') || [];
                state.actions = prop(data, 'Actions') || [];

                // catalogue lookup
                state.actionById = {};
                state.actions.forEach(function (a) {
                    state.actionById[Number(prop(a, 'Action_Id'))] = a;
                });

                // AvailableActions -> per role+page list + grant map
                state.avail.clear();
                state.grants.clear();
                state.dirty.clear();

                (prop(data, 'AvailableActions') || []).forEach(function (row) {
                    var rid = Number(prop(row, 'Role_Id'));
                    var pid = Number(prop(row, 'Page_Id'));
                    var aid = Number(prop(row, 'Action_Id'));
                    var enabled = truthy(prop(row, 'Is_Enabled'));
                    //var eff = normalizeEffect(prop(row, 'Effect'));
                    var eff = normalizeEffect(prop(row, 'Is_Allow'));
                    var rk = rpKey(rid, pid);
                    if (!state.avail.has(rk)) state.avail.set(rk, []);
                    if (enabled) state.avail.get(rk).push({ actionId: aid });
                    if (eff) state.grants.set(key(rid, pid, aid), eff);   // saved grant -> pre-check

                });

                state.groups = buildGroups(state.pages);
                if (state.roles.length && state.groups.length) {
                    renderMatrix();
                    $("#imgchooseflt").hide();
                    $('#dvMatrix').removeClass('d-none');
                } else {
                    $('#dvMatrix').addClass('d-none');
                    $("#imgchooseflt").show();
                    //showAlert('No records found for this product.', 'Warning', 'Ok');
                }

            },

            error: function (xhr, status, error) {
                $('#dvProcessing').addClass('d-none');
                $('#dvMatrix').addClass('d-none');
                $('#dvMatrix').addClass('d-none');
                showAlert('An error occurred while loading permissions: ' + error, 'Error', 'Ok');
            }
        });
    }

    // ---- Gating helpers --------------------------------------------------

    // enabled actions for a role+page (from AvailableActions), resolved to catalogue rows

    function availableActionsFor(roleId, pageId) {

        var list = state.avail.get(rpKey(Number(roleId), Number(pageId))) || [];

        var out = [];

        list.forEach(function (item) {
            var a = state.actionById[item.actionId];
            if (a) out.push(a);
        });
        return out;
    }

    function cellEnabled(roleId, pageId) {
        return availableActionsFor(roleId, pageId).length > 0;
    }

    // ---- Build tree ------------------------------------------------------

    function buildGroups(pages) {
        var byId = {};

        pages.forEach(function (p) {
            byId[prop(p, 'Page_Id')] = p;
        });

        var childrenOf = {}, isChild = {};

        function addChild(parentId, childPage) {
            if (parentId === null || parentId === undefined || parentId === 0) return;
            var cid = prop(childPage, 'Page_Id');
            if (!childrenOf[parentId]) childrenOf[parentId] = [];
            var exists = childrenOf[parentId].some(function (c) { return prop(c, 'Page_Id') === cid; });
            if (!exists) childrenOf[parentId].push(childPage);
            isChild[cid] = true;
        }

        pages.forEach(function (p) {
            var subs = prop(p, 'subMenus') || [];
            subs.forEach(function (sm) {
                addChild(prop(p, 'Page_Id'), byId[prop(sm, 'Page_Id')] || sm);
            });
            var pid = prop(p, 'Parent_Id');
            if (pid && byId[pid]) addChild(pid, p);
        });

        var groups = [], orphanBuckets = {}, orphanOrder = [];

        pages.forEach(function (p) {

            var id = prop(p, 'Page_Id');
            if (isChild[id]) return;
            var kids = childrenOf[id] || [];
            if (kids.length) { groups.push({ title: prop(p, 'Page_Name') || '', pages: kids }); }
            else {
                var master = prop(p, 'Master_Name') || 'Pages';
                if (!orphanBuckets[master]) {
                    orphanBuckets[master] = { title: master, pages: [] };
                    orphanOrder.push(orphanBuckets[master]);
                }
                orphanBuckets[master].pages.push(p);
            }
        });
        return groups.concat(orphanOrder);

    }

    // ---- Render ----------------------------------------------------------

    function renderMatrix() {
        var head = '<tr><th class="permHeadCol">Action Permissions</th>';
        state.roles.forEach(function (role, i) {
            head += '<th class="roleCol">' +
                '<span class="roleDot" style="background-color:' + ROLE_COLORS[i % ROLE_COLORS.length] + '"></span>' +
                escapeHtml((prop(role, 'Role_Name') || '').toUpperCase()) + '</th>';
        });
        head += '</tr>';
        $('#permMatrixHead').html(head);
        var body = '';
        state.groups.forEach(function (g) {
            var count = g.pages.length;
            body += '<tr class="permGroupRow"><td colspan="' + (state.roles.length + 1) + '">' +
                '<span class="permGroupSticky">' +
                '<span class="permGroupTitle"><i class="ugo-icons">grid_view</i>' + escapeHtml(g.title) + '</span>' +
                '<span class="permGroupCount">' + count + ' page' + (count === 1 ? '' : 's') + '</span>' +
                '</span></td></tr>';
            g.pages.forEach(function (page) {
                body += renderPageRow(page);
            });
        });
        $('#permMatrixBody').html(body);
    }

    function renderPageRow(page) {

        var pageId = prop(page, 'Page_Id');
        var html = '<tr><td class="permPageCell">' + escapeHtml(prop(page, 'Page_Name') || '') + '</td>';

        state.roles.forEach(function (role) {
            html += renderCell(prop(role, 'Role_Id'), pageId);
        });
        return html + '</tr>';

    }

    function renderCell(roleId, pageId) {
        var avail = availableActionsFor(roleId, pageId);

        // ---- disabled cell (no page permission OR no mapped actions) ----
        if (avail.length === 0) {
            var msg = 'No access';
            return '<td class="permCell permCell--disabled" data-role-id="' + roleId + '" data-page-id="' + pageId + '">' +
                '<button type="button" class="permSelectBtn" disabled ' +
                'title="No actions are mapped for this page. Please configure Page Action Mapping first.">' +
                '<span class="permSelectLabel text-muted">' + msg + '</span>' +
                '<span class="permSelectMeta"><span class="permSelectCount">0</span></span>' +
                '</button></td>';
        }

        // ---- enabled cell: only the enabled actions ----

        var sel = selectedActions(roleId, pageId);
        var opts = '';
        avail.forEach(function (a) {
            var actionId = prop(a, 'Action_Id');
            var risky = !!prop(a, 'Is_Risky');
            var checked = isGranted(roleId, pageId, actionId) ? ' checked' : '';
            opts += '<li><label class="permOption' + (risky ? ' isRisky' : '') + '">' +
                '<input type="checkbox" class="permActionCheckbox" data-action-id="' + actionId + '"' + checked + ' />' +
                '<span class="permOptionName">' + escapeHtml(prop(a, 'Action_Name') || '') + '</span>' +
                (risky ? '<span class="permRiskTag">RISK</span>' : '') + '</label></li>';
        });

        return '<td class="permCell" data-role-id="' + roleId + '" data-page-id="' + pageId + '">' +
            '<div class="permMultiSelect dropdown">' +
            '<button type="button" class="permSelectBtn" data-bs-toggle="dropdown" data-bs-auto-close="outside">' +
            '<span class="permSelectLabel">' + escapeHtml(cellLabel(sel, avail.length)) + '</span>' +
            '<span class="permSelectMeta">' +
            '<span class="permSelectCount' + (sel.length === avail.length && sel.length ? ' isFull' : '') + '">' + sel.length + '</span>' +
            '<i class="ugo-icons">expand_more</i></span></button>' +
            '<div class="dropdown-menu permDropdownMenu">' +
            '<div class="permDropdownActions">' +
            '<button type="button" class="permMiniBtn permSelectAll">Select all</button>' +
            '<button type="button" class="permMiniBtn permClearAll">Clear</button></div>' +
            '<ul class="permOptionList">' + opts + '</ul></div></div></td>';
    }

    // ---- Helpers ---------------------------------------------------------

    function isGranted(roleId, pageId, actionId) {
        //return state.grants.get(key(roleId, pageId, actionId)) === 'A';
        return state.grants.get(key(roleId, pageId, actionId)) === true;
    }

    function selectedActions(roleId, pageId) {
        return availableActionsFor(roleId, pageId).filter(function (a) {
            return isGranted(roleId, pageId, prop(a, 'Action_Id'));
        });
    }

    function cellLabel(sel, availCount) {
        if (!sel.length) return 'No access';
        if (availCount && sel.length === availCount) return 'Full access';
        var names = sel.map(function (a) { return prop(a, 'Action_Name'); });
        return names.length <= 3 ? names.join(', ') : names.slice(0, 3).join(', ') + '...';
    }

    function bindCellEvents() {

        var $body = $('#permMatrixBody');

        $body.on('change', '.permActionCheckbox', function () {

            var $cell = $(this).closest('.permCell');

            //setGrant(+$cell.data('role-id'), +$cell.data('page-id'), +$(this).data('action-id'), this.checked ? 'A' : 'D');
            setGrant(+$cell.data('role-id'), +$cell.data('page-id'), +$(this).data('action-id'), this.checked ? true : false);

            refreshCell($cell);

        });

        $body.on('click', '.permSelectAll', function (e) {
            e.preventDefault(); e.stopPropagation();
            var $cell = $(this).closest('.permCell');
            var r = +$cell.data('role-id'), p = +$cell.data('page-id');
            $cell.find('.permActionCheckbox').prop('checked', true);
            //availableActionsFor(r, p).forEach(function (a) { setGrant(r, p, prop(a, 'Action_Id'), 'A'); });
            availableActionsFor(r, p).forEach(function (a) { setGrant(r, p, prop(a, 'Action_Id'), true); });
            refreshCell($cell);

        });

        $body.on('click', '.permClearAll', function (e) {
            e.preventDefault(); e.stopPropagation();
            var $cell = $(this).closest('.permCell');
            var r = +$cell.data('role-id'), p = +$cell.data('page-id');
            $cell.find('.permActionCheckbox').prop('checked', false);
            availableActionsFor(r, p).forEach(function (a) { setGrant(r, p, prop(a, 'Action_Id'), false); });
            refreshCell($cell);

        });

    }

    function setGrant(r, p, a, effect) {
        var k = key(r, p, a); state.grants.set(k, effect);
        state.dirty.set(k, effect);
    }

    function refreshCell($cell) {

        var r = +$cell.data('role-id'), p = +$cell.data('page-id');
        var avail = availableActionsFor(r, p);
        var sel = selectedActions(r, p);
        $cell.find('.permSelectCount').text(sel.length).toggleClass('isFull', avail.length > 0 && sel.length === avail.length);
        $cell.find('.permSelectLabel').text(cellLabel(sel, avail.length));
    }

    // ---- Save (delta only) ----------------------------------------------

    function saveMatrix() {
        if (!state.productId) {
            showAlert('Please select a product first.', 'Warning', 'Ok');
            return;
        }
        if (state.dirty.size === 0) {
            showAlert('No changes to save.', 'Info', 'Ok');
            return;
        }

        var permissions = [];
        state.dirty.forEach(function (effect, k) {
            var p = k.split('_');
            permissions.push({ Role_Id: parseInt(p[0], 10), Page_Id: parseInt(p[1], 10), Action_Id: parseInt(p[2], 10), Is_Allow: effect });
        });

        //$('#permMatrixBody')
        //    .find('.permCell')
        //    .not('.permCell--disabled')
        //    .each(function () {

        //        var roleId = +$(this).data('role-id');
        //        var pageId = +$(this).data('page-id');

        //        $(this).find('.permActionCheckbox').each(function () {

        //            permissions.push({
        //                Role_Id: roleId,
        //                Page_Id: pageId,
        //                Action_Id: +$(this).data('action-id'),
        //                Is_Allow: $(this).is(':checked')
        //            });

        //        });

        //    });

        $('#dvProcessing').removeClass('d-none');

        $('#btnSavePermissions').prop('disabled', true);

        $.ajax({

            url: '/IAM/RolePageAction/Save',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            headers: { 'RequestVerificationToken': antiforgery() },
            data: JSON.stringify({ Product_Id: parseInt(state.productId, 10), Permissions: permissions }),
            success: function (res) {
                $('#dvProcessing').addClass('d-none');
                $('#btnSavePermissions').prop('disabled', false);
                if (prop(res, 'Status')) { state.dirty.clear(); showAlert(prop(res, 'Message') || 'Permissions saved successfully.', 'Success', 'Ok'); }
                else {
                    showAlert(prop(res, 'Message') || 'Save failed.', 'Error', 'Ok');
                }

            },
            error: function (xhr, status, error) {
                $('#dvProcessing').addClass('d-none');
                $('#btnSavePermissions').prop('disabled', false);
                showAlert('An error occurred while saving: ' + error, 'Error', 'Ok');
            }
        });

    }

    // ---- Export ----------------------------------------------------------

    function exportGrants() {

        if (!state.groups.length) {
            showAlert('Nothing to export. Load a product first.', 'Info', 'Ok');
            return;
        }

        var rows = [['Group', 'Page'].concat(state.roles.map(function (r) {
            return prop(r, 'Role_Name');
        }))];

        state.groups.forEach(function (g) {
            g.pages.forEach(function (page) {
                var row = [g.title, prop(page, 'Page_Name') || ''];
                state.roles.forEach(function (role) {
                    row.push(selectedActions(prop(role, 'Role_Id'), prop(page, 'Page_Id')).map(function (a) {
                        return prop(a, 'Action_Name');
                    }).join(' | '));
                });
                rows.push(row);
            });
        });
        var csv = rows.map(function (r) {
            return r.map(csvEscape).join(',');
        }).join('\n');

        var link = document.createElement('a');
        link.href = URL.createObjectURL(new Blob([csv], { type: 'text/csv;charset=utf-8;' }));
        link.download = 'role_page_action_grants.csv';
        document.body.appendChild(link); link.click(); document.body.removeChild(link);
    }

    function csvEscape(v) {
        v = (v == null ? '' : String(v));
        return /[",\n]/.test(v) ? '"' + v.replace(/"/g, '""') + '"' : v;
    }

    function escapeHtml(s) {
        return (s == null ? '' : String(s)).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/'/g, '&#39;');
    }

    function clearState() {
        state.productId = null; state.roles = []; state.pages = []; state.actions = []; state.groups = [];
        state.actionById = {}; state.avail.clear(); state.grants.clear(); state.dirty.clear();
    }

    window.RolePageAction = {
        load: loadMatrix, save: saveMatrix, getState: function () {
            return state;
        }
    };

})();
