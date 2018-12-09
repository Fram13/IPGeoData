using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IPGeoData.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IPGeoData.WebService.Infrastructure;
using IPGeoData.WebService.Models;

namespace IPGeoData.WebService
{
    public class Startup
    {
        public ApplicationConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .Build();

            Configuration = new ApplicationConfiguration();
            config.Bind(Configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            
            services.AddScoped<IGeoWebServicesClientFactory, GeoWebServicesClientFactory>(serviceProvider =>
            {
                return new GeoWebServicesClientFactory(Configuration.AccountID, Configuration.LicenseKey);
            });

            services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(Configuration.ConnectionString));
            services.AddScoped<IDataContext, DataContext>();
            services.AddScoped<IPLocationsManager>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
