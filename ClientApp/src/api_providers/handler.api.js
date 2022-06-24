function handleResponse(response) {
    return new Promise((resolve, reject) => {
        if (response.ok) {
            // return json if it was returned in the response
            var contentType = response.headers.get("content-type");
            if (contentType && contentType.includes("application/json")) {
                response.json().then(json => resolve(json));
            } else {
                resolve();
            }
        } else {
            // return error message from response body
            if(response.status == 403) {
                let error = { Success: false, ErrorCode: "AUTH001", Data: null, Message: "Unauthorized!" };
                
                // To catch in next .catch, use reject
                return reject(error);

                // To catch in next .then, use resolve
                return resolve(error);
            }
            // console.log(response.status);
            // response.text().then(text => {
            //     console.log(text);
            //     reject(text);            
            // });
        }
    });
}

function handleError(error) {
    // Convert it into a resolve so can be caught by .then()
    return Promise.resolve(error);
}

export { handleResponse, handleError };