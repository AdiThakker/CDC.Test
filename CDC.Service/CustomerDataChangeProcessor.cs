using CDC.Service.Events;
using System;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace CDC.Service
{
    public class CustomerDataChangeProcessor
    {
        private readonly Timer _timer;
        private readonly AutoResetEvent changeDetectionWaitHandle = new AutoResetEvent(false);
        private readonly object processingLockObject = new object();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly CustomerDataChangeDetector _customerDataChangeDetector;

        public event EventHandler<CustomerCDCEventArgs<CustomerNotificationEvent>> HandleCustomerNotificationEvent;

        public CustomerDataChangeProcessor(int pollIntervalInSeconds = 10)
        {
            _customerDataChangeDetector = new CustomerDataChangeDetector();
            _customerDataChangeDetector.HandleCustomerNotificationEvent += CustomerDataChangeDetector_HandleCustomerNotificationEvent;
            _timer = new Timer(pollIntervalInSeconds * 1000);
            _timer.Enabled = true;
            _timer.Elapsed += Timer_Elapsed;
        }

        public void Start()
        {
            var processingThread = new Thread(() =>
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    changeDetectionWaitHandle.WaitOne();

                    lock (processingLockObject)
                    {
                        StartChangeDetection();
                    }
                }
            });

            processingThread.Start();
        }

        private void StartChangeDetection()
        {
            _customerDataChangeDetector.CheckForAndPublishChanges();
        }

        private void CustomerDataChangeDetector_HandleCustomerNotificationEvent(object sender, Events.CustomerCDCEventArgs<Events.CustomerNotificationEvent> e)
        {
            HandleCustomerNotificationEvent?.Invoke(this, e);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            changeDetectionWaitHandle.Set();
        }
    }
}
