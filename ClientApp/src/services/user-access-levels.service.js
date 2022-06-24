import { UserAccessLevelsApiProvider } from '../api_providers';

export const userAccessLevelsService = { getUserAccessLevels, addUserAccessLevels, updateUserAccessLevels, deleteUserAccessLevels };

function getUserAccessLevels() {
    return UserAccessLevelsApiProvider.getUserAccessLevels();
}

function addUserAccessLevels(rows) {
    return UserAccessLevelsApiProvider.addUserAccessLevels(rows);
}

function updateUserAccessLevels(name, rows ) {
    return UserAccessLevelsApiProvider.updateUserAccessLevels( name, rows);
}

function deleteUserAccessLevels(rows) {
    return UserAccessLevelsApiProvider.deleteUserAccessLevels(rows);
}

