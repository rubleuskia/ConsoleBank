using System;
using System.Threading.Tasks;

namespace Accounting
{
    public interface IAccountsManager
    {
        Task<Guid> CreateAccount();
        Task DeleteAccount();
        Task TransferMoney(Guid from, Guid to);
    }
}
