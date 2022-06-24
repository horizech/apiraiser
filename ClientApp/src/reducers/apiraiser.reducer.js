import { apiraiserConstants } from "../constants";

let isInitialized = JSON.parse(localStorage.getItem("apiraiserStatus"));
const initialState = isInitialized
  ? isInitialized
  : {
      checkingStatus: false,
      initializing: false,
      isInitialized: undefined,
      message: "Apiraiser status unknown",
    };

export function apiraiserReducer(state = initialState, action) {
  switch (action.type) {
    case apiraiserConstants.IS_INITIALIZED_REQUEST:
      return {
        checkingStatus: true,
        isInitialized: false,
        message: "Checking",
      };
    case apiraiserConstants.IS_INITIALIZED_SUCCESS:
      return {
        checkingStatus: false,
        isInitialized: true,
        message: action.message,
      };
    case apiraiserConstants.IS_INITIALIZED_FAILURE:
      return {
        checkingStatus: false,
        isInitialized: false,
        message: action.message,
      };
    case apiraiserConstants.IS_INITIALIZED_ERROR:
      return {
        checkingStatus: false,
        isInitialized: undefined,
        message: action.message,
      };
    case apiraiserConstants.INITIALIZE_REQUEST:
      return {
        initializing: true,
        isInitialized: false,
        message: "Checking",
      };
    case apiraiserConstants.INITIALIZE_SUCCESS:
      return {
        initializing: false,
        isInitialized: true,
        message: action.message,
      };
    case apiraiserConstants.INITIALIZE_FAILURE:
      return {
        initializing: false,
        isInitialized: false,
        message: action.message,
      };
    default:
      return state;
  }
}
