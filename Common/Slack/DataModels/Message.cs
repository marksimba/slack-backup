using System.Collections.Generic;
namespace Common.Slack.DataModels
{
    public class Message
    {
        public string type { get; set; }
        public string channel { get; set; }
        public string ts { get; set; }
        public string user { get; set; }
        public string text { get; set; }
        public bool? is_starred { get; set; }
        public List<Reaction> reactions { get; set; }
        public string username { get; set; }
        public string bot_id { get; set; }
        public List<Attachment> attachments { get; set; }
        public string subtype { get; set; }
        public string client_msg_id { get; set; }
        public int reply_count { get; set; }
        public int reply_users_count { get; set; }
        public List<string> reply_users { get; set; }
        public string thread_ts { get; set; }
        public string parent_user_is { get; set; }
        public Edited edited { get; set; }
        public string team { get; set; }
        public bool? upload { get; set; }
        public List<File> files { get; set; }
    }
}