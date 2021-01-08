using Conexus.Opal.InboxPattern.HostedServices;
using Conexus.Opal.InboxPattern.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Conexus.Opal.InboxPattern.Common
{
    public static class InboxPatternServiceCollectionExtensions
    {
        public static IServiceCollection AddInboxQueueMessage(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<InboxOptions>().Bind(configuration.GetSection(nameof(InboxOptions)));

            services.AddTransient<IInboxQueue, InboxQueue>();
            services.AddHostedService<InboxMessageCleaner>();

            return services;
        }
    }
}
