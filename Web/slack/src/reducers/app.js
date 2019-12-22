const initialState = {
    params: {}
}

const app = ( state = initialState, action ) => {
    const { type, payload } = action;
    switch( type ) {
        case "SET_URL_PARAMS" : {
            const { params } = payload;
            return {
                ...state,
                params
            };
        }
        case "SET_CHANNELS" : {
            const { channels } = payload;
            let channelObj = {};
            channels
                .forEach(channel => {
                                        const parsedChannel = JSON.parse(channel.value);
                                        channelObj[parsedChannel.id] = parsedChannel
                                    })
            return {
                ...state,
                channels: channelObj
            };
        }
        case "SET_USERS" : {
            const { users } = payload;
            let usersObj = {}
            users
                .forEach(user => {
                    const parsedUser = JSON.parse(user.value);
                    usersObj[parsedUser.id] = parsedUser
                })
            return {
                ...state,
                users: usersObj
            };
        }
        case "SET_MESSAGES" : {
            const { messages } = payload;
            const formatedMessages = messages
                                        .map(message => JSON.parse(message.value))
                                        .sort((a,b)=>{return new Date(b.ts * 1000) - new Date(a.ts * 1000)})
            return {
                ...state,
                messages: formatedMessages
            };
        }
        default:
            return state;
    }
};

export default app;