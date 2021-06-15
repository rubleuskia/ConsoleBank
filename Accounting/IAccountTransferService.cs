using System;
using System.Threading.Tasks;

namespace Accounting
{
    public interface IAccountTransferService
    {
        Task Transfer(AccountTransferParameters parameters);
        event Action<Guid, Guid, decimal> Transferred;
    }
}
