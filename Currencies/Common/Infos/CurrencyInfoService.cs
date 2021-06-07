using System;
using System.Linq;
using System.Threading.Tasks;
using Currencies.Common.Caching;
using Currencies.Common.Conversion;

namespace Currencies.Common.Infos
{
    public class CurrencyInfoService : ICurrencyInfoService
    {
        private readonly ICurrenciesConverter _converter;
        private readonly ICurrenciesApiCacheService _api;
        // TODO obsolete
        private readonly string[] _availableCurrencies = { "USD", "EUR", "RUB" };

        public CurrencyInfoService(ICurrenciesApiCacheService currenciesApi, ICurrenciesConverter converter)
        {
            _api = currenciesApi;
            _converter = converter;
        }

        public async Task<string[]> GetAvailableCurrencies()
        {
            CurrencyModel[] currencies = await _api.GetCurrencies();

            return currencies
                .Where(currency => _availableCurrencies.Contains(currency.CharCode))
                .Select(currency => $"{currency.CharCode} - {currency.Name}")
                .ToArray();
        }

        public async Task<double> GetCurrencyRate(string charCode, DateTime? onDate)
        {
            CurrencyRateModel rate = await GetCurrencyRateInternal(charCode, onDate);
            return rate?.Rate ?? 0d;
        }
        //
        // public async Task<double> ConvertTo(double amount, string abbreviation)
        // {
        //     var rate = await GetCurrencyRateInternal(abbreviation);
        //     return rate != null ? _converter.ConvertToLocal(amount, rate) : 0;
        // }
        //
        // public async Task<double> ConvertFrom(double amount, string abbreviation)
        // {
        //     var rate = await GetCurrencyRateInternal(abbreviation);
        //     return rate != null ? _converter.ConvertFromLocal(amount, rate) : 0;
        // }

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

        private async Task<CurrencyRateModel> GetCurrencyRateInternal(string abbreviation, DateTime? onDate = null)
        {
            if (!_availableCurrencies.Contains(abbreviation))
            {
                return null;
            }

            return await _api.GetCurrencyRate(abbreviation, onDate);
        }
    }
}
