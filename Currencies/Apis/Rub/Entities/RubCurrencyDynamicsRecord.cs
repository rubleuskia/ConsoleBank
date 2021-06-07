using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Currencies.Apis.Rub.Entities
{
    public class RubCurrencyDynamicsRecord
    {
        [XmlIgnore]
        public DateTime Date { get; set; }

        [XmlElement("Date")]
        public string DateSerialized
        {
            get => Date.ToString("dd'.'MM'.'yyyy");
            set => Date = DateTime.ParseExact(value, "dd'.'MM'.'yyyy", CultureInfo.InvariantCulture);
        }

        [XmlElement("Nominal")]
        public int Nominal { get; set; }

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
