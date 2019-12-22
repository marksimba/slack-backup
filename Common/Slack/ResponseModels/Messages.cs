using System.Collections.Generic;
using Common.Slack.DataModels;

namespace Common.Slack.ResponseModels 
{
    public class Messages : SlackResponse
    {
        public List<Message> messages { get; set; }
        public bool has_more { get; set; }
        public string latest { get; set; }
    }
}