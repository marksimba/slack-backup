using System.Collections.Generic;

namespace Common.Slack.DataModels
{
    public class Channel
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool is_channel { get; set; }
        public int created { get; set; }
        public string creator { get; set; }
        public bool is_archived { get; set; }
        public bool is_general { get; set; }
        public string name_normalized { get; set; }
        public bool is_shared { get; set; }
        public bool is_org_shared { get; set; }
        public bool is_member { get; set; }
        public bool is_private { get; set; }
        public bool is_mpim { get; set; }
        public List<string> members { get; set; }
        public Topic topic { get; set; }
        public Purpose purpose { get; set; }
        public List<object> previous_names { get; set; }
        public int num_members { get; set; }
    }
}