using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using HomeNotify.API.Database;
using HomeNotify.API.Database.Implementation;
using HomeNotify.API.Models;
using HomeNotify.API.Services;
using HomeNotify.API.Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            var dbConnectivityProvider = container.Resolve<PostgresDatabaseConnectivityProvider>();
            var connectionResult = dbConnectivityProvider.Connect();
            if (connectionResult != ConnectionResult.Success)
            {
                Console.WriteLine("Database failed to initialize; check logs for further details. Exiting...");
                Environment.Exit(1);
            }

            container.RegisterInstance(dbConnectivityProvider, new SingletonLifetimeManager());

            // repositories
            container.RegisterType(typeof(ILogger<>), typeof(PostgresLogger<>),
                new ContainerControlledLifetimeManager());
            
            // services
            container.RegisterType<ITopicsService, MemoryTopicsService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMessageService, FirebaseMessageService>(new ContainerControlledLifetimeManager());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            applicationLifetime.ApplicationStopping.Register(OnShutdown);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private void OnShutdown()
        {
            this.container.Resolve<PostgresDatabaseConnectivityProvider>().Disconnect();
            Console.WriteLine("Disconnected from database successfully.");
            System.Threading.Thread.Sleep(1000);
        }
    }
}