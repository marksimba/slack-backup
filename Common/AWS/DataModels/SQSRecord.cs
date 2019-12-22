using Newtonsoft.Json;
namespace Common.AWS.DataModels
{
    public class SQSRecord
    {
        [JsonProperty("messageId")]
        public string MessageId { get; set;}
        [JsonProperty("receiptHandle")]
        public string ReceiptHandle { get; set;}
        [JsonProperty("body")]
        public string Body { get; set;}
        [JsonProperty("attributes")]
        public dynamic Attributes { get; set;}
        [JsonProperty("messageAttributes")]
        public dynamic MessageAttributes { get; set;}
        [JsonProperty("md5OfBody")]
        public string Md5OfBody { get; set;}
        [JsonProperty("eventSource")]
        public string EventSource { get; set;}
        [JsonProperty("eventSourceARN")]
        public string EventSourceARN { get; set;}
        [JsonProperty("awsRegion")]
        public string AwsRegion { get; set;}
    }
}