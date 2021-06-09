using System;
using System.Threading.Tasks;

namespace Accounting
{
    public interface IAccountManagementService
    {
        Task<Guid> CreateAccount(Guid userId, string currencyCharCode);

        Task DeleteAccount(Guid accountId);

        Task Withdraw(Guid accountId, decimal amount);

        Task Acquire(Guid accountId, decimal amount);

        Task Transfer(AccountTransferParameters parameters);

        Task<Account> GetAccount(Guid accountId);
    }
}
