using System.Xml.Serialization;

namespace Currencies.Apis.Rub.Entities
{
    [XmlRoot("ValCurs")]
    public class RubCurrencyDynamicsResponse
    {
        [XmlElement("Record")]
        public RubCurrencyDynamicsRecord[] Records { get; set; }
    }
}
