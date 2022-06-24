import { RolePermissionGroupMappingsApiProvider } from '../api_providers';

export const rolePermissionGroupMappingsService = { getRolePermissionGroupMappings, addRolePermissionGroupMapping, updateRolePermissionGroupMapping, deleteRolePermissionGroupMapping };

function getRolePermissionGroupMappings() {
    return RolePermissionGroupMappingsApiProvider.getRolePermissionGroupMappings();
}

function addRolePermissionGroupMapping(rows) {
    return RolePermissionGroupMappingsApiProvider.addRolePermissionGroupMapping(rows);
}

function updateRolePermissionGroupMapping(name, rows ) {
    return RolePermissionGroupMappingsApiProvider.updateRolePermissionGroupMapping( name, rows);
}

function deleteRolePermissionGroupMapping(rows) {
    return RolePermissionGroupMappingsApiProvider.deleteRolePermissionGroupMapping(rows);
}

