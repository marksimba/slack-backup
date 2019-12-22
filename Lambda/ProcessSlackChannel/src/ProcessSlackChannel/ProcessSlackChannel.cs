using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using Common.Slack.DataModels;
using Common.AWS.DataModels;
using Common.Slack;
using Common;


using SlackResponseModels = Common.Slack.ResponseModels;
using SlackDataModels = Common.Slack.DataModels;

using Amazon.Lambda.Core;
using Amazon.Runtime;
using Amazon.SQS.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ProcessSlackChannel
{
    public class ProcessSlackChannel
    {

        private readonly SlackActions _slack;
        private readonly Dynamo _dynamo;
        private readonly Logger _logger;
        private SQS _sqs;
        private const string _rootFolder = "Slack Backup";

        public ProcessSlackChannel()
        {
            string table = Environment.GetEnvironmentVariable("TABLE");
            _slack = new SlackActions();
            _dynamo = new Dynamo(table);
            _logger = new Logger();
        }
        //Testing only

        public ProcessSlackChannel(BasicAWSCredentials credentials)
        {
            string table = Environment.GetEnvironmentVariable("TABLE");
            _slack = new SlackActions();
            _dynamo = new Dynamo(table);
            _logger = new Logger();
        }
        
        /// <summary>
        /// A Function tht takes a SQSRoot object and Stores off the Slack Channel
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void FunctionHandler(SQSRoot input, ILambdaContext context)
        {
            _sqs = new SQS();
            Event slackEvent = JsonConvert.DeserializeObject<Event>(input.Records[0].Body);
            string channelId;
            //Channel can either be a channel object or a string (the id)
            try
            {
                Channel ch = slackEvent.channel.ToObject<Channel>();
                channelId = ch.id;
            }
            catch(Exception e)
            {
                channelId = slackEvent.channel;
            }

            if ( slackEvent.backup ) // We should get all the messages for this channel and
            {                        // send them to their respective queues. 
                _logger.WriteLine("process-slack-channel", Severity.Info, DateTime.Now, $"Full backup request. Getting all messages");
                List<SlackDataModels :: Message> messages = _slack.GetAllMessages(channelId);

                // Iterate through messages and send them to the message queue.
                foreach ( SlackDataModels :: Message message in messages)
                {
                    Event @event  = new Event(){
                        ts = message.ts
                    };

                    string eventString = JsonConvert.SerializeObject(@event);
                    _logger.WriteLine("process-slack-channel", Severity.Info, DateTime.Now, $"Message identified. Sending message ({@event.ts}) to the message QUEUE");
                    SendMessageResponse userResponse = _sqs.SendMessage(eventString, System.Environment.GetEnvironmentVariable("MESSAGEQUEUE") );
                    _logger.WriteLine("process-slack-channel", Severity.Info, DateTime.Now, $"QUEUE response {userResponse.HttpStatusCode}");
                }
            }

            SlackResponseModels :: Channel channelResponse = _slack.GetChannel(channelId).Result;
            SlackDataModels :: Channel channel = channelResponse.channel;


            _logger.WriteLine("process-slack-channel", Severity.Info, DateTime.Now, $"Saving channel {channel.id} to Dynamodb");

            Status messageStatus = _dynamo.CreateOrUpdateItem(channel.id, "channel", JsonConvert.SerializeObject(channel), channel.id).Result;
            
            if(messageStatus == Status.Success)
            {
                _logger.WriteLine("process-slack-channel", Severity.Info, DateTime.Now, $"Saving of channel {channel.id} successful");
            }
            else
            {
                _logger.WriteLine("process-slack-channel", Severity.Info, DateTime.Now, $"Saving of channel {channel.id} failed");
            }
        }
    }
}
