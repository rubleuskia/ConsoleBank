using Newtonsoft.Json;

namespace Currencies.Entities
{
    public class CurrencyRate
    {
        [JsonProperty("Cur_ID")]
        public int Id { get; set; }

        [JsonProperty("Cur_Scale")]
        public int Scale { get; set; }

        [JsonProperty("Cur_Abbreviation")]
        public string Abbreviation { get; set; }

        [JsonProperty("Cur_OfficialRate")]
        public double Rate { get; set; }

        public override string ToString()
        {
            return $"{Id} - {Scale} - {Rate}";
        }
    }
}
