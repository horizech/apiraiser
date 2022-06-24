import { authHeader, config } from '../helpers';
import { handleError, handleResponse } from './handler.api';


class PermissionGroupMappingsApiProvider {
    static async getPermissionGroupMappings() {
        const requestOptions = {
            method: 'get',
            headers: authHeader()
        };

        return fetch(`${config.apiUrl}/API/PermissionGroupMappings/GetPermissionGroupMappings`, requestOptions)
            .then(handleResponse, handleError)
            .then(result => {
                return result;
            });
    }


    static async addPermissionGroupMapping(rows) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;
        let body = {
            "Permission": rows.Permission,
            "PermissionGroup": rows.PermissionGroup,
            "IsSystem": rows.IsSystem 
            }
        const requestOptions = {
            method: 'POST',
            headers: headers,
            body: JSON.stringify(body)
        };
    
        return fetch(`${config.apiUrl}/api/PermissionGroupMappings/AddPermissionGroupMapping`, requestOptions).then(handleResponse);
    }

    static async updatePermissionGroupMapping( rows) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;

        let body = {
            "Permission": rows.Permission,
            "PermissionGroup": rows.PermissionGroup,
            "IsSystem": rows.IsSystem 
            }

        const requestOptions = {
            method: 'PUT',
            headers: headers,
            body: JSON.stringify(body)
        };
    
        return fetch(`${config.apiUrl}/api/PermissionGroupMappings/UpdatePermissionGroupMapping?Id=${rows.Id}`, requestOptions).then(handleResponse);
    }

    static async deletePermissionGroupMapping(rows) {
        let headers = authHeader();
 
        const requestOptions = {
            method: 'DELETE',
            headers: headers
        };
    
        

        return fetch(`${config.apiUrl}/API/PermissionGroupMappings/DeletePermissionGroupMapping?Id=${rows.Id}`, requestOptions).then(handleResponse);
    }

}


    
export { PermissionGroupMappingsApiProvider };
