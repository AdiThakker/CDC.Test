using System;
using System.Data;

namespace CDC.Service.Records.Base
{
    public enum CDCOperation
    {
        Delete = 1,
        Insert = 2,
        UpdateBeforeChange = 3,
        UpdateAfterChange = 4,

        /// <summary>
        ///     This is used with 'all with merge' row filter to indicate an Insert or Update happened to the row
        /// </summary>
        Upsert = 5
    }

    public class CDCRecord
    {
        private const string StartLSNFieldName = "__$start_lsn";
        private const string SeqValFieldName = "__$seqval";
        private const string OperationFieldName = "__$operation";
        private const string UpdateMaskFieldName = "__$update_mask";

        public CDCRecord()
        {
        }

        public CDCRecord(IDataReader reader)
        {
            if (reader.IsClosed) throw new ArgumentException("You have passed a closed Data Reader", nameof(reader));

            StartLSN = (byte[])reader.GetValue(reader.GetOrdinal(StartLSNFieldName));
            LSNString = Convert.ToBase64String(StartLSN);
            Operation = (CDCOperation)reader.GetInt32(reader.GetOrdinal(OperationFieldName));
            UpdateMask = (byte[])reader.GetValue(reader.GetOrdinal(UpdateMaskFieldName));

            //The seqval column is not there in net results but that can be OK.
            SeqVal = ExtractValueFromReaderIfPresent<object>(reader, SeqValFieldName);
        }

        public string LSNString { get; set; }

        public byte[] StartLSN { get; set; }

        public CDCOperation Operation { get; set; }

        public byte[] UpdateMask { get; set; }

        public object SeqVal { get; set; }

        public static CDCRecord Update(IDataReader reader, ref CDCRecord cdcRecord)
        {
            if (reader.IsClosed) throw new ArgumentException("You have passed a closed Data Reader", nameof(reader));
            if (cdcRecord == null) throw new ArgumentNullException(nameof(cdcRecord));

            cdcRecord.StartLSN = (byte[])reader.GetValue(reader.GetOrdinal(StartLSNFieldName));
            cdcRecord.LSNString = Convert.ToBase64String(cdcRecord.StartLSN);
            cdcRecord.Operation = (CDCOperation)reader.GetInt32(reader.GetOrdinal(OperationFieldName));
            cdcRecord.UpdateMask = (byte[])reader.GetValue(reader.GetOrdinal(UpdateMaskFieldName));

            //The seqval column is not there in net results but that can be OK.
            cdcRecord.SeqVal = cdcRecord.ExtractValueFromReaderIfPresent<object>(reader, SeqValFieldName);

            return cdcRecord;
        }

        protected T ExtractValueFromReaderIfPresent<T>(IDataReader reader, string column)
        {
            var schema = reader.GetSchemaTable();
            for (var x = 0; x < schema.Rows.Count; x++)
            {
                var row = schema.Rows[x];
                if (string.CompareOrdinal(row["ColumnName"].ToString(), column) == 0)
                {
                    var value = reader.GetValue(x);
                    if (value != DBNull.Value)
                        return (T)value;
                }
            }

            return default;
        }
    }
}
