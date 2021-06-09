using System.Threading.Tasks;

namespace Accounting
{
    public interface IAccountTransferService
    {
        Task Transfer(AccountTransferParameters parameters);
    }
}
