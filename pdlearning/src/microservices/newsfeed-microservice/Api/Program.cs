using System.Net;
using Conexus.Opal.Connector.RabbitMQ;
using Microservice.NewsFeed.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Timing;

namespace Microservice.NewsFeed
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Setting clock provider, using utc time
            Clock.SetProvider(new UtcClockProvider());

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(builder =>
                {
                    builder.AddJsonFile("sharedsettings.json", false, false);
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddSimpleConsole(options => options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.F ");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureKestrel((context, options) =>
                        {
                            options.AddServerHeader = false;
                        })
                        .ConfigureServices((context, collection) =>
                        {
                            collection.AddThunderModuleSystem();
                            collection.AddOpalRabbitMQConnector(context.Configuration);
                            collection.AddMongo(context.Configuration);
                        })
                        .UseStartup<Startup>();
                });
    }
}
