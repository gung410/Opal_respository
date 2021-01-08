using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.OutboxPattern.HostedServices.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Wrap;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Conexus.Opal.OutboxPattern.HostedServices
{
    public class OutboxMessageSender : OutboxMessageHostedService, IOutboxMessageSender
    {
        private const int RetryEntriesWhenConcurrencyHappened = 2;

        private static readonly object _senderLock = new object();

        private readonly IServiceProvider _provider;
        private readonly RabbitMQOptions _rabbitMQOptions;
        private readonly OutboxOptions _outboxOptions;
        private readonly ILogger<OutboxMessageSender> _logger;
        private readonly PolicyWrap _policy;

        public OutboxMessageSender(
            IHostApplicationLifetime lifetime,
            IServiceProvider provider,
            IOptions<RabbitMQOptions> rabbitMQOptions,
            IOptions<OutboxOptions> outboxOptions,
            ILoggerFactory loggerFactory) : base(lifetime)
        {
            _provider = provider;
            _rabbitMQOptions = rabbitMQOptions.Value;
            _outboxOptions = outboxOptions.Value;
            _logger = loggerFactory.CreateLogger<OutboxMessageSender>();

            // Polly init
            var retryPolicy = Policy
                .Handle<DbUpdateConcurrencyException>()
                .WaitAndRetry(
                    RetryEntriesWhenConcurrencyHappened,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            var fallbackPolicy = Policy
                .Handle<DbUpdateConcurrencyException>()
                .Fallback(
                    fallbackAction: (outcome, context, token) =>
                    {
                    },
                    onFallback: (exception, context) =>
                    {
                        _logger.ConcurrencyExceptionWhenGettingMessageFromQueue(exception);
                    });

            _policy = fallbackPolicy.Wrap(retryPolicy);
        }

        protected override TimeSpan TimerPeriodConfig()
        {
            return TimeSpan.FromMilliseconds(1000);
        }

        protected override void Process(object state)
        {
            // We need this check to ensure that the database was seeded successfully in the Startup.cs class.
            // This process needs database tables to interact with.
            if (!ApplicationStarted)
            {
                return;
            }

            // To avoid this method stuck due to waiting for the sender.
            // Check if the sender was acquired an exclusive look then perform send messages.
            // Otherwise, this method was overlapping called by Timer so there is nothing happen.
            if (!Monitor.TryEnter(_senderLock))
            {
                return;
            }

            using (var scope = _provider.CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
                var producer = _provider.GetRequiredService<IOpalMessageProducer>();
                var outboxRepository = scope.ServiceProvider.GetRequiredService<IRepository<OutboxMessage>>();

                List<Guid> messagesIds = null;

                _policy.Execute(() =>
                {
                    messagesIds = PopTopQueuedMessage(uowManager, outboxRepository);
                });

                if (messagesIds is null)
                {
                    _logger.NoMessageAvailableFromTheOutboxQueue();
                    return;
                }

                uowManager.StartNewTransaction(() =>
                {
                    foreach (var messagesId in messagesIds)
                    {
                        var queueMessageInfo = outboxRepository.Get(messagesId);
                        var exchange = string.IsNullOrEmpty(queueMessageInfo.Exchange)
                            ? _rabbitMQOptions.DefaultEventExchange
                            : queueMessageInfo.Exchange;
                        try
                        {
                            producer.Send(queueMessageInfo.MessageData, exchange, queueMessageInfo.RoutingKey);
                            queueMessageInfo.Status = MessageStatus.Sent;
                        }
                        catch (Exception exception)
                        {
                            queueMessageInfo.FailReason = exception.Message;
                            queueMessageInfo.Status = MessageStatus.Failure;
                        }

                        queueMessageInfo.SendTimes += 1;
                        outboxRepository.Update(queueMessageInfo);
                    }
                });
            }

            Monitor.Exit(_senderLock);
        }

        private List<Guid> PopTopQueuedMessage(IUnitOfWorkManager uowManager, IRepository<OutboxMessage> outboxRepository)
        {
            List<Guid> result = null;

            uowManager.StartNewTransaction(() =>
            {
                List<OutboxMessage> messagesToSend = outboxRepository
                    .GetAll()
                    .Where(p => p.Status == MessageStatus.New)
                    .OrderBy(m => m.CreatedDate)
                    .Take(_outboxOptions.SendBatchSize)
                    .ToList();

                result = messagesToSend.Select(m => m.Id).ToList();

                messagesToSend.ForEach(m =>
                {
                    m.Status = MessageStatus.Preparing;
                    m.PreparedAt = Clock.Now;
                });

                try
                {
                    outboxRepository.UpdateMany(messagesToSend);
                }
                catch (DbUpdateConcurrencyException exception)
                {
                    _logger.ConcurrencyExceptionWhenTryingToPrepareMessagesForSending(exception);
                    throw;
                }
            });

            return result;
        }
    }
}
