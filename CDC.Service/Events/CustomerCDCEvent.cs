using CDC.Service.Events.Base;

namespace CDC.Service.Events
{
    public class CustomerCDCEvent : CDCEvent
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }
    }
}
