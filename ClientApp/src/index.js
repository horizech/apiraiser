import "bootstrap/dist/css/bootstrap.css";
import React from "react";
import ReactDOM from "react-dom";
import { BrowserRouter } from "react-router-dom";
import registerServiceWorker from "./registerServiceWorker";
import "./themes/apiraiser.scss";

import { App } from "./App";

import { render } from "react-dom";
import { Provider } from "react-redux";

import { store } from "./helpers";

const baseUrl = document.getElementsByTagName("base")[0].getAttribute("href");
const rootElement = document.getElementById("root");

ReactDOM.render(
  <Provider store={store} basename={baseUrl}>
    <App />
  </Provider>,
  rootElement
);

registerServiceWorker();
