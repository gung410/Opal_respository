using System;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;

namespace Conexus.Opal.Connector.RabbitMQ.Core
{
    public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>
    {
        private readonly RetryPolicy _retryPolicy;
        private readonly RabbitMQOptions _options;
        private readonly IConnection _connection;

        public RabbitModelPooledObjectPolicy(IOptions<RabbitMQOptions> options)
        {
            _options = options.Value;

            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    _options.RetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            _connection = CreateConnection();
        }

        public IModel Create()
        {
            return _connection.CreateModel();
        }

        public bool Return(IModel obj)
        {
            if (obj.IsOpen)
            {
                return true;
            }

            obj?.Dispose();

            return false;
        }

        private IConnection CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(15),
                UserName = _options.Username,
                Password = _options.Password,
                VirtualHost = "/",
                Port = AmqpTcpEndpoint.UseDefaultPort,
                DispatchConsumersAsync = true,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(5)
            };

            if (_options.SslEnabled)
            {
                factory.Ssl.Enabled = true;
                factory.Ssl.Version = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
                factory.Ssl.AcceptablePolicyErrors =
                    SslPolicyErrors.RemoteCertificateNotAvailable
                    | SslPolicyErrors.RemoteCertificateChainErrors
                    | SslPolicyErrors.RemoteCertificateNameMismatch;
            }

            var hostNames = _options.HostNames.Split(',').Where(hostName => !string.IsNullOrEmpty(hostName)).ToArray();
            var policyResult = _retryPolicy.ExecuteAndCapture(() => factory.CreateConnection(hostNames));
            if (policyResult.FinalException != null)
            {
                throw policyResult.FinalException;
            }

            // The final handled result captured. Will be IConnection if the policy executed successfully, or terminated with an exception.
            return policyResult.Result;
        }
    }
}
