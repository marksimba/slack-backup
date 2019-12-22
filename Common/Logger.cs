using System;

namespace Common
{
    public enum Severity
    {
        Info,
        Warning,
        Error
    }
    public class Log
    {
        String _type;
        Severity _severity;
        DateTime _timeStamp;
        String _message;

        public Log(String type, Severity severity, DateTime timestamp, String message)
        {
            _type = type;
            _severity = severity;
            _timeStamp = timestamp;
            _message = message;
        }


        public override string ToString()
        {
            return $"({_type}) | {_severity} | {_timeStamp.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")} | {_message}";
        }
    }
    public class Logger
    {
        Guid _sessionId;
        public Logger()
        {
            _sessionId = Guid.NewGuid();
        }

        public void WriteLine(String type, Severity severity, DateTime timestamp, String message)
        {
            Console.WriteLine($"SessionId: {_sessionId}. Log: {new Log(type, severity, timestamp, message).ToString()}");
        }
    }
}