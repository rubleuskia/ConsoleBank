using System;

namespace Accounting.EventHub
{
    public class AccountAcquiredEvent : IEvent
    {
        public Guid AccountId { get; set; }

        public decimal Amount { get; set; }
    }
}
