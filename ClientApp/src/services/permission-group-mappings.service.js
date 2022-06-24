import { PermissionGroupMappingsApiProvider } from '../api_providers';

export const permissionGroupMappingsService = { getPermissionGroupMappings, addPermissionGroupMapping, updatePermissionGroupMapping, deletePermissionGroupMapping };

function getPermissionGroupMappings() {
    return PermissionGroupMappingsApiProvider.getPermissionGroupMappings();
}

function addPermissionGroupMapping(rows) {
    return PermissionGroupMappingsApiProvider.addPermissionGroupMapping(rows);
}

function updatePermissionGroupMapping(name, rows ) {
    return PermissionGroupMappingsApiProvider.updatePermissionGroupMapping( name, rows);
}

function deletePermissionGroupMapping(rows) {
    return PermissionGroupMappingsApiProvider.deletePermissionGroupMapping(rows);
}

