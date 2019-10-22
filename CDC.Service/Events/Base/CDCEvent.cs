using System;

namespace CDC.Service.Events.Base
{
    public enum CDCEventTypeEnum
    {
        Delete = 1,
        Insert = 2,
        UpdateBeforeChange = 3,
        UpdateAfterChange = 4,
        Upsert = 5
    }
    public class CDCEvent
    {
        public DateTimeOffset EventDateTime { get; set; }

        public string CorrelationId { get; set; }

        public long ChangeId { get; set; }
    }
}
