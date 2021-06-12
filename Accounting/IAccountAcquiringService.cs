using System;
using System.Threading.Tasks;

namespace Accounting
{
    public delegate void WithdrawnHandler(Guid accountId, decimal amount);
    public delegate void AcquiredHandler(Guid accountId, decimal amount);

    public interface IAccountAcquiringService
    {
        Task<bool> TryLock(Guid accountId, Guid key);
        Task Withdraw(Guid accountId, decimal amount, Guid lockKey);
        Task Acquire(Guid accountId, decimal amount, Guid lockKey);
        Task Unlock(Guid accountId, Guid lockKey);
        event WithdrawnHandler Withdrawn;
        event AcquiredHandler Acquired;
    }
}
