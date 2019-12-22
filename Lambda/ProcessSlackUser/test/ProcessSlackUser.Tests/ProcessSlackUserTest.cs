using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Common.AWS.DataModels;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Runtime;

using ProcessSlackUser;

namespace ProcessSlackUser.Tests
{
    public class ProcessSlackUserTest
    {
        BasicAWSCredentials _creds;
         public void FunctionHandlerTest()
        {

            SQSRoot testPayload = new SQSRoot()
            {
                Records = new List<SQSRecord>(){
                    new SQSRecord(){
                        MessageId = "ff203af4-1376-48ad-83ff-d1145eb4fc74",
                        ReceiptHandle = "AQEBQpMktLTgcHhaZyszU6UNgDznQV/5fh9vzDvGnzFb1irSFIvkwXxlTzcAkzZeT/RpQ1UJpDqicp9V3auXh1vYrvNjZV5N1twghFYnzbiLo0sL6K//D2USOwqETU7CN6D8pdVFz0YAtm3mbsXihYH2Ww6PCudSASeNNR2KnqBGopERTgWnyApADRQIAJoDmtEzv35VReyyWX2mjURkwr37FyZx6DaoBwL4wDPvS+Hg2V+9XnUMHpbREsNQEIMR0IKN424FBT10+SRyq68Ovkk+IV2mzJOlUZKbEgDZNli5u2h0Cy1AsN7IdBIh6duyml0cVSznOdo27D1OXkNK+x5YkQHc2MY47WHwQfUykfG7a1+hDZg/2Cx32G4l3zYFw5/QcvBTbQYLa0mmn9vMQCbcCg==",
                        Body = "{\"client_msg_id\":\"1c96e27c-a78f-4109-b7bc-2a96dcb0b3a8\",\"type\":\"message\",\"text\":\"test\",\"user\":\"U9PUT4GTU\",\"ts\":\"1576272595.008600\",\"team\":\"T9PUT4GH0\",\"channel\":\"C9PSEKQ3D\",\"event_ts\":\"1576257943.001800\",\"channel_type\":\"channel\"}",
                        Attributes = "",
                        MessageAttributes = "",
                        Md5OfBody = "c549fd3a58a0caf46512ff7ffc7aed69",
                        EventSource = "aws:sqs",
                        EventSourceARN = "arn:aws:sqs:us-west-2:265005857390:slack-messages",
                        AwsRegion = "us-west-2"
                    }
                }
            };
            var function = new ProcessSlackUser(_creds);
            var context = new TestLambdaContext();
            function.FunctionHandler(testPayload, context);
            //Assert.Equal("HELLO WORLD", upperCase);
        }
    }
}
