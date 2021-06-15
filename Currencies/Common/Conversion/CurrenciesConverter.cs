namespace Currencies.Common.Conversion
{
    // 1. Currencies.Test
    // 2. add nuget packages, xUnit, FA, SDK
    // 3. тестовый класс - CurrenciesConverterTest
    // 4. много тестов на 1 метод - входные параметры
    // 5. ИмяМетода_Условие_Результат
    // 6. ААА = arrange + act + assert

    // TODO write unit tests
    public static class CurrenciesConverter
    {
        public static decimal ConvertToLocal(decimal amount, CurrencyRateModel rate)
        {
            return amount * (decimal)rate.Rate / (rate.Nominal);
        }

        public static decimal ConvertFromLocal(decimal amount, CurrencyRateModel rate)
        {
            return amount / (decimal)rate.Rate * rate.Nominal;
        }
    }
}
