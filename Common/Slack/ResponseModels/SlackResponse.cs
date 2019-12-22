using System;

namespace Common.Slack.ResponseModels 
{
    public class SlackResponse
    {
        public bool ok { get; set; }
        public ResponseMetadata response_metadata { get; set; }
        public string error { get; set; }
    }

    public class ResponseMetadata
    {
        public string next_cursor { get; set; }
    }

}