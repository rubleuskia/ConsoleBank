using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Currencies.Common.Exchange;

namespace Accounting
{
    public class MoneyTransferService : IMoneyTransferService
    {
        private readonly IAccountsRepository _accountsRepository;
        private readonly ICurrencyExchangeService _currencyExchangeService;
        private readonly List<Transaction> _transactionsLog = new();

        public MoneyTransferService(
            IAccountsRepository accountsRepository,
            ICurrencyExchangeService currencyExchangeService)
        {
            _accountsRepository = accountsRepository;
            _currencyExchangeService = currencyExchangeService;
        }

        // TODO operation result
        public async Task Transfer(Guid from, Guid to, decimal amount)
        {
            Account fromAccount = _accountsRepository.GetAccount(from);
            var toAccount = _accountsRepository.GetAccount(to);

            if (fromAccount.CurrencyCode == toAccount.CurrencyCode)
            {
                Transfer(fromAccount, toAccount, amount);
            }
            else
            {
                await ConvertAndTransfer(fromAccount, toAccount, amount);
            }
        }

        private void Transfer(Account fromAccount, Account toAccount, decimal amount)
        {
            var transaction = new Transaction
            {
                Amount = amount,
                From = fromAccount.Id,
                To = toAccount.Id,
                CurrencyCodeFrom = fromAccount.CurrencyCode,
                CurrencyCodeTo = toAccount.CurrencyCode
            };

            _transactionsLog.Add(transaction);

            fromAccount.Withdraw(amount);
            transaction.IsWithdrawn = true;

            fromAccount.Acquire(amount);
            transaction.IsAcquired = true;
        }

        private async Task ConvertAndTransfer(Account fromAccount, Account toAccount, decimal amount)
        {
            var transaction = new Transaction
            {
                Amount = amount,
                From = fromAccount.Id,
                To = toAccount.Id,
                CurrencyCodeFrom = fromAccount.CurrencyCode,
                CurrencyCodeTo = toAccount.CurrencyCode
            };

            _transactionsLog.Add(transaction);

            fromAccount.Withdraw(amount);
            transaction.IsWithdrawn = true;

            var localAmount = await _currencyExchangeService.ConvertToLocal(amount, fromAccount.CurrencyCode);
            var toAmount = await _currencyExchangeService.ConvertFromLocal(localAmount, toAccount.CurrencyCode);
            toAccount.Acquire(toAmount);
            transaction.IsAcquired = true;
        }
    }
}
