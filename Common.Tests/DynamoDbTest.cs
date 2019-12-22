using System;
using System.Collections.Generic;
using Amazon.Runtime;
using Xunit;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Common.Tests
{
    public class DynamoDbTest
    {

        string _table;
        Dynamo _dynamo;
        BasicAWSCredentials _creds;
        
        public DynamoDbTest()
        {
            _creds = new BasicAWSCredentials("AKIAT3M4UPJXJRI4KGZV", "+RG+NZftosCe0ssljiw0EL7R35RwCZW+QoCBrAsb");
            _table = "slack";
            _dynamo = new Dynamo(_table, _creds);
        }

        [Fact]
        public void DynamoDbConstructorTest()
        {
            var test = new Dynamo(_table);
            Assert.NotNull(test);
        }

        [Fact]
        public void PutItemTest()
        {
            var item = new Dictionary<string, AttributeValue>()
            {
                {"Key", new AttributeValue{ S = "Key"}},
                {"EntityType", new AttributeValue{ S = "Message"}},
                {"Value", new AttributeValue{ S = "{\"key\":\"value\"}"}},
                {"Channel", new AttributeValue{ S = "Test"}}
            };

            var result = _dynamo.putItem(item).Result;
            Assert.Equal(Status.Success, result);
        }
    }
}