using System;
using HomeNotify.API.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;

namespace HomeNotify.API.Database.Implementation
{
    [Collection("Logs")]
    public class MongoLogger<T> : MongoRepository<LogInfo>, ILogger<T>, IDisposable
    {
        public MongoLogger(IMongoDatabase db) : base(db, NullLogger<MongoRepository<LogInfo>>.Instance) {}
        
        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public async void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var logInfo = new LogInfo(logLevel.ToString(), typeof(T).ToString(), formatter.Invoke(state, exception), exception);
            await Insert(logInfo);
        }

        public void Dispose()
        {
        }
    }
}