using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Datahub.Queue.Manager
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(AppsettingsFileNameWithEnvironment(), optional: true)
            .AddEnvironmentVariables()
            .Build();
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.WithCorrelationIdHeader("Correlation-Id")
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("Getting the service running...");

                CreateWebHostBuilder(args).Build().Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>()
                   .UseConfiguration(Configuration)
                   .UseSerilog();

        private static string AppsettingsFileNameWithEnvironment()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrEmpty(environmentName))
            {
                environmentName = "Production";
            }
            else
            {
                //The deployment tool is set ASPNETCORE_ENVIRONMENT value as lower case
                //So we need to convert first letter to upper case
                environmentName = environmentName.First().ToString().ToUpper() + environmentName.Substring(1);
            }
            return $"appsettings.{environmentName}.json";
        }
    }
}
