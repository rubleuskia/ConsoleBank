using System;
using System.Threading.Tasks;

namespace Accounting
{
    public delegate void AccountTransferHandler(Guid from, Guid to, decimal amount);

    public interface IAccountTransferService
    {
        Task Transfer(AccountTransferParameters parameters);
        event AccountTransferHandler TransferPerformed;
    }
}
