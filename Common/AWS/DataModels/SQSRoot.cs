using System.Collections.Generic;
namespace Common.AWS.DataModels
{
    public class SQSRoot
    {
        public List<SQSRecord> Records { get; set; }
    }
}