using CDC.Service.Events.Base;

namespace CDC.Service.Events
{
    public class CustomerNotificationEvent : CDCEvent
    {
        public CDCEventTypeEnum CDCEventType { get; set; }

        public CustomerCDCEvent CustomerEvent { get; set; }
    }
}
