using System;

namespace Accounting.EventHub
{
    public class AccountWithdrawnEvent : IEvent
    {
        public Guid AccountId { get; set; }

        public decimal Amount { get; set; }
    }
}
