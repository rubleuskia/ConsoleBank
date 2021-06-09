using System;
using System.Linq;
using System.Threading.Tasks;
using Currencies.Apis.Byn.Entities;
using Currencies.Common;
using Currencies.Exceptions;
using Flurl;
using Flurl.Http;

namespace Currencies.Apis.Byn
{
    public class BynCurrenciesApi : ICurrenciesApi
    {
        // TODO BYN
        private const string BaseApiUrl = "https://www.nbrb.by/api/exrates";
        private readonly string _currencyRatesDynamicsApiUrl = $"{BaseApiUrl}/rates/dynamics";
        private readonly string _currencyRatesApiUrl = $"{BaseApiUrl}/rates";
        private readonly string _currenciesApiUrl = $"{BaseApiUrl}/currencies";

        public async Task<CurrencyModel[]> GetCurrencies(DateTime? onDate = null)
        {
            BynCurrency[] currencies = await GetCurrenciesInternal();
            return currencies.Select(x => new CurrencyModel
            {
                Id = x.Id.ToString(),
                Name = x.Name,
                CharCode = x.Abbreviation
            }).ToArray();
        }

        public async Task<CurrencyRateModel> GetCurrencyRate(string charCode, DateTime? onDate)
        {
            if (charCode == "BYN")
            {
                return new CurrencyRateModel
                {
                    Date = DateTime.Today,
                    Id = "0",
                    Name = "BYN",
                    Nominal = 1,
                    Rate = 1,
                    CharCode = "BYN"
                };
            }

            var result = _currencyRatesApiUrl
                .AppendPathSegment(charCode)
                .SetQueryParams(new
                {
                    parammode = 2,
                    ondate = onDate?.ToString()
                });

            var rate = await CallApi(() => result.GetJsonAsync<BynCurrencyRate>());
            return new CurrencyRateModel
            {
                Date = onDate ?? DateTime.Today,
                Id = rate.Id.ToString(),
                Name = rate.Name,
                Nominal = rate.Scale,
                Rate = rate.Rate,
                CharCode = charCode,
            };
        }

        public async Task<CurrencyRateModel[]> GetDynamics(string charCode, DateTime start, DateTime end)
        {
            BynCurrency[] currencies = await GetCurrenciesInternal();
            var currency = currencies.Single(x => x.Abbreviation == charCode);

            var dynamics = await CallApi(() => _currencyRatesDynamicsApiUrl
                .AppendPathSegment(currency.Id)
                .SetQueryParams(new
                {
                    startdate = start.ToString(),
                    enddate = end.ToString(),
                })
                .GetJsonAsync<BynCurrencyRateShort[]>());

            return dynamics.Select(rateShort => new CurrencyRateModel
            {
                Date = rateShort.Date,
                Id = currency.Id.ToString(),
                Name = currency.Name,
                Nominal = currency.Scale,
                Rate = rateShort.Rate,
                CharCode = currency.Abbreviation,
            }).ToArray();
        }

        private Task<BynCurrency[]> GetCurrenciesInternal()
        {
            return CallApi(() => _currenciesApiUrl.GetJsonAsync<BynCurrency[]>());
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
