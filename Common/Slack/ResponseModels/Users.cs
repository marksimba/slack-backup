using System.Collections.Generic;
using SlackDataModels = Common.Slack.DataModels;

namespace Common.Slack.ResponseModels 
{
    public class Users : SlackResponse
    {
        public List<SlackDataModels :: User> members { get; set; }
        public int cache_ts { get; set; }
    }
}