using System;
using System.Collections.Generic;
using Common;
using Common.Accounting;

namespace Accounting.Tracking
{
    public delegate DateTime GetNowAtSite();

    // event Action<Guid, Guid, decimal> Transferred;
    // event Action<Guid, decimal> Acquired;
    // event Action<Guid, decimal> Withdrawn;
    public class AccountOperationsTrackingService : IAccountOperationsTrackingService
    {
        private readonly GetNowAtSite _getNowAtSite;
        private readonly IEventBus _eventBus;
        private readonly List<AccountOperationInfo> _operations = new();

        public AccountOperationsTrackingService(GetNowAtSite getNowAtSite, IEventBus eventBus)
        {
            _getNowAtSite = getNowAtSite;
            _eventBus = eventBus;

            SubscribeOnEvents();
        }

        private void SubscribeOnEvents()
        {
            _eventBus.Subscribe<AccountAcquiredEvent>(HandleAcquiringEvent);
            _eventBus.Subscribe<AccountWithdrawEvent>(HandleWithdrawnEvent);
            _eventBus.Subscribe<AccountTransferEvent>(HandleTransferredEvent);
        }

        public AccountOperationInfoCollection GetOperations()
        {
            return new(_operations);
        }

        private void HandleAcquiringEvent(AccountAcquiredEvent @event)
        {
            AddOperationInfo(@event.AccountId, @event.Amount, AccountOperationType.Acquire);
        }

        private void HandleWithdrawnEvent(AccountWithdrawEvent @event)
        {
            AddOperationInfo(@event.AccountId, @event.Amount, AccountOperationType.Withdraw);
        }

        private void HandleTransferredEvent(AccountTransferEvent @event)
        {
            AddOperationInfo(@event.FromAccount, -@event.Amount, AccountOperationType.Transfer);
            AddOperationInfo(@event.ToAccount, @event.Amount, AccountOperationType.Transfer);
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
