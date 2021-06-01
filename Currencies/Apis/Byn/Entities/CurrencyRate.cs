using System;
using Newtonsoft.Json;

namespace Currencies.Apis.Byn.Entities
{
    internal class CurrencyRate
    {
        [JsonProperty("Cur_ID")]
        public int Id { get; set; }

        [JsonProperty("Cur_Scale")]
        public int Scale { get; set; }

        [JsonProperty("Cur_Abbreviation")]
        public string Abbreviation { get; set; }

        [JsonProperty("Cur_Name")]
        public string Name { get; set; }

        [JsonProperty("Cur_OfficialRate")]
        public double Rate { get; set; }

        [JsonProperty("Date")]
        public DateTime Date { get; set; }
    }
}
