using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HomeNotify.API.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DatabaseCredentials
    {
        /// <summary>
        /// The database hostname.
        /// </summary>
        [BsonElement("host")]
        public string Host;

        /// <summary>
        /// The database username.
        /// </summary>
        [BsonElement("username")]
        public string Username;

        /// <summary>
        /// The database password.
        /// </summary>
        [BsonElement("password")]
        public string Password;

        /// <summary>
        /// The database name.
        /// </summary>
        [BsonElement("database")]
        public string Database;
    }
}