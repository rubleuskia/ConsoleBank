using System.Xml.Serialization;

namespace Currencies.Apis.Rub.Entities
{
    [XmlRoot("Valuta")]
    public class CurrenciesResponse
    {
        [XmlElement("Item")]
        public CurrencyInfo[] Items { get; set; }
    }
}
