using System;

namespace Accounting.Tracking
{
    public class AccountOperationInfo
    {
        public AccountOperationType Type { get; set; }

        public Guid AccountId { get; set; }

        public decimal Amount { get; set; }

        public DateTime Now { get; set; }
    }
}
