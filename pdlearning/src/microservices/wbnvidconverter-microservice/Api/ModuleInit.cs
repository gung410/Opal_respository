using System.Collections.Generic;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Dependency;
using Conexus.Opal.Microservice.Infrastructure;
using MediatR;
using Microservice.WebinarVideoConverter.Application;
using Microservice.WebinarVideoConverter.Application.BackgroundServices;
using Microservice.WebinarVideoConverter.Application.Services;
using Microservice.WebinarVideoConverter.Application.Services.Abstractions;
using Microservice.WebinarVideoConverter.Infrastructure;
using Microservice.WebinarVideoConverter.Infrastructure.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Modules;
using Thunder.Platform.Cqrs.Dependency;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Logging;

[assembly: ThunderModuleAssembly]

namespace Microservice.WebinarVideoConverter
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddTransient<IDbContextSeeder, WebinarRecordSeeder>();

            services.Replace(ServiceDescriptor.Scoped<IDbContextResolver, WebinarRecordDbContextResolver>());

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<WebinarRecordMangementDbContext>(ServiceLifetime.Transient);
            services.AddQueryTrackingSupport();

            services.AddTransient(typeof(IRepository<>), typeof(WebinarRecordGenericRepository<>));

            services.AddSingleton<IAuditLogConventions>(provider => new AuditLogConventions(new Dictionary<AuditLogActionType, string[]>
            {
                { AuditLogActionType.Created, new[] { "Create" } },
                { AuditLogActionType.Updated, new[] { "Update", "Save", "Process" } },
                { AuditLogActionType.Deleted, new[] { "Delete" } }
            }));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditLogBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QueryTrackingBehavior<,>));

            services.AddTransient<IPlaybackScannerService, PlaybackScannerService>();
            services.AddTransient<IConvertingTrackingService, ConvertingTrackingService>();
            services.AddTransient<IAwsFargateTaskRunnerService, AwsFargateTaskRunnerService>();
            services.AddTransient<IUploaderService, UploadService>();
            services.AddTransient<IRecordFileService, FileRecordConvertedService>();
            services.AddTransient<IAmazonS3>(provider =>
            {
                var s3Options = provider.GetRequiredService<IOptions<AmazonS3Options>>().Value;
                return new AmazonS3Client(
                    new BasicAWSCredentials(s3Options.AccessKey, s3Options.SecretKey),
                    RegionEndpoint.GetBySystemName(s3Options.Region));
            });

            // Background services.
            services.AddHostedService<RecordingCollectorService>();
            services.AddHostedService<PlaybackConverterService>();
            services.AddHostedService<RecordUploaderService>();
            services.AddHostedService<ConvertSuccessDetectorService>();
            services.AddHostedService<FailedRecordHandlingService>();
            services.AddHostedService<RecordCleanerService>();

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
