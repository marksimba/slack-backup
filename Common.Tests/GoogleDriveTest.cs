using System;
using Amazon.Runtime;
using Xunit;

namespace Common.Tests
{
    public class GoogleDriveTest
    {
        private GoogleDrive _googleDrive;
        private BasicAWSCredentials _creds;
        
        public GoogleDriveTest()
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
            _creds = new BasicAWSCredentials("","");;
            _googleDrive = new GoogleDrive( _creds );
        }

        [Fact]
        public void GoogleDriveConstructorTest()
        {
            var test = new GoogleDrive(_creds);
            Assert.NotNull(test);
        }
    }
}