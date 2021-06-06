namespace Currencies.Common.Conversion
{
    public interface ICurrenciesConverter
    {
        decimal ConvertToLocal(decimal amount, CurrencyRateModel rate);
        decimal ConvertFromLocal(decimal amount, CurrencyRateModel rate);
    }
}
