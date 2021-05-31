using System;
using System.Linq;
using System.Threading.Tasks;
using Currencies.Entities;

namespace Currencies
{
    public class CurrencyInfoService : ICurrencyInfoService
    {
        private readonly ICurrenciesConverter _converter;
        private readonly ICurrenciesApi _api;
        private readonly string[] _availableCurrencies = { "USD", "EUR", "RUB" };

        public CurrencyInfoService(ICurrenciesApi currenciesApi, ICurrenciesConverter converter)
        {
            _api = currenciesApi;
            _converter = converter;
        }

        public async Task<string[]> GetAvailableCurrencies()
        {
            Currency[] currencies = await _api.GetCurrencies();

            return currencies
                .Where(currency => _availableCurrencies.Contains(currency.Abbreviation))
                .Select(currency => $"{currency.Abbreviation} - {currency.Name}")
                .ToArray();
        }

        public async Task<double> GetCurrencyRate(string abbreviation, DateTime? onDate)
        {
            CurrencyRate rate = await GetCurrencyRateInternal(abbreviation, onDate);
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
        private async Task<CurrencyRateShort[]> GetDynamics(string abbreviation, DateTime start, DateTime end)
        {
            if (start.Year <= 2016)
            {
                throw new NotImplementedException("Dates before denomination are not supported yet.");
            }

            var currencyId = await GetCurrencyId(abbreviation);
            return await _api.GetDynamics(currencyId, start, end);
        }

        // TODO: support denomination (add DateTime onDate)
        private async Task<int> GetCurrencyId(string abbreviation)
        {
            Currency[] currencies = await _api.GetCurrencies();
            return currencies.Single(x => x.Abbreviation == abbreviation).Id;
        }

        private async Task<CurrencyRate> GetCurrencyRateInternal(string abbreviation, DateTime? onDate = null)
        {
            if (!_availableCurrencies.Contains(abbreviation))
            {
                return null;
            }

            return await _api.GetCurrencyRate(abbreviation, onDate);
        }
    }
}
