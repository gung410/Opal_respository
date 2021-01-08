using System;
using System.Net.Security;
using System.Security.Authentication;

using Datahub.Queue.Manager.Configurations;
using Datahub.Queue.Manager.Data;
using Datahub.Queue.Manager.Data.MongoDb;
using Datahub.Queue.Manager.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

namespace Datahub.Queue.Manager
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Application Services
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            services.AddTransient(typeof(IMongoRepository<>), typeof(MongoRepository<>));
            services.AddTransient(typeof(ITransactionalMongoRepository<>), typeof(TransactionalMongoRepository<>));
            services.AddTransient<IMappingService, MappingService>();
            services.AddRabbitMQ(logger);
        }

        private static void AddRabbitMQ(this IServiceCollection services, ILogger logger)
        {
            var factory = new ConnectionFactory();
            var rabbitmqHostNames = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAMES");
            var rabbitmqUsername = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME");
            var rabbitmqPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");
            var sslEnabled = false;
            bool.TryParse(Environment.GetEnvironmentVariable("RABBITMQ_SSL_ENABLED"), out sslEnabled);
            if (string.IsNullOrEmpty(rabbitmqHostNames) || string.IsNullOrEmpty(rabbitmqUsername) || string.IsNullOrEmpty(rabbitmqPassword))
            {
                throw new ArgumentNullException("Missing Rabbitmq configure!");
            }
            var rabbitmqSettings = new RabbitMQSettings
            {
                Username = rabbitmqUsername,
                Password = rabbitmqPassword,
                HostNames = rabbitmqHostNames.Split(","),
                SslEnabled = sslEnabled
            };
            logger.LogInformation($"HostNames: {rabbitmqHostNames} Username: {rabbitmqUsername}");
            services.AddSingleton<RabbitMQSettings>(c => rabbitmqSettings);
            factory.UserName = rabbitmqUsername;
            factory.Password = rabbitmqPassword;
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            if (sslEnabled)
            {
                factory.Ssl.Enabled = true;
                factory.Ssl.Version = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
                factory.Ssl.AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNotAvailable |
                    SslPolicyErrors.RemoteCertificateChainErrors |
                    SslPolicyErrors.RemoteCertificateNameMismatch;
            }
            var connection = factory.CreateConnection(rabbitmqSettings.HostNames);
            services.AddSingleton<ConnectionFactory>(c => factory);
            services.AddSingleton<IConnection>(c => connection);
        }
    }
}
