using System;
using Newtonsoft.Json;

namespace Currencies.Apis.Byn.Entities
{
    internal class Currency
    {
        [JsonProperty("Cur_ID")]
        public int Id { get; set; }

        [JsonProperty("Cur_Code")]
        public int Code { get; set; }

        [JsonProperty("Cur_Abbreviation")]
        public string Abbreviation { get; set; }

        [JsonProperty("Cur_Name")]
        public string Name { get; set; }

        [JsonProperty("Cur_DateEnd")]
        public DateTime DateEnd { get; set; }
    }
}
