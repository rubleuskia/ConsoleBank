using System;

namespace Accounting.ActivityTracking
{
    public class AccountActivity
    {
        public ActivityType Type { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}