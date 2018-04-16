using Newtonsoft.Json;
using System.Collections.Generic;

namespace BLink.Business.Models
{
    public class ClubDetails
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Country { get; set; }
    }
}