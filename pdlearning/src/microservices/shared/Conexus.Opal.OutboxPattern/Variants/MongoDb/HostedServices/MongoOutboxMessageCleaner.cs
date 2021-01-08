using System;
using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.OutboxPattern.HostedServices.Abstractions;
using Conexus.Opal.OutboxPattern.Variants.MongoDb.Entities;
using Conexus.Opal.OutboxPattern.Variants.MongoDb.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Thunder.Platform.Core.Timing;

namespace Conexus.Opal.OutboxPattern.Variants.MongoDb.HostedServices
{
    public class MongoOutboxMessageCleaner : OutboxMessageHostedService, IOutboxMessageCleaner
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<MongoOutboxMessageCleaner> _logger;
        private readonly OutboxOptions _outboxOptions;

        public MongoOutboxMessageCleaner(
            IHostApplicationLifetime lifetime,
            IServiceProvider provider,
            IOptions<OutboxOptions> options,
            ILoggerFactory loggerFactory) : base(lifetime)
        {
            _provider = provider;
            _outboxOptions = options.Value;
            _logger = loggerFactory.CreateLogger<MongoOutboxMessageCleaner>();
        }

        protected override TimeSpan TimerPeriodConfig()
        {
            return TimeSpan.FromDays(1);
        }

        protected override void Process(object state)
        {
            // We need this check to ensure that the database was seeded successfully in the Startup.cs class.
            // This process needs database tables to interact with.
            if (!ApplicationStarted)
            {
                return;
            }

            using (var scope = _provider.CreateScope())
            {
                var mongoDbContext = scope.ServiceProvider.GetRequiredService<IHasOutboxCollection>();
                var outboxCollection = mongoDbContext.OutboxMessageCollection;

                bool continueDelete = true;

                while (continueDelete)
                {
                    var legacyMessages = TakeMessageToDelete(outboxCollection);
                    if (!legacyMessages.Any())
                    {
                        break;
                    }

                    var legacyMessageIds = legacyMessages.Select(p => p.Id);

                    outboxCollection.DeleteMany(p => legacyMessageIds.Contains(p.Id));

                    _logger.NumberOfCleanedMessages(legacyMessages.Count);
                    continueDelete = legacyMessages.Count >= _outboxOptions.DeleteBatchSize;
                }
            }
        }

        private List<MongoOutboxMessage> TakeMessageToDelete(IMongoCollection<MongoOutboxMessage> outboxCollection)
        {
            var deleteCheckpoint = Clock.Now.AddDays(-1 * _outboxOptions.DeleteMessageAfterDays);

            try
            {
                var result = outboxCollection
                    .AsQueryable()
                    .Where(m => m.CreatedDate < deleteCheckpoint)
                    .Where(m => m.Status == MessageStatus.Sent)
                    .Take(_outboxOptions.DeleteBatchSize)
                    .ToList();

                foreach (var sentMessage in result)
                {
                    sentMessage.ReadyToDelete = true;
                    outboxCollection.UpdateWithIncreaseVersion(sentMessage);
                }

                return result;
            }
            catch (Exception exception)
            {
                _logger.ExceptionWhenDeleteMessageFromQueue(exception);
                return new List<MongoOutboxMessage>();
            }
        }
    }
}
