using System;
using System.Threading.Tasks;

namespace Accounting
{
    public class AccountAcquiringService : IAccountAcquiringService
    {
        private readonly object _syncRoot = new();
        private readonly IAccountsRepository _accountsRepository;

        public AccountAcquiringService(IAccountsRepository accountsRepository)
        {
            _accountsRepository = accountsRepository;
        }

        public event WithdrawnHandler Withdrawn = (_, _) => {};

        public event AcquiredHandler Acquired = (_, _) => {};

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
            Withdrawn(accountId, amount);
        }

        public async Task Acquire(Guid accountId, decimal amount, Guid lockKey)
        {
            var account = await _accountsRepository.GetById(accountId);
            if (account.LockKey != lockKey)
            {
                throw new InvalidOperationException("Account is locked");
            }

            account.Amount += amount;
            Acquired(accountId, amount);
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
