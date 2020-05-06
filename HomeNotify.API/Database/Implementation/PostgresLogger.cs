using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HomeNotify.API.Database.Implementation
{
    public class PostgresLogger<T> : ILogger<T>, IDisposable
    {
        private readonly PostgresDatabaseConnectivityProvider db;

        public PostgresLogger(PostgresDatabaseConnectivityProvider db)
        {
            this.db = db;
            EnsureSchema();
        }

        private void EnsureSchema()
        {
            var createSchemaCommand = db.CreateCommand(
                @"
                    CREATE TABLE IF NOT EXISTS logs (
                        level       text        NOT NULL,
                        source      text        NOT NULL,
                        message     text        NOT NULL,
                        exception   json        NULL,
                        utc         timestamp   NOT NULL    DEFAULT (now() at time zone 'utc')
                    );
                ");

            createSchemaCommand.ExecuteNonQuery();
        }
        
        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var query = exception != null
                ? @"
                    INSERT INTO logs (level, source, message, exception)
                    VALUES (@level, @source, @message, @exception);
                "
                : @"
                    INSERT INTO logs (level, source, message)
                    VALUES (@level, @source, @message);
                ";
            var logCommand = db.CreateCommand(query);
            logCommand.Parameters.AddWithValue("level", logLevel.ToString());
            logCommand.Parameters.AddWithValue("source", typeof(T).ToString());
            logCommand.Parameters.AddWithValue("message", formatter.Invoke(state, exception));
            if (exception != null) logCommand.Parameters.AddWithValue("exception", 
                JsonConvert.SerializeObject(exception));

            logCommand.ExecuteNonQuery();
        }

        public void Dispose()
        {
        }
    }
}