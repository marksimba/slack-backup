using System.Collections.Generic;
namespace Common.Slack.DataModels
{
    public class Event
{
    public string client_msg_id { get; set; }
    public string type { get; set; }
    public string subtype { get; set; }
    public string text { get; set; }
    public dynamic user { get; set; }
    public string ts { get; set; }
    public string team { get; set; }
    public dynamic channel { get; set; }
    public string event_ts { get; set; }
    public string channel_type { get; set; }
    public bool backup { get; set; } = false ;
}
}