using System;
using System.Xml.Serialization;

namespace Currencies.Apis.Rub.Entities
{
    [XmlRoot("ValCurs")]
    public class CurrenciesRateResponse
    {
        [XmlElement("Valute")]
        public CurrencyRateItem[] Items { get; set; }

        [XmlElement("Date", DataType="date")]
        public DateTime Date { get; set; }
    }
}
