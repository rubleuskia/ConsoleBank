using System.Xml.Serialization;

namespace Currencies.Apis.Rub.Entities
{
    public class CurrencyInfo
    {
        [XmlAttribute("ID")]
        public string Id { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Nominal")]
        public int Nominal { get; set; }

        [XmlElement("ISO_Char_Code")]
        public string CharCode { get; set; }
    }
}
