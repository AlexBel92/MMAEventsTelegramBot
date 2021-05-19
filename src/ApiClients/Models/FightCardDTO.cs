using System.CodeDom.Compiler;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MMAEvents.ApiClients.Models
{
    [GeneratedCode("NJsonSchema", "10.4.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class FightCardDTO
    {
        [JsonProperty("name", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("fights", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<FightDTO> Fights { get; set; }
    }
}