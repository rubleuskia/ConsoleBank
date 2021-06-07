namespace Currencies.Common.Conversion
{
    // TODO write unit tests
    public class CurrenciesConverter : ICurrenciesConverter
    {
        public double ConvertToLocal(double amount, CurrencyRateModel rate)
        {
            return amount * rate.Rate / rate.Nominal;
        }

        public double ConvertFromLocal(double amount, CurrencyRateModel rate)
        {
            return amount / rate.Rate * rate.Nominal;
        }
    }
}
