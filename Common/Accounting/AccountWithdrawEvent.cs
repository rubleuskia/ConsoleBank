using System;

namespace Common.Accounting
{
    public class AccountWithdrawEvent : IEvent
    {
        public Guid AccountId { get; set; }

        public decimal Amount { get; set; }
    }
}
