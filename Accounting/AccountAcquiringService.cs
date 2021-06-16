using System;
using System.Threading.Tasks;
using Common;
using Common.Accounting;

namespace Accounting
{
    // Action => delegate void Name();
    // Action<Guid, decimal> => delegate void EventHandler(Guid accountId, decimal amount);
    public class AccountAcquiringService : IAccountAcquiringService
    {

        private readonly IEventBus _eventBus;
        private readonly IAccountsRepository _accountsRepository;

        public AccountAcquiringService(IAccountsRepository accountsRepository, IEventBus eventBus)
        {
            _accountsRepository = accountsRepository;
            _eventBus = eventBus;
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
            _eventBus.Publish(new AccountWithdrawEvent
            {
                AccountId = accountId,
                Amount = amount
            });
        }

        public async Task Acquire(Guid accountId, decimal amount)
        {
            var account = await _accountsRepository.GetById(accountId);
            account.Amount += amount;

            _eventBus.Publish(new AccountAcquiredEvent
            {
                AccountId = accountId,
                Amount = amount
            });
        }
    }
}
