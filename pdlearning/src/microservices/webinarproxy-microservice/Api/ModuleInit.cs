using System.Collections.Generic;
using System.Net.Http;
using Conexus.Opal.Microservice.Infrastructure;
using Microservice.WebinarProxy.Application.Constants;
using Microservice.WebinarProxy.Application.Services;
using Microservice.WebinarProxy.Infrastructure.Caches;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Caching;
using Thunder.Platform.Caching.Redis;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Modules;

[assembly: ThunderModuleAssembly]

namespace Microservice.WebinarProxy
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services
                .AddHttpClient(HttpClientSchemaConstant.BigBlueButtonClient)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AllowAutoRedirect = false,
                    UseCookies = false
                })
                .Services
                .AddTransient<IMeetingUrlHelper, MeetingUrlHelper>()
                .AddTransient<IChecksumHelper, ChecksumHelper>()
                .AddTransient<ISessionValidator, SessionValidator>()
                .AddSingleton<ICacheRepository>(provider =>
                {
                    var accessorProvider = provider.GetService<IRedisAccessorProvider>();
                    var logger = provider.GetService<ILoggerFactory>();
                    return new RedisCacheRepository(nameof(TicketStoreCacheKey), accessorProvider, logger);
                });

            services.AddSingleton<HttpService>();
        }

        protected override List<IConventionalDependencyRegistrar> DeclareConventionalRegistrars()
        {
            var registrars = base.DeclareConventionalRegistrars();

            return registrars;
        }
    }
}
