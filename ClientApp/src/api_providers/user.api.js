import { authHeader, config } from "../helpers";
import { handleError, handleResponse } from "./handler.api";

class UserApiProvider {
    static async login(Username, Password) {
        const requestOptions = {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ Username, Password }),
        };

        return fetch(
            config.apiUrl + "/API/Authentication/login",
            requestOptions
        )
            .then(handleResponse, handleError)
            .then((result) => {
                return result;
            });
    }

    static logout() {
        // remove user from local storage to log user out
        localStorage.removeItem("user");
    }

    static getAllUsers() {
        const requestOptions = {
            method: "GET",
            headers: authHeader(),
        };

        return fetch(
            config.apiUrl + "/API/Users/GetUsers",
            requestOptions
        ).then(handleResponse, handleError);
    }

    static getUserById(id) {
        const requestOptions = {
            method: "GET",
            headers: authHeader(),
        };

        return fetch(config.apiUrl + "/users/" + id, requestOptions).then(
            handleResponse,
            handleError
        );
    }

    static registerUser(user) {
        const requestOptions = {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(user),
        };

        return fetch(
            config.apiUrl + "/API/Authentication/Signup",
            requestOptions
        ).then(handleResponse, handleError);
    }

    static updateUser(Id, user) {
        console.log(user);
        const requestOptions = {
            method: "PUT",
            headers: { ...authHeader(), "Content-Type": "application/json" },
            body: JSON.stringify(user),
        };

        return fetch(
            config.apiUrl + `/API/Users/UpdateUser?Id=${Id}`,
            requestOptions
        ).then(handleResponse, handleError);
    }
    static addUser(user) {
        const requestOptions = {
            method: "POST",
            headers: { ...authHeader(), "Content-Type": "application/json" },
            body: JSON.stringify(user),
        };
        console.log(user);
        return fetch(config.apiUrl + `/API/Users/AddUser`, requestOptions).then(
            handleResponse,
            handleError
        );
    }
    // prefixed function name with underscore because delete is a reserved word in javascript
    static deleteUser(Id) {
        const requestOptions = {
            method: "DELETE",
            headers: authHeader(),
        };

        return fetch(
            config.apiUrl + `API/Users/DeleteUser?Id=${Id}`,
            requestOptions
        ).then(handleResponse, handleError);
    }
}

export { UserApiProvider };
