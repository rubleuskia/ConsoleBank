using System;
using System.Threading.Tasks;

namespace Accounting
{
    public interface IAccountsRepository
    {
        Task Add(Account account);
        Task Delete(Guid accountId);
        Task<Account> GetById(Guid accountId);
    }
}
