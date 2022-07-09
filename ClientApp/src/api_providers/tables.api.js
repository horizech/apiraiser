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
            `${config.apiUrl}/API/${schema}/GetTablesList`,
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
            `${config.apiUrl}/API/${schema}/${table}/Columns`,
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
            `${config.apiUrl}/api/GetPredefinedColumns`,
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
        const url = `${config.apiUrl}/API/${schema}/${table}`;

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
            `${config.apiUrl}/API/${schema}/${table}`,
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
            `${config.apiUrl}/API/${schema}/${table}/${row.Id}`,
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
            `${config.apiUrl}/API/${schema}/${table}/${id}`,
            requestOptions
        ).then(handleResponse);
    }

    static async createTable(schema, table, ColumnsInfo) {
        let headers = authHeader();
        headers["Content-Type"] = "application/json";

        const requestOptions = {
            method: "POST",
            headers: headers,
            body: JSON.stringify(ColumnsInfo),
        };

        return fetch(
            `${config.apiUrl}/API/${schema}/CreateTable?table=${table}`,
            requestOptions
        ).then(handleResponse);
    }
    static async addColumn(schema, table, ColumnsInfo) {
        let headers = authHeader();
        headers["Content-Type"] = "application/json";

        const requestOptions = {
            method: "POST",
            headers: headers,
            body: JSON.stringify(ColumnsInfo),
        };

        return fetch(
            `${config.apiUrl}/API/${schema}/${table}/Column`,
            requestOptions
        ).then(handleResponse);
    }
    static async deleteColumn(schema, table, column) {
        let headers = authHeader();
        headers["Content-Type"] = "application/json";

        const requestOptions = {
            method: "DELETE",
            headers: headers,
        };

        return fetch(
            `${config.apiUrl}/API/${schema}/${table}/Column/${column}`,
            requestOptions
        ).then(handleResponse);
    }
}
export { TablesApiProvider };
