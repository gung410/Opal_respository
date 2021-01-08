
using System;
using System.Net.Security;
using System.Security.Authentication;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using RabbitMQ.Client;

namespace Communication.Processor.Sender.Extensions
{
    public static class RabbitMQServiceExtension
    {

        public static void AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {

            var factory = new ConnectionFactory();
            var rabbitmqHostNames = configuration["RABBITMQ_HOSTNAMES"];
            var rabbitmqUsername = configuration["RABBITMQ_USERNAME"];
            var rabbitmqPassword = configuration["RABBITMQ_PASSWORD"];
            var sslEnabled = false;
            bool.TryParse(configuration["RABBITMQ_SSL_ENABLED"], out sslEnabled);
            if (string.IsNullOrEmpty(rabbitmqHostNames) || string.IsNullOrEmpty(rabbitmqUsername) || string.IsNullOrEmpty(rabbitmqPassword))
            {
                throw new ArgumentNullException("Missing Rabbitmq configure!");
            }

            //logger.LogInformation($"HostNames: {rabbitmqHostNames} Username: {rabbitmqUsername}");

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
            var connection = factory.CreateConnection(rabbitmqHostNames.Split(","));
            services.AddSingleton<ConnectionFactory>(c => factory);
            services.AddSingleton<IConnection>(c => connection);
        }

     
    }
}
