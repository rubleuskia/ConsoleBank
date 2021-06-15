namespace Accounting.Tracking
{
    public interface IAccountOperationsTrackingService
    {
        AccountOperationInfoCollection GetOperations();
    }
}
