using System.Threading.Tasks;

namespace Currencies.Common.Exchange
{
    public interface ICurrencyExchangeService
    {
        Task<decimal> ConvertToLocal(decimal amount, string charCode);
        Task<decimal> ConvertFromLocal(decimal amount, string charCode);
    }
}
