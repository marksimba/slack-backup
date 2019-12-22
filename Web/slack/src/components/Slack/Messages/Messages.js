import React, { useState, useEffect } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { setMessages } from '../../../actions/app';
import { LIST_MESSAGES_BY_CHANNEL } from '../../../constants/query';

/** AWS  */
import AWS from 'aws-sdk';
import LAMBDA from '../../../constants/lambda'

/* React Spinners */
import "react-loader-spinner/dist/loader/css/react-spinner-loader.css"
import Loader from 'react-loader-spinner'

/* React Semantic */
import { Segment, Feed, Header } from 'semantic-ui-react'

const Messages = ({
    channel,
    users,
    setMessages,
    messages,
    credentials,
    searchTerm
}) => {

    const [height, setHeight] = useState('200px');
    const [loading, setLoading] = useState(true);
    const [loadingMessage, setLoadingMessage] = useState(`Loading Messages For '${channel.name}'`);
    const [filteredMessages, setFilteredMessages] = useState(messages);

    // Load Messages for {channel} whenever the channel changes.
    // Enable loader at the begin, and disable at the end.

    useEffect(() => {
        setLoading(true);
        setLoadingMessage(`Loading Messages For '${channel.name}'`);
        AWS.config.update({ region: LAMBDA.region });
        AWS.config.credentials = credentials;

        const lambda = new AWS.Lambda({apiVersion: LAMBDA.apiVersion});

        /** Messages Query */
        const messagesParams = {
            FunctionName: LAMBDA.functionName,
            Payload :  JSON.stringify({sql: LIST_MESSAGES_BY_CHANNEL(channel.id)})
        };
        
        lambda.invoke(messagesParams)
            .promise()
            .then(resp => (JSON.parse(resp.Payload).results.Items))
            .then((data) => {
                setMessages(data)
                setLoading(false);
            })
            .catch(err => console.error(err))

    }, [channel]);

    // Update filtered messages to be all messages, whenever messages changes.

    useEffect(() => {
        setFilteredMessages(messages)
    }, [messages])

    // Filter the messages whenever the search term changes. 

    useEffect(() => {
        if( messages ) {
            setFilteredMessages(
                messages
                    .filter(item => item.text.toLowerCase().includes(searchTerm.toLowerCase()))
            )
        }
    }, [searchTerm])

    // Every time the page updates, grab the height
    // and update the height which is used in the 
    // main component

    useEffect(() => {
        const height = window.innerHeight
        setHeight(`${ height/1.7 }px`)
    });
   

    return (
            !loading ? 
            <div
                style={{
                    margin: 'auto',
                    width: '95%',
                    height
                }}
            >
                <Header 
                    style={{
                        textAlign:'center'
                    }}
                    basic 
                    as='h5' 
                    attached='top'
                    >
                    {`Messages for channel: ${channel.name}`}
                </Header>
                <Segment
                    style={{
                        margin: '0px',
                        height,
                        overflowY: 'auto'
                    }}
                >
                <Feed>
                {
                    filteredMessages.map(message =>
                        <Feed.Event>
                        <Feed.Content>
                            <Feed.Summary>
                            <Feed.User>{users[message.user].profile.display_name}</Feed.User>
                            <Feed.Date>{new Date(message.ts * 1000).toLocaleDateString()}</Feed.Date>
                            </Feed.Summary>
                          <Feed.Extra text>
                            {message.text}
                          </Feed.Extra>
                        </Feed.Content>
                        </Feed.Event>
                        )
                }
                </Feed>
                </Segment>
            </div> :
            <div>
                <div 
                style={{
                    position: 'fixed',
                    top: '47%',
                    left: '47%',
                }}
                >
                    {loadingMessage}
                </div>
                <br/>
                <br/>
                <Loader 
                    style={{
                        position: 'fixed',
                        top: '50%',
                        left: '50%',
                    }}
                    type='MutatingDots'
                    color="#00BFFF"
                    height={100}
                    width={100}
                />
            </div>
            
    )
}

const mapStateToProps = state => ({
    messages: state.app.messages,
    users: state.app.users,
    credentials: state.credentials.credentials
});

const mapDispatchToProps = dispatch => ({
    setMessages: bindActionCreators(setMessages, dispatch)
});

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(Messages);