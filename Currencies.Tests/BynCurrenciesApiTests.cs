using System;
using System.Net.Http;
using System.Threading.Tasks;
using Currencies.Apis.Byn;
using Currencies.Common;
using FluentAssertions;
using Flurl.Http.Testing;
using Xunit;

namespace Currencies.Tests
{
    public class BynCurrenciesApiTests
    {
        [Fact]
        public async Task GetCurrencyRate_Always_ReturnsRate()
        {
            // arrange
            using var httpTest = new HttpTest();

            var ondate = DateTime.Now;

            httpTest
                .ForCallsTo("https://www.nbrb.by/api/exrates/rates/USD")
                .WithVerb(HttpMethod.Get)
                .WithQueryParam("parammode", 2)
                .WithQueryParam("ondate", ondate.ToString())
                .RespondWith(
                    "{\"Cur_ID\":145,\"Date\":\"2021-06-15T00:00:00\",\"Cur_Abbreviation\":\"USD\",\"Cur_Scale\":1,\"Cur_Name\":\"Доллар США\",\"Cur_OfficialRate\":2.4892}");

            // act
            var api = new BynCurrenciesApi();
            var result = await api.GetCurrencyRate("USD", ondate);

            // assert
            result.Should().BeEquivalentTo(new CurrencyRateModel
            {
                Date = ondate,
                Nominal = 1,
                Rate = 2.4892,
                Id = "145",
                Name = "Доллар США",
                CharCode = "USD"
            });
        }
    }
}
