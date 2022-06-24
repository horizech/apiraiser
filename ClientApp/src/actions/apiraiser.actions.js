import { apiraiserConstants } from "../constants";
import { apiraiserService } from "../services";
import { alertActions } from "./";
import { history } from "../helpers";
import { toastActions } from "./toast.actions";

export const apiraiserActions = {
  checkInitialized,
  initialize,
};

function checkInitialized() {
  return (dispatch) => {
    dispatch(request());

    apiraiserService.checkInitialized().then(
      (result) => {
        if (result.Success) {
          dispatch(success(result.Message));
        } else {
          dispatch(failure(result.Message));
          dispatch(alertActions.error("Error", result.Message));
        }
        localStorage.setItem(
          "apiraiserStatus",
          JSON.stringify({
            checkingStatus: false,
            isInitialized: result.Success,
            message: result.Message,
          })
        );
      },
      (err) => {
        dispatch(error(err));
        dispatch(alertActions.error(err));
        localStorage.setItem(
          "apiraiserStatus",
          JSON.stringify({
            checkingStatus: false,
            isInitialized: undefined,
            message: err,
          })
        );
      }
    );
  };

  function request() {
    return { type: apiraiserConstants.IS_INITIALIZED_REQUEST };
  }
  function success(message) {
    return { type: apiraiserConstants.IS_INITIALIZED_SUCCESS, message };
  }
  function failure(message) {
    return { type: apiraiserConstants.IS_INITIALIZED_FAILURE, message };
  }
  function error(message) {
    return { type: apiraiserConstants.IS_INITIALIZED_ERROR, message };
  }
}

function initialize(username, email, password) {
  return (dispatch) => {
    dispatch(request());
    let userInfo = {
      Username: username,
      Email: email,
      Password: password,
    };
    apiraiserService.initialize(userInfo).then(
      (result) => {
        if (result.Success) {
          dispatch(success());
          // history.push(link);
          dispatch(toastActions.success("Apiraiser Initialized"));
          // dispatch(getTableRows(table));
          return "Apiraiser Initialized";
        } else {
          dispatch(failure(result.Message));
          dispatch(alertActions.error(result.Message));
          return result.Message;
        }
      },
      (error) => {
        dispatch(failure(error.toString()));
        dispatch(alertActions.error("Error", error.toString()));
      }
    );
    dispatch(success({ Success: true, Message: "Success!" }));
    // dispatch(success({ Success: false, Message: "Error!" }));
  };

  function request() {
    return { type: apiraiserConstants.INITIALIZE_REQUEST };
  }
  function success(message) {
    return { type: apiraiserConstants.INITIALIZE_SUCCESS, message };
  }
  function failure(message) {
    return { type: apiraiserConstants.INITIALIZE_FAILURE, message };
  }
}
