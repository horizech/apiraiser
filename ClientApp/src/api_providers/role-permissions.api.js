import { authHeader, config } from '../helpers';
import { handleError, handleResponse } from './handler.api';


class RolePermissionsApiProvider {
    static async getRolePermissions() {
        const requestOptions = {
            method: 'get',
            headers: authHeader()
        };

        return fetch(`${config.apiUrl}/api/RolePermissions/GetRolePermissions`, requestOptions)
            .then(handleResponse, handleError)
            .then(result => {
                return result;
            });
    }


    static async addRolePermissions(rows) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;
        let body = {
            "Role": rows.Role,
            "Permission": rows.Permission 
            }
        const requestOptions = {
            method: 'POST',
            headers: headers,
            body: JSON.stringify(body)
        };
    
        return fetch(`${config.apiUrl}/api/RolePermissions/AddRolePermission`, requestOptions).then(handleResponse);
    }

    static async updateRolePermissions( rows) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;

        let body = {
            "Role": rows.Role,
            "Permission": rows.Permission 
            }

        const requestOptions = {
            method: 'PUT',
            headers: headers,
            body: JSON.stringify(body)
        };
    
        return fetch(`${config.apiUrl}/api/RolePermissions/UpdateRolePermission?Id=${rows.Id}`, requestOptions).then(handleResponse);
    }

    static async deleteRolePermissions(rows) {
        let headers = authHeader();
 
        const requestOptions = {
            method: 'DELETE',
            headers: headers
        };
    
        

        return fetch(`${config.apiUrl}/API/RolePermissions/DeleteRolePermission?Id=${rows.Id}`, requestOptions).then(handleResponse);
    }

}


    
export { RolePermissionsApiProvider };
