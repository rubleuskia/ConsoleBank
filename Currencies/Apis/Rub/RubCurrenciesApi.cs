using System;
using System.Linq;
using System.Threading.Tasks;
using Currencies.Apis.Rub.Entities;
using Currencies.Common;
using Currencies.Exceptions;
using Flurl;
using Flurl.Http;
using static Currencies.Apis.Rub.RubCurrencyUtils;

namespace Currencies.Apis.Rub
{
    public class RubCurrenciesApi : ICurrenciesApi
    {
        private const string BaseApiUrl = "http://www.cbr.ru/scripts";
        private readonly string _currencyRatesDynamicsApiUrl = $"{BaseApiUrl}/XML_dynamic.asp";
        private readonly string _currencyRatesApiUrl = $"{BaseApiUrl}/XML_daily.asp";
        private readonly string _currenciesApiUrl = $"{BaseApiUrl}/XML_valFull.asp"; // TODO d0/d1

        public async Task<CurrencyModel[]> GetCurrencies()
        {
            var xmlResult = await CallApi(() => _currenciesApiUrl.GetStringAsync());
            var response = XmlUtils.Parse<CurrenciesResponse>(xmlResult);
            return response != null
                ? response.Items.Select(FromCurrencyItem).ToArray()
                : Array.Empty<CurrencyModel>();
        }

        public async Task<CurrencyModel> GetCurrency(string charCode)
        {
            var xmlResult = await CallApi(() => _currenciesApiUrl.GetStringAsync());
            var response = XmlUtils.Parse<CurrenciesResponse>(xmlResult);
            return FromCurrencyItem(response?.Items?.SingleOrDefault(x => x.CharCode == charCode));
        }

        public async Task<CurrencyRateModel> GetCurrencyRate(string charCode, DateTime? onDate = null)
        {
            var xmlResult = await CallApi(() => _currencyRatesApiUrl
                .SetQueryParam("date_req", onDate?.ToString("dd/MM/yyyy"))
                .GetStringAsync());

            var response = XmlUtils.Parse<CurrenciesRateResponse>(xmlResult);
            return FromCurrencyRateItem(response?.Items?.SingleOrDefault(x => x.CharCode == charCode), response.Date);
        }

        public async Task<CurrencyRateModel[]> GetDynamics(string charCode, DateTime start, DateTime end)
        {
            var currencyRate = await GetCurrencyRate(charCode);
            if (currencyRate == null)
            {
                return Array.Empty<CurrencyRateModel>();
            }

            var request = _currencyRatesDynamicsApiUrl
                .SetQueryParams(new
                {
                    date_req1 = start.ToString("dd/MM/yyyy"),
                    date_req2 = end.ToString("dd/MM/yyyy"),
                    VAL_NM_RQ = currencyRate.Id,
                });

            var xmlResult = await CallApi(() => request.GetStringAsync());
            var response = XmlUtils.Parse<CurrencyDynamicsResponse>(xmlResult);
            return response?.Items != null
                ? response.Items.Select(item => FromCurrencyDynamicsItem(item, currencyRate)).ToArray()
                : Array.Empty<CurrencyRateModel>();
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
