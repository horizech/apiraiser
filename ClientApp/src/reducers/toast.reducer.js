import { toastConstants } from '../constants';

export function toastReducer(state = {}, action) {
    switch (action.type) {
        case toastConstants.SUCCESS:
            return {
                type: 'success',
                message: action.message
            };
        case toastConstants.ERROR:
            return {
                type: 'error',
                message: action.message
            };
        case toastConstants.CLEAR:
            return {};
        default:
            return state
    }
}