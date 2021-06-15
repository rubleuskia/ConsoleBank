using System;
using System.Threading.Tasks;
using Currencies.Common.Conversion;

namespace Accounting
{
    public class AccountTransferService : IAccountTransferService
    {
        private readonly IAccountsRepository _accountsRepository;
        private readonly IAccountAcquiringService _accountAcquiringService;
        private readonly ICurrencyConversionService _currencyConversionService;

        public AccountTransferService(
            IAccountsRepository accountsRepository,
            IAccountAcquiringService accountAcquiringService,
            ICurrencyConversionService currencyConversionService)
        {
            _accountsRepository = accountsRepository;
            _accountAcquiringService = accountAcquiringService;
            _currencyConversionService = currencyConversionService;
        }

        public event Action<Guid, Guid, decimal> Transferred = (_, _, _) => {};

        public async Task Transfer(AccountTransferParameters parameters)
        {
            var fromAccount = await _accountsRepository.GetById(parameters.FromAccount);
            var toAccount = await _accountsRepository.GetById(parameters.ToAccount);

            // create guid
            // account1.LockTransactions(guid); | account.State = TransferInprogres
            // account2.LockTransactions(guid);

            var withdrawAmount = await _currencyConversionService.Convert(
                parameters.CurrencyCharCode,
                fromAccount.CurrencyCharCode,
                parameters.Amount);

            // Withdraw(guid)
            await _accountAcquiringService.Withdraw(parameters.FromAccount, withdrawAmount);

            // exception?
            // TODO: transaction logic
            // 1. persist transaction history
            // 2. rollback changes if operation fails
            var acquireAmount = await _currencyConversionService.Convert(
                parameters.CurrencyCharCode,
                toAccount.CurrencyCharCode,
                parameters.Amount);

            await _accountAcquiringService.Acquire(parameters.ToAccount, acquireAmount);
            // finally - unlock/rollback/etc

            Transferred(fromAccount.Id, toAccount.Id, parameters.Amount); // + charCode
        }
    }
}
