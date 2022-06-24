import { authHeader, config } from '../helpers';
import { handleError, handleResponse } from './handler.api';


class RolePermissionGroupMappingsApiProvider {
    static async getRolePermissionGroupMappings() {
        const requestOptions = {
            method: 'get',
            headers: authHeader()
        };

        return fetch(`${config.apiUrl}/api/RolePermissionGroupMappings/GetRolePermissionGroupMappings`, requestOptions)
            .then(handleResponse, handleError)
            .then(result => {
                return result;
            });
    }


    static async addRolePermissionGroupMapping(rows) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;
        let body = {
            "Role": rows.Role,
            "PermissionGroup": rows.PermissionGroup,
            "UserAccessLevel": rows.UserAccessLevel,
            "IsSystem": rows.IsSystem 
        }
        const requestOptions = {
            method: 'POST',
            headers: headers,
            body: JSON.stringify(body)
        };
    
        return fetch(`${config.apiUrl}/api/RolePermissionGroupMappings/AddRolePermissionGroupMapping`, requestOptions).then(handleResponse);
    }

    static async updateRolePermissionGroupMapping( rows) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;

        let body = {
            "Role": rows.Role,
            "PermissionGroup": rows.PermissionGroup,
            "UserAccessLevel": rows.UserAccessLevel,
            "IsSystem": rows.IsSystem  
        }

        const requestOptions = {
            method: 'PUT',
            headers: headers,
            body: JSON.stringify(body)
        };
    
        return fetch(`${config.apiUrl}/api/RolePermissionGroupMappings/UpdateRolePermissionGroupMapping?Id=${rows.Id}`, requestOptions).then(handleResponse);
    }

    static async deleteRolePermissionGroupMapping(rows) {
        let headers = authHeader();
        console.log(rows.Id);
        const requestOptions = {
            method: 'DELETE',
            headers: headers
        };
    
        

        return fetch(`${config.apiUrl}/API/RolePermissionGroupMappings/DeleteRolePermissionGroupMapping?Id=${rows.Id}`, requestOptions).then(handleResponse);
    }

}


    
export { RolePermissionGroupMappingsApiProvider };
