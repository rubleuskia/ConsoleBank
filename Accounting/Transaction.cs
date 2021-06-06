using System;

namespace Accounting
{
    public class Transaction
    {
        public Guid Guid => Guid.NewGuid();

        public Guid From { get; set; }

        public Guid To { get; set; }

        public bool IsWithdrawn { get; set; }

        public bool IsAcquired { get; set; }

        public string CurrencyCodeFrom { get; set; }

        public string CurrencyCodeTo { get; set; }

        public decimal Amount { get; set; }
    }
}
