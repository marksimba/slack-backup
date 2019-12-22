using System;
using Newtonsoft.Json;

using Common.Slack.DataModels;
using Common.AWS.DataModels;
using Common.Slack;
using Common;


using SlackResponseModels = Common.Slack.ResponseModels;
using SlackDataModels = Common.Slack.DataModels;

using Amazon.Lambda.Core;
using Amazon.Runtime;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ProcessSlackUser
{
    public class ProcessSlackUser
    {

        private readonly SlackActions _slack;
        private readonly Dynamo _dynamo;
        private readonly Logger _logger;
        private const string _rootFolder = "Slack Backup";

        public ProcessSlackUser()
        {
            string table = Environment.GetEnvironmentVariable("TABLE");
            _slack = new SlackActions();
            _dynamo = new Dynamo(table);
            _logger = new Logger();
        }
        //Testing only

        public ProcessSlackUser(BasicAWSCredentials credentials)
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
            Event slackEvent = JsonConvert.DeserializeObject<Event>(input.Records[0].Body);
            //Because user can either be a string or an JObject, I need to convert the JObject to User
            SlackDataModels :: User userObject = slackEvent.user.ToObject<SlackDataModels :: User>();
            SlackResponseModels :: User userResponse = _slack.GetUser(userObject.id).Result;
            SlackDataModels :: User user = userResponse.user;


            _logger.WriteLine("process-slack-user", Severity.Info, DateTime.Now, $"Saving user {user.id} to Dynamodb");

            Status messageStatus = _dynamo.CreateOrUpdateItem(user.id, "user", JsonConvert.SerializeObject(user), user.id).Result;
            
            if(messageStatus == Status.Success)
            {
                _logger.WriteLine("process-slack-user", Severity.Info, DateTime.Now, $"Saving of user {user.id} successful");
            }
            else
            {
                _logger.WriteLine("process-slack-user", Severity.Info, DateTime.Now, $"Saving of user {user.id} failed");
            }
        }
    }
}
