using System;

namespace Accounting.EventHub
{
    public class AccountTransferredEvent : IEvent
    {
        public Guid From { get; set; }

        public Guid To { get; set; }

        public decimal Amount { get; set; }
    }
}
