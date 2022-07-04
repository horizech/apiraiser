import { Schemas } from "../constants";
import { authHeader, config } from "../helpers";
import { handleError, handleResponse } from "./handler.api";

class TablesApiProvider {
    static async getTables(schema) {
        const requestOptions = {
            method: "get",
            headers: authHeader(),
        };

        return fetch(
            `${config.apiUrl}/api/Table/GetTablesList?schema=${schema}`,
            requestOptions
        )
            .then(handleResponse, handleError)
            .then((result) => {
                return result;
            });
    }

    static async getTableColumns(schema, table) {
        const requestOptions = {
            method: "get",
            headers: authHeader(),
        };

        return fetch(
            `${config.apiUrl}/api/Table/GetTableColumns?schema=${schema}&table=${table}`,
            requestOptions
        )
            .then(handleResponse, handleError)
            .then((result) => {
                return result;
            });
    }

    static async getPredefinedColumns() {
        const requestOptions = {
            method: "get",
            headers: authHeader(),
        };

        return fetch(
            `${config.apiUrl}/api/Table/GetPredefinedColumns`,
            requestOptions
        )
            .then(handleResponse, handleError)
            .then((result) => {
                // console.log(result);
                return result;
            });
    }

    static async getTableRows(schema, table) {
        const requestOptions = {
            method: "get",
            headers: authHeader(),
        };

        // Example of search query
        // const url = `${config.apiUrl}/api/${table}?limit=5&offset=5&orderBy=Id&groupBy=Id`;
        const url = `${config.apiUrl}/${
            schema === Schemas.Administration ? "API/Administration" : "api"
        }/${table}`;

        return fetch(url, requestOptions)
            .then(handleResponse, handleError)
            .then((result) => {
                return result;
            });
    }

    static async insertRow(schema, table, row) {
        let headers = authHeader();
        headers["Content-Type"] = "application/json";

        const requestOptions = {
            method: "POST",
            headers: headers,
            body: JSON.stringify(row),
        };

        return fetch(
            `${config.apiUrl}/${
                schema === Schemas.Administration ? "API/Administration" : "api"
            }/${table}`,
            requestOptions
        ).then(handleResponse);
    }

    static async updateRow(schema, table, row) {
        let headers = authHeader();
        headers["Content-Type"] = "application/json";

        const params = row;

        const requestOptions = {
            method: "PUT",
            headers: headers,
            body: JSON.stringify(params),
        };

        return fetch(
            `${config.apiUrl}/${
                schema === Schemas.Administration ? "API/Administration" : "api"
            }/${table}/${row.Id}`,
            requestOptions
        ).then(handleResponse);
    }

    static async deleteRow(schema, table, id) {
        let headers = authHeader();
        headers["Content-Type"] = "application/json";

        const params = {};
        const requestOptions = {
            method: "DELETE",
            headers: headers,
            body: JSON.stringify(params),
        };

        return fetch(
            `${config.apiUrl}/${
                schema === Schemas.Administration ? "API/Administration" : "api"
            }/${table}/${id}`,
            requestOptions
        ).then(handleResponse);
    }

    static async createTable(table, ColumnsInfo) {
        let headers = authHeader();
        headers["Content-Type"] = "application/json";

        const requestOptions = {
            method: "POST",
            headers: headers,
            body: JSON.stringify(ColumnsInfo),
        };

        return fetch(
            `${config.apiUrl}/api/Table/CreateTable?table=${table}`,
            requestOptions
        ).then(handleResponse);
    }
    static async addColumn(table, ColumnsInfo) {
        let headers = authHeader();
        headers["Content-Type"] = "application/json";

        const requestOptions = {
            method: "POST",
            headers: headers,
            body: JSON.stringify(ColumnsInfo),
        };

        return fetch(
            `${config.apiUrl}/api/Table/AddColumn?table=${table}`,
            requestOptions
        ).then(handleResponse);
    }
    static async deleteColumn(table, column) {
        let headers = authHeader();
        headers["Content-Type"] = "application/json";

        const requestOptions = {
            method: "DELETE",
            headers: headers,
        };

        return fetch(
            `${config.apiUrl}/api/Table/DeleteColumn?table=${table}&column=${column}`,
            requestOptions
        ).then(handleResponse);
    }
}
export { TablesApiProvider };
