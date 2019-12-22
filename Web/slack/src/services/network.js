import URLS from '../constants/urls';
import COGNITO from '../constants/cognito';

/**
 * Exchanges AWS Cognito code for AWS tokens.
 * 
 * Upon successfully authentiction, AWS Cognito will redirect to this app
 * with the 'code' paramter. Using that value, this function returns the
 * needed tokens to access AWS resources.
 * 
 * @param {string} code AWS Cognito access code.
 * 
 * @returns {obj} AWS Object containing access tokens
 */

const getAWSCognitoTokens = ( code ) => {
    const Authorization = `Basic ${btoa(`${COGNITO.clientID}:${COGNITO.clientSecret}`)}`;
    const params = {
        'code': code,
        'grant_type': 'authorization_code',
        'client_id': COGNITO.clientID,
        'redirect_uri': URLS.redirectUri
    }
   const body = Object.keys(params).map(key => {
        return `${encodeURIComponent(key)}=${encodeURIComponent(params[key])}`
   }).join('&');

    return fetch(URLS.token, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                Authorization
            },
            body
        })
        .then( response => response.json())
        .catch(err => console.error(err))
};

export{
    getAWSCognitoTokens
};