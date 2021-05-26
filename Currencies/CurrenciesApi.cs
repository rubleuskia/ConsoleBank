using System;
using System.Threading.Tasks;
using Currencies.Entities;
using Flurl;
using Flurl.Http;

namespace Currencies
{
    public class CurrenciesApi : ICurrenciesApi
    {
        private const string CurrencyRatesApiUrl = "https://www.nbrb.by/api/exrates/rates";
        private const string CurrenciesApiUrl = "https://www.nbrb.by/api/exrates/currencies";

        public Task<Currency[]> GetCurrencies()
        {
            return CallApi(() => CurrenciesApiUrl.GetJsonAsync<Currency[]>());
        }

        public Task<CurrencyRate> GetCurrencyRate(int currencyId)
        {
            return CallApi(() => CurrencyRatesApiUrl.AppendPathSegment(currencyId).GetJsonAsync<CurrencyRate>());
        }

        private static async Task<T> CallApi<T>(Func<Task<T>> func)
        {
            try
            {
                return await func();
            }
            catch (FlurlHttpException e) when (e.StatusCode == 404)
            {
                throw new CurrencyNotAvailableException("Currency not available");
            }
        }
    }
}
