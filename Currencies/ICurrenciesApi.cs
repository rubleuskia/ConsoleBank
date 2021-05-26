using System.Threading.Tasks;
using Currencies.Entities;

namespace Currencies
{
    public interface ICurrenciesApi
    {
        Task<Currency[]> GetCurrencies();
        Task<CurrencyRate> GetCurrencyRate(int currencyId);
    }
}
