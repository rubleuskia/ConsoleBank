using System;
using System.Threading.Tasks;

namespace Accounting
{
    public class AccountAcquiringService : IAccountAcquiringService
    {
        private readonly IAccountsRepository _accountsRepository;

        public AccountAcquiringService(IAccountsRepository accountsRepository)
        {
            _accountsRepository = accountsRepository;
        }

        public async Task Withdraw(Guid accountId, decimal amount)
        {
            var account = await _accountsRepository.GetById(accountId);

            if (account.Amount < amount)
            {
                throw new InvalidOperationException("Not enough money.");
            }

            account.Amount -= amount;
        }

        public async Task Acquire(Guid accountId, decimal amount)
        {
            var account = await _accountsRepository.GetById(accountId);
            account.Amount += amount;
        }
    }
}
