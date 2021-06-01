using System;
using Currencies.Apis.Rub.Entities;
using Currencies.Common;

namespace Currencies.Apis.Rub
{
    public static class RubCurrencyUtils
    {
        public static CurrencyModel FromCurrencyItem(CurrencyInfo item)
        {
            if (item == null)
            {
                return null;
            }

            return new CurrencyModel
            {
                Id = item.Id,
                Name = item.Name,
                CharCode = item.CharCode,
            };
        }


        public static CurrencyRateModel FromCurrencyRateItem(CurrencyRateItem item, DateTime date)
        {
            if (item == null)
            {
                return null;
            }

            return new CurrencyRateModel
            {
                Id = item.Id,
                Name = item.Name,
                CharCode = item.CharCode,
                Date = date,
                Nominal = item.Nominal,
                Rate = item.Rate,
            };
        }


        public static CurrencyRateModel FromCurrencyDynamicsItem(CurrencyDynamicsInfo item, CurrencyRateModel rateItem)
        {
            if (item == null)
            {
                return null;
            }

            return new CurrencyRateModel
            {
                Id = rateItem.Id,
                Name = rateItem.Name,
                CharCode = rateItem.CharCode,
                Date = item.Date,
                Nominal = item.Nominal,
                Rate = item.Rate,
            };
        }
    }
}
