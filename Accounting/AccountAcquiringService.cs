using System;
using System.Threading.Tasks;
using Accounting.EventHub;

namespace Accounting
{
    public class AccountAcquiringService : IAccountAcquiringService
    {
        private readonly object _syncRoot = new();
        private readonly IAccountsRepository _accountsRepository;
        private readonly IEventBus _eventBus;

        public AccountAcquiringService(IAccountsRepository accountsRepository, IEventBus eventBus)
        {
            _accountsRepository = accountsRepository;
            _eventBus = eventBus;
        }

        public async Task Withdraw(Guid accountId, decimal amount, Guid lockKey)
        {
            var account = await _accountsRepository.GetById(accountId);

            if (account.LockKey != lockKey)
            {
                throw new InvalidOperationException("Account is locked");
            }

            if (account.Amount < amount)
            {
                throw new InvalidOperationException("Not enough money.");
            }

            account.Amount -= amount;
            _eventBus.Publish(new AccountWithdrawnEvent
            {
                AccountId = accountId,
                Amount = amount,
            });
        }

        public async Task Acquire(Guid accountId, decimal amount, Guid lockKey)
        {
            var account = await _accountsRepository.GetById(accountId);
            if (account.LockKey != lockKey)
            {
                throw new InvalidOperationException("Account is locked");
            }

            account.Amount += amount;
            _eventBus.Publish(new AccountAcquiredEvent
            {
                AccountId = accountId,
                Amount = amount,
            });
        }

        public async Task<bool> TryLock(Guid accountId, Guid key)
        {
            var account = await _accountsRepository.GetById(accountId);

            lock (_syncRoot)
            {
                if (account.LockKey.HasValue)
                {
                    return false;
                }

                account.LockKey = key;
                return true;
            }
        }

        public async Task Unlock(Guid accountId, Guid lockKey)
        {
            var account = await _accountsRepository.GetById(accountId);

            lock (_syncRoot)
            {
                if (!account.LockKey.HasValue || account.LockKey != lockKey)
                {
                    throw new InvalidOperationException("Attempt to unlock another transaction");
                }

                account.LockKey = null;
            }
        }
    }
}
