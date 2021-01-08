using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;

namespace Datahub.Queue.Manager.Test.MongoDbProcessor
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Application Services
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRabbitMQ();
        }

        private static void AddRabbitMQ(this IServiceCollection services)
        {
            var factory = new ConnectionFactory();
            var rabbitmqHostNames = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAMES");
            var rabbitmqUsername = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME");
            var rabbitmqPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");
            var queueManagerAPI = Environment.GetEnvironmentVariable("QUEUE_MANAGER_API");
            if (string.IsNullOrEmpty(rabbitmqHostNames) || string.IsNullOrEmpty(rabbitmqUsername) || string.IsNullOrEmpty(rabbitmqPassword))
            {
                throw new ArgumentNullException("Missing Rabbitmq configure!");
            }
            var rabbitmqSettings = new RabbitMQSettings
            {
                Username = rabbitmqUsername,
                Password = rabbitmqPassword,
                HostNames = rabbitmqHostNames.Split(","),
                QueueManagerAPI = queueManagerAPI
            };
            services.AddSingleton<RabbitMQSettings>(c => rabbitmqSettings);
            factory.UserName = rabbitmqUsername;
            factory.Password = rabbitmqPassword;
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            services.AddSingleton<ConnectionFactory>(c => factory);
        }
    }
}
