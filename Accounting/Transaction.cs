using System;

namespace Accounting
{
    public class Transaction
    {
        public Guid From { get; set; }

        public Guid To { get; set; }

        public decimal Amount { get; set; }

        public decimal PreviousAmountFrom { get; set; }
        public decimal PreviousAmountTo { get; set; }

        public bool IsWithdrawCompleted { get; set; }

        public bool IsAcquireCompleted { get; set; }

        public Guid LockKey { get; set; }
    }
}
