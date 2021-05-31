using Currencies.Entities;

namespace Currencies
{
    // TODO write unit tests
    public class CurrenciesConverter : ICurrenciesConverter
    {
        public double ConvertTo(double amount, CurrencyRate rate)
        {
            return amount * rate.Rate / rate.Scale;
        }

        public double ConvertFrom(double amount, CurrencyRate rate)
        {
            return amount / rate.Rate * rate.Scale;
        }
    }
}
