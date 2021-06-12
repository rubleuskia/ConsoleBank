using System;
using System.Collections.Generic;

namespace Accounting.ActivityTracking
{
    public interface IAccountActivityTracker
    {
        IEnumerable<AccountActivity> GetActivities(Guid accountId);
    }
}
