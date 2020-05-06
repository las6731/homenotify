using System;
using System.Data;
using System.IO;
using HomeNotify.API.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;

namespace HomeNotify.API.Database.Implementation
{
    public class PostgresDatabaseConnectivityProvider : IDatabaseConnectivityProvider<NpgsqlCommand>
    {
        private NpgsqlConnection connection;

        public ConnectionResult Connect()
        {
            var path = Environment.GetEnvironmentVariable("DB_CREDENTIALS");
            if (path == null)
            {
                Console.WriteLine("Database credentials environment variable not set.");
                return ConnectionResult.FileNotFound;
            }

            PostgresCredentials dbCredentials;
            FileStream fileStream = null;
            StreamReader streamReader = null;
            
            try
            {
                fileStream = new FileStream(path, FileMode.Open);
                streamReader = new StreamReader(fileStream);
                dbCredentials = JsonConvert.DeserializeObject<PostgresCredentials>(streamReader.ReadToEnd());
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Database connection credentials file not found.");
                return ConnectionResult.FileNotFound;
            }
            finally
            {
                streamReader?.Close();
                fileStream?.Close();
            }
            
            var connectionString = $"Host={dbCredentials.Host};Username={dbCredentials.Username};Password={dbCredentials.Password};Database={dbCredentials.Database};";

            try
            {
                connection = new NpgsqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to connect to database: {dbCredentials.Database}");
                Console.WriteLine(e);
                return ConnectionResult.Failed;
            }
            
            Console.WriteLine($"Successfully connected to database: {dbCredentials.Database}");
            return ConnectionResult.Success;
        }

        public void Disconnect()
        {
            connection.Close();
        }

        public NpgsqlCommand CreateCommand(string query)
        {
            return new NpgsqlCommand(query, connection);
        }
    }
}