using System;
using System.Collections.Generic;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Threading.Tasks;

namespace Common.AWS
{
    public class SQS
    {
        private AmazonSQSClient _client;
        private string _table;
        private RegionEndpoint _region = RegionEndpoint.USWest2;

        public SQS(BasicAWSCredentials credentials, RegionEndpoint region = null)
        {
            //Default region is defined in the class. You can pass the region if you'd like
            _client = new AmazonSQSClient(credentials, region == null ? _region : RegionEndpoint.USWest2); 
        }

        public SQS(RegionEndpoint region = null)
        {
            //Default region is defined in the class. You can pass the region if you'd like
            _client = new AmazonSQSClient(region == null ? _region : RegionEndpoint.USWest2); 
        }

        /// <summary>
        /// A Function that send a message to a queue
        /// </summary>
        /// <param name="message"> Message to send</param>
        /// <param name="queue"> Queue to send to</param>
        /// <returns>SendMessageResponse</returns>
        public SendMessageResponse SendMessage(string message, string queue)
        {
            SendMessageRequest sendMessageRequest = new SendMessageRequest(){
                QueueUrl = queue,
                MessageBody = message
            };

            return _client.SendMessageAsync(sendMessageRequest).Result;
        }
        
    }
}