import { userConstants } from '../constants';

let user = JSON.parse(localStorage.getItem('user'));
const initialState = user
    ? {
        loggingIn: false,
        loggedIn: true,
        user
    }
    : {
        loggedIn: false,
        loggingIn: false,
        user: null
    };

export function authenticationReducer(state = initialState, action) {
    switch (action.type) {
        case userConstants.LOGIN_REQUEST:
            return {
                loggedIn: false,
                loggingIn: true,
                user: action.user
            };
        case userConstants.LOGIN_SUCCESS:
            return {
                loggedIn: true,
                loggingIn: false,
                user: action.user
            };
        case userConstants.REGISTER_SUCCESS:
            return {
                loggedIn: true,
                loggingIn: false,
                user: action.user
            };
        case userConstants.LOGIN_FAILURE:
            return {
                loggedIn: false,
                loggingIn: false,
                user: null
            };
        case userConstants.LOGOUT:
            return {
                loggedIn: false,
                loggingIn: false,
                user: null
            };
        default:
            return state
    }
}