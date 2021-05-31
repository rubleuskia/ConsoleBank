using System;
using System.Threading.Tasks;
using Currencies.Entities;

namespace Currencies
{
    interface ICurrenciesApiCacheService
    {
        Task Initialize();

        Task<Currency[]> GetCurrencies(bool afterDenomination = true);

        Task<CurrencyRate> GetCurrencyRate(int currencyId);

        Task<CurrencyRate> GetCurrencyRate(string currencyAbbreviation, DateTime? onDate = null);

        Task<CurrencyRateShort[]> GetDynamics(int currencyId, DateTime start, DateTime end);
    }
}
