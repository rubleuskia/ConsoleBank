namespace Currencies.Common.Conversion
{
    public class CurrenciesConverter : ICurrenciesConverter
    {
        public double ConvertTo(double amount, CurrencyRateModel rate)
        {
            return amount * rate.Rate / rate.Nominal;
        }

        public double ConvertFrom(double amount, CurrencyRateModel rate)
        {
            return amount / rate.Rate * rate.Nominal;
        }
    }
}
