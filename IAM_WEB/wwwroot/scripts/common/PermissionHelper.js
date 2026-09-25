

window.Permission = (function () {
    'use strict';

    // ==================== PRIVATE VARIABLES ====================
    const PERMISSION_DATA_KEY = 'PermissionData';
    const CURRENT_PAGE_KEY = 'CurrentPageName';
    
    let permissionData = null;
    let currentPageName = null;
    let currentPagePermissions = null;
    let observerEnabled = true;

    // inject css for permission-hidden
    (function injectCss() {
        try {
            const css = '.permission-hidden{display:none !important; pointer-events:none !important; visibility:hidden !important;}';
            const style = document.createElement('style');
            style.type = 'text/css';
            style.appendChild(document.createTextNode(css));
            document.head.appendChild(style);
        } catch (e) {
            // ignore
        }
    })();

    // ==================== PRIVATE METHODS ====================

    /**
     * Load permission data from SessionStorage
     */
    function loadFromSessionStorage() {
        try {
            const data = sessionStorage.getItem(PERMISSION_DATA_KEY);
            if (data) {
                permissionData = JSON.parse(data);
                //console.debug('PermissionHelper: Loaded PermissionData', permissionData);
                return true;
            }
            //console.warn('PermissionHelper: No permission data in SessionStorage');
            return false;
        } catch (error) {
            //console.error('PermissionHelper: Error parsing SessionStorage data', error);
            return false;
        }
    }
    function urlKey(url) {
        if (!url) return '';
        var parts = url.toString().toLowerCase().split('?')[0].split('#')[0].split('/').filter(Boolean);
        return parts.slice(0, 2).join('/');
    }/** * Prefer an explicit server-rendered page key (see F3), fall back to the URL. */
    function detectCurrentPage() {
        try {
            var explicit = document.body && document.body.getAttribute('data-page-name');
            if (explicit) { currentPageName = explicit; } else {
                var pathArray = window.location.pathname.split('/').filter(Boolean);
                currentPageName = pathArray.length >= 2 ? pathArray[1] : (pathArray[0] || null);
            } currentPageUrlKey = urlKey(window.location.pathname);
            if (currentPageName) sessionStorage.setItem(CURRENT_PAGE_KEY, currentPageName);
            return currentPageName;
        } catch (error) {
            //console.error('PermissionHelper: Error detecting current page', error);
            return null;
        }
    }


    /**
     * Find the current page in permission data
     * Searches in top-level pages and child pages (subMenus)
     */
    function findCurrentPageInData(pageName) {
        if (!permissionData || !Array.isArray(permissionData)) {
            return null;
        }

        // Search in top-level pages
        for (const page of permissionData) {
            if (page.Page_Name && 
                normalizePageName(page.Page_Name) === normalizePageName(pageName)) {
                return page;
            }

            // Search in child pages
            if (Array.isArray(page.subMenus)) {
                for (const subMenu of page.subMenus) {
                    if (subMenu.Page_Name && 
                        normalizePageName(subMenu.Page_Name) === normalizePageName(pageName)) {
                        return subMenu;
                    }
                }
            }
        }

        return null;
    }

    /**
     * Normalize page name for comparison
     * "Department" vs "DEPARTMENT" -> both match
     */
    function normalizePageName(name) {
        if (!name) return '';
        return name.toString().toLowerCase().replace(/[\s_\-]+/g, '').trim();
    }


    /**
     * Normalize URL for comparison
     * "/iam/department/add" vs "/IAM/Department/Add" -> both match
     */
    function normalizeUrl(url) {
        if (!url) return '';
        return url.toString().toLowerCase().trim();
    }

    /**
     * Get the action object by name
     */
    function getActionByName(actionName) {
        if (!currentPagePermissions || !Array.isArray(currentPagePermissions.subAttributes)) {
            return null;
        }

        return currentPagePermissions.subAttributes.find(attr => 
            attr.Action_Name && 
            normalizePageName(attr.Action_Name) === normalizePageName(actionName)
        );
    }

    /**
     * Get the action object by code
     */
    function getActionByCode(actionCode) {
        if (!currentPagePermissions || !Array.isArray(currentPagePermissions.subAttributes)) {
            return null;
        }

        return currentPagePermissions.subAttributes.find(attr => 
            attr.Action_Code && 
            attr.Action_Code.toString().toUpperCase() === actionCode.toString().toUpperCase()
        );
    }

    /**
     * Check if an action is allowed
     */
    function isActionAllowed(actionName) {
        const action = getActionByName(actionName) || getActionByCode(actionName);
        if (action) {
            //console.debug('PermissionHelper: Found action for', actionName, action);
        }
        return action ? action.Is_Allow === true : false;
    }

    /**
     * Process and hide/show elements with permission-action attribute
     */
    function processActionAttributes() {
        // Select all elements with permission-action attribute
        document.querySelectorAll('[permission-action]').forEach(element => {
            const raw = element.getAttribute('permission-action');
            if (!raw) return;

            // support comma-separated actions
            const actions = raw.split(',').map(a => a.trim()).filter(Boolean);
            if (actions.length === 0) return;

            // if any action is allowed, show; otherwise hide
            let allowed = false;
            for (const a of actions) {
                if (isActionAllowed(a)) {
                    allowed = true;
                    break;
                }
            }

            if (allowed) {
                // Show element
                element.classList.remove('permission-hidden');
                element.style.removeProperty('display');
                element.removeAttribute('aria-hidden');
                element.removeAttribute('disabled');
                
                // Special handling for table headers: show entire column
                if (element.tagName === 'TH') {
                    showTableColumn(element);
                }
            } else {
                // Hide element (strong)
                element.classList.add('permission-hidden');
                element.setAttribute('aria-hidden', 'true');
                element.setAttribute('data-permission-hidden', 'true');
                element.setAttribute('disabled', 'disabled');
                
                // Special handling for table headers: hide entire column
                if (element.tagName === 'TH') {
                    hideTableColumn(element);
                }
            }
        });
    }

    /**
     * Hide entire table column and its cells
     * Uses DataTables API when available to avoid layout collapse
     */
    function hideTableColumn(headerElement) {
        try {
            const table = headerElement.closest('table');
            if (!table) return;

            const headers = Array.from(table.querySelectorAll('thead th'));
            const colIndex = headers.indexOf(headerElement);
            if (colIndex < 0) return;

            // If DataTables is present, use its column visibility API
            if (window.jQuery && $.fn && $.fn.dataTable && $.fn.dataTable.isDataTable(table)) {
                try {
                    const dt = $(table).DataTable();
                    // Use column().visible(false) which keeps layout intact
                    dt.column(colIndex).visible(false, false);
                    dt.columns.adjust();
                    return;
                } catch (dtErr) {
                    // fallback to DOM hide
                    //console.debug('PermissionHelper: DataTable hide failed, falling back to DOM hide', dtErr);
                }
            }

            // Fallback: hide header and each cell in column
            headerElement.classList.add('permission-hidden');
            const rows = table.querySelectorAll('tbody tr');
            rows.forEach(row => {
                const cells = row.querySelectorAll('td');
                if (cells[colIndex]) {
                    cells[colIndex].classList.add('permission-hidden');
                }
            });

            //console.debug('PermissionHelper: Hidden table column at index', colIndex);
        } catch (error) {
            //console.error('PermissionHelper: Error hiding table column', error);
        }
    }

    /**
     * Show entire table column and its cells
     */
    function showTableColumn(headerElement) {
        try {
            const table = headerElement.closest('table');
            if (!table) return;

            const headers = Array.from(table.querySelectorAll('thead th'));
            const colIndex = headers.indexOf(headerElement);
            if (colIndex < 0) return;

            // If DataTables is present, use its column visibility API
            if (window.jQuery && $.fn && $.fn.dataTable && $.fn.dataTable.isDataTable(table)) {
                try {
                    const dt = $(table).DataTable();
                    dt.column(colIndex).visible(true, false);
                    dt.columns.adjust();
                    return;
                } catch (dtErr) {
                    //console.debug('PermissionHelper: DataTable show failed, falling back to DOM show', dtErr);
                }
            }

            // Fallback: show header and each cell in column
            headerElement.classList.remove('permission-hidden');
            const rows = table.querySelectorAll('tbody tr');
            rows.forEach(row => {
                const cells = row.querySelectorAll('td');
                if (cells[colIndex]) {
                    cells[colIndex].classList.remove('permission-hidden');
                }
            });

            //console.debug('PermissionHelper: Showed table column at index', colIndex);
        } catch (error) {
            //console.error('PermissionHelper: Error showing table column', error);
        }
    }

    /**
     * Process and hide/show elements with permission-page attribute
     */
    function processPageAttributes() {
        document.querySelectorAll('[permission-page]').forEach(element => {
            const pageName = element.getAttribute('permission-page');
            
            if (!pageName) return;

            if (hasPageAccess(pageName)) {
                element.classList.remove('permission-hidden');
                element.style.removeProperty('display');
            } else {
                element.classList.add('permission-hidden');
            }
        });
    }

    /**
     * Process and hide/show elements with permission-role attribute
     */
    function processRoleAttributes() {
        document.querySelectorAll('[permission-role]').forEach(element => {
            const roleName = element.getAttribute('permission-role');
            
            if (!roleName) return;

            // This can be extended based on user roles from UserInfo
            // For now, we'll support basic role checking
            if (checkUserRole(roleName)) {
                element.classList.remove('permission-hidden');
            } else {
                element.classList.add('permission-hidden');
            }
        });
    }

    /**
     * Check if user has a specific role
     */
    function checkUserRole(roleName) {
        try {
            const userInfo = sessionStorage.getItem('UserInfo');
            if (!userInfo) return false;

            const user = JSON.parse(userInfo);
            if (!user.Roles || !Array.isArray(user.Roles)) return false;

            return user.Roles.some(role => 
                role.toLowerCase().trim() === roleName.toLowerCase().trim()
            );
        } catch (error) {
            //console.error('PermissionHelper: Error checking role', error);
            return false;
        }
    }

    /**
     * Check if user has access to a page
     */
    function hasPageAccess(pageName) {
        if (!permissionData || !Array.isArray(permissionData)) {
            return false;
        }

        for (const page of permissionData) {
            if (page.Page_Name && 
                normalizePageName(page.Page_Name) === normalizePageName(pageName)) {
                return true;
            }

            if (Array.isArray(page.subMenus)) {
                for (const subMenu of page.subMenus) {
                    if (subMenu.Page_Name && 
                        normalizePageName(subMenu.Page_Name) === normalizePageName(pageName)) {
                        return true;
                    }

                    // Fallback: try matching against Page_Url (normalized)
                    if (subMenu.Page_Url && 
                        normalizeUrl(subMenu.Page_Url) === normalizePageName(pageName)) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /**
     * Setup MutationObserver to watch for dynamically added elements
     * This allows permission-action attributes to work on AJAX-loaded content
     */
    function setupMutationObserver() {
        if (!observerEnabled) return;

        const observer = new MutationObserver(function (mutations) {
            // Debounce to avoid multiple rapid calls
            clearTimeout(observer.timeoutId);
            observer.timeoutId = setTimeout(function () {
                processActionAttributes();
                processPageAttributes();
                processRoleAttributes();
            }, 100);
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true,
            attributes: false,
            characterData: false
        });
    }

    // ==================== PUBLIC API ====================

    return {
        /**
         * Initialize permission framework
         * @param {string} customDataKey - Optional custom key for SessionStorage
         */
        initialize: function (customDataKey) {
            // Use custom key if provided
            const dataKey = customDataKey || PERMISSION_DATA_KEY;
            
            // Load permission data
            if (!loadFromSessionStorage()) {
                //console.error('PermissionHelper: Failed to load permission data');
                return false;
            }

            // Detect current page
            const detectedPage = detectCurrentPage();
            if (!detectedPage) {
                //console.warn('PermissionHelper: Could not detect current page from URL');
            }

            // Setup mutation observer for dynamic content
            setupMutationObserver();

            //console.log('PermissionHelper: Initialized. Current page:', detectedPage);
            return true;
        },

        /**
         * Apply permissions to all elements on current page
         */
        apply: function () {
            if (!permissionData) {
                //console.error('PermissionHelper: Permission data not loaded. Call initialize() first.');
                return false;
            }

            // Find current page
            currentPagePermissions = findCurrentPageInData(currentPageName);
            
            if (!currentPagePermissions) {
                //console.log('PermissionHelper: No permissions found for page:', currentPageName);
            } else {
                //console.log('PermissionHelper: Current page permissions', currentPagePermissions);
            }

            // Process all permission attributes
            processActionAttributes();
            processPageAttributes();
            processRoleAttributes();

            //console.log('PermissionHelper: Permissions applied. Page:', currentPageName);
            return true;
        },

        /**
         * Get current page name
         */
        getCurrentPage: function () {
            return currentPageName || detectCurrentPage();
        },

        /**
         * Get current page permissions
         */
        getCurrentPagePermission: function () {
            return currentPagePermissions;
        },

        /**
         * Get all actions for current page
         */
        getActions: function () {
            if (!currentPagePermissions) {
                return [];
            }
            return currentPagePermissions.subAttributes || [];
        },

        /**
         * Check if a specific action is allowed
         * @param {string} actionName - Action name or action code
         */
        hasAction: function (actionName) {
            return isActionAllowed(actionName);
        },

        /**
         * Check if user has access to a page
         * @param {string} pageName - Page name to check
         */
        hasPage: function (pageName) {
            return hasPageAccess(pageName);
        },

        /**
         * Check if action is allowed (alias for hasAction)
         * @param {string} actionName - Action name or action code
         */
        isAllowed: function (actionName) {
            return isActionAllowed(actionName);
        },

        /**
         * Hide all unauthorized controls
         * Alias for apply() - kept for semantic clarity
         */
        hideUnauthorizedControls: function () {
            return this.apply();
        },

        /**
         * Get all pages accessible to user
         */
        getAccessiblePages: function () {
            if (!permissionData || !Array.isArray(permissionData)) {
                return [];
            }
            return permissionData;
        },

        /**
         * Refresh permissions from SessionStorage
         * Useful after permission updates
         */
        refresh: function () {
            permissionData = null;
            currentPagePermissions = null;
            return this.initialize();
        },

        /**
         * Disable automatic mutation observer for dynamic content
         * Useful if you want to manually control when permissions are applied
         */
        disableMutationObserver: function () {
            observerEnabled = false;
        },

        /**
         * Enable automatic mutation observer for dynamic content
         */
        enableMutationObserver: function () {
            observerEnabled = true;
        },

        /**
         * Manual reload permissions for newly added elements
         * Call this after adding elements dynamically via AJAX
         */
        reapply: function () {
            processActionAttributes();
            processPageAttributes();
            processRoleAttributes();
        },

        /**
         * Get debugging information
         */
        debug: function () {
            return {
                currentPage: currentPageName,
                currentPagePermissions: currentPagePermissions,
                allPermissions: permissionData,
                isInitialized: permissionData !== null
            };
        }
    };
})();

document.addEventListener('DOMContentLoaded', function () {
    // Only auto-apply if Permission was explicitly initialized
    // This prevents automatic application if dev hasn't set permissions up
    if (typeof Permission !== 'undefined' && Permission.debug && Permission.debug().isInitialized) {
        // Developer should call Permission.apply() explicitly
        // Or we can auto-call if needed by uncommenting:
        // Permission.apply();
    }
});
