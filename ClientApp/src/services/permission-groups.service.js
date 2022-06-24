import { PermissionGroupsApiProvider } from '../api_providers';

export const permissionGroupsService = { getPermissionGroups, addPermissionGroup, updatePermissionGroup, deletePermissionGroup };

function getPermissionGroups() {
    return PermissionGroupsApiProvider.getPermissionGroups();
}

function addPermissionGroup(rows) {
    return PermissionGroupsApiProvider.addPermissionGroup(rows);
}

function updatePermissionGroup(name, rows ) {
    return PermissionGroupsApiProvider.updatePermissionGroup( name, rows);
}

function deletePermissionGroup(rows) {
    return PermissionGroupsApiProvider.deletePermissionGroup(rows);
}

