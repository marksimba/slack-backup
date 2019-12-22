using System;
using Xunit;

namespace Common.Tests
{
    public class LoggerTest
    {
        Logger _logger;

        public LoggerTest()
        {
            _logger = new Logger();
        }

        [Fact]
        public void WriteLineTest() //Used to verify function works. 
        {
            _logger.WriteLine("Test Service", Severity.Info, DateTime.Now, "This is a test log");
            _logger.WriteLine("Test Service", Severity.Warning, DateTime.Now, "Test Log 2");
            _logger.WriteLine("Test Service", Severity.Error, DateTime.Now, "Test log three");
        }
    }
}