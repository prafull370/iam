
//window.PermissionLoader = (function () {
//    'use strict';

//    const PERMISSION_DATA_KEY = 'PermissionData';
//    const API_BASE = '/UserPermission/getPagesById';

//    /**
//     * Load permissions from API and store in SessionStorage
//     * @param {number} pageId - Page ID (usually Module ID)
//     * @param {string} flag - Flag for the request
//     * @param {number} productId - Product ID
//     */
//    async function loadPermissionsFromAPI(pageId, flag, productId) {
//        try {
//            //console.log('PermissionLoader: Loading permissions from API...');
            
//            const url = `${API_BASE}?Id=${pageId}&Flag=${flag}&Product_Id=${productId}`;
//            const response = await fetch(url, {
//                method: 'GET',
//                headers: {
//                    'Content-Type': 'application/json',
//                    'X-Requested-With': 'XMLHttpRequest'
//                },
//                credentials: 'include'
//            });

//            if (!response.ok) {
//                console.error(`PermissionLoader: API returned status ${response.status}`);
//                return false;
//            }

//            const result = await response.json();

//            if (result.Status && result.ResponseData) {
//                // Store permissions in SessionStorage
//                sessionStorage.setItem(PERMISSION_DATA_KEY, JSON.stringify(result.ResponseData));
//                //console.log('PermissionLoader: Permissions loaded and stored successfully');
//                //console.log('PermissionLoader: Available pages:', result.ResponseData.length);
//                return true;
//            } else {
//                console.error('PermissionLoader: API returned unsuccessful status');
//                return false;
//            }
//        } catch (error) {
//            console.error('PermissionLoader: Error loading permissions:', error);
//            return false;
//        }
//    }

//    /**
//     * Check if permissions are already loaded
//     */
//    function isLoaded() {
//        const data = sessionStorage.getItem(PERMISSION_DATA_KEY);
//        return data !== null && data !== undefined && data !== '';
//    }

//    /**
//     * Clear stored permissions
//     */
//    function clear() {
//        sessionStorage.removeItem(PERMISSION_DATA_KEY);
//        //console.log('PermissionLoader: Permissions cleared');
//    }

//    /**
//     * Get stored permissions from SessionStorage
//     */
//    function getPermissions() {
//        try {
//            const data = sessionStorage.getItem(PERMISSION_DATA_KEY);
//            return data ? JSON.parse(data) : null;
//        } catch (error) {
//            console.error('PermissionLoader: Error parsing permissions:', error);
//            return null;
//        }
//    }

//    // Public API
//    return {
//        load: loadPermissionsFromAPI,
//        isLoaded: isLoaded,
//        clear: clear,
//        getPermissions: getPermissions
//    };
//})();
