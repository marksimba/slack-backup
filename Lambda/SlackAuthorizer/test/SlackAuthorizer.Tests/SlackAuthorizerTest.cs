using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using SlackAuthorizer;
using Common.Slack.DataModels;
using Newtonsoft.Json;

namespace SlackAuthorizer.Tests
{
    public class SlackAuthorizerTest
    {
        const string _payload = "{\"type\":\"REQUEST\",\"methodArn\":\"arn:aws:execute-api:us-west-2:265005857390:b95ztyt1be/slack/POST/message/save\",\"resource\":\"/message/save\",\"path\":\"/message/save\",\"httpMethod\":\"POST\",\"headers\":{\"Accept\":\"*/*\",\"Accept-Encoding\":\"gzip,deflate\",\"Content-Length\":\"619\",\"Content-Type\":\"application/json\",\"Host\":\"b95ztyt1be.execute-api.us-west-2.amazonaws.com\",\"User-Agent\":\"Slackbot 1.0 (+https://api.slack.com/robots)\",\"X-Amzn-Trace-Id\":\"Root=1-5dec06ed-3ea71bd65a839dc8aa055e4f\",\"X-Forwarded-For\":\"54.90.136.152\",\"X-Forwarded-Port\":\"443\",\"X-Forwarded-Proto\":\"https\",\"X-Slack-Request-Timestamp\":\"1575749357\",\"X-Slack-Retry-Num\":\"1\",\"X-Slack-Retry-Reason\":\"http_error\",\"X-Slack-Signature\":\"v0=4aeec9e89e89155ac9c28dbaaf6cc5f50bcef60e3f117dd2474ad8af7dd971ac\"},\"multiValueHeaders\":{\"Accept\":[\"*/*\"],\"Accept-Encoding\":[\"gzip,deflate\"],\"Content-Length\":[\"619\"],\"Content-Type\":[\"application/json\"],\"Host\":[\"b95ztyt1be.execute-api.us-west-2.amazonaws.com\"],\"User-Agent\":[\"Slackbot 1.0 (+https://api.slack.com/robots)\"],\"X-Amzn-Trace-Id\":[\"Root=1-5dec06ed-3ea71bd65a839dc8aa055e4f\"],\"X-Forwarded-For\":[\"54.90.136.152\"],\"X-Forwarded-Port\":[\"443\"],\"X-Forwarded-Proto\":[\"https\"],\"X-Slack-Request-Timestamp\":[\"1575749357\"],\"X-Slack-Retry-Num\":[\"1\"],\"X-Slack-Retry-Reason\":[\"http_error\"],\"X-Slack-Signature\":[\"v0=4aeec9e89e89155ac9c28dbaaf6cc5f50bcef60e3f117dd2474ad8af7dd971ac\"]},\"queryStringParameters\":{},\"multiValueQueryStringParameters\":{},\"pathParameters\":{},\"stageVariables\":{},\"requestContext\":{\"resourceId\":\"nu1btn\",\"resourcePath\":\"/message/save\",\"httpMethod\":\"POST\",\"extendedRequestId\":\"EWYFLEoHPHcFv1Q=\",\"requestTime\":\"07/Dec/2019:20:09:17 +0000\",\"path\":\"/slack/message/save\",\"accountId\":\"265005857390\",\"protocol\":\"HTTP/1.1\",\"stage\":\"slack\",\"domainPrefix\":\"b95ztyt1be\",\"requestTimeEpoch\":1575749357806,\"requestId\":\"e56a9583-5320-40d8-a15a-98cd978d8c70\",\"identity\":{\"cognitoIdentityPoolId\":null,\"accountId\":null,\"cognitoIdentityId\":null,\"caller\":null,\"sourceIp\":\"54.90.136.152\",\"principalOrgId\":null,\"accessKey\":null,\"cognitoAuthenticationType\":null,\"cognitoAuthenticationProvider\":null,\"userArn\":null,\"userAgent\":\"Slackbot 1.0 (+https://api.slack.com/robots)\",\"user\":null},\"domainName\":\"b95ztyt1be.execute-api.us-west-2.amazonaws.com\",\"apiId\":\"b95ztyt1be\"}}";
        private SlackAuthorizer _slackAuthorizer;
        private TestLambdaContext _context;
        private Authorizer _input;
        public SlackAuthorizerTest()
        {
            Environment.SetEnvironmentVariable("SLACKSIGNATUREVERSION", "v0");
            _slackAuthorizer = new SlackAuthorizer();
            _context = new TestLambdaContext();
            _input = JsonConvert.DeserializeObject<Authorizer>(_payload);
        }

        [Fact]
        public void TestVerifyCorrectHeaders()
        {
            //Sets Timestamp
            _input.headers["X-Slack-Request-Timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            var response = _slackAuthorizer.VerifyCorrectHeaders(_input, _context);

            Assert.Equal("Allow", response.PolicyDocument.Statement[0].Effect);
        }

        [Fact]
        public void TestVerifyCorrectHeadersMissingTimestampHeader()
        {
            _input.headers.Remove("X-Slack-Request-Timestamp");

            var response = _slackAuthorizer.VerifyCorrectHeaders(_input, _context);

            Assert.Equal("Deny", response.PolicyDocument.Statement[0].Effect);
        }

        [Fact]
        public void TestVerifyCorrectHeadersOver5Minutes()
        {
            TimeSpan currentEpochTime = DateTime.UtcNow.Add(new TimeSpan(0,-5,0)) - new DateTime(1970, 1, 1);
            int totalSecond = (int)currentEpochTime.TotalSeconds;
            _input.headers["X-Slack-Request-Timestamp"] = totalSecond.ToString();

            var response = _slackAuthorizer.VerifyCorrectHeaders(_input, _context);

            Assert.Equal("Deny", response.PolicyDocument.Statement[0].Effect);
        }

        [Fact]
        public void TestVerifyCorrectHeadersMissingSignatureHeader()
        {
            _input.headers.Remove("X-Slack-Signature");

            var response = _slackAuthorizer.VerifyCorrectHeaders(_input, _context);

            Assert.Equal("Deny", response.PolicyDocument.Statement[0].Effect);
        }

        [Fact]
        public void TestVerifyCorrectHeadersWrongSignatureVersion()
        {
            _input.headers["X-Slack-Signature"] = _input.headers["X-Slack-Signature"].Replace("v0", "wrongVersion");

            var response = _slackAuthorizer.VerifyCorrectHeaders(_input, _context);

            Assert.Equal("Deny", response.PolicyDocument.Statement[0].Effect);
        }

    }
}
