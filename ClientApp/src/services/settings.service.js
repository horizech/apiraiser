import { SettingsApiProvider } from '../api_providers';

export const settingsService = { getSettings, addSettings, updateSettings, deleteSettings };

function getSettings() {
    return SettingsApiProvider.getSettings();
}

function addSettings(rows) {
    return SettingsApiProvider.addSettings(rows);
}

function updateSettings(key, rows ) {
    return SettingsApiProvider.updateSettings( key, rows);
}

function deleteSettings(rows) {
    return SettingsApiProvider.deleteSettings(rows);
}

