import { PermissionsApiProvider } from '../api_providers';

export const permissionsService = { getPermissions, addPermissions, updatePermissions, deletePermissions };

function getPermissions() {
    return PermissionsApiProvider.getPermissions();
}

function addPermissions(rows) {
    return PermissionsApiProvider.addPermissions(rows);
}

function updatePermissions(name, rows ) {
    return PermissionsApiProvider.updatePermissions( name, rows);
}

function deletePermissions(rows) {
    return PermissionsApiProvider.deletePermissions(rows);
}

