using CDC.Service.Records.Base;
using System.Data;

namespace CDC.Service.Records
{
    public class CustomerCDCRecord : OutboxCDCRecord
    {
        private const string _firstName = "FirstName";
        private const string _lastName = "LastName";
        private const string _address = "Address";
        private const string _city = "City";
        private const string _state = "State";
        private const string _country = "Country";

        public CustomerCDCRecord()
        {
        }

        public CustomerCDCRecord(IDataReader reader)
            : base(reader)
        {
            FirstName = ExtractValueFromReaderIfPresent<string>(reader, _firstName);
            LastName = ExtractValueFromReaderIfPresent<string>(reader, _lastName);
            Address = ExtractValueFromReaderIfPresent<string>(reader, _address);
            City = ExtractValueFromReaderIfPresent<string>(reader, _city);
            State = ExtractValueFromReaderIfPresent<string>(reader, _state);
            Country = ExtractValueFromReaderIfPresent<string>(reader, _country);
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public static CustomerCDCRecord Update(IDataReader reader, ref CustomerCDCRecord cdcRecord)
        {
            var outboxRrecord = (OutboxCDCRecord)cdcRecord;
            Update(reader, ref outboxRrecord);

            cdcRecord.FirstName = cdcRecord.ExtractValueFromReaderIfPresent<string>(reader, _firstName);
            cdcRecord.LastName = cdcRecord.ExtractValueFromReaderIfPresent<string>(reader, _lastName);
            cdcRecord.Address = cdcRecord.ExtractValueFromReaderIfPresent<string>(reader, _address);
            cdcRecord.City = cdcRecord.ExtractValueFromReaderIfPresent<string>(reader, _city);
            cdcRecord.State = cdcRecord.ExtractValueFromReaderIfPresent<string>(reader, _state);
            cdcRecord.Country = cdcRecord.ExtractValueFromReaderIfPresent<string>(reader, _country);

            return cdcRecord;
        }
    }

}
