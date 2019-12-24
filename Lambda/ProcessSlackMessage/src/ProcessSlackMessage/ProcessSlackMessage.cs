using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Common.Slack.DataModels;
using Common.Slack.ResponseModels;
using Common.AWS.DataModels;
using Common.AWS;
using Common.Slack;
using Common;

using SlackDataModels = Common.Slack.DataModels;

using Amazon.Lambda.Core;
using Amazon.Runtime;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Lambda
{
    public class ProcessSlackMessage
    {

        private readonly SlackActions _slack;
        private readonly GoogleDrive _googleDrive;
        private readonly Dynamo _dynamo;
        private readonly Logger _logger;
        private const string _rootFolder = "Slack Backup";

        public ProcessSlackMessage()
        {
            string table = Environment.GetEnvironmentVariable("TABLE");
            _slack = new SlackActions();
            _googleDrive = new GoogleDrive();
            _dynamo = new Dynamo(table);
            _logger = new Logger();
        }
        //Testing only

        public ProcessSlackMessage(BasicAWSCredentials credentials)
        {
            string table = Environment.GetEnvironmentVariable("TABLE");
            _slack = new SlackActions();
            _googleDrive = new GoogleDrive(credentials);
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
            Event slackEvent = JsonConvert.DeserializeObject<Event>(input.Records[0].Body);
            Messages messages = _slack.GetChannelHistory(slackEvent.channel, slackEvent.ts, 1, true).Result;
            Message message = messages.messages[0];

            if(message.files != null)
            {
                foreach(SlackDataModels :: File file in message.files)
                {
                    if(file.mode == "tombstone") //File was upload, but has been deleted.
                    {
                        continue;
                    }
                    //Transfer to Google Drive
                    //_logger.WriteLine("file-transfer", Severity.Info, DateTime.Now, $"Fake Transfering File: {file.id} to Google Drive");
                    Stream stream = _slack.DownloadFile(file.url_private, file.id);

                    string rootFolderId = _googleDrive.CreateFolder(_rootFolder);
                    string currentFolderId = _googleDrive.CreateFolder(slackEvent.channel, new List<string>(){rootFolderId});

                    _googleDrive.UploadFile(file, stream, new List<string>(){rootFolderId, currentFolderId});
                    //Files arent't automatically tied to messages
                    file.messageTs = message.ts;
                    _logger.WriteLine("process-slack-file", Severity.Info, DateTime.Now, $"Saving file {file.id} to Dynamodb");

                    Status fileStatus = _dynamo.CreateOrUpdateItem(file.id, "file", JsonConvert.SerializeObject(file), slackEvent.channel, message.ts).Result;
                    
                    if(fileStatus == Status.Success)
                    {
                        _logger.WriteLine("process-slack-file", Severity.Info, DateTime.Now, $"Saving of file {file.id} successful");
                    }
                    else
                    {
                        _logger.WriteLine("process-slack-file", Severity.Info, DateTime.Now, $"Saving of file {file.id} failed");
                    }
                }
            }

            _logger.WriteLine("process-slack-message", Severity.Info, DateTime.Now, $"Saving message {message.ts} to Dynamodb");

            Status messageStatus = _dynamo.CreateOrUpdateItem(message.ts, "message", JsonConvert.SerializeObject(message), slackEvent.channel).Result;
                
            if(messageStatus == Status.Success)
            {
                _logger.WriteLine("process-slack-message", Severity.Info, DateTime.Now, $"Saving of message {message.ts} successful");
            }
            else
            {
                _logger.WriteLine("process-slack-message", Severity.Info, DateTime.Now, $"Saving of message {message.ts} failed");
            }
        }
    }
}
