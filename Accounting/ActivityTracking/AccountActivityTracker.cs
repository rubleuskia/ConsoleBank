using System;
using System.Collections.Generic;
using System.Linq;

namespace Accounting.ActivityTracking
{
    public enum ActivityType
    {
        Withdraw,
        Acquire,
        Transfer,
    }

    public class AccountActivityTracker : IAccountActivityTracker
    {
        private readonly List<AccountActivity> _activities = new();

        public AccountActivityTracker(IAccountAcquiringService acquiringService, IAccountTransferService transferService)
        {
            acquiringService.Acquired += OnAccountAcquired;
            acquiringService.Withdrawn += OnAccountWithdraw;

            transferService.TransferPerformed += OnAccountTransferred;
        }

        private void OnAccountTransferred(Guid from, Guid to, decimal amount)
        {
            OnAccountActivity(ActivityType.Transfer, from, -amount);
            OnAccountActivity(ActivityType.Transfer, to, amount);
        }

        private void OnAccountWithdraw(Guid accountId, decimal amount)
        {
            OnAccountActivity(ActivityType.Withdraw, accountId, amount);
        }

        private void OnAccountAcquired(Guid accountId, decimal amount)
        {
            OnAccountActivity(ActivityType.Acquire, accountId, amount);
        }

        private void OnAccountActivity(ActivityType type, Guid accountId, decimal amount)
        {
            _activities.Add(new AccountActivity
            {
                Amount = amount,
                Type = type,
                AccountId = accountId,
            });
        }

        public IEnumerable<AccountActivity> GetActivities(Guid accountId)
        {
            return _activities.Where(activity => activity.AccountId == accountId);
        }
    }
}
