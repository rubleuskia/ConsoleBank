using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Currencies.Entities;

namespace Currencies
{
    public class CurrenciesApiCacheService : ICurrenciesApiCacheService
    {
        private Dictionary<string, List<CurrencyRate>> _ratesCache = new();

        private readonly ICurrenciesApi _currenciesApi;

        public CurrenciesApiCacheService(ICurrenciesApi _currenciesApi)
        {
            this._currenciesApi = _currenciesApi;
        }

        public Task Initialize()
        {
            // addition request for today's rates
            return Task.CompletedTask;
        }

        public Task<Currency[]> GetCurrencies(bool afterDenomination = true)
        {
            return _currenciesApi.GetCurrencies(afterDenomination);
        }

        public Task<CurrencyRate> GetCurrencyRate(int currencyId)
        {
            return _currenciesApi.GetCurrencyRate(currencyId);
        }

        public async Task<CurrencyRate> GetCurrencyRate(string currencyAbbreviation, DateTime? onDate = null)
        {
            // TODO: ConvertToKey(DateTime onDate)
            var date = (onDate ?? DateTime.Today).ToString();
            if (_ratesCache.ContainsKey(date)) // null
            {
                var rate = _ratesCache[onDate.ToString()].SingleOrDefault(x => x.Abbreviation == currencyAbbreviation);
                if (rate == null)
                {
                    rate = await _currenciesApi.GetCurrencyRate(currencyAbbreviation, onDate);
                    AddToCache(date, rate);
                    return rate;
                }
            }

            // TODO: move to common method
            var newRate = await _currenciesApi.GetCurrencyRate(currencyAbbreviation, onDate);
            AddToCache(date, newRate);
            return newRate;
        }

        public Task<CurrencyRateShort[]> GetDynamics(int currencyId, DateTime start, DateTime end)
        {
            return _currenciesApi.GetDynamics(currencyId, start, end);
        }

        private void AddToCache(string key, CurrencyRate rate)
        {
            if (_ratesCache.ContainsKey(key))
            {
                var value = _ratesCache[key];
                value.Add(rate);
            }
            else
            {
                _ratesCache.Add(key, new List<CurrencyRate> {rate});
            }
        }
    }
}
