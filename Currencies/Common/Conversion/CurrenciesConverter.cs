namespace Currencies.Common.Conversion
{
    // TODO write unit tests
    public class CurrenciesConverter : ICurrenciesConverter
    {
        public decimal ConvertToLocal(decimal amount, CurrencyRateModel rate)
        {
            return amount * (decimal)rate.Rate / rate.Nominal;
        }

        public decimal ConvertFromLocal(decimal amount, CurrencyRateModel rate)
        {
            return amount / (decimal)rate.Rate * rate.Nominal;
        }
    }
}
