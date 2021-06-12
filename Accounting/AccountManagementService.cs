using System;
using System.Threading.Tasks;

namespace Accounting
{
    public class AccountManagementService : IAccountManagementService
    {
        private readonly IAccountsRepository _accountsRepository;
        private readonly IAccountAcquiringService _accountAcquiringService;
        private readonly IAccountTransferService _accountTransferService;

        public AccountManagementService(
            IAccountsRepository accountsRepository,
            IAccountAcquiringService accountAcquiringService,
            IAccountTransferService accountTransferService)
        {
            _accountsRepository = accountsRepository;
            _accountAcquiringService = accountAcquiringService;
            _accountTransferService = accountTransferService;
        }

        public async Task<Guid> CreateAccount(Guid userId, string currencyCharCode)
        {
            var accountId = Guid.NewGuid();
            await _accountsRepository.Add(new Account
            {
                Amount = 0,
                Id = accountId,
                UserId = userId,
                CurrencyCharCode = currencyCharCode,
            });
            return accountId;
        }

        public Task DeleteAccount(Guid accountId)
        {
            return _accountsRepository.Delete(accountId);
        }

        public async Task Withdraw(Guid accountId, decimal amount)
        {
            AssertValidAmount(amount);
            var lockKey = Guid.NewGuid();
            if (await _accountAcquiringService.TryLock(accountId, lockKey))
            {
                await _accountAcquiringService.Withdraw(accountId, amount, lockKey);
                await _accountAcquiringService.Unlock(accountId, lockKey);
            }
        }

        public async Task Acquire(Guid accountId, decimal amount)
        {
            AssertValidAmount(amount);
            var lockKey = Guid.NewGuid();
            if (await _accountAcquiringService.TryLock(accountId, lockKey))
            {
                await _accountAcquiringService.Acquire(accountId, amount, lockKey);
                await _accountAcquiringService.Unlock(accountId, lockKey);
            }
        }

        public Task Transfer(AccountTransferParameters parameters)
        {
            return _accountTransferService.Transfer(parameters);
        }

        public Task<Account> GetAccount(Guid accountId)
        {
            return _accountsRepository.GetById(accountId);
        }

        private static void AssertValidAmount(decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException($"Invalid amount value: {amount}");
            }
        }
    }
}
