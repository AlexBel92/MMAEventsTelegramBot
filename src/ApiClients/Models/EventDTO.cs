using System.CodeDom.Compiler;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MMAEvents.ApiClients.Models
{
    [GeneratedCode("NJsonSchema", "10.4.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class EventDTO
    {
        [JsonProperty("id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long Id { get; set; }

        [JsonProperty("name", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("date", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public System.DateTimeOffset Date { get; set; }

        [JsonProperty("imgSrc", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string ImgSrc { get; set; }

        [JsonProperty("location", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Location { get; set; }

        [JsonProperty("isScheduled", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool IsScheduled { get; set; }

        [JsonProperty("isCanceled", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool IsCanceled { get; set; } = false;

        [JsonProperty("fightCard", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<FightCardDTO> FightCard { get; set; }

        [JsonProperty("bonusAwards", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> BonusAwards { get; set; }

    }
}