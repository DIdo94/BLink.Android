using Newtonsoft.Json;

namespace BLink.Business.Models
{
    public class UserCredentials
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        public string Id { get; set; }

        public string Roles { get; set; }
    }
}