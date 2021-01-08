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

namespace Communication.Processor.Sender
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(AppsettingsFileNameWithEnvironment(), optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration)
            .Enrich.WithCorrelationIdHeader("Correlation-Id")
            .Enrich.FromLogContext().WriteTo
            .Console().CreateLogger();

             CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
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
