using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.OutboxPattern.HostedServices.Abstractions;
using Conexus.Opal.OutboxPattern.Variants.MongoDb.Entities;
using Conexus.Opal.OutboxPattern.Variants.MongoDb.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Polly;
using Polly.Wrap;
using Thunder.Platform.Core.Timing;

namespace Conexus.Opal.OutboxPattern.Variants.MongoDb.HostedServices
{
    public class MongoOutboxMessageSender : OutboxMessageHostedService, IOutboxMessageSender
    {
        private const int RetryEntriesWhenConcurrencyHappened = 2;

        private static readonly object _senderLock = new object();

        private readonly IServiceProvider _provider;
        private readonly RabbitMQOptions _rabbitMQOptions;
        private readonly OutboxOptions _outboxOptions;
        private readonly ILogger<MongoOutboxMessageSender> _logger;
        private readonly PolicyWrap _policy;

        public MongoOutboxMessageSender(
            IHostApplicationLifetime lifetime,
            IServiceProvider provider,
            IOptions<RabbitMQOptions> rabbitMQOptions,
            IOptions<OutboxOptions> outboxOptions,
            ILoggerFactory loggerFactory) : base(lifetime)
        {
            _provider = provider;
            _rabbitMQOptions = rabbitMQOptions.Value;
            _outboxOptions = outboxOptions.Value;
            _logger = loggerFactory.CreateLogger<MongoOutboxMessageSender>();

            // Polly init
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    RetryEntriesWhenConcurrencyHappened,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            var fallbackPolicy = Policy
                .Handle<Exception>()
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
                var producer = _provider.GetRequiredService<IOpalMessageProducer>();
                var mongoDbContext = scope.ServiceProvider.GetRequiredService<IHasOutboxCollection>();
                var outboxCollection = mongoDbContext.OutboxMessageCollection;

                List<Guid> messagesIds = null;

                _policy.Execute(() =>
                {
                    messagesIds = PopTopQueuedMessage(outboxCollection);
                });

                if (messagesIds is null)
                {
                    _logger.NoMessageAvailableFromTheOutboxQueue();
                    return;
                }

                foreach (var messagesId in messagesIds)
                {
                    var queueMessageInfo = outboxCollection
                        .AsQueryable()
                        .FirstOrDefault(m => m.Id == messagesId);

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
                    outboxCollection.UpdateWithIncreaseVersion(queueMessageInfo);
                }
            }

            Monitor.Exit(_senderLock);
        }

        private List<Guid> PopTopQueuedMessage(IMongoCollection<MongoOutboxMessage> outboxCollection)
        {
            List<MongoOutboxMessage> messagesToSend = outboxCollection
                .AsQueryable()
                .Where(p => p.Status == MessageStatus.New)
                .OrderBy(m => m.CreatedDate)
                .Take(_outboxOptions.SendBatchSize)
                .ToList();

            messagesToSend.ForEach(m =>
            {
                m.Status = MessageStatus.Preparing;
                m.PreparedAt = Clock.Now;
                try
                {
                    outboxCollection.ReplaceOne(p => p.Id == m.Id, m);
                }
                catch (Exception exception)
                {
                    _logger.ExceptionWhenSendMessageFromQueue(exception);
                    throw;
                }
            });

            return messagesToSend
                .Select(p => p.Id)
                .ToList();
        }
    }
}
