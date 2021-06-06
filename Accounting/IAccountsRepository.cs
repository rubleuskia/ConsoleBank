using System;

namespace Accounting
{
    public interface IAccountsRepository
    {
        Account GetAccount(Guid id);

        Account CreateAccount();

        void DeleteAccount(Guid id);
    }
}
