using System;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Attributes;

namespace HomeNotify.API.Models
{
    public class LogInfo : ModelBase
    {
        [BsonElement("level")]
        public string Level;

        [BsonElement("source")]
        public string Source;

        [BsonElement("message")]
        public string Message;

        [BsonElement("exception")]
        [BsonIgnoreIfNull]
        public Exception Exception;

        [BsonElement("utc")]
        public DateTime Utc;

        public LogInfo(string level, string source, string message, Exception exception = null) : base()
        {
            Level = level;
            Source = source;
            Message = message;
            Exception = exception;
            Utc = DateTime.UtcNow;
        }
    }
}