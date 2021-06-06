using System;
using System.Linq;
using System.Threading.Tasks;
using Currencies.Common.Caching;

namespace Currencies.Common.Infos
{
    public class CurrencyInfoService : ICurrencyInfoService
    {
        private readonly ICurrenciesApiCacheService _api;

        public CurrencyInfoService(ICurrenciesApiCacheService currenciesApi)
        {
            _api = currenciesApi;
        }

        public async Task<string[]> GetAvailableCurrencies()
        {
            CurrencyModel[] currencies = await _api.GetCurrencies();

            return currencies
                .Select(currency => $"{currency.CharCode} - {currency.Name}")
                .ToArray();
        }

        public async Task<double> GetCurrencyRate(string charCode, DateTime? onDate)
        {
            CurrencyRateModel rate = await _api.GetCurrencyRate(charCode, onDate);
            return rate?.Rate ?? 0d;
        }

        public async Task<double> GetMinRate(string abbreviation, DateTime start, DateTime end)
        {
            var dynamics = await GetDynamics(abbreviation, start, end);
            return dynamics.Min(x => x.Rate);
        }

        public async Task<double> GetMaxRate(string abbreviation, DateTime start, DateTime end)
        {
            var dynamics = await GetDynamics(abbreviation, start, end);
            return dynamics.Max(x => x.Rate);
        }

        public async Task<double> GetAvgRate(string abbreviation, DateTime start, DateTime end)
        {
            var dynamics = await GetDynamics(abbreviation, start, end);
            return dynamics.Average(x => x.Rate);
        }

        private async Task<CurrencyRateModel[]> GetDynamics(string charCode, DateTime start, DateTime end)
        {
            return await _api.GetDynamics(charCode, start, end);
        }
    }
}
