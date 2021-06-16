using System;

namespace Common.Accounting
{
    public class AccountTransferEvent : IEvent
    {
        public Guid ToAccount { get; set; }

        public Guid FromAccount { get; set; }

        public decimal Amount { get; set; }
    }
}
