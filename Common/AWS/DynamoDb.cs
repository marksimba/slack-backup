using System;
using System.Collections.Generic;
using Amazon;
using Amazon.Runtime;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Threading.Tasks;

namespace Common
{
    public enum Status
    {
        Success,
        Failure
    }
    public class Dynamo
    {
        AmazonDynamoDBClient _client;
        string _table;
        RegionEndpoint _region = RegionEndpoint.USWest2;

        // Constructer only used for testing
        public Dynamo(string table, BasicAWSCredentials credentials, RegionEndpoint region = null)
        {
            //Default region is defined in the class. You can pass the region if you'd like
            _client = new AmazonDynamoDBClient(credentials, region == null ? _region : RegionEndpoint.USWest2); 
            _table = table;
        }

        // Primary constructer
        public Dynamo(string table, RegionEndpoint region = null)
        {
            //Default region is defined in the class. You can pass the region if you'd like
            _client = new AmazonDynamoDBClient(region == null ? _region : RegionEndpoint.USWest2); 
            _table = table;
        }

        /// <summary>
        /// A Function that takes Dictionry Item, and adds/updates a record in DynamoDb
        /// </summary>
        /// <param name="item"> Item to add to DynamoDb </param>\
        /// <returns>Status</returns>

        public async Task<Status> putItem(Dictionary<string, AttributeValue> item)
        {
            //Always add a TimeStamp
            item.Add(
                    "Timestamp",
                    new AttributeValue{ S = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")}
            );

            var request = new PutItemRequest
            {
                TableName = _table,
                Item = item
            };

            try
            {
                var results = _client.PutItemAsync(request).Result;
                return Status.Success;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return Status.Failure;
            }

        }

        /// <summary>
        /// A Function that gets raw data and calls putItem to process
        /// </summary>
        /// <param name="key"> Primary Key</param>
        /// <param name="entityType"> Channel, User, or Message</param>
        /// <param name="value"> Raw json </param>
        /// <param name="channel"> What channel it's associated with</param>
        /// <param name="messageTs"> Message Primary Key</param>
        /// <returns></returns>
        public async Task<Status> CreateOrUpdateItem(string key, string entityType, string value, string channel = null, string messageTs = null)
        {
             var item = new Dictionary<string, AttributeValue>()
            {
                {"Key", new AttributeValue{ S = key}},
                {"EntityType", new AttributeValue{ S = entityType}},
                {"Value", new AttributeValue{ S = value}}
            };

            if(channel != null)
            {
                item.Add("Channel", new AttributeValue{ S = channel});
            }

            if(messageTs != null)
            {
                item.Add("MessageTs", new AttributeValue{ S = messageTs});
            }

            return await putItem(item);
        }
        
    }
}