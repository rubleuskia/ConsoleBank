using System;
using System.Linq;
using System.Threading.Tasks;
using Currencies.Common.Cache;
using Currencies.Common.Conversion;

namespace Currencies.Common.Info
{
    public class CurrencyInfoService : ICurrencyInfoService
    {
        private readonly ICurrenciesConverter _converter;
        private readonly ICurrenciesApiCacheService _api;
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

        public async Task<double> GetCurrencyRate(string abbreviation, DateTime? onDate)
        {
            CurrencyRateModel rate = await GetCurrencyRateInternal(abbreviation, onDate);
            return rate?.Rate ?? 0d;
        }

        public async Task<double> ConvertTo(double amount, string abbreviation)
        {
            var rate = await GetCurrencyRateInternal(abbreviation);
            return rate != null ? _converter.ConvertTo(amount, rate) : 0;
        }

        public async Task<double> ConvertFrom(double amount, string abbreviation)
        {
            var rate = await GetCurrencyRateInternal(abbreviation);
            return rate != null ? _converter.ConvertFrom(amount, rate) : 0;
        }

        public async Task<double> GetMinRate(string abbreviation, DateTime start, DateTime end)
        {
            var dynamics = await GetDynamics(abbreviation, start, end);
            return dynamics.Min(x => x.Rate);
        }

        public async Task<double> GetManRate(string abbreviation, DateTime start, DateTime end)
        {
            var dynamics = await GetDynamics(abbreviation, start, end);
            return dynamics.Max(x => x.Rate);
        }

        public async Task<double> GetAvgRate(string abbreviation, DateTime start, DateTime end)
        {
            var dynamics = await GetDynamics(abbreviation, start, end);
            return dynamics.Average(x => x.Rate);
        }

        // TODO: start < 2016 => additional handling (?)
        private async Task<CurrencyRateModel[]> GetDynamics(string charCode, DateTime start, DateTime end)
        {
            if (start.Year <= 2016)
            {
                throw new NotImplementedException("Dates before denomination are not supported yet.");
            }

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
