using CDC.Service.Events;
using CDC.Service.Events.Base;
using CDC.Service.Records;
using CDC.Service.Records.Base;
using CDC.Service.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CDC.Service
{
    public class CustomerDataChangeDetector
    {
        public event EventHandler<CustomerCDCEventArgs<CustomerCDCEvent>> HandleCustomerCDCEvent;

        public void CheckForAndPublishChanges()
        {
            var allChangeRecords = new Dictionary<string, OutboxCDCRecord>();
            var orderedChanges = new List<string>();

            RunStoredProcedure("CDC.GetCustomerCDCUpdates", reader =>
            {
                while (reader.Read())
                {
                    var record = new CustomerCDCRecord(reader);
                    if (!allChangeRecords.ContainsKey(record.LSNString))
                        allChangeRecords.Add(record.LSNString, record);
                    else
                        CustomerCDCRecord.Update(reader, ref record);
                }

                reader.NextResult();

                reader.NextResult();

                while (reader.Read()) orderedChanges.Add(Convert.ToBase64String((byte[])reader.GetValue(0)));
            });

            var sentMessages = new DataTable
            {
                Columns =
                {
                    new DataColumn("ChangeId", typeof(long)),
                    new DataColumn("EventSentUTC", typeof(DateTimeOffset))
                }
            };

            foreach (var lsn in orderedChanges)
            {
                var changeRecord = allChangeRecords[lsn];

                if (changeRecord is CustomerCDCRecord customerCDC)
                {
                    SendCustomerCDCEvent(customerCDC);
                    sentMessages.Rows.Add(changeRecord.ChangeId, DateTimeOffset.UtcNow);
                }
            }

            //Store the record in outbox postmarks
            if (sentMessages.Rows.Count > 0)
                RunStoredProcedure("cdc.UpdateOutboxEventSentTimeStamp", null,
                    new Dictionary<string, object> { { "@postmarks", sentMessages } });
        }

        private void SendCustomerCDCEvent(CustomerCDCRecord cdc)
        {
            var customerEvent = default(CustomerCDCEvent);

            switch (cdc.Operation)
            {
                case CDCOperation.Delete:
                    customerEvent = new CustomerCDCEvent
                    {
                        CDCEventType = CDCEventTypeEnum.Delete,
                        CustomerEvent = GetLoanProperties(cdc)
                    };
                    break;
                case CDCOperation.Insert:
                    customerEvent = new CustomerCDCEvent
                    {
                        CDCEventType = CDCEventTypeEnum.Insert,
                        LoanEvent = GetLoanProperties(cdc)
                    };
                    break;
                case CDCOperation.UpdateBeforeChange:
                    break;
                case CDCOperation.UpdateAfterChange:
                case CDCOperation.Upsert:

                    customerEvent = new CustomerCDCEvent
                    {
                        CDCEventType = CDCEventTypeEnum.Upsert,
                        LoanEvent = GetLoanProperties(cdc)
                    };
                    break;
            }

            // publish Event
            if (customerEvent != default(CustomerCDCEvent))
            {
                var args = new LoanCDCEventArgs<LoanCDCEvent>(customerEvent);
                HandleLoanCDCEvent?.Invoke(this, args);
            }
        }

        private static CustomerCDCEvent GetLoanProperties(CustomerCDCRecord cdc)
        {
            return new CustomerCDCEvent
            {
                PFINumber = cdc.PFINumber,
                PFILoanNumber = cdc.PFILoanNumber,
                LoanAmount = cdc.LoanAmount,
                LoanPurpose = cdc.LoanPurpose,
                MCNumber = cdc.MCNumber,
                CorrelationId = Guid.NewGuid().ToString() 
            };
        }

        private void RunStoredProcedure(string procedureName, Action<IDataReader> loadDataAction,
            Dictionary<string, object> parameters = null)
        {
            using (var connection = CDCHelper.GetConnection(_config.ConnectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    foreach (var p in parameters)
                    {
                        var param = command.Parameters.AddWithValue(p.Key, p.Value);
                        if (p.Value is DataTable) param.SqlDbType = SqlDbType.Structured;
                    }

                connection.Open();

                if (loadDataAction != null)
                {
                    var reader = command.ExecuteReader();

                    loadDataAction(reader);
                }
                else
                {
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
