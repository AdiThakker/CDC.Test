using System.Data;

namespace CDC.Service.Records.Base
{
    public class OutboxCDCRecord : CDCRecord
    {
        private const string ChangeIdFieldName = "ChangeId";

        public OutboxCDCRecord()
        {
        }

        public OutboxCDCRecord(IDataReader reader) : base(reader)
        {
            ChangeId = reader.GetInt64(reader.GetOrdinal(ChangeIdFieldName));
        }

        public long ChangeId { get; set; }

        public static OutboxCDCRecord Update(IDataReader reader, ref OutboxCDCRecord outboxCDCRecord)
        {
            var cdcRecord = (CDCRecord)outboxCDCRecord;
            Update(reader, ref cdcRecord);
            outboxCDCRecord.ChangeId = reader.GetInt64(reader.GetOrdinal(ChangeIdFieldName));
            return outboxCDCRecord;
        }
    }
}
