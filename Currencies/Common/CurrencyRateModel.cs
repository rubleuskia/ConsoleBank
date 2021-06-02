using System;

namespace Currencies.Common
{
    public class CurrencyRateModel : CurrencyModel
    {
        public int Nominal { get; set; }

        public double Rate { get; set; }

        public DateTime Date { get; set; }
    }
}
