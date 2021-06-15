using System;
using System.Collections.Generic;

namespace Accounting.Tracking
{
    public delegate DateTime GetNowAtSite();

    // event Action<Guid, Guid, decimal> Transferred;
    // event Action<Guid, decimal> Acquired;
    // event Action<Guid, decimal> Withdrawn;
    public class AccountOperationsTrackingService : IAccountOperationsTrackingService
    {
        private readonly GetNowAtSite _getNowAtSite;
        private readonly List<AccountOperationInfo> _operations = new();

        public AccountOperationsTrackingService(
            IAccountAcquiringService accountAcquiringService,
            IAccountTransferService transferService,
            GetNowAtSite getNowAtSite)
        {
            _getNowAtSite = getNowAtSite;
            accountAcquiringService.Acquired += HandleAcquiringEvent;
            accountAcquiringService.Withdrawn += HandleWithdrawnEvent;
            transferService.Transferred += HandleTransferredEvent;
        }

        public AccountOperationInfoCollection GetOperations()
        {
            return new(_operations);
        }

        private void HandleAcquiringEvent(Guid accountId, decimal amount)
        {
            AddOperationInfo(accountId, amount, AccountOperationType.Acquire);
        }

        private void HandleWithdrawnEvent(Guid accountId, decimal amount)
        {
            AddOperationInfo(accountId, amount, AccountOperationType.Withdraw);
        }

        private void HandleTransferredEvent(Guid fromAccount, Guid toAccount, decimal amount)
        {
            AddOperationInfo(fromAccount, -amount, AccountOperationType.Transfer);
            AddOperationInfo(toAccount, amount, AccountOperationType.Transfer);
        }

        private void AddOperationInfo(Guid accountId, decimal amount, AccountOperationType type)
        {
            _operations.Add(new AccountOperationInfo
            {
                Amount = amount,
                AccountId = accountId,
                Type = type,
                Now = _getNowAtSite(),
            });
        }
    }
}
