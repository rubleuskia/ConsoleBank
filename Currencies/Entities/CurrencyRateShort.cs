using System;
using Newtonsoft.Json;

namespace Currencies.Entities
{
    public class CurrencyRateShort
    {
        [JsonProperty("Cur_ID")]
        public int Id { get; set; }

        [JsonProperty("Date")]
        public DateTime Date { get; set; }

        [JsonProperty("Cur_OfficialRate")]
        public double Rate { get; set; }
    }
}
