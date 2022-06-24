import { RolePermissionsApiProvider } from '../api_providers';

export const rolePermissionsService = { getRolePermissions, addRolePermissions, updateRolePermissions, deleteRolePermissions };

function getRolePermissions() {
    return RolePermissionsApiProvider.getRolePermissions();
}

function addRolePermissions(rows) {
    return RolePermissionsApiProvider.addRolePermissions(rows);
}

function updateRolePermissions( rows ) {
    return RolePermissionsApiProvider.updateRolePermissions( rows);
}

function deleteRolePermissions(rows) {
    return RolePermissionsApiProvider.deleteRolePermissions(rows);
}

