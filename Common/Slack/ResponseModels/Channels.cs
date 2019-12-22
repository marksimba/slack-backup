using System.Collections.Generic;
using SlackDataModels = Common.Slack.DataModels;

namespace Common.Slack.ResponseModels 
{
    public class Channels : SlackResponse
    {
        public List<SlackDataModels :: Channel> channels { get; set; }
    }
}