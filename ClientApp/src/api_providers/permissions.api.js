import { authHeader, config } from '../helpers';
import { handleError, handleResponse } from './handler.api';


class PermissionsApiProvider {
    static async getPermissions() {
        const requestOptions = {
            method: 'get',
            headers: authHeader()
        };

        return fetch(`${config.apiUrl}/api/Permissions/GetPermissions`, requestOptions)
            .then(handleResponse, handleError)
            .then(result => {
                return result;
            });
    }


    static async addPermissions(rows) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;
        let body = {
            "Name": rows.Name,
            "Description": rows.Description 
            }
        const requestOptions = {
            method: 'POST',
            headers: headers,
            body: JSON.stringify(body)
        };
    
        return fetch(`${config.apiUrl}/api/Permissions/AddPermission`, requestOptions).then(handleResponse);
    }

    static async updatePermissions(name, rows) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;

        let body = {
            "Name": rows.Name,
            "Description": rows.Description 
            }

        const requestOptions = {
            method: 'PUT',
            headers: headers,
            body: JSON.stringify(body)
        };
    
        return fetch(`${config.apiUrl}/api/Permissions/UpdatePermission?name=${name}`, requestOptions).then(handleResponse);
    }

    static async deletePermissions(rows) {
        let headers = authHeader();
 
        const requestOptions = {
            method: 'DELETE',
            headers: headers
        };
    
        

        return fetch(`${config.apiUrl}/API/Permissions/DeletePermission?name=${rows.Name}`, requestOptions).then(handleResponse);
    }

}


    
export { PermissionsApiProvider };
