import { authHeader, config } from '../helpers';
import { handleError, handleResponse } from './handler.api';


class PermissionGroupsApiProvider {
    static async getPermissionGroups() {
        const requestOptions = {
            method: 'get',
            headers: authHeader()
        };

        return fetch(`${config.apiUrl}/API/PermissionGroups/GetPermissionGroups`, requestOptions)
            .then(handleResponse, handleError)
            .then(result => {
                return result;
            });
    }


    static async addPermissionGroup(rows) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;
        let body = {
            "Name": rows.Name,
            "Description": rows.Description,
            "IsSystem": rows.IsSystem
            }
        const requestOptions = {
            method: 'POST',
            headers: headers,
            body: JSON.stringify(body)
        };
    
        return fetch(`${config.apiUrl}/api/PermissionGroups/AddPermissionGroup`, requestOptions).then(handleResponse);
    }

    static async updatePermissionGroup(name, rows) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;

        let body = {
            "Name": rows.Name,
            "Description": rows.Description,
            "IsSystem": rows.IsSystem 
            }

        const requestOptions = {
            method: 'PUT',
            headers: headers,
            body: JSON.stringify(body)
        };
    
        return fetch(`${config.apiUrl}/api/PermissionGroups/UpdatePermissionGroup?name=${name}`, requestOptions).then(handleResponse);
    }

    static async deletePermissionGroup(rows) {
        let headers = authHeader();
 
        const requestOptions = {
            method: 'DELETE',
            headers: headers
        };
    
        

        return fetch(`${config.apiUrl}/API/PermissionGroups/DeletePermissionGroup?name=${rows.Name}`, requestOptions).then(handleResponse);
    }

}


    
export { PermissionGroupsApiProvider };
