using System;
using System.Reflection;
using Conexus.Opal.Shared.MongoDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MongoDBMigrations;

namespace Microservice.Badge.Infrastructure
{
    public class MongoMigrationStartupFilter : IStartupFilter
    {
        private readonly MongoOptions _options;

        public MongoMigrationStartupFilter(IOptions<MongoOptions> options)
        {
            _options = options.Value;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            var runner = new MigrationEngine().UseDatabase(_options.ConnectionString, _options.Database)
                .UseAssembly(Assembly.GetExecutingAssembly()) // Required
                .UseSchemeValidation(false);
            /*  .UseCancellationToken(token) //Optional if you wanna have possibility to cancel migration process. Might be useful when you have many migrations and some interaction with user.
                .UseProgressHandler(Action <> action) // Optional some delegate that will be called each migration */

            if (MongoDatabaseStateChecker.IsDatabaseOutdated(_options.ConnectionString, _options.Database))
            {
                var target = new MongoDBMigrations.Version(_options.DatabaseLatestVersion);
                runner.Run(target);
            }

            return next;
        }
    }
}
