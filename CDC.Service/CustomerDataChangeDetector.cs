using CDC.Service.Events;
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
