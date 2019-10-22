using CDC.Service.Records.Base;
using System.Data;

namespace CDC.Service.Records
{
    public class CustomerCDCRecord : OutboxCDCRecord
    {
        private const string _pfiNumberFieldName = "PFINumber";
        private const string _pfiLoanNumberFieldName = "PFILoanNumber";
        private const string _loanAmountFieldName = "LoanAmount";
        private const string _mcNumberFieldName = "MCNumber";
        private const string _loanPurposeFieldName = "LoanPurpose";

        public CustomerCDCRecord()
        {
        }

        public CustomerCDCRecord(IDataReader reader)
            : base(reader)
        {
            PFINumber = ExtractValueFromReaderIfPresent<int>(reader, _pfiNumberFieldName);
            PFILoanNumber = ExtractValueFromReaderIfPresent<string>(reader, _pfiLoanNumberFieldName);
            LoanAmount = ExtractValueFromReaderIfPresent<decimal>(reader, _loanAmountFieldName);
            MCNumber = ExtractValueFromReaderIfPresent<int>(reader, _mcNumberFieldName);
            LoanPurpose = ExtractValueFromReaderIfPresent<string>(reader, _loanPurposeFieldName) ?? string.Empty;
        }

        public int? PFINumber { get; set; }

        public string PFILoanNumber { get; set; }

        public decimal LoanAmount { get; set; }

        public int? MCNumber { get; set; }

        public string LoanPurpose { get; set; }

        public static CustomerCDCRecord Update(IDataReader reader, ref CustomerCDCRecord loanCDCRecord)
        {
            var outboxRrecord = (OutboxCDCRecord)loanCDCRecord;
            Update(reader, ref outboxRrecord);
            loanCDCRecord.PFINumber = loanCDCRecord.ExtractValueFromReaderIfPresent<int>(reader, _pfiNumberFieldName);
            loanCDCRecord.PFILoanNumber =
                loanCDCRecord.ExtractValueFromReaderIfPresent<string>(reader, _pfiLoanNumberFieldName);
            loanCDCRecord.LoanAmount =
                loanCDCRecord.ExtractValueFromReaderIfPresent<decimal>(reader, _loanAmountFieldName);
            loanCDCRecord.MCNumber = loanCDCRecord.ExtractValueFromReaderIfPresent<int>(reader, _mcNumberFieldName);
            loanCDCRecord.LoanPurpose =
                loanCDCRecord.ExtractValueFromReaderIfPresent<string>(reader, _loanPurposeFieldName) ?? string.Empty;

            return loanCDCRecord;
        }
    }

}
