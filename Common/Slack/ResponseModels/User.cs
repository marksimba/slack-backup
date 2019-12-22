using SlackDataModels = Common.Slack.DataModels;
namespace Common.Slack.ResponseModels 
{
    public class User : SlackResponse
    {
        public SlackDataModels :: User user { get; set; }
    }
}