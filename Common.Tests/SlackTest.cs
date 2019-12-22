using System;
using System.Net;
using Xunit;
using Common.Slack.ResponseModels;
using Common.Slack;
using System.Collections.Generic;


using SlackDataModels = Common.Slack.DataModels;

namespace Common.Tests
{
    public class SlackActionsTest
    {
        private SlackActions _slack;

        public SlackActionsTest()
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
            _slack = new SlackActions();
        }

        [Fact]
        public void TestSlackConstructorTest()
        {
            var test = new SlackActions();
            Assert.NotNull(test);
        }

        [Fact]
        public void TestSlackMessageTestNoChannel()
        {
            HttpStatusCode testResponse = _slack.Message("Testing....").Result;

            Assert.Equal(HttpStatusCode.OK, testResponse);
        }

        [Fact]
        public void TestSlackMessageTestWithChannel()
        {
            HttpStatusCode testResponse = _slack.Message("This is a test from Deputy 2.0 in a different channel", "CGQV2U7UN").Result;

            Assert.Equal(HttpStatusCode.OK, testResponse);
        }

        [Fact]
        public void TestSlackGetAllUsers()
        {
            List<SlackDataModels :: User> users = _slack.GetAllUsers().Result;
            Assert.NotNull(users);
        }

        [Fact]
        public void TestSlackGetAllUsersException()
        {
            SlackActions slack = new SlackActions("invalid-token");
            Assert.Throws<AggregateException>(() => slack.GetAllUsers().Result);
        }

        [Fact]
        public void TestSlackGetAllChannels()
        {
            List<SlackDataModels :: Channel>  channels = _slack.GetAllChannels().Result;
            Assert.NotNull(channels);
        }

        [Fact]
        public void TestSlackGetAllChannelsException()
        {
            SlackActions slack = new SlackActions("invalid-token");
            Assert.Throws<AggregateException>(() => slack.GetAllChannels().Result);
        }

        [Fact]
        public void TestSlackChannelHistory()
        {
            Messages messages = _slack.GetChannelHistory("CE1Q9SUAX").Result;
            Assert.True(messages.ok);
        }

        [Fact]
        public void TestSlackGetChannelHistoryException()
        {
            SlackActions slack = new SlackActions("invalid-token");
            Assert.Throws<AggregateException>(() => slack.GetChannelHistory("CGQV2U7UN").Result);
        }
    }
}
