using Currencies.Apis.Byn.Entities;
using Currencies.Common;

namespace Currencies.Apis.Byn
{
    internal static class BynCurrencyUtils
    {
        public static CurrencyModel FromCurrency(Currency currency)
        {
            return new()
            {
                Id = currency.Id.ToString(),
                Name = currency.Name,
                CharCode = currency.Abbreviation,
            };
        }

        public static CurrencyRateModel FromCurrencyRate(CurrencyRate rate)
        {
            return new()
            {
                Id = rate.Id.ToString(),
                Name = rate.Name,
                Nominal = rate.Scale,
                Rate = rate.Rate,
                CharCode = rate.Abbreviation,
                Date = rate.Date
            };
        }

        public static CurrencyRateModel FromCurrencyRateShort(CurrencyRateShort rateShort, CurrencyRate rate)
        {
            return new()
            {
                Id = rate.Id.ToString(),
                Name = rate.Name,
                Nominal = rate.Scale,
                Rate = rateShort.Rate,
                CharCode = rate.Abbreviation,
                Date = rate.Date,
            };
        }
    }
}
