using System;
using System.Threading.Tasks;

namespace Accounting
{
    // Action => delegate void Name();
    // Action<Guid, decimal> => delegate void EventHandler(Guid accountId, decimal amount);
    public class AccountAcquiringService : IAccountAcquiringService
    {
        private readonly IAccountsRepository _accountsRepository;

        public event Action<Guid, decimal> Acquired = (accountId, amount) => {};
        public event Action<Guid, decimal> Withdrawn = (accountId, amount) => {};

        public AccountAcquiringService(IAccountsRepository accountsRepository)
        {
            _accountsRepository = accountsRepository;
        }

        // transfer or direct (?)
        public async Task Withdraw(Guid accountId, decimal amount)
        {
            var account = await _accountsRepository.GetById(accountId);

            if (account.Amount < amount)
            {
                throw new InvalidOperationException("Not enough money.");
            }

            account.Amount -= amount;
            Withdrawn(accountId, amount);
        }

        public async Task Acquire(Guid accountId, decimal amount)
        {
            var account = await _accountsRepository.GetById(accountId);
            account.Amount += amount;
            Acquired(accountId, amount);
        }
    }
}
