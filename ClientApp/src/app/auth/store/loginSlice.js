import { createSlice } from "@reduxjs/toolkit";
import { showMessage } from "app/store/fuse/messageSlice";
import firebaseService from "app/services/firebaseService";
import jwtService from "app/services/jwtService";
import { setUserData } from "./userSlice";
import { addNotification } from "app/fuse-layouts/shared-components/notificationPanel/store/dataSlice";
import NotificationModel from "app/fuse-layouts/shared-components/notificationPanel/model/NotificationModel";

export const submitLogin =
    ({ email, password }) =>
    async (dispatch) => {
        return jwtService
            .signInWithEmailAndPassword(email, password)
            .then((user) => {
                if (typeof user == "object") {
                    user = {
                        uuid: user.Id,
                        from: "custom-db",
                        role: user.Roles[0].Name, //'admin', //user.Role.Name.toLower(),
                        roleIds: user.Roles.map((x) => x.Id),
                        data: {
                            displayName: user.Fullname,
                            photoURL: "assets/images/avatars/Abbott.jpg",
                            email: user.Email,
                            settings: {
                                layout: {
                                    style: "layout3",
                                    config: {
                                        scroll: "content",
                                        navbar: {
                                            display: true,
                                            folded: true,
                                            position: "left",
                                        },
                                        toolbar: {
                                            display: true,
                                            style: "fixed",
                                            position: "below",
                                        },
                                        footer: {
                                            display: true,
                                            style: "fixed",
                                            position: "below",
                                        },
                                        mode: "container",
                                    },
                                },
                                customScrollbars: true,
                                theme: {
                                    main: "light11", //"defaultDark",
                                    navbar: "light11", //"defaultDark",
                                    toolbar: "light11", //"defaultDark",
                                    footer: "light11", //"defaultDark",
                                },
                            },
                            shortcuts: ["calendar", "mail", "contacts"],
                        },
                    };

                    dispatch(setUserData(user));

                    return dispatch(loginSuccess());
                } else {
                    return dispatch(loginError([user]));
                }
            })
            .catch((errors) => {
                return dispatch(loginError(errors));
            });
    };

export const submitLoginWithFireBase =
    ({ email, password }) =>
    async (dispatch) => {
        if (!firebaseService.auth) {
            console.warn(
                "Firebase Service didn't initialize, check your configuration"
            );

            return () => false;
        }
        return firebaseService.auth
            .signInWithEmailAndPassword(email, password)
            .then(() => {
                return dispatch(loginSuccess());
            })
            .catch((error) => {
                const emailErrorCodes = [
                    "auth/email-already-in-use",
                    "auth/invalid-email",
                    "auth/operation-not-allowed",
                    "auth/user-not-found",
                    "auth/user-disabled",
                ];
                const passwordErrorCodes = [
                    "auth/weak-password",
                    "auth/wrong-password",
                ];
                const response = [];

                if (emailErrorCodes.includes(error.code)) {
                    response.push({
                        type: "email",
                        message: error.message,
                    });
                }

                if (passwordErrorCodes.includes(error.code)) {
                    response.push({
                        type: "password",
                        message: error.message,
                    });
                }

                if (error.code === "auth/invalid-api-key") {
                    dispatch(showMessage({ message: error.message }));
                }

                return dispatch(loginError(response));
            });
    };

const initialState = {
    success: false,
    errors: [],
};

const loginSlice = createSlice({
    name: "auth/login",
    initialState,
    reducers: {
        loginSuccess: (state, action) => {
            state.success = true;
            state.errors = [];
        },
        loginError: (state, action) => {
            state.success = false;
            state.errors = action.payload;
        },
    },
    extraReducers: {},
});

export const { loginSuccess, loginError } = loginSlice.actions;

export default loginSlice.reducer;
