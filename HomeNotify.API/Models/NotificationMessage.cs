using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HomeNotify.API.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class NotificationMessage
    {
        public string Title { get; set; }
        
        public string Body { get; set; }
    }
}