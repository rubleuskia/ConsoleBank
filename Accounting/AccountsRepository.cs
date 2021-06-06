using System;
using System.Collections.Generic;

namespace Accounting
{
    public class AccountsRepository : IAccountsRepository
    {
        public List<Account> Accounts { get; set; }

        public Account GetAccount(Guid id)
        {
            throw new NotImplementedException();
        }

        public Account CreateAccount()
        {
            throw new NotImplementedException();
        }

        public void DeleteAccount(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
