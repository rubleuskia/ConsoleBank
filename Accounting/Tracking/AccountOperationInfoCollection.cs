using System.Collections;
using System.Collections.Generic;

namespace Accounting.Tracking
{
    public class AccountOperationInfoCollection : IEnumerable<AccountOperationInfo>
    {
        private List<AccountOperationInfo> _operations = new();

        public AccountOperationInfoCollection(List<AccountOperationInfo> operations)
        {
            _operations = operations;
        }

        public AccountOperationInfoCollection GetTranferredInfos()
        {
            _operations.RemoveAll(x => x.Type != AccountOperationType.Transfer);
            return this;
        }

        public IEnumerator<AccountOperationInfo> GetEnumerator()
        {
            return _operations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
