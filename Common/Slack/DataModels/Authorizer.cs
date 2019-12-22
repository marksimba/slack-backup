using System.Collections.Generic;
namespace Common.Slack.DataModels
{
    public class Authorizer
    {
        public string type { get; set; }
        public string methodArn { get; set; }
        public string resource { get; set; }
        public string httpMethod { get; set; }
        public Dictionary<string, string> headers { get; set;}
    }
}