using System;
using System.Linq;
using System.Threading.Tasks;
using Currencies.Apis.Rub.Entities;
using Currencies.Common;
using Currencies.Exceptions;
using Flurl;
using Flurl.Http;

namespace Currencies.Apis.Rub
{
    public class RubCurrenciesApi : ICurrenciesApi
    {
        // TODO RUB
        private const string RequestDateFormat = "dd/MM/yyyy";
        private const string BaseApiUrl = "http://www.cbr.ru/scripts";
        private readonly string _currencyRatesDynamicsApiUrl = $"{BaseApiUrl}/XML_dynamic.asp";
        private readonly string _currencyRatesApiUrl = $"{BaseApiUrl}/XML_daily.asp";
        private readonly string _currenciesApiUrl = $"{BaseApiUrl}/XML_valFull.asp";

        public async Task<CurrencyRateModel[]> GetDynamics(string charCode, DateTime start, DateTime end)
        {
            CurrencyRateModel currencyRate = await GetCurrencyRate(charCode, start);
            string xmlResponse = await CallApi(() => _currencyRatesDynamicsApiUrl
                .SetQueryParams(new
                {
                    date_req1 = GetFormattedDate(start),
                    date_req2 = GetFormattedDate(end),
                    VAL_NM_RQ = currencyRate.Id,
                })
                .GetStringAsync());

            var response = XmlUtils.ParseXml<RubCurrencyDynamicsResponse>(xmlResponse);
            return response.Records.Select(x => new CurrencyRateModel
            {
                Id = currencyRate.Id,
                Name = currencyRate.Name,
                CharCode = charCode,
                Date = x.Date,
                Nominal = x.Nominal,
                Rate = x.Rate,
            }).ToArray();
        }

        public async Task<CurrencyModel[]> GetCurrencies(DateTime? onDate = null)
        {
            var xmlResponse = await CallApi(() => _currenciesApiUrl.GetStringAsync());
            var response = XmlUtils.ParseXml<RubCurrenciesResponse>(xmlResponse);
            return response.Items.Select(x => new CurrencyModel
            {
                Id = x.Id,
                Name = x.Name,
                CharCode = x.CharCode
            }).ToArray();
        }

        public async Task<CurrencyRateModel> GetCurrencyRate(string charCode, DateTime? onDate = null)
        {
            var date = onDate ?? DateTime.Today;
            string xmlResponse = await CallApi(() => _currencyRatesApiUrl
                .SetQueryParam("date_req", GetFormattedDate(date))
                .GetStringAsync());

            var response = XmlUtils.ParseXml<RubCurrencyRateResponse>(xmlResponse);
            RubCurrencyRate rate = response.Items.Single(x => x.CharCode == charCode);
            return new CurrencyRateModel
            {
                Date = date,
                Id = rate.Id,
                Name = rate.Name,
                Nominal = rate.Nominal,
                Rate = rate.Rate,
                CharCode = rate.CharCode,
            };
        }

        // TODO: base class?
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

        private string GetFormattedDate(DateTime date)
        {
            return date.ToString(RequestDateFormat);
        }
    }
}
