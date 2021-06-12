using System;
using System.Collections.Generic;
using System.Linq;
using Accounting.EventHub;

namespace Accounting.ActivityTracking
{
    public class AccountActivityTracker : IAccountActivityTracker
    {
        private readonly List<AccountActivity> _activities = new();

        public AccountActivityTracker(IEventBus eventBus)
        {
            eventBus.Subscribe<AccountTransferredEvent>(OnAccountTransferred);
            eventBus.Subscribe<AccountWithdrawnEvent>(OnAccountWithdraw);
            eventBus.Subscribe<AccountAcquiredEvent>(OnAccountAcquired);
        }

        private void OnAccountTransferred(AccountTransferredEvent @event)
        {
            OnAccountActivity(ActivityType.Transfer, @event.From, -@event.Amount);
            OnAccountActivity(ActivityType.Transfer, @event.To, @event.Amount);
        }

        private void OnAccountWithdraw(AccountWithdrawnEvent @event)
        {
            OnAccountActivity(ActivityType.Withdraw, @event.AccountId, @event.Amount);
        }

        private void OnAccountAcquired(AccountAcquiredEvent @event)
        {
            OnAccountActivity(ActivityType.Acquire, @event.AccountId, @event.Amount);
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
