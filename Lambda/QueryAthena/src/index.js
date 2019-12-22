const aws = require('aws-sdk');
const AthenaExpress = require('athena-express');
const awsCredentials = {
    region: 'us-west-2'
};

aws.config.update(awsCredentials);

const athenaExpressConfig = { 
    aws,
    db: 'dynamodb_exports',
    getStats: true
};


/**
 * Takes a slack event and queries athena
 * 
 * @param {event} aws event
 * 
 * @returns {obj} athena results
 */
exports.handler = async (event) => {
    const { sql } = event;

    const athenaExpress = new AthenaExpress(athenaExpressConfig);
    
    const myQuery = {
        sql
    }
    
    const results = await athenaExpress.query(myQuery);

    const response = {
        results,
        statusCode: 200,
    };
    return response;
};