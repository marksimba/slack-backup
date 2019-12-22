const setUrlParams = params => dispatch => {
    dispatch({
        type: 'SET_URL_PARAMS',
        payload: {
            params
        }
    })
};

const setChannels = channels => dispatch => {
    dispatch({
        type: 'SET_CHANNELS',
        payload: {
            channels
        }
    })
};

const setUsers = users => dispatch => {
    dispatch({
        type: 'SET_USERS',
        payload: {
            users
        }
    })
};

const setMessages = messages => dispatch => {
    dispatch({
        type: 'SET_MESSAGES',
        payload: {
            messages
        }
    })
};

export {
    setUrlParams,
    setChannels,
    setUsers,
    setMessages
};