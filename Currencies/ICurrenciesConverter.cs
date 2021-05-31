using Currencies.Entities;

namespace Currencies
{
    public interface ICurrenciesConverter
    {
        double ConvertTo(double amount, CurrencyRate rate);
        double ConvertFrom(double amount, CurrencyRate rate);
    }
}