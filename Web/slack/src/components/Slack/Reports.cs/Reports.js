import React, { useState, useEffect } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { setMessages } from '../../../actions/app';
import { COUNT_MESSAGES_BY_USER, COUNT_MESSAGES_BY_CHANNEL, COUNT_MESSAGE_BY_USERS_AND_CHANNEL } from '../../../constants/query';

/** AWS  */
import AWS from 'aws-sdk';
import LAMBDA from '../../../constants/lambda'

/* React Spinners */
import "react-loader-spinner/dist/loader/css/react-spinner-loader.css"
import Loader from 'react-loader-spinner'

/* React Semantic */
import { Message, Label } from 'semantic-ui-react'

const Reports = ({
    credentials,
}) => {


    const [loading, setLoading] = useState(false);
    const [loadingMessage, setLoadingMessage] = useState();

    /**
     * Gets report from Athena.
     * 
     * Using the report string, query Athena to get the report. 
     * With the report, download object as csv
     * 
     * @param {string} report 
     */

    const getReport = (report) => {
        setLoading(true);
        setLoadingMessage(`Getting report: ${report}`);
        AWS.config.update({ region: LAMBDA.region });
        AWS.config.credentials = credentials;

        const lambda = new AWS.Lambda({apiVersion: LAMBDA.apiVersion});

        let params = {
            FunctionName: LAMBDA.functionName,
        }

        switch(report){
            case "Message Count By Channel":
                params.Payload = JSON.stringify({sql: COUNT_MESSAGES_BY_CHANNEL})
                break;
            case "Message Count By User":
                params.Payload = JSON.stringify({sql: COUNT_MESSAGES_BY_USER})
                break;
            case "Message Count By User and Channel":
                params.Payload = JSON.stringify({sql: COUNT_MESSAGE_BY_USERS_AND_CHANNEL})
                break;
            default:
                break;
        }
        
        
        lambda.invoke(params)
            .promise()
            .then(resp => (JSON.parse(resp.Payload).results.Items))
            .then((data) => {
                console.log(data)
                let csv = "";
                const keys = Object.keys(data[0]);
                keys.forEach(key => {
                    csv += `${key},`
                })
                csv += "\r\n"
                data.forEach(line => {
                    keys.forEach(key =>{
                        csv += `${line[key]},`
                    })
                    csv += "\r\n"
                })
                downloadCsv(csv, `${report} - ${new Date().toLocaleDateString()}.csv`)
                setLoading(false);
            })
            .catch(err => console.error(err))

    }

    /**
     * Takes a csv and downloads it.
     * 
     * @param {string} csv 
     * @param {sring} name 
     */

    const downloadCsv = (csv, name) => {
        var blob = new Blob([csv], {type: 'data:text/csv;charset=utf-8;'})
        var link = document.createElement('a');
        var url = URL.createObjectURL(blob)
        link.setAttribute('href', url)
        link.setAttribute('download', `${name}`)
        link.style.visibility = 'hidden'
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }

    return (
           <Message>
               {
                !loading ?
                [
               <Message.Header style={{
                   textAlign: 'center'
               }}>
                Reports:
               </Message.Header>,
               <Label.Group>
                   <Label 
                    as='a'
                    onClick={(e,d)=>{getReport("Message Count By Channel")}}
                    >
                    Message Count By Channel
                   </Label >
                   <Label 
                    as='a'
                    key='COUNT_MESSAGES_BY_USER'
                    onClick={(e,d)=>{getReport("Message Count By User")}}
                    >
                    Message Count By User
                   </Label>
                   <Label 
                    as='a'
                    key='COUNT_MESSAGE_BY_USERS_AND_CHANNEL'
                    onClick={(e,d)=>{getReport("Message Count By User and Channel")}}
                >
                    Message Count By User and Channel
                   </Label>
               </Label.Group>
                ]
               :
               <Message.Header style={{
                    textAlign: 'center'
                }}>
                    {loadingMessage}
                <Loader 
                    type='BallTriangle'
                    color="#00BFFF"
                    height={100}
                    width={100}
                />
                </Message.Header>
               }
           </Message> 
    )
}

const mapStateToProps = state => ({
    credentials: state.credentials.credentials
});

export default connect(
  mapStateToProps,
  null
)(Reports);