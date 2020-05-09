using System;
using System.IO;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using HomeNotify.API.Database.Implementation;
using HomeNotify.API.Models;
using HomeNotify.API.Repositories;
using HomeNotify.API.Repositories.Implementation;
using HomeNotify.API.Services;
using HomeNotify.API.Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Unity;
using Unity.Lifetime;

namespace HomeNotify.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private IUnityContainer container;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }
        
        /// <summary>
        /// Configure the <see cref="IUnityContainer"/>.
        /// </summary>
        /// <param name="container">The <see cref="IUnityContainer"/>.</param>
        public void ConfigureContainer(IUnityContainer container)
        {
            this.container = container;
            
            // setup firebase
            var firebaseApp = FirebaseApp.Create((new AppOptions
            {
                Credential = GoogleCredential.FromFile(Environment.GetEnvironmentVariable("GOOGLE_CREDENTIAL"))
            }));
            container.RegisterInstance(firebaseApp, new SingletonLifetimeManager());
            container.RegisterInstance(FirebaseMessaging.DefaultInstance,
                new SingletonLifetimeManager());
            
            // setup database
            var dbCreds = ReadDatabaseCredentials();

            if (dbCreds == null)
            {
                Console.WriteLine("Failed to connect to database. Exiting...");
                Environment.Exit(1);
            }
            
            var mongoClient = new MongoClient(
                $"mongodb://{dbCreds.Username}:{dbCreds.Password}@{dbCreds.Host}/{dbCreds.Database}");
            var db = mongoClient.GetDatabase(dbCreds.Database);

            container.RegisterInstance(db, new SingletonLifetimeManager());

            // repositories
            container.RegisterType(typeof(ILogger<>), typeof(MongoLogger<>),
                new ContainerControlledLifetimeManager());
            container.RegisterType<ITopicRepository, MongoTopicRepository>(new ContainerControlledLifetimeManager());
            
            // services
            container.RegisterType<ITopicsService, TopicsService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMessageService, FirebaseMessageService>(new ContainerControlledLifetimeManager());
        }

        private DatabaseCredentials ReadDatabaseCredentials()
        {
            var path = Environment.GetEnvironmentVariable("DB_CREDENTIALS");
            if (path == null)
            {
                Console.WriteLine("Database credentials environment variable not set.");
                return null;
            }
            
            FileStream fileStream = null;
            StreamReader streamReader = null;
            
            try
            {
                fileStream = new FileStream(path, FileMode.Open);
                streamReader = new StreamReader(fileStream);
                return BsonSerializer.Deserialize<DatabaseCredentials>(streamReader.ReadToEnd());
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Database connection credentials file not found.");
                return null;
            }
            finally
            {
                streamReader?.Close();
                fileStream?.Close();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}