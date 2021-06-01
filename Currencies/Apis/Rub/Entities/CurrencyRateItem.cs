using System.Globalization;
using System.Xml.Serialization;

namespace Currencies.Apis.Rub.Entities
{
    public class CurrencyRateItem
    {
        [XmlAttribute("ID")] public string Id { get; set; }

        [XmlElement("Name")] public string Name { get; set; }

        [XmlElement("NumCode")] public int NumericCode { get; set; }

        [XmlElement("CharCode")] public string CharCode { get; set; }

        [XmlElement("Nominal")] public int Nominal { get; set; }

        [XmlIgnore]
        public double Rate { get; set; }

        [XmlElement("Value")]
        public string RateSerialized
        {
            get => Rate.ToString("G17");
            set => Rate = double.Parse(value.Replace(",", "."));
        }
    }
}
