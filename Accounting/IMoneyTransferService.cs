using System;
using System.Threading.Tasks;

namespace Accounting
{
    public interface IMoneyTransferService
    {
        Task Transfer(Guid from, Guid to, decimal amount);
    }
}
