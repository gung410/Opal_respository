using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace cxOrganization.WebServiceAPI
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
                                                                  .SetBasePath(Directory.GetCurrentDirectory())
                                                                  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                                  .AddJsonFile("property.mapping.config.json", optional: false, reloadOnChange: true)
                                                                  .AddJsonFile("mass.user.creation.message.json", optional: false, reloadOnChange: true)
                                                                  .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                                                                  .AddEnvironmentVariables()
                                                                  .Build();
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("Getting the service running...");

                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
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
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    const string accessSettingsFolder = "AccessSettings";
                    var accessSettingFiles = Directory.EnumerateFiles(accessSettingsFolder, "*.json", SearchOption.AllDirectories);
                    foreach (var jsonFilename in accessSettingFiles)
                    {
                        configurationBuilder.AddJsonFile(jsonFilename);
                    }
                })
                .UseSerilog();
    }
}
