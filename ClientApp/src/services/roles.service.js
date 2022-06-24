import { RolesApiProvider } from '../api_providers';

export const rolesService = { getRoles, addRoles, updateRoles, deleteRoles };

function getRoles() {
    return RolesApiProvider.getRoles();
}

function addRoles(rows) {
    return RolesApiProvider.addRoles(rows);
}

function updateRoles(name, rows ) {
    return RolesApiProvider.updateRoles( name, rows);
}

function deleteRoles(rows) {
    return RolesApiProvider.deleteRoles(rows);
}

