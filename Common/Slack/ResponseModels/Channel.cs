using SlackDataModels = Common.Slack.DataModels;
namespace Common.Slack.ResponseModels 
{
    public class Channel : SlackResponse
    {
        public SlackDataModels :: Channel channel { get; set; }
    }
}