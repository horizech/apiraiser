import { config } from "../helpers";
import { handleError, handleResponse } from "./handler.api";

class ApiraiserApiProvider {
  static async checkInitialized() {
    const requestOptions = {
      method: "get",
      headers: { "Content-Type": "application/json" },
    };

    return fetch(config.apiUrl + "/api/apiraiser/IsInitialized", requestOptions)
      .then(handleResponse, handleError)
      .then((result) => {
        return result;
      });
  }

  static async initialize(userInfo) {
    const requestOptions = {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(userInfo),
    };
    return fetch(config.apiUrl + "/api/apiraiser/initialize", requestOptions)
      .then(handleResponse, handleError)
      .then((result) => {
        return result;
      });
  }
}

export { ApiraiserApiProvider };
