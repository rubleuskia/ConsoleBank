using System;
using System.Threading.Tasks;

namespace Accounting
{
    public interface IAccountAcquiringService
    {
        Task<bool> TryLock(Guid accountId, Guid key);
        Task Withdraw(Guid accountId, decimal amount, Guid lockKey);
        Task Acquire(Guid accountId, decimal amount, Guid lockKey);
        Task Unlock(Guid accountId, Guid lockKey);
    }
}
