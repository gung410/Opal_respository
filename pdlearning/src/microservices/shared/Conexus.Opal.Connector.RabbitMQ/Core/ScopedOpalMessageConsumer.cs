using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Conexus.Opal.Connector.RabbitMQ.Core
{
    /// <summary>
    /// Using this in case of need to send command or use repository in the consumer.
    /// We use method parameter injection pattern to avoid unnecessary instances injection from constructor when
    /// looping to the consumer list to check by the <see cref="IOpalMessageConsumer.CanProcess"/> method.
    /// </summary>
    /// <typeparam name="TMessage">Type of Message.</typeparam>
    public abstract class ScopedOpalMessageConsumer<TMessage> : IOpalMessageConsumer<TMessage>
        where TMessage : class
    {
        private bool _disposed;

        public OpalMQMessage<TMessage> OriginMessage { get; private set; }

        public async Task HandleAsync(IServiceScope serviceScope, OpalMQMessage<TMessage> message)
        {
            var logger = serviceScope.ServiceProvider
                .GetService<ILogger<ScopedOpalMessageConsumer<TMessage>>>();

            OriginMessage = message;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var isContinue = await OnBeforeMessageHandling(serviceScope);

                if (!isContinue)
                {
                    return;
                }

                var unitOfWorkManager = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

                using (var uow = unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
                {
                    logger.LogInformation("Begin to handle message...");

                    // Dear developers, these following lines of code is the Method Injection pattern.
                    // This pattern is used by ASP.NET Core in the Startup.cs and the Configure method,
                    // that you can inject whatever you want into the parameters of the method.
                    // Actually, to achieve the same thing as ASP.NET Core, just need few lines of code :)
                    var method = GetInternalHandleAsyncMethod();

                    var parameters = method.GetParameters();
                    if (parameters.Length == 0)
                    {
                        throw new InvalidOperationException(
                            "The method InternalHandleAsync needs at least 1 parameter which is the message type.");
                    }

                    var paramType = parameters[0].ParameterType;
                    var bodyType = message.Payload.Body.GetType();
                    if (paramType != bodyType)
                    {
                        throw new InvalidOperationException(
                            $"The type {paramType} is different from {bodyType}");
                    }

                    var arguments = new object[parameters.Length];
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        var parameter = parameters[i];
                        arguments[i] = i == 0
                            ? message.Payload.Body
                            : serviceScope.ServiceProvider.GetRequiredService(parameter.ParameterType);
                    }

                    // Invoke the method.
                    await (Task)method.Invoke(this, arguments);

                    await uow.CompleteAsync();
                }

                await OnAfterMessageHandling(serviceScope);

                stopwatch.Stop();

                logger.LogDebug("~~~|~~~ End handling message. Total time: {milliseconds}.", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "There is an exception when trying to send the message. Total time: {ElapsedMilliseconds} milliseconds.",
                    stopwatch.ElapsedMilliseconds);
            }
            finally
            {
                OriginMessage = null;
                logger.LogInformation(
                    "End handling message. Total time: {ElapsedMilliseconds} milliseconds.",
                    stopwatch.ElapsedMilliseconds);
            }
        }

        public bool CanProcess(string routingKey)
        {
            var consumerAttributes = GetType().GetCustomAttributes<OpalConsumerAttribute>().ToList();
            if (!consumerAttributes.Any())
            {
                throw new ArgumentNullException($"Please declare {nameof(OpalConsumerAttribute)} for the consumer class.");
            }

            return consumerAttributes.Any(p => p.RoutingKey == routingKey);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual Task<bool> OnBeforeMessageHandling(IServiceScope currentServiceScope)
        {
            return Task.FromResult(true);
        }

        protected virtual Task OnAfterMessageHandling(IServiceScope currentServiceScope)
        {
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }

        private MethodInfo GetInternalHandleAsyncMethod()
        {
            var methods = GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => string.Equals(m.Name, "InternalHandleAsync", StringComparison.Ordinal))
                .ToArray();

            if (methods.Length == 1)
            {
                return methods[0];
            }

            if (methods.Length == 0)
            {
                throw new InvalidOperationException("The consumer class must define a 'InternalHandleAsync' method.");
            }

            throw new InvalidOperationException("Overloading the 'InternalHandleAsync' method is not supported.");
        }
    }
}
