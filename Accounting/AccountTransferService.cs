using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Currencies.Common.Conversion;

namespace Accounting
{
    public class AccountTransferService : IAccountTransferService
    {
        private readonly IAccountsRepository _accountsRepository;
        private readonly IAccountAcquiringService _accountAcquiringService;
        private readonly ICurrencyConversionService _currencyConversionService;
        private readonly List<Transaction> _transactions = new();

        public AccountTransferService(
            IAccountsRepository accountsRepository,
            IAccountAcquiringService accountAcquiringService,
            ICurrencyConversionService currencyConversionService)
        {
            _accountsRepository = accountsRepository;
            _accountAcquiringService = accountAcquiringService;
            _currencyConversionService = currencyConversionService;
        }

        public event AccountTransferHandler TransferPerformed = (_, _, _) => {};

        public async Task Transfer(AccountTransferParameters parameters)
        {
            await RunWithTransaction(
                parameters,
                (lockKey) => PerformWithdraw(parameters, lockKey),
                (lockKey) => PerformAcquire(parameters, lockKey));
        }

        private async Task PerformAcquire(AccountTransferParameters parameters, Guid lockKey)
        {
            var toAccount = await _accountsRepository.GetById(parameters.ToAccount);

            var acquireAmount = await _currencyConversionService.Convert(
                parameters.CurrencyCharCode,
                toAccount.CurrencyCharCode,
                parameters.Amount);

            await _accountAcquiringService.Acquire(parameters.ToAccount, acquireAmount, lockKey);
        }

        private async Task PerformWithdraw(AccountTransferParameters parameters, Guid lockKey)
        {
            var fromAccount = await _accountsRepository.GetById(parameters.FromAccount);

            var withdrawAmount = await _currencyConversionService.Convert(
                parameters.CurrencyCharCode,
                fromAccount.CurrencyCharCode,
                parameters.Amount);

            await _accountAcquiringService.Withdraw(parameters.FromAccount, withdrawAmount, lockKey);
        }

        private async Task RunWithTransaction(
            AccountTransferParameters parameters,
            Func<Guid, Task> withdrawFunc,
            Func<Guid, Task> acquireFunc)
        {
            var lockKey = Guid.NewGuid();
            var toAccount = await _accountsRepository.GetById(parameters.ToAccount);
            var fromAccount = await _accountsRepository.GetById(parameters.FromAccount);

            var transaction = new Transaction
            {
                From = parameters.FromAccount,
                To = parameters.ToAccount,
                Amount = parameters.Amount,
                PreviousAmountFrom = fromAccount.Amount,
                PreviousAmountTo = toAccount.Amount,
                LockKey = lockKey,
            };

            _transactions.Add(transaction);

            try
            {
                if (!await _accountAcquiringService.TryLock(parameters.FromAccount, lockKey)
                    || !await _accountAcquiringService.TryLock(parameters.ToAccount, lockKey))
                {
                    return;
                }

                await withdrawFunc(lockKey);
                transaction.IsWithdrawCompleted = true;

                await acquireFunc(lockKey);
                transaction.IsAcquireCompleted = true;
                TransferPerformed(fromAccount.Id, toAccount.Id, parameters.Amount);
            }
            catch
            {
                await TryRollbackTransaction(transaction);
                throw;
            }
            finally
            {
                await _accountAcquiringService.Unlock(parameters.FromAccount, lockKey);
                await _accountAcquiringService.Unlock(parameters.ToAccount, lockKey);
            }
        }

        private async Task TryRollbackTransaction(Transaction transaction)
        {
            if (
                (transaction.IsAcquireCompleted && transaction.IsWithdrawCompleted)
                ||
                (!transaction.IsWithdrawCompleted && !transaction.IsAcquireCompleted)
            )
            {
                return;
            }

            if (!transaction.IsWithdrawCompleted)
            {
                await _accountAcquiringService.Withdraw(transaction.To, transaction.Amount, transaction.LockKey);
                return;
            }

            if (!transaction.IsAcquireCompleted)
            {
                await _accountAcquiringService.Acquire(transaction.From, transaction.Amount, transaction.LockKey);
                return;
            }
        }
    }
}
