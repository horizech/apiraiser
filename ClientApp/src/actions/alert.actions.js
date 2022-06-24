import { alertConstants } from '../constants';

export const alertActions = {
    success,
    error,
    clear
};

function success(title, message) {
    return { type: alertConstants.SUCCESS, title, message };
}

function error(title, message) {
    return { type: alertConstants.ERROR, title, message };
}

function clear() {
    return { type: alertConstants.CLEAR };
}