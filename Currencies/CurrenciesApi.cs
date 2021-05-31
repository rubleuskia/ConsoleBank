using System;
using System.Linq;
using System.Threading.Tasks;
using Currencies.Entities;
using Flurl;
using Flurl.Http;

namespace Currencies
{
    public class CurrenciesApi : ICurrenciesApi
    {
        private const string BaseApiUrl = "https://www.nbrb.by/api/exrates";
        private readonly string _currencyRatesDynamicsApiUrl = $"{BaseApiUrl}/rates/dynamics";
        private readonly string _currencyRatesApiUrl = $"{BaseApiUrl}/rates";
        private readonly string _currenciesApiUrl = $"{BaseApiUrl}/currencies";

        public async Task<Currency[]> GetCurrencies(bool afterDenomination)
        {
            var currencies = await CallApi(() => _currenciesApiUrl.GetJsonAsync<Currency[]>());
            return afterDenomination
                ? currencies.Where(currency => currency.DateEnd > DateTime.Now).ToArray()
                : currencies;
        }

        public Task<CurrencyRate> GetCurrencyRate(int currencyId)
        {
            return CallApi(() => _currencyRatesApiUrl.AppendPathSegment(currencyId).GetJsonAsync<CurrencyRate>());
        }

        public Task<CurrencyRate> GetCurrencyRate(string currencyAbbreviation, DateTime? onDate)
        {
            var result = _currencyRatesApiUrl
                .AppendPathSegment(currencyAbbreviation)
                .SetQueryParams(new
                {
                    parammode = 2,
                    ondate = onDate?.ToString()
                });

            return CallApi(() => result.GetJsonAsync<CurrencyRate>());
        }

        public Task<CurrencyRateShort[]> GetDynamics(int currencyId, DateTime start, DateTime end)
        {
            return CallApi(() => _currencyRatesDynamicsApiUrl
                .AppendPathSegment(currencyId)
                .SetQueryParams(new
                {
                    startdate = start.ToString(),
                    enddate = end.ToString(),
                })
                .GetJsonAsync<CurrencyRateShort[]>());
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
