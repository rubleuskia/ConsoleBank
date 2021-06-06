using System;
using System.Xml.Serialization;

namespace Currencies.Apis.Rub.Entities
{
    [XmlRoot("ValCurs")]
    public class RubCurrencyRateResponse
    {
        // TODO fix
        [XmlElement("Date", DataType="date")]
        public DateTime Date { get; set; }

        [XmlElement("Valute")]
        public RubCurrencyRate[] Items { get; set; }
    }
}
