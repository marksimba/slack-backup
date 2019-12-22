const initialState = {
    authenticated: false
}

const credentials = ( state = initialState, action ) => {
    const { type, payload } = action;
    switch( type ) {
        case "SET_AWS_COGNITO_CREDENTIALS" : {
            const { credentials } = payload;
            
            return {
                ...state,
                authenticated: true,
                credentials
            };
        }
        default:
            return state;
    }
};

export default credentials;