import React, { useEffect, useState } from 'react';
import AWS from 'aws-sdk';
import { LIST_CHANNELS, LIST_USERS} from '../../constants/query';
import LAMBDA from '../../constants/lambda'
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { setChannels, setUsers, setMessages } from '../../actions/app';
import Messages from './Messages/Messages';

/* React Semantic */
import 'semantic-ui-css/semantic.min.css'
import { Segment, Grid, Dropdown, Form, Button, Header } from 'semantic-ui-react'

/* React Spinners */
import "react-loader-spinner/dist/loader/css/react-spinner-loader.css"
import Loader from 'react-loader-spinner'

import './Slack.css';
import Reports from './Reports.cs/Reports';

const topBottomMargin = 50;

const channelOptions = ( channels ) => {
    if( channels ) {
        const channelKeys = Object.keys(channels)
        const channelOptions = channelKeys.map( value => 
                                ({
                                    key: value,
                                    value: value,
                                    text: channels[value].name,
                                }))
                                //return and sorty by channel name
        return channelOptions.sort((a,b) => (a.text > b.text) ? 1 : ((b.text > a.text) ? -1 : 0))
    }
}

const Slack = ({
    credentials,
    channels,
    setChannels,
    setUsers
}) => {

    const [height, setHeight] = useState('200px');
    const [loading, setLoading] = useState(true);
    const [loadingMessage, setLoadingMessage] = useState('Preparing view from your Slack Archives');
    const [selectedChannel, setSelectedChannel] = useState();
    const [searchTerm, setSearchTerm] = useState();

    // Loads Channels and Messages. 

    useEffect(() => {
        AWS.config.update({ region: LAMBDA.region });
        AWS.config.credentials = credentials;

        const lambda = new AWS.Lambda({apiVersion: LAMBDA.apiVersion});

        let athenaPromises = [];

        // Load channels and users asynchronously.

        /** Channels Query */
        const channelParams = {
            FunctionName: LAMBDA.functionName,
            Payload :  JSON.stringify({sql: LIST_CHANNELS})
        };

        athenaPromises.push(
            lambda.invoke(channelParams)
                .promise()
                .then(resp => (JSON.parse(resp.Payload).results.Items))
            )

        /** Users Query */
        const userParams = {
            FunctionName: LAMBDA.functionName,
            Payload :  JSON.stringify({sql: LIST_USERS})
        };
        athenaPromises.push(
            lambda.invoke(userParams)
                .promise()
                .then(resp => (JSON.parse(resp.Payload).results.Items))
        )

        Promise.all(athenaPromises)
            .then((data) => {
                setChannels(data[0]);
                setUsers(data[1]);
                setLoading(false);
            })
            .catch(err => console.error(err))

    }, []);

    // Every time the page updates, grab the height
    // and update the height which is used in the 
    // main component

    useEffect(() => {
        const height = window.innerHeight
        setHeight(`${ height - ( topBottomMargin * 2 ) }px`)
    });

    return (
            !loading ?
            <Segment 
                style={{
                    margin: 'auto',
                    marginTop: topBottomMargin,
                    width: '95%',
                    height
                }}
            >
            <Header attatched='top'>
                <Reports/>
            </Header>
            <Grid>
                <Grid.Row
                    style={{
                        height: '20px'
                    }}
                >
                    <Grid.Column width={4} style={{
                        paddingTop: '0px'
                    }}>
                    <label>Channel:</label>
                    </Grid.Column>
                    <Grid.Column width={5}/>
                    <Grid.Column width={5} style={{
                        paddingTop: '0px'
                    }}>
                        <label>Search Term:</label>
                    </Grid.Column>
                    <Grid.Column width={1}/>
                </Grid.Row>
                <Grid.Row>
                    <Grid.Column width={4} style={{
                        paddingTop: '0px'
                    }}>
                        <Dropdown
                            placeholder='Select Channel'
                            fluid
                            search
                            selection
                            onChange={(_ ,d) => {setSelectedChannel(d.value)}}
                            options={channelOptions(channels)}
                        />
                    </Grid.Column>
                    <Grid.Column width={5}/>
                    <Grid.Column width={5} style={{
                        paddingTop: '0px'
                    }}>
                        <Form>
                            <Form.Input 
                                placeholder='Search...'
                                onChange={(e, d) => {
                                    setSearchTerm(d.value)
                                }}
                                />
                        </Form>
                    </Grid.Column>
                    <Grid.Column width={1} style={{
                        padding: '0px'
                    }}>
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row>
                    {
                        selectedChannel ?
                        <Messages 
                            channel={channels[selectedChannel]} 
                            searchTerm={searchTerm}
                        /> : ''
                    }
                </Grid.Row>
            </Grid> 
            </Segment> :
            <div>
                <div 
                style={{
                    position: 'fixed',
                    top: '47%',
                    left: '45%',
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
                    type='CradleLoader'
                    color="#00BFFF"
                    height={100}
                    width={100}
                />
            </div>
    );
}

const mapStateToProps = state => ({
    credentials: state.credentials.credentials,
    channels: state.app.channels,
    users: state.app.users,
    messages: state.app.messages
});

const mapDispatchToProps = dispatch => ({
    setChannels: bindActionCreators(setChannels, dispatch),
    setUsers: bindActionCreators(setUsers, dispatch),
    setMessages: bindActionCreators(setMessages, dispatch)
});

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(Slack);