using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Common;
using Common.Slack;
using Common.Slack.DataModels;
using Common.Slack.ResponseModels;
using Amazon;
using Amazon.SQS.Model;
using Common.AWS;
using Newtonsoft.Json;
using Amazon.Runtime;
using SlackDataModels = Common.Slack.DataModels;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Lambda
{
    public class SlackBackup
    {
        private readonly SlackActions _slack;
        private readonly Logger _logger;
        private readonly GoogleDrive _googleDrive;
        private SQS _sqs;

        public SlackBackup()
        {
            _slack = new SlackActions();
            _logger = new Logger();
            _googleDrive = new GoogleDrive();
            _sqs = new SQS();
        }
        //To use for testing
         public SlackBackup(BasicAWSCredentials credentials)
        {
            _slack = new SlackActions();
            _logger = new Logger();
            _googleDrive = new GoogleDrive(credentials);
            _sqs = new SQS(credentials);
        }
        
        /// <summary>
        /// A Function tht takes a dynamc input and starts the backup
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>  
        public async Task<int> SlackBackupHandler(dynamic input, ILambdaContext context)
        {
            await _slack.Message("```Starting Backup```");

            

            //Ensures Root Folder Exists
            string rootFolder = _googleDrive.CreateFolder("Slack Backup");

            List<SlackDataModels::User> users = _slack.GetAllUsers().Result;

            _logger.WriteLine("users", Severity.Info, DateTime.Now, $"Processing {users.Count} users");
            foreach(SlackDataModels::User user in users)
            {
                Event @event  = new Event(){
                    user = user
                };
                string eventString = JsonConvert.SerializeObject(@event);
                _logger.WriteLine("slack-backup", Severity.Info, DateTime.Now, $"User identified. Sending message ({@event.user}) to the user QUEUE");
                SendMessageResponse userResponse = _sqs.SendMessage(eventString, System.Environment.GetEnvironmentVariable("USERQUEUE") );
                _logger.WriteLine("slack-backup", Severity.Info, DateTime.Now, $"QUEUE response {userResponse.HttpStatusCode}");
            }

            List<SlackDataModels::Channel> channels = _slack.GetAllChannels().Result;

            Dictionary<string,int> counts = new Dictionary<string, int>();

            string report = $"***Backup report***\n{"Channel:".PadRight(25)}{"Messages:".PadRight(15)}{"Files:".PadRight(15)}\n";

            _logger.WriteLine("channels", Severity.Info, DateTime.Now, $"Processing {channels.Count} channels");
            foreach(SlackDataModels::Channel channel in channels)
            {
                Event @event  = new Event(){
                    channel = channel.id,
                    backup = true
                };
                string eventString = JsonConvert.SerializeObject(@event);
                
                _logger.WriteLine("slack-backup", Severity.Info, DateTime.Now, $"Channel identified. Sending message ({@event.channel}) to the channel QUEUE");
                SendMessageResponse userResponse = _sqs.SendMessage(eventString, System.Environment.GetEnvironmentVariable("CHANNELQUEUE") );
                _logger.WriteLine("slack-backup", Severity.Info, DateTime.Now, $"QUEUE response {userResponse.HttpStatusCode}");

                List<string> parents = new List<string>(){rootFolder};

                //Creates Folder for Channel
                string channelFolder = _googleDrive.CreateFolder(channel.id, parents);
            }

            return 0;
        }

    }
}
