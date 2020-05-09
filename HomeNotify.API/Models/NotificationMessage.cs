using MongoDB.Bson.Serialization.Attributes;

namespace HomeNotify.API.Models
{
    public class NotificationMessage : ModelBase
    {
        [BsonElement("title")]
        public string Title { get; set; }
        
        [BsonElement("body")]
        public string Body { get; set; }
    }
}