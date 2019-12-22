const LIST_CHANNELS = `SELECT value FROM snapshots_slack WHERE entitytype = 'channel';`;
const LIST_USERS = `SELECT value FROM snapshots_slack WHERE entitytype = 'user';`;

/**
 * Using a Slack Channel ID, return properly sql string.
 * 
 * @param {string} channel Slack Channel.
 * 
 * @returns {string} Sql query string
 */
const LIST_MESSAGES_BY_CHANNEL = ( channel ) => {
    return `SELECT messages.value FROM snapshots_slack AS messages JOIN snapshots_slack AS channels ON messages.channel = json_extract_scalar(channels.value, '$.id') WHERE messages.channel = '${channel}' AND messages.entityType = 'message';`
}

const COUNT_MESSAGES_BY_USER = `SELECT distinct(json_extract_scalar(p1.value, '$.user')) as userId, json_extract_scalar(p2.value, '$.name') as Name, COUNT(json_extract_scalar(p1.value, '$.user')) as messageCount
FROM snapshots_slack as p1 join snapshots_slack as p2
on json_extract_scalar(p1.value, '$.user') = json_extract_scalar(p2.value, '$.id')
WHERE p1.entitytype = 'message'
GROUP BY json_extract_scalar(p1.value, '$.user'), json_extract_scalar(p2.value, '$.name'), p1.entitytype
ORDER BY COUNT(json_extract_scalar(p1.value, '$.user')) Desc
;`;

const COUNT_MESSAGES_BY_CHANNEL = `SELECT COUNT(messages.value) as Count, json_extract_scalar(channels.value, '$.name') as Channel
FROM snapshots_slack AS messages 
   JOIN snapshots_slack AS channels
       ON messages.channel = json_extract_scalar(channels.value, '$.id')
GROUP BY json_extract_scalar(channels.value, '$.name');`;


const COUNT_MESSAGE_BY_USERS_AND_CHANNEL = `SELECT COUNT(json_extract_scalar(users.value, '$.name')) as MessageCount, json_extract_scalar(users.value, '$.name') as Name, json_extract_scalar(channels.value, '$.name') as Channel
FROM snapshots_slack AS messages 
   JOIN snapshots_slack AS users
       ON json_extract_scalar(messages.value, '$.user') = json_extract_scalar(users.value, '$.id')
   JOIN snapshots_slack AS channels
       ON messages.channel = json_extract_scalar(channels.value, '$.id')
GROUP BY json_extract_scalar(users.value, '$.name'), json_extract_scalar(channels.value, '$.name')
ORDER BY json_extract_scalar(channels.value, '$.name');`;

export {
    LIST_CHANNELS,
    LIST_USERS,
    LIST_MESSAGES_BY_CHANNEL,
    COUNT_MESSAGES_BY_USER,
    COUNT_MESSAGES_BY_CHANNEL,
    COUNT_MESSAGE_BY_USERS_AND_CHANNEL
};