using System.Collections.Generic;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.Connector.RabbitMQ.Dependency;
using Conexus.Opal.Microservice.Infrastructure;
using Microservice.Uploader.Infrastructure;
using Microservice.Uploader.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Modules;
using Thunder.Platform.Cqrs.Dependency;
using Thunder.Platform.EntityFrameworkCore;

[assembly: ThunderModuleAssembly]

namespace Microservice.Uploader
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.AddTransient<IDbContextSeeder, UploaderSeeder>();
            services.Replace(ServiceDescriptor.Scoped<IDbContextResolver, UploaderDbContextResolver>());
            services.AddTransient<IAmazonS3UploaderService, AmazonS3UploaderService>();
            services.AddTransient<IDownloadLearningContentService, DownloadLearningContentService>();
            services.AddTransient<IAmazonS3PersonalSpaceUploaderService, AmazonS3PersonalSpaceUploaderService>();
            services.AddTransient<IAmazonS3KeyBuilderService, AmazonS3KeyBuilderService>();
            services.AddTransient<IStorageService, StorageService>();

            services.AddEntityFrameworkSqlServer()
               .AddDbContext<UploaderDbContext>(ServiceLifetime.Transient);
            services.AddTransient(typeof(IRepository<>), typeof(UploaderGenericRepository<>));

            services.AddTransient<IAccessControlContext, AccessControlContext>();

            services.AddSingleton<HttpService>();
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
