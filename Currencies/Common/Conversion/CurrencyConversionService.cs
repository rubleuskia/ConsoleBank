using System.Threading.Tasks;
using Currencies.Common.Caching;

namespace Currencies.Common.Conversion
{
    public class CurrencyConversionService : ICurrencyConversionService
    {
        private readonly ICurrenciesApiCacheService _api;

        public CurrencyConversionService(ICurrenciesApiCacheService api)
        {
            _api = api;
        }

        public async Task<decimal> ConvertToLocal(decimal amount, string charCode)
        {
            var rate = await GetCurrencyRateInternal(charCode);
            return rate != null ? CurrenciesConverter.ConvertToLocal(amount, rate) : 0;
        }

        public async Task<decimal> ConvertFromLocal(decimal amount, string charCode)
        {
            var rate = await GetCurrencyRateInternal(charCode);
            return rate != null ? CurrenciesConverter.ConvertFromLocal(amount, rate) : 0;
        }

        /// <summary>
        /// (?) API
        /// from EUR
        /// to USD
        /// EUR -> (?) = LOCAL
        /// (?) -> USD
        /// </summary>
        public async Task<decimal> Convert(string fromCharCode, string toCharCode, decimal amount)
        {
            if (fromCharCode == toCharCode)
            {
                return amount;
            }

            var localFromAmount = await ConvertToLocal(amount, fromCharCode);
            return await ConvertFromLocal(localFromAmount, toCharCode);
        }

        private async Task<CurrencyRateModel> GetCurrencyRateInternal(string charCode)
        {
            return await _api.GetCurrencyRate(charCode);
        }
    }
}
