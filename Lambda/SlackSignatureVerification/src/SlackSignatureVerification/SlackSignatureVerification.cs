using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Specialized;

using System.Web;

using Amazon.Lambda.Core;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon;
using Amazon.SQS.Model;
using Common;
using Common.Slack.DataModels;
using Common.AWS;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Lambda
{
    public class SlackSignatureVerification
    {
        private SQS _sqs;
        private readonly Logger _logger;
		private readonly AmazonLambdaClient _lambdaClient;

        public SlackSignatureVerification()
        {
            _logger = new Logger();
			_lambdaClient = new AmazonLambdaClient(RegionEndpoint.USWest2);
        }
        
        /// <summary>
        /// A Function tht takes a EventRequest object and verifies the signature is correct. 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>  
        public dynamic FunctionHandler(EventRequest input, ILambdaContext context)
        {
            string contentType = input.Headers["Content-Type"];
            string timeStamp = input.Headers["X-Slack-Request-Timestamp"];
            string receivedSignature = input.Headers["X-Slack-Signature"];
            string bodyString = input.RawBody;

            string computedSignature = verifySignature(timeStamp, receivedSignature, bodyString); 

            if( computedSignature == receivedSignature )
            {
                _logger.WriteLine("slack-signature-verification", Severity.Info, DateTime.Now, "Request Authorized");
                switch(contentType)
                {
                    //Slack events and verifications use this case.
                    case "application/json":
                        //Used for Slack Verification
                        if(input.Body.challenge != null)
                        {
                            return input.Body;
                        }  
                        SendMessageToQueue(input.Body.@event);
                        break;
                    //Slack commands use this case. 
                    case "application/x-www-form-urlencoded":

                        NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(input.RawBody);
                        string command = nameValueCollection.Get("command");

                        //Used to differentiate between commands.
                        switch(command)
                        {
                            case "/backup":
                                _logger.WriteLine("slack-signature-verification", Severity.Info, DateTime.Now, $"Starting Backup...");
                                StartBackup();
                                return "Backup Started";
                            default:
                                _logger.WriteLine("slack-signature-verification", Severity.Error, DateTime.Now, $"Command {command} is not recognized");
                                break;
                        }

                        break;
                    default : 
                        _logger.WriteLine("slack-signature-verification", Severity.Error, DateTime.Now, $"Content-Type {contentType} is not recognized");
                        break;

                }
            }
            else
            {

                _logger.WriteLine("slack-signature-verification", Severity.Error, DateTime.Now, $"Request Not Authorized. Computed Signature of {computedSignature} is not equal to Received Signature: {receivedSignature}");
            }

            return input.Body;
            
        }

        private void SendMessageToQueue(Event slackEvent)
        {
            _sqs = new SQS();
            string eventString = JsonConvert.SerializeObject(slackEvent);


            //When a channel is changed, a message is also sent.
            //So I will send the message to two QUEUES if it's a channel change. 
            if(slackEvent.subtype != null && slackEvent.subtype.Contains("channel"))
            {
                _logger.WriteLine("slack-signature-verification", Severity.Info, DateTime.Now, $"Channel identified. Sending message ({slackEvent.channel}) to the channel QUEUE");
                _sqs.SendMessage(eventString, System.Environment.GetEnvironmentVariable("CHANNELQUEUE") );
            }

            switch(slackEvent.type)
            {
                case string type when type.Contains("channel") :
                    _logger.WriteLine("slack-signature-verification", Severity.Info, DateTime.Now, $"Channel identified. Sending message ({slackEvent.channel}) to the channel QUEUE");
                    SendMessageResponse channelResponse = _sqs.SendMessage(eventString, System.Environment.GetEnvironmentVariable("CHANNELQUEUE") );
                    _logger.WriteLine("slack-signature-verification", Severity.Info, DateTime.Now, $"QUEUE response {channelResponse.HttpStatusCode}");
                    break;
                case string type when type.Contains("user") :
                    _logger.WriteLine("slack-signature-verification", Severity.Info, DateTime.Now, $"User identified. Sending message ({slackEvent.user}) to the user QUEUE");
                    SendMessageResponse userResponse = _sqs.SendMessage(eventString, System.Environment.GetEnvironmentVariable("USERQUEUE") );
                    _logger.WriteLine("slack-signature-verification", Severity.Info, DateTime.Now, $"QUEUE response {userResponse.HttpStatusCode}");
                    break;
                case string type when type.Contains("message")  :
                    _logger.WriteLine("slack-signature-verification", Severity.Info, DateTime.Now, $"Message identified. Sending message ({slackEvent.ts}) to the message QUEUE");
                     SendMessageResponse messageResponse = _sqs.SendMessage(eventString, System.Environment.GetEnvironmentVariable("MESSAGEQUEUE") );
                    _logger.WriteLine("slack-signature-verification", Severity.Info, DateTime.Now, $"QUEUE response {messageResponse.HttpStatusCode}");
                    break;
                default :
                    _logger.WriteLine("slack-signature-verification", Severity.Info, DateTime.Now, $"{slackEvent.type} is not recognized as a valid type");
                    break;
            }
        }

        private string verifySignature(string timeStamp, string receivedSignature, string bodyString)
        {
            string signatureVersion = System.Environment.GetEnvironmentVariable("SLACKSIGNATUREVERSION"); 
            string signature = System.Environment.GetEnvironmentVariable("SLACKSIGNATURE"); 
            string signatureString = $"{signatureVersion}:{timeStamp}:{bodyString}";

            var shaKeyBytes = Encoding.UTF8.GetBytes(signature);
            using (var shaAlgorithm = new HMACSHA256(shaKeyBytes))
            {
                var signatureBytes = Encoding.UTF8.GetBytes(signatureString);
                var signatureHashBytes = shaAlgorithm.ComputeHash(signatureBytes);
                var computedSignature = $"{signatureVersion}={ByteArrayToString(signatureHashBytes)}";
                
                return computedSignature;
            }
        }

        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        private void StartBackup()
        {
            var function = System.Environment.GetEnvironmentVariable("BACKUPFUNCTION");
            var request = new InvokeRequest
            {
                FunctionName = function,
                Payload = "{}" //Perhaps I'll parameratize this at some point... 
            };

            var response = _lambdaClient.InvokeAsync(request).Result;
        }
    }
}
