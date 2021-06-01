using System;

namespace Currencies.Common
{
    public class CurrencyModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string CharCode { get; set; }
    }

    public class CurrencyRateModel : CurrencyModel
    {
        public int Nominal { get; set; }

        public double Rate { get; set; }

        public DateTime Date { get; set; }
    }
}
