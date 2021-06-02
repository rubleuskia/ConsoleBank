using System;
using System.Xml.Serialization;

namespace Currencies.Apis.Rub.Entities
{
    public class CurrencyDynamicsInfo
    {
        [XmlElement("Date", DataType="date")]
        public DateTime Date { get; set; }

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