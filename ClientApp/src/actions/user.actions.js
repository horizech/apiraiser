import { userConstants } from '../constants';
import { userService } from '../services';
import { alertActions } from './';
import { history } from '../helpers';
import { toastActions } from './toast.actions';

export const userActions = {
    login,
    logout,
    register,
    getAll,
    checkSession,
    delete: _delete
};

function login(username, password) {
    return dispatch => {
        dispatch(request({ username }));

        userService.login(username, password)
            .then(
                result => {
                    if (result.Success) {
                        let user = result.Data;
                        // login successful if there's a jwt token in the response
                        if (user && user.Token) {
                            // store user details and jwt token in local storage to keep user logged in between page refreshes
                            localStorage.setItem('user', JSON.stringify(user));
                        }
                        dispatch(success(user));
                        // history.push('/');
                        // dispatch(alertActions.success("Signed in!"));
                    }
                    else {
                        dispatch(failure(result.Message));
                        dispatch(alertActions.error("Error", result.Message));
                    }
                },
                error => {
                    dispatch(failure(error));
                    dispatch(alertActions.error("Error", error));
                }
            );
    };

    function request(user) { return { type: userConstants.LOGIN_REQUEST, user } }
    function success(user) { return { type: userConstants.LOGIN_SUCCESS, user } }
    function failure(error) { return { type: userConstants.LOGIN_FAILURE, error } }
}

function logout() {
    userService.logout();
    return { type: userConstants.LOGOUT };
}

function register(user) {
    return dispatch => {
        dispatch(request(user));
        
        userService.registerUser(user)
        .then(
            result => {
                if (result.Success) {
                    let user = result.Data;
                    // login successful if there's a jwt token in the response
                    if (user && user.Token) {
                        // store user details and jwt token in local storage to keep user logged in between page refreshes
                        localStorage.setItem('user', JSON.stringify(user));
                    }
                    dispatch(success(user));
                    history.push('/');
                    // dispatch(alertActions.success("Signed in!"));
                }
                else {
                    dispatch(failure(result.Message));
                    dispatch(alertActions.error(result.Message));
                }
            },
            error => {
                dispatch(failure(error));
                dispatch(alertActions.error(error));
            }
        );
    };

    function request(user) { return { type: userConstants.REGISTER_REQUEST, user } }
    function success(user) { return { type: userConstants.REGISTER_SUCCESS, user } }
    function failure(error) { return { type: userConstants.REGISTER_FAILURE, error } }
}

function getAll() {
    return dispatch => {
        dispatch(request());

        userService.getAll()
            .then(
                users => dispatch(success(users)),
                error => dispatch(failure(error))
            );
    };

    function request() { return { type: userConstants.GETALL_REQUEST } }
    function success(users) { return { type: userConstants.GETALL_SUCCESS, users } }
    function failure(error) { return { type: userConstants.GETALL_FAILURE, error } }
}

function checkSession() {
    return dispatch => {
        dispatch(request(null));
        let user = localStorage.getItem("user");
        if (user) {
            user = JSON.parse(user);
            console.log('user', user);
            dispatch(success(user));
        }
    };

    function request() { return { type: userConstants.GETALL_REQUEST } }
    function success(user) { return { type: userConstants.LOGIN_SUCCESS, user } }
}

// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    return dispatch => {
        dispatch(request(id));

        userService.delete(id)
            .then(
                () => {
                    dispatch(success(id));
                },
                error => {
                    dispatch(failure(id, error));
                }
            );
    };

    function request(id) { return { type: userConstants.DELETE_REQUEST, id } }
    function success(id) { return { type: userConstants.DELETE_SUCCESS, id } }
    function failure(id, error) { return { type: userConstants.DELETE_FAILURE, id, error } }
}