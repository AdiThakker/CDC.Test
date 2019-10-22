using CDC.Service;
using System;

namespace CDC.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello CDC!");

            CustomerDataChangeProcessor changeDataProcessor = new CustomerDataChangeProcessor();
            changeDataProcessor.HandleCustomerNotificationEvent += ChangeDataProcessor_HandleCustomerNotificationEvent;
            changeDataProcessor.Start();

            System.Console.ReadKey();
        }

        private static void ChangeDataProcessor_HandleCustomerNotificationEvent(object sender, Service.Events.CustomerCDCEventArgs<Service.Events.CustomerNotificationEvent> e)
        {
            System.Console.WriteLine($"Event Type: { e.EventType.CDCEventType} Customer Name: {e.EventType.CustomerEvent.FirstName}");
        }
    }
}
