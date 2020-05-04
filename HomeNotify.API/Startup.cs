using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using HomeNotify.API.Services;
using HomeNotify.API.Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddLogging();
        }
        
        /// <summary>
        /// Configure the <see cref="IUnityContainer"/>.
        /// </summary>
        /// <param name="container">The <see cref="IUnityContainer"/>.</param>
        public void ConfigureContainer(IUnityContainer container)
        {
            // setup firebase
            var firebaseApp = FirebaseApp.Create((new AppOptions
            {
                Credential = GoogleCredential.FromFile(Environment.GetEnvironmentVariable("GOOGLE_CREDENTIAL"))
            }));
            container.RegisterInstance(firebaseApp, new SingletonLifetimeManager());
            container.RegisterInstance(FirebaseMessaging.DefaultInstance,
                new SingletonLifetimeManager());
            
            // services
            container.RegisterType<ITopicsService, MemoryTopicsService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMessageService, FirebaseMessageService>(new ContainerControlledLifetimeManager());
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