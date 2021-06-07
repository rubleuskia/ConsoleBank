using System;
using System.Threading.Tasks;
using Currencies.Apis.Byn;
using Currencies.Apis.Rub;
using Currencies.Common.Caching;
using Currencies.Common.Conversion;
using Currencies.Common.Infos;

namespace Portal
{
    class Program
    {
        private static ICurrencyInfoService _infoService = new CurrencyInfoService(
            new CurrenciesApiCacheService(new RubCurrenciesApi()),
            new CurrenciesConverter()
        );

        static async Task Main(string[] args)
        {
            var currencies = await _infoService.GetAvailableCurrencies();

            foreach (var currency in currencies)
            {
                Console.WriteLine(currency);
            }

            var usdRate = await _infoService.GetCurrencyRate("USD");
            var eurRate = await _infoService.GetCurrencyRate("EUR", DateTime.Now.AddDays(-1000));
            Console.WriteLine($"USD rate: {usdRate}");
            Console.WriteLine($"EUR rate: {eurRate}");

            var result1 = await _infoService.ConvertFrom(100, "USD");
            Console.WriteLine("1: " + result1);
            // var result2 = await _infoService.ConvertTo(1000, "RUB");
            // Console.WriteLine("2: " + result2);

            var avg = await _infoService.GetAvgRate("USD", new DateTime(2020, 1, 1), new DateTime(2020, 12, 31));
            var min = await _infoService.GetMinRate("USD", new DateTime(2020, 1, 1), new DateTime(2020, 12, 31));
            var max = await _infoService.GetMaxRate("USD", new DateTime(2020, 1, 1), new DateTime(2020, 12, 31));

            Console.WriteLine("avg: " + avg);
            Console.WriteLine("min: " + min);
            Console.WriteLine("max: " + max);

            Console.ReadLine();
        }
    }
}
