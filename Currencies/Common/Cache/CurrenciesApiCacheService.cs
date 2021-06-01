using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Currencies.Common.Cache
{
    public class CurrenciesApiCacheService : ICurrenciesApiCacheService
    {
        private readonly Dictionary<string, List<CurrencyRateModel>> _ratesCache = new();
        private readonly Dictionary<string, CurrencyModel> _currenciesCache = new();

        private readonly ICurrenciesApi _currenciesApi;

        public CurrenciesApiCacheService(ICurrenciesApi currenciesApi)
        {
            _currenciesApi = currenciesApi;
        }

        public async Task<CurrencyModel[]> GetCurrencies()
        {
            await EnsureCurrenciesCache();
            return _currenciesCache.Values.ToArray();
        }

        public async Task<CurrencyModel> GetCurrency(string charCode)
        {
            await EnsureCurrenciesCache();
            return _currenciesCache[charCode];
        }

        public async Task<CurrencyRateModel> GetCurrencyRate(string charCode, DateTime? onDate = null)
        {
            var key = GetRateKey(onDate ?? DateTime.Today);
            if (_ratesCache.ContainsKey(key))
            {
                var rate = _ratesCache[key].SingleOrDefault(x => x.CharCode == charCode);
                if (rate != null)
                {
                    return rate;
                }
            }

            return await RequestNewRate(charCode, onDate, key);
        }

        public async Task<CurrencyRateModel[]> GetDynamics(string charCode, DateTime start, DateTime end)
        {
            if ((end - start).Days > 365)
            {
                return Array.Empty<CurrencyRateModel>();
            }

            if (IsCachedPeriod(charCode, start, end))
            {
                return GetCachedDynamics(charCode, start, end);
            }

            return await GetDynamicsInternal(charCode, start, end);
        }

        private async Task<CurrencyRateModel[]> GetDynamicsInternal(string charCode, DateTime start, DateTime end)
        {
            var dynamics = await _currenciesApi.GetDynamics(charCode, start, end);
            foreach (var rateModel in dynamics)
            {
                var key = GetRateKey(rateModel.Date.Date);
                AddToCache(key, rateModel);
            }

            return dynamics;
        }

        private CurrencyRateModel[] GetCachedDynamics(string charCode, DateTime start, DateTime end)
        {
            var result = new List<CurrencyRateModel>();
            for (var dt = start; dt <= end; dt = dt.AddDays(1))
            {
                var key = GetRateKey(dt.Date);
                result.Add(_ratesCache[key].Single(x => x.CharCode == charCode));
            }

            return result.ToArray();
        }

        private async Task<CurrencyRateModel> RequestNewRate(string charCode, DateTime? onDate, string key)
        {
            var rate = await _currenciesApi.GetCurrencyRate(charCode, onDate);
            if (rate != null)
            {
                AddToCache(key, rate);
            }

            return rate;
        }

        private string GetRateKey(DateTime date)
        {
            return date.ToString("MM/dd/yyyy");
        }

        private string GetCurrencyKey(CurrencyModel model)
        {
            return $"{model.CharCode}:{model.Id}";
        }

        private void AddToCache(string key, CurrencyRateModel rate)
        {
            if (_ratesCache.ContainsKey(key))
            {
                var value = _ratesCache[key];
                if (value.All(x => x.CharCode != rate.CharCode))
                {
                    value.Add(rate);
                }
            }
            else
            {
                _ratesCache.Add(key, new List<CurrencyRateModel> {rate});
            }
        }

        private async Task EnsureCurrenciesCache()
        {
            if (!_currenciesCache.Any())
            {
                var currencies = await _currenciesApi.GetCurrencies();
                foreach (var currency in currencies)
                {
                    _currenciesCache.Add(GetCurrencyKey(currency), currency);
                }
            }
        }

        private bool IsCachedPeriod(string charCode, DateTime start, DateTime end)
        {
            for (int i = 0; i < (end - start).Days; i++)
            {
                var key = GetRateKey(start.Date.AddDays(i));
                if (!_ratesCache.ContainsKey(key))
                {
                    return false;
                }

                if (_ratesCache[key].All(r => r.CharCode != charCode))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
