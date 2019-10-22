using CDC.Service.Events.Base;

namespace CDC.Service.Events
{
    public class CustomerCDCEvent : CDCEvent
    {
        public int? PFINumber { get; set; }

        public string PFILoanNumber { get; set; }

        public decimal LoanAmount { get; set; }

        public int? MCNumber { get; set; }

        public string LoanPurpose { get; set; }
    }
}
