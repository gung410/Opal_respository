using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ.Dependency;
using Conexus.Opal.Microservice.Infrastructure;
using Conexus.Opal.Microservice.Metadata.DataProviders;
using Conexus.Opal.Microservice.Metadata.DataSync;
using Conexus.Opal.Microservice.Tagging.Cache;
using Conexus.Opal.Microservice.Tagging.DataProviders;
using Conexus.Opal.Microservice.Tagging.Infrastructure;
using Conexus.Opal.Microservice.Tagging.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Caching;
using Thunder.Platform.Caching.Redis;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Modules;
using Thunder.Platform.Cqrs.Dependency;
using Thunder.Platform.EntityFrameworkCore;

[assembly: ThunderModuleAssembly]

namespace Conexus.Opal.Microservice.Tagging
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.Replace(ServiceDescriptor.Scoped<IDbContextResolver, TaggingDbContextResolver>());
            services.AddTransient<IDbContextSeeder, TaggingSeeder>();

            services.AddSingleton<HttpService>();
            services.AddSingleton<IAuthenticationTokenService, AuthenticationTokenService>();

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<TaggingDbContext>(ServiceLifetime.Transient);

            services.AddTransient<IMetadataSynchronizer, MetadataSynchronizer>();
            services.AddTransient<ITaggingDataProvider, TaggingDataProvider>();
            services.AddTransient<IMetadataDataProvider, TaggingMetadataDataProvider>();

            // Inject Application Services
            services.AddTransient<ResourceService>();

            // Add caching feature.
            services.AddSingleton<ICacheRepository>(provider =>
            {
                var accessorProvider = provider.GetService<IRedisAccessorProvider>();
                var logger = provider.GetService<ILoggerFactory>();
                return new RedisCacheRepository(nameof(MetadataCacheKey), accessorProvider, logger);
            });
        }

        protected override List<IConventionalDependencyRegistrar> DeclareConventionalRegistrars()
        {
            var registrars = base.DeclareConventionalRegistrars();
            registrars.Add(new CqrsConventionalRegistrar());
            registrars.Add(new OpalRabbitMQConventionalRegistrar());

            return registrars;
        }
    }
}
