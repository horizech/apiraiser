import { ApiraiserApiProvider } from "../api_providers";

export const apiraiserService = {
  checkInitialized,
  initialize,
};

function checkInitialized() {
  return ApiraiserApiProvider.checkInitialized();
}

function initialize(userInfo) {
  return ApiraiserApiProvider.initialize(userInfo);
}
