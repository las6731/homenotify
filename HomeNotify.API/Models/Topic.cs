using MongoDB.Bson.Serialization.Attributes;

namespace HomeNotify.API.Models
{
    /// <summary>
    /// A notification topic.
    /// </summary>
    public class Topic : ModelBase
    {
        /// <summary>
        /// The topic name.
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; }
    }
}