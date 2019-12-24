using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Runtime;

namespace Lambda.Tests
{
    public class SlackBackupTest
    {
        BasicAWSCredentials _creds;
        public SlackBackupTest()
        {
            //Channel to message
            Environment.SetEnvironmentVariable("CHANNEL", "");
            //Slack token
            Environment.SetEnvironmentVariable("SLACKTOKEN", "");
            // Message Queue
            Environment.SetEnvironmentVariable("MESSAGEQUEUE", "");
            // User Queue
            Environment.SetEnvironmentVariable("USERQUEUE", "");
            // Channel Queue
            Environment.SetEnvironmentVariable("CHANNELQUEUE", "");
            // Slack Dynamodb Table
            Environment.SetEnvironmentVariable("TABLE", "slack");
            // Where the token is saved off to
            Environment.SetEnvironmentVariable("GOOGLEDRIVETOKENDIRECTORY", "");
            // Where Where the reponse file is saved locally
            Environment.SetEnvironmentVariable("GOOGLEDRIVETOKENRESPONSEFILE", "");
            // Where the credential file is located on s3
            Environment.SetEnvironmentVariable("GOOGLEDRIVETOKENS3CREDENTIALKEY", "credentials.json");
            // Where the reponse file is is located on s3
            Environment.SetEnvironmentVariable("GOOGLEDRIVETOKENS3TOKENRESPONSEKEY", "");
            // The s3 bucket to get your google creds
            Environment.SetEnvironmentVariable("S3BUCKET", "");
            // AWS credentials
            _creds = new BasicAWSCredentials("","");
        }
        [Fact]
        public void TestSlackBackupHandler()
        {
            // Invoke the lambda function ......
            var function = new SlackBackup( _creds );
            var context = new TestLambdaContext();
            var response = function.SlackBackupHandler("hello world", context).Result;

            Assert.Equal(0, response);
        }
    }
}
