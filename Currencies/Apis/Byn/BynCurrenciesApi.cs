using System;
using System.Linq;
using System.Threading.Tasks;
using Currencies.Apis.Byn.Entities;
using Currencies.Common;
using Currencies.Exceptions;
using Flurl;
using Flurl.Http;
using static Currencies.Apis.Byn.BynCurrencyUtils;

namespace Currencies.Apis.Byn
{
    public class BynCurrenciesApi : ICurrenciesApi
    {
        private const string BaseApiUrl = "https://www.nbrb.by/api/exrates";
        private readonly string _currencyRatesDynamicsApiUrl = $"{BaseApiUrl}/rates/dynamics";
        private readonly string _currencyRatesApiUrl = $"{BaseApiUrl}/rates";
        private readonly string _currenciesApiUrl = $"{BaseApiUrl}/currencies";

        public async Task<CurrencyModel[]> GetCurrencies()
        {
            var currencies = await GetCurrenciesInternal();
            return currencies.Select(FromCurrency).ToArray();
        }

        public async Task<CurrencyModel> GetCurrency(string charCode)
        {
            var currencies = await GetCurrenciesInternal();
            return FromCurrency(currencies.Single(x => x.Abbreviation == charCode));
        }

        public async Task<CurrencyRateModel> GetCurrencyRate(string charCode, DateTime? onDate = null)
        {
            var rate = await GetCurrencyRateInternal(charCode, onDate);
            return FromCurrencyRate(rate);
        }

        private Task<CurrencyRate> GetCurrencyRateInternal(string charCode, DateTime? onDate = null)
        {
            var request = _currencyRatesApiUrl
                .AppendPathSegment(charCode)
                .SetQueryParams(new
                {
                    parammode = 2,
                    ondate = onDate?.ToString()
                });

            return CallApi(() => request.GetJsonAsync<CurrencyRate>());
        }

        public async Task<CurrencyRateModel[]> GetDynamics(string charCode, DateTime start, DateTime end)
        {
            var currencyId = await GetCurrencyId(charCode);
            var rate = await GetCurrencyRateInternal(charCode);
            var dynamics = await GetDynamicsInternal(currencyId, start, end);
            return dynamics.Select(shortRate => FromCurrencyRateShort(shortRate, rate)).ToArray();
        }

        private Task<Currency[]> GetCurrenciesInternal()
        {
            return CallApi(() => _currenciesApiUrl.GetJsonAsync<Currency[]>());
        }

        private Task<CurrencyRateShort[]> GetDynamicsInternal(int currencyId, DateTime start, DateTime end)
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

        private async Task<int> GetCurrencyId(string charCode)
        {
            var currencies = await GetCurrenciesInternal();
            return currencies.Single(x => x.Abbreviation == charCode).Id;
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
