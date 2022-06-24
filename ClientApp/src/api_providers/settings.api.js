import { authHeader, config } from '../helpers';
import { handleError, handleResponse } from './handler.api';


class SettingsApiProvider {
    static async getSettings() {
        const requestOptions = {
            method: 'get',
            headers: authHeader()
        };

        return fetch(`${config.apiUrl}/api/Settings/GetSettings`, requestOptions)
            .then(handleResponse, handleError)
            .then(result => {
                return result;
            });
    }


    static async addSettings(rows) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;
        let body = {
            "Key": rows.Key,
            "Value": rows.Value 
            }
        const requestOptions = {
            method: 'POST',
            headers: headers,
            body: JSON.stringify(body)
        };
    
        return fetch(`${config.apiUrl}/api/Settings/AddSetting`, requestOptions).then(handleResponse);
    }

    static async updateSettings(key, rows) {
        let headers = authHeader();
        headers['Content-Type'] = 'application/json' ;

        let body = {
            "Key": rows.Key,
            "Value": rows.Value 
            }

        const requestOptions = {
            method: 'PUT',
            headers: headers,
            body: JSON.stringify(body)
        };
    
        return fetch(`${config.apiUrl}/api/Settings/UpdateSetting?key=${key}`, requestOptions).then(handleResponse);
    }

    static async deleteSettings(rows) {
        let headers = authHeader();
 
        const requestOptions = {
            method: 'DELETE',
            headers: headers
        };
    
        

        return fetch(`${config.apiUrl}/API/Settings/DeleteSetting?key=${rows.Key}`, requestOptions).then(handleResponse);
    }

}


    
export { SettingsApiProvider };
