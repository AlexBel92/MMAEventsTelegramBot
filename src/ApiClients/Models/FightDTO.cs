using System.CodeDom.Compiler;
using Newtonsoft.Json;

namespace MMAEvents.ApiClients.Models
{
    [GeneratedCode("NJsonSchema", "10.4.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class FightDTO
    {
        [JsonProperty("weightClass", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string WeightClass { get; set; }

        [JsonProperty("firtsFighter", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string FirtsFighter { get; set; }

        [JsonProperty("secondFighter", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string SecondFighter { get; set; }

        [JsonProperty("method", Required = Required.Default, NullValueHandling =NullValueHandling.Ignore)]
        public string Method { get; set; }

        [JsonProperty("round", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public int Round { get; set; }

        [JsonProperty("time", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Time { get; set; }
    }
}