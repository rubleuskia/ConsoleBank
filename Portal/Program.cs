using System;
using System.Threading.Tasks;
using Currencies;
using Currencies.Entities;

namespace Portal
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var api = new CurrenciesApi();
            Currency[] currencies = await api.GetCurrencies();

            foreach (var currency in currencies)
            {
                Console.WriteLine(currency);
            }

            CurrencyRate rate = await api.GetCurrencyRate(20000);
            Console.WriteLine(rate);
        }
    }
}
