using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting
{
    public class AccountsRepository : IAccountsRepository
    {
        private readonly List<Account> _accounts = new();

        public Task Add(Account account)
        {
            if (_accounts.All(x => x.Id != account.Id))
            {
                _accounts.Add(account);
            }

            return Task.CompletedTask;
        }

        public async Task Delete(Guid accountId)
        {
            Account result = await GetById(accountId);
            _accounts.Remove(result);
        }

        public Task<Account> GetById(Guid accountId)
        {
            var account = _accounts.SingleOrDefault(x => x.Id == accountId);
            if (account == null)
            {
                throw new InvalidOperationException("Cannot find an account with ID:" + accountId);
            }

            return Task.FromResult(account);
        }
    }
}
