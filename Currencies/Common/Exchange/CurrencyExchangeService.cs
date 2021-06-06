using System;
using System.Threading.Tasks;
using Currencies.Common.Caching;
using Currencies.Common.Conversion;

namespace Currencies.Common.Exchange
{
    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly ICurrenciesApiCacheService _api;
        private readonly ICurrenciesConverter _currenciesConverter;

        public CurrencyExchangeService(ICurrenciesApiCacheService api, ICurrenciesConverter currenciesConverter)
        {
            _api = api;
            _currenciesConverter = currenciesConverter;
        }

        public Task<CurrencyRateModel> GetCurrencyRate(string charCode, DateTime? onDate = null)
        {
            return _api.GetCurrencyRate(charCode, onDate);
        }

        public async Task<decimal> ConvertToLocal(decimal amount, string charCode)
        {
            var rate = await GetCurrencyRate(charCode);
            return rate != null ? _currenciesConverter.ConvertToLocal(amount, rate) : 0;
        }

        public async Task<decimal> ConvertFromLocal(decimal amount, string charCode)
        {
            var rate = await GetCurrencyRate(charCode);
            return rate != null ? _currenciesConverter.ConvertFromLocal(amount, rate) : 0;
        }
    }
}
