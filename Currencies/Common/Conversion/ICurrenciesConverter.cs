namespace Currencies.Common.Conversion
{
    public interface ICurrenciesConverter
    {
        double ConvertTo(double amount, CurrencyRateModel rate);
        double ConvertFrom(double amount, CurrencyRateModel rate);
    }
}
