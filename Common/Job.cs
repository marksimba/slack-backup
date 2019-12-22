using System;

namespace Common
{
    public enum EventType
    {
        Channel,
        Message,
        User
    }
    
    public class Job
    {
        public DateTime SessionId { get; set; }
        public EventType EventType { get; set; }
        public bool Authorized { get; set; }
    }
}