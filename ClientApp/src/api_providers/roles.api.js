import { authHeader, config } from '../helpers';
import { handleError, handleResponse } from './handler.api';


class RolesApiProvider {
    static async getRoles() {
        const requestOptions = {
            method: 'get',
            headers: authHeader()
        };

        return fetch(`${config.apiUrl}/api/Roles/GetRoles`, requestOptions)
            .then(handleResponse, handleError)
            .then(result => {
                return result;
            });
    }


    static async addRoles(rows) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;
        let body = {
            "Name": rows.Name,
            "Description": rows.Description,
            "Level": rows.Level 
            }
        const requestOptions = {
            method: 'POST',
            headers: headers,
            body: JSON.stringify(body)
        };
    
        return fetch(`${config.apiUrl}/api/Roles/AddRole`, requestOptions).then(handleResponse);
    }

    static async updateRoles(name, rows) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;

        let body = {
            "Name": rows.Name,
            "Description": rows.Description,
            "Level": rows.Level  
            }

        const requestOptions = {
            method: 'PUT',
            headers: headers,
            body: JSON.stringify(body)
        };
    
        return fetch(`${config.apiUrl}/api/Roles/UpdateRole?name=${name}`, requestOptions).then(handleResponse, handleError).catch(handleError);
    }

    static async deleteRoles(rows) {
        let headers = authHeader();
 
        const requestOptions = {
            method: 'DELETE',
            headers: headers
        };
    
        

        return fetch(`${config.apiUrl}/API/Roles/DeleteRole?name=${rows.Name}`, requestOptions).then(handleResponse);
    }

}


    
export { RolesApiProvider };
