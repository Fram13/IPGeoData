using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using IPGeoData.Model;
using MaxMind.GeoIP2;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IPGeoData.DatabaseUpdater
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddSerilog())
                .AddDbContext<DatabaseContext>(options => options.UseNpgsql(config["ConnectionString"]))
                .AddScoped<IDataContext, DataContext>()
                .BuildServiceProvider();

            var app = new CommandLineApplication<Application>();
            app.Conventions.UseDefaultConventions().UseConstructorInjection(serviceProvider);

            app.Execute(args);
        }
    }
}