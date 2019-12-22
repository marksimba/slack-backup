using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Common.Slack.DataModels
{
    public class EventRequest
    {
        [JsonProperty("body")]
        public EventRoot Body { get; set;}
        [JsonProperty("rawBody")]
        public string RawBody { get; set;}
        [JsonProperty("headers")]
        public Dictionary<string, string> Headers { get; set; }
    }
}