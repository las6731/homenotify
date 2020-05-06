using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HomeNotify.API.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class PostgresCredentials
    {
        /// <summary>
        /// The database hostname.
        /// </summary>
        public string Host;

        /// <summary>
        /// The database username.
        /// </summary>
        public string Username;

        /// <summary>
        /// The database password.
        /// </summary>
        public string Password;

        /// <summary>
        /// The database name.
        /// </summary>
        public string Database;
    }
}