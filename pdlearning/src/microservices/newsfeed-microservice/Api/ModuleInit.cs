using System.Collections.Generic;
using System.Text;
using Conexus.Opal.Connector.RabbitMQ.Dependency;
using Conexus.Opal.Microservice.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Modules;
using Thunder.Platform.Cqrs.Dependency;

[assembly: ThunderModuleAssembly]

namespace Microservice.NewsFeed
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            ConventionRegistry.Register(
                "EnumStringConvention",
                new ConventionPack
                {
                    new EnumRepresentationConvention(BsonType.String)
                },
                t => true);

            // https://medium.com/@samichkhachkhi/no-data-is-available-for-encoding-1252-8bc14651d631
            // There is an exception when generating a pdf report.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

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
