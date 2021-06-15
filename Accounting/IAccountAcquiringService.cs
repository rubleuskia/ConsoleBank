using System;
using System.Threading.Tasks;

namespace Accounting
{
    public interface IAccountAcquiringService
    {
        Task Withdraw(Guid accountId, decimal amount);
        Task Acquire(Guid accountId, decimal amount);
        event Action<Guid, decimal> Acquired;
        event Action<Guid, decimal> Withdrawn;
    }
}
