using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Newtonsoft.Json;

using Common.Slack.DataModels;

namespace Lambda.Tests
{
    public class SlackSignatureVerificationTest
    {
        const string _payload = "{\"method\":\"POST\",\"body\":{\"token\":\"Yz7CmIkbWTwsCjk9MZ2JwtyU\",\"team_id\":\"T9PUT4GH0\",\"api_app_id\":\"AC2DJJPU4\",\"event\":{\"client_msg_id\":\"e07cc63b-1bf8-4900-b56e-026ab969b634\",\"type\":\"message\",\"text\":\"test\",\"user\":\"U9PUT4GTU\",\"ts\":\"1575954390.000300\",\"team\":\"T9PUT4GH0\",\"blocks\":[{\"type\":\"rich_text\",\"block_id\":\"lgMg/\",\"elements\":[{\"type\":\"rich_text_section\",\"elements\":[{\"type\":\"text\",\"text\":\"test\"}]}]}],\"thread_ts\":\"1575700127.016200\",\"parent_user_id\":\"U9PUT4GTU\",\"channel\":\"C9PSEKQ3D\",\"event_ts\":\"1575954390.000300\",\"channel_type\":\"channel\"},\"type\":\"event_callback\",\"authed_teams\":[\"T9PUT4GH0\"],\"event_id\":\"EvR5485S1Z\",\"event_time\":1575954390},\"headers\":{\"Accept\":\"*/*\",\"Accept-Encoding\":\"gzip,deflate\",\"Content-Type\":\"application/json\",\"Host\":\"b95ztyt1be.execute-api.us-west-2.amazonaws.com\",\"User-Agent\":\"Slackbot 1.0 (+https://api.slack.com/robots)\",\"X-Amzn-Trace-Id\":\"Root=1-5def27d7-bf2ce980da639d40696be180\",\"X-Forwarded-For\":\"54.210.143.156\",\"X-Forwarded-Port\":\"443\",\"X-Forwarded-Proto\":\"https\",\"X-Slack-Request-Timestamp\":\"1575954391\",\"X-Slack-Signature\":\"v0=d3a52789743d986ee968a65db55242aeec480a3d4910bee543c4f7a630878e20\"}}";
        private TestLambdaContext _context;
        private EventRequest _input;
        private SlackSignatureVerification _slackSignatureVerification;

        public SlackSignatureVerificationTest()
        {
            Environment.SetEnvironmentVariable("SLACKSIGNATUREVERSION", "v0");
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
            _slackSignatureVerification = new SlackSignatureVerification();
            _context = new TestLambdaContext();
            _input = JsonConvert.DeserializeObject<EventRequest>(_payload);
        }

        [Fact]
        public void FunctionHandlerTest()
        {

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new SlackSignatureVerification();
            var context = new TestLambdaContext();
            var upperCase = function.FunctionHandler(_input, context);

            //Assert.Equal("HELLO WORLD", upperCase);
        }
    }
}
