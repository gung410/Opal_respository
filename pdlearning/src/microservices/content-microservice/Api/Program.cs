using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.InboxPattern.Common;
using Conexus.Opal.OutboxPattern;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Timing;

namespace Microservice.Content
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
                        .ConfigureKestrel(serverOptions =>
                        {
                            serverOptions.AddServerHeader = false;
                        })
                        .ConfigureServices((context, collection) =>
                        {
                            collection.AddThunderModuleSystem();

                            collection.AddOpalRabbitMQConnector(context.Configuration);

                            collection.AddOutboxQueueMessage(context.Configuration);

                            collection.AddInboxQueueMessage(context.Configuration);
                        })
                        .UseStartup<Startup>();
                });
    }
}
