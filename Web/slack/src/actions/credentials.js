const setAWSCognitoCredentials = credentials => dispatch => {
    dispatch({
        type: 'SET_AWS_COGNITO_CREDENTIALS',
        payload: {
            credentials
        }
    })
};

export {
    setAWSCognitoCredentials
};