using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Thunder.Platform.Testing
{
    public abstract class BaseThunderTest
    {
        protected BaseThunderTest()
        {
            Configuration = CreateInMemoryConfiguration();

            Provider = CreateAppServiceCollection()
                .BuildServiceProvider();
        }

        public IServiceProvider Provider { get; }

        protected IConfiguration Configuration { get; }

        protected virtual IEnumerable<KeyValuePair<string, string>> SetupInMemoryConfiguration()
        {
            return new KeyValuePair<string, string>[0];
        }

        protected virtual IServiceCollection SetupAppServices(IServiceCollection services)
        {
            return services;
        }

        private IConfigurationRoot CreateInMemoryConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(SetupInMemoryConfiguration())
                .Build();

            return configuration;
        }

        private IServiceCollection CreateAppServiceCollection()
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging()
                .AddThunderModuleSystem()
                .AddSingleton<IConfiguration>(provider => CreateInMemoryConfiguration());

            SetupAppServices(serviceCollection);

            return serviceCollection;
        }
    }
}
