import { authHeader, config } from '../helpers';
import { handleError, handleResponse } from './handler.api';


class UserAccessLevelsApiProvider {
    static async getUserAccessLevels() {
        const requestOptions = {
            method: 'get',
            headers: authHeader()
        };

        return fetch(`${config.apiUrl}/api/UserAccessLevels/GetUserAccessLevels`, requestOptions)
            .then(handleResponse, handleError)
            .then(result => {
                return result;
            });
    }


    static async addUserAccessLevels(rows) {
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
    
        return fetch(`${config.apiUrl}/api/UserAccessLevels/AddUserAccessLevel`, requestOptions).then(handleResponse);
    }

    static async updateUserAccessLevels(name, rows) {
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
    
        return fetch(`${config.apiUrl}/api/UserAccessLevels/UpdateUserAccessLevel?name=${name}`, requestOptions).then(handleResponse, handleError).catch(handleError);
    }

    static async deleteUserAccessLevels(rows) {
        let headers = authHeader();
 
        const requestOptions = {
            method: 'DELETE',
            headers: headers
        };
    
        

        return fetch(`${config.apiUrl}/API/UserAccessLevels/DeleteUserAccessLevel?name=${rows.Name}`, requestOptions).then(handleResponse);
    }

}


    
export { UserAccessLevelsApiProvider };
