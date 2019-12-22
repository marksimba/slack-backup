using System.Collections.Generic;
namespace Common.Slack.DataModels
{
    public class EventRoot
{
    public string token { get; set; }
    public string team_id { get; set; }
    public string api_app_id { get; set; }
    public Event @event { get; set; }
    public string type { get; set; }
    public List<string> authed_teams { get; set; }
    public string event_id { get; set; }
    public int event_time { get; set; }
    public string challenge { get; set; }
}
}