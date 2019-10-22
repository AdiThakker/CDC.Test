using System;

namespace CDC.Service.Events
{
    public class CustomerCDCEventArgs<TEventType> : EventArgs
    {
        public CustomerCDCEventArgs(TEventType eventType)
        {
            EventType = eventType;
        }

        public TEventType EventType { get; }

        public bool EventPublishedSuccessfully { get; set; }
    }
}
