using System.Collections.Generic;
namespace Common.Slack.DataModels
{
    public class Reaction
    {
        public string name { get; set; }
        public int count { get; set; }
        public List<string> users { get; set; }
        public string id { get; set; }
    }
}