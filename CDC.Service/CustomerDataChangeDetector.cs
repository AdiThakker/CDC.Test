using CDC.Service.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDC.Service
{
    public class CustomerDataChangeDetector
    {
        public event EventHandler<CustomerCDCEventArgs<CustomerCDCEvent>> HandleCustomerCDCEvent;
    }
}
