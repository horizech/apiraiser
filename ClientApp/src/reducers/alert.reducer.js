import { alertConstants } from '../constants';

export function alertReducer(state = {}, action) {
    switch (action.type) {
        case alertConstants.SUCCESS:
            return {
                type: 'success',
                title: action.title,
                message: action.message
            };
        case alertConstants.ERROR:
            return {
                type: 'error',
                title: action.title,
                message: action.message
            };
        case alertConstants.CLEAR:
            return {};
        default:
            return state
    }
}